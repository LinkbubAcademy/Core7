using Common.Lib.Authentication;
using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using Test.Lib.Authentication;

namespace Test.Lib.Models
{
    public partial class Person : User
    {
        public string Name { get; set; } = string.Empty;

        #region Posts

        public List<Post> Posts { get; set; } = new();

        public Task<QueryResult<List<Post>>> PostsAsync 
        {
            get
            {
                var repo = ContextFactory?.GetRepository<Post>();
                if (repo == null)
                    return Task.FromResult(new QueryResult<List<Post>>()
                    {
                        IsSuccess = false,
                        Message = ContextFactory == null ?
                                        "ContextFactory is null. Use ContextFactory to create a model" :
                                        "Post Repository is not inyected"
                    });

                return repo.Where(PostOwner.EqualsTo(Id)).ToListAsync();
            }
        }

        public async Task<ISaveResult<Post>> AddPostAsync(string message)
        {
            if (ContextFactory == null)
                throw new ContextFactoryNullException("Person", "AddPostAsync");

            var post = ContextFactory.NewModel<Post>();

            post.Message = message;
            post.OwnerId = this.Id;
            return await post.SaveAsync();
        }

        #endregion

        public Person() 
        {
            SaveAction = async () => await SaveAsync();
        }

        #region Clone

        /// <summary>
        /// Retrieves a copy marked as IsNew false and with the original entity
        /// to be compared to get changes
        /// </summary>
        /// <returns>Person</returns>
        public new Person Clone()
        {
            return Clone<Person>();
        }

        public override T Clone<T>()
        {
            if (base.Clone<T>() is Person output && output is T result)
            {
                output.Name = Name;

                return result;
            }

            throw new ArgumentException($"Type {typeof(T).FullName} does not derived from Person");
        }

        public override async Task IncludeChildren(Dictionary<Guid, Entity> refEnts, int nestingLevel)
        {
            await base.IncludeChildren(refEnts, nestingLevel);

            if (nestingLevel > 0)
            {
                var qr2 = (await PostsAsync);
                (await PostsAsync)?.Value?.DoForeach(x => refEnts.TryAdd(x.Id, x));
            }
        }
        public override void AssignChildren(QueryResult qr)
        {
            if (!IsAlreadyAssigned && qr.ReferencedEntities.Count != 0)
            {
                base.AssignChildren(qr);
                IsAlreadyAssigned = true;

                qr.ReferencedEntities.Values.OfType<Post>().Where(e => e.OwnerId == this.Id).DoForeach(e => { Posts.Add(e); e.AssignChildren(qr); e.Owner = this; });
            }
        }

        #endregion

        #region Changes

        public override EntityInfo GetChanges()
        {
            var output = base.GetChanges();

            if (output.IsNew || Origin == null)
            {
                output.AddChange(PersonMetadata.Name, Name);
            }
            else
            {
                if (Origin is not Person origin || Origin == null)
                    throw new InvalidOperationException("Clone must be called in order to modify an entity");

                if (origin.Name != Name)
                    output.AddChange(PersonMetadata.Name, Name);

            }

            return output;
        }

        public override List<ChangeUnit> ApplyChanges(List<ChangeUnit> changes)
        {
            var remainingChanges = base.ApplyChanges(changes);

            foreach (var change in remainingChanges)
            {
                switch (change.MetadataId)
                {
                    case PersonMetadata.Name:
                        Name = change.GetValueAsString();
                        break;
                    default:
                        return remainingChanges.Where(x => x.MetadataId > PersonMetadata.Last).ToList();
                }
            }

            return remainingChanges.Where(x => x.MetadataId > PersonMetadata.Last).ToList();
        }

        #endregion

        #region Save

        public new virtual async Task<ISaveResult<Person>> SaveAsync()
        {
            return await SaveAsync<Person>();
        }

        public override async Task<ISaveResult<T>> SaveAsync<T>(IUnitOfWork? uow = null)
        {
            return await base.SaveAsync<T>();
        }

        #endregion
    }

    #region metadata maps
    public partial class Person
    {

        public static new Dictionary<int, ValueTypes> MetadataMaps
        {
            get
            {
                if (metadataMaps.Count == 0)
                {
                    metadataMaps = new Dictionary<int, ValueTypes>();

                    foreach (var item in User.MetadataMaps)
                        metadataMaps.Add(item.Key, item.Value);

                    metadataMaps.Add(PersonMetadata.Name, ValueTypes.String);
                }

                return metadataMaps;
            }
        }
        static Dictionary<int, ValueTypes> metadataMaps = new Dictionary<int, ValueTypes>();

    }

    public static class PersonMetadata
    {
        public const int Name = UserMetadata.Last + 1;

        public const int Last = Name;
    }

    #endregion

    public enum PersonActions
    {
        Create = 0,
        Read = 1,
        Update = 2,
        Delete = 3
    }

    public class PersonActionsRolesMaps
    {
        Dictionary<PersonActions, Dictionary<TestRoles, RolActionMap>> RolActionMaps { get; set; } = new();

        public PersonActionsRolesMaps()
        {
            RegisterCreate();
            RegisterUpdate();

            Person.AddScopes(RolActionMaps);
        }

        void RegisterCreate()
        {
            var systemPermissions = new RolActionMap(TestRolesHandler.System)
            {
                AllowAll = true
            };

            var writerPermissions = new RolActionMap(TestRolesHandler.Writer);
            var editorPermissions = new RolActionMap(TestRolesHandler.Editor);
            var readerPermissions = new RolActionMap(TestRolesHandler.Reader);

            var maps = new Dictionary<TestRoles, RolActionMap>();
            RolActionMaps.Add(PersonActions.Create, maps);

            maps.Add(TestRoles.System, systemPermissions);
            maps.Add(TestRoles.Writer, writerPermissions);
            maps.Add(TestRoles.Editor, editorPermissions);
            maps.Add(TestRoles.Reader, readerPermissions);
        }

        void RegisterUpdate()
        {
            var systemPermissions = new RolActionMap(TestRolesHandler.System)
            {
                AllowAll = true
            };

            var writerPermissions = new RolActionMap(TestRolesHandler.Writer);
            var editorPermissions = new RolActionMap(TestRolesHandler.Editor);
            var readerPermissions = new RolActionMap(TestRolesHandler.Reader);

            var maps = new Dictionary<TestRoles, RolActionMap>();
            RolActionMaps.Add(PersonActions.Update, maps);

            maps.Add(TestRoles.System, systemPermissions);
            maps.Add(TestRoles.Writer, writerPermissions);
            maps.Add(TestRoles.Editor, editorPermissions);
            maps.Add(TestRoles.Reader, readerPermissions);
        }
    }
}
