using Common.Lib.Core.Context;
using Common.Lib.Core;
using Common.Lib.Core.Tracking;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Services.ParamsCarriers
{
    public interface IParamsCarrierFactory
    {
        ISaveEntityParamsCarrier CreateSaveEntityParams(Guid userId, string userToken, DateTime actionTime, IEntityInfo entityInfo);

        IQueryRepositoryParamsCarrier CreateQueryRepositoryParams(
                                            Guid userId,
                                            string userToken,
                                            DateTime actionTime,
                                            string repoTypeName,
                                            List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations,
                                            int nestingLevel);
    }
}
