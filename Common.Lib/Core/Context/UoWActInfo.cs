using System;
using Common.Lib.Core.Tracking;

namespace Common.Lib.Core.Context
{
    public enum ActionInfoTypes
    {
        Save,
        Delete
    }

    public interface IUoWActInfo
    {
        IEntityInfo Change { get; set; }

        ActionInfoTypes ActionInfoType { get; set; }
    }

    public class UoWActInfo : IUoWActInfo
    {
        public IEntityInfo Change { get; set; }

        public ActionInfoTypes ActionInfoType { get; set; }
    }
}
