namespace Common.Lib.Services.ParamsCarriers
{    
    public interface IServiceActionParamsCarrier : IParamsCarrierInfo
    {
        string ServiceType { get; set; }
        string ServiceActionName { get; set; }
        string[] SerializedValues { get; set; }
    }
}
