using Common.Lib.Core.Tracking;

namespace Common.Lib.Services.ParamsCarriers
{    
    public interface ISaveEntityParamsCarrier : IParamsCarrierInfo
    {
        IEntityInfo EntityInfo { get; set; }
    }
}
