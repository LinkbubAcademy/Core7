using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Services.ParamsCarriers
{    
    public interface IUnitOfWorkParamsCarrier : IParamsCarrierInfo
    {
        IEnumerable<IUoWActInfo> UowActions { get; }
    }
}
