namespace Common.Lib.Services.ParamsCarriers
{    
    public interface IDeleteEntityParamsCarrier : IParamsCarrierInfo
    {
        Guid EntityId { get; set; }
        
        string EntityModelType { get; set; }
    }
}
