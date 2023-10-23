using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;

namespace Common.Lib.Services.Protobuf
{
    public partial class UowActionInfo : IUoWActInfo
    {
        public UowActionInfo(IUoWActInfo info)
        {
            Change = info.Change;
            ActionInfoType = info.ActionInfoType;
        }

        public ActionInfoTypes ActionInfoType
        {
            get
            {
                return (ActionInfoTypes)NumActionInfoType;
            }
            set
            {
                NumActionInfoType = (int)value;
            }
        }
        public IEntityInfo Change
        {
            get
            {
                return SChange;
            }
            set
            {
                SChange = new EntityInfo(value);
            }
        }
    }
}
