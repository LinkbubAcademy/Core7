using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Common.Lib.Core.Tracking;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public class ParamsCarrierFactory : IParamsCarrierFactory
    {
        public IQueryRepositoryParamsCarrier CreateQueryRepositoryParams(
                                                Guid userId, 
                                                string userToken, 
                                                DateTime actionTime, 
                                                string repoTypeName, 
                                                List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations, 
                                                int nestingLevel)
        {
            return new QueryRepositoryParamsCarrier(userId, userToken, actionTime, repoTypeName, operations, nestingLevel);

        }

        public ISaveEntityParamsCarrier CreateSaveEntityParams(Guid userId, 
                                                                string userToken, 
                                                                DateTime actionTime, 
                                                                IEntityInfo entityInfo)
        {
            return new SaveEntityParamsCarrier(userId, userToken, actionTime, entityInfo);
        }
    }
}
