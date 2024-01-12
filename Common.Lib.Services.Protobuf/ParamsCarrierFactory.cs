using Common.Lib.Authentication;
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

        public IDeleteEntityParamsCarrier CreateDeleteEntityParams(Guid userId,
                                                                string userToken,
                                                                DateTime actionTime,
                                                                Guid entityId,
                                                                string entityModelType)
        {
            return new DeleteEntityParamsCarrier(userId, userToken, actionTime, entityId, entityModelType);
        }


        public IParametricActionParamsCarrier CreateParametricActionParams(
                                                Guid userId,
                                                string userToken,
                                                DateTime actionTime,
                                                string repoTypeName,
                                                Guid entityId,
                                                string paramActionName,
                                                object[] values)
        {

            var sValues = EntityMetadata.SerializeParamActionValues(repoTypeName + "." + paramActionName, values);

            return new ParametricActionParamsCarrier(userId,
                                                userToken,
                                                actionTime,
                                                repoTypeName,
                                                entityId,
                                                paramActionName,
                                                sValues);
        }

        public IServiceActionParamsCarrier CreateServiceActionParams(
                                                Guid userId,
                                                string userToken,
                                                DateTime actionTime,
                                                string serviceTypeName,
                                                string serviceActionName,
                                                object[] values)
        {

            //var sValues = EntityMetadata.SerializeParamActionValues(repoTypeName + "." + paramActionName, values);
            var sValues = values.Select(x=>x.ToString()).ToArray(); 
            return new ServiceActionParamsCarrier(userId,
                                                userToken,
                                                actionTime,
                                                serviceTypeName,
                                                serviceActionName,
                                                sValues);
        }

        public IUnitOfWorkParamsCarrier CreateUnitOfWorkParams(Guid userId,
                                                            string userToken,
                                                            DateTime actionTime,
                                                            IEnumerable<IUoWActInfo> actions)
        {
            return new UnitOfWorkParamsCarrier(userId, userToken, actionTime, actions);

        }
    }
}
