﻿using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using System.Reflection;
using System.Xml.Linq;

namespace Common.Lib.Authentication
{
    public partial class User : Entity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int AccessLevel { get; set; }

        public User() 
        {
             
        }

        #region Clone

        /// <summary>
        /// Retrieves a copy marked as IsNew false and with the original entity
        /// to be compared to get changes
        /// </summary>
        /// <returns>User</returns>
        public new User Clone(Dictionary<Guid, Entity> alreadyCloned = null)
        {
            alreadyCloned ??= [];
            return Clone<User>(alreadyCloned);
        }

        public override T Clone<T>(Dictionary<Guid, Entity> alreadyCloned)
        {
            if (base.Clone<T>(alreadyCloned) is User output && output is T result)
            {
                output.Email = Email;
                output.Password = Password;
                output.AccessLevel = AccessLevel;

                return result;
            }

            throw new ArgumentException($"Type {typeof(T).FullName} does not derived from User");
        }

        public override void IncludeChildren(Dictionary<Guid, Entity> refEnts, int nestingLevel)
        {
            base.IncludeChildren(refEnts, nestingLevel);
        }

        public override void AssignChildren(QueryResult qr)
        {
            base.AssignChildren(qr);
        }

        #endregion

        #region Changes

        public override EntityInfo GetChanges()
        {
            var output = base.GetChanges();

            if (output.IsNew || Origin == null)
            {
                output.AddChange(UserMetadata.Email, Email);
                output.AddChange(UserMetadata.Password, Password);
                output.AddChange(UserMetadata.AccessLevel, AccessLevel);
            }
            else
            {
                if (Origin is not User origin || Origin == null)
                    throw new InvalidOperationException("Clone must be called in order to modify an entity");

                if (origin.Email != Email)
                    output.AddChange(UserMetadata.Email, Email);

                if (origin.Password != Password)
                    output.AddChange(UserMetadata.Password, Password);

                if (origin.AccessLevel != AccessLevel)
                    output.AddChange(UserMetadata.AccessLevel, AccessLevel);

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
                    case UserMetadata.Email:
                        Email = change.GetValueAsString();
                        break;
                    case UserMetadata.Password:
                        Password = change.GetValueAsString();
                        break;
                    case UserMetadata.AccessLevel:
                        AccessLevel = change.GetValueAsInt();
                        break;
                    default:
                        return remainingChanges.Where(x => x.MetadataId > UserMetadata.Last).ToList();
                }
            }
            return remainingChanges.Where(x => x.MetadataId > UserMetadata.Last).ToList();
        }

        #endregion

        #region Save & Delete

        public virtual async Task<ISaveResult<User>> SaveAsync()
        {
            return await this.SaveAsync<User>();
        }

        public override async Task<ISaveResult<T>> SaveAsync<T>(AuthInfo? info = null, ITraceInfo? trace = null) 
        {
            return await base.SaveAsync<T>(info, trace);
        }        

        public virtual async Task<IDeleteResult> DeleteAsync(AuthInfo? info = null, ITraceInfo? trace = null)
        {
            return await this.DeleteAsync<User>(info, trace);
        }

        public override async Task<IDeleteResult> DeleteAsync<T>(AuthInfo? info = null, ITraceInfo? trace = null)
        {
            return await base.DeleteAsync<T>(info, trace);
        }

        #endregion
    }

    public partial class User
    {
        #region metadata maps

        public static new Dictionary<int, ValueTypes> MetadataMaps
        {
            get
            {
                if (metadataMaps.Count == 0)
                {
                    metadataMaps = new Dictionary<int, ValueTypes>();

                    foreach(var item in Entity.MetadataMaps)
                        metadataMaps.Add(item.Key, item.Value);

                    metadataMaps.Add(UserMetadata.Email, ValueTypes.String);
                    metadataMaps.Add(UserMetadata.Password, ValueTypes.String);                        
                    metadataMaps.Add(UserMetadata.AccessLevel, ValueTypes.Int);                        
                }

                return metadataMaps;
            }
        }
        static Dictionary<int, ValueTypes> metadataMaps = new Dictionary<int, ValueTypes>();

        public static new Dictionary<int, string> MetadataNames
        {
            get
            {
                if (metadataNames.Count == 0)
                {
                    metadataNames = [];

                    foreach (var item in Entity.MetadataNames)
                        metadataNames.Add(item.Key, item.Value);

                    metadataNames.Add(UserMetadata.Email, "UserEmail");
                    metadataNames.Add(UserMetadata.Password, "UserPassword");
                    metadataNames.Add(UserMetadata.AccessLevel, "UserAccessLevel");
                }

                return metadataNames;
            }
        }
        static Dictionary<int, string> metadataNames = [];

        #endregion
    }


    public static class UserMetadata
    {
        public const int Email = EntityMetadata.Last + 1;
        public const int Password = Email + 1;
        public const int AccessLevel = Password + 1;

        public const int Last = AccessLevel;

        public static void RegisterParametricActions()
        {

        }
    }
}
