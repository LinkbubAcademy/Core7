﻿using Common.Lib.Authentication;
using Common.Lib.Core.Context;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core
{
    public partial class WorkflowEntity : Entity
    {
        #region Properties

        public string StatusCode { get; set; } = string.Empty;

        #endregion

        #region Save

        public override async Task<ISaveResult<T>> SaveAsync<T>(AuthInfo? info = null, ITraceInfo? trace = null)
        {
            return await base.SaveAsync<T>(info, trace);
        }

        #endregion
    }

    public partial class WorkflowEntity 
    {
        #region Metadata Maps

        public static new Dictionary<int, ValueTypes> MetadataMaps 
        {
            get 
            {
                if (metadataMaps.Count == 0) 
                {
                    metadataMaps = new Dictionary<int, ValueTypes>();

                    foreach (var item in Entity.MetadataMaps)
                        metadataMaps.Add(item.Key, item.Value);
                }

                return metadataMaps;
            }
        }
        static Dictionary<int, ValueTypes> metadataMaps = new Dictionary<int, ValueTypes>();

        #endregion
    }


    public static class WorkflowEntityMetadata 
    {
        public const int Last = EntityMetadata.Last;
    }
}
