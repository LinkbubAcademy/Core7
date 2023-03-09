using Common.Lib.Core.Expressions;

namespace Common.Lib.Services.ParamsCarriers
{    
    public interface IQueryRepositoryParamsCarrier : IParamsCarrierInfo
    {
        string RepositoryType { get; set; }
        int NestingLevel { get; set; }
        IEnumerable<IQueryOperationInfo> Operations { get; }
    }
}
