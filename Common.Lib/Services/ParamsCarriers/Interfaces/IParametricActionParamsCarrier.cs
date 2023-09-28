namespace Common.Lib.Services.ParamsCarriers
{    
    public interface IParametricActionParamsCarrier : IParamsCarrierInfo
    {
        string RepositoryType { get; set; }
        Guid EntityId { get; set; }
        string ParametricActionName { get; set; }
        string[] SerializedValues { get; set; }
    }
}
