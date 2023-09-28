using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure.Actions;

namespace Delicapp.Lib.Models
{
    public partial class Post : Entity
    {
        public string Message { get; set; } = string.Empty;

        #region ParentPerson

        public Guid OwnerId { get; set; }

        public Person? Owner { get; set; }

        public Task<QueryResult<Person>> OwnerAsync
        {
            get
            {
                var repo = ContextFactory?.GetRepository<Person>();
                if (repo == null)
                    return Task.FromResult(new QueryResult<Person>()
                    {
                        IsSuccess = false,
                        Message = ContextFactory == null ?
                                        "ContextFactory is null. Use ContextFactory to create a model" :
                                        "Person Repository is not injected"
                    });

                return repo.FindAsync(OwnerId);
            }
        }

        #endregion

        public Post()
        {
            SaveAction = SaveAsync;
        }

        #region Clone

        /// <summary>
        /// Retrieves a copy marked as IsNew false and with the original entity
        /// to be compared to get changes
        /// </summary>
        /// <returns>Post</returns>
        public new Post Clone()
        {
            return Clone<Post>();
        }

        public override T Clone<T>()
        {
            if (base.Clone<T>() is Post output && output is T result)
            {
                output.Message = Message;
                output.OwnerId = OwnerId;
                output.Owner = Owner;

                return result;
            }

            throw new ArgumentException($"Type {typeof(T).FullName} does not derived from Post");
        }

        public override async Task<Dictionary<Guid, Entity>> IncludeChildren(QueryResult qr, int nestingLevel)
        {
            var output = await base.IncludeChildren(qr, nestingLevel);

            if (nestingLevel > 0)
            {
                (await OwnerAsync)?.Value?.Do<Person>(x => output.TryAdd(x.Id, x));
            }

            return output;
        }

        public override void AssignChildren(QueryResult qr)
        {
            Console.WriteLine("Post.AssignChildren");
            if (!IsAlreadyAssigned && qr.ReferencedEntities.Count != 0)
            {
                base.AssignChildren(qr);
                IsAlreadyAssigned = true;

                if (qr.ReferencedEntities.ContainsKey(OwnerId)) { Owner = (Person)Owner; Owner?.AssignChildren(qr); }
            }
        }

        #endregion

        #region Changes

        public override EntityInfo GetChanges()
        {
            var output = base.GetChanges();

            if (output.IsNew || Origin == null)
            {
                output.AddChange(PostMetadata.Message, Message);
                output.AddChange(PostMetadata.Owner, OwnerId);
            }
            else
            {
                if (Origin is not Post origin || Origin == null)
                    throw new InvalidOperationException("Clone must be called in order to modify an entity");

                if (origin.Message != Message)
                    output.AddChange(PostMetadata.Message, Message);

                if (origin.OwnerId != OwnerId)
                    output.AddChange(PostMetadata.Owner, OwnerId);

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
                    case PostMetadata.Message:
                        Message = change.GetValueAsString();
                        break;

                    case PostMetadata.Owner:
                        OwnerId = change.GetValueAsGuid();
                        break;

                    default:
                        return remainingChanges.Where(x => x.MetadataId > PostMetadata.Last).ToList();
                }
            }

            return remainingChanges.Where(x => x.MetadataId > PostMetadata.Last).ToList();
        }

        #endregion


        #region Save

        public virtual async Task<SaveResult> SaveAsync()
        {
            return await SaveAsync<Post>();
        }

        public override async Task<SaveResult> SaveAsync<T>(IUnitOfWork? uow = null)
        {
            return await base.SaveAsync<T>();
        }

        #endregion

    }

    public partial class Post
    {
        #region metadata maps

        public static new Dictionary<int, ValueTypes> MetadataMaps
        {
            get
            {
                if (metadataMaps.Count == 0)
                {
                    metadataMaps = new Dictionary<int, ValueTypes>();

                    foreach (var item in Entity.MetadataMaps)
                        metadataMaps.Add(item.Key, item.Value);

                    metadataMaps.Add(PostMetadata.Message, ValueTypes.String);
                    metadataMaps.Add(PostMetadata.Owner, ValueTypes.Guid);
                }

                return metadataMaps;
            }
        }
        static Dictionary<int, ValueTypes> metadataMaps = new Dictionary<int, ValueTypes>();

        #endregion
    }

    public static class PostMetadata
    {
        public const int Message = EntityMetadata.Last + 1;
        public const int Owner = Message + 1;

        public const int Last = Owner;
    }
}
