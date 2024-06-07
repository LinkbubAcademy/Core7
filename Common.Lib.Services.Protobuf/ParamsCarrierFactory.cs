using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public class ParamsCarrierFactory : IParamsCarrierFactory
    {
        public ITraceInfo? TraceInfo { get; set; }
        public string UserToken { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public IQueryRepositoryParamsCarrier CreateQueryRepositoryParams(
                                                Guid userId, 
                                                string userToken, 
                                                string userEmail,
                                                ITraceInfo? traceInfo,
                                                DateTime actionTime, 
                                                string repoTypeName, 
                                                List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations, 
                                                int nestingLevel)
        {
            UserToken = userToken;
            UserEmail = userEmail;
            TraceInfo = traceInfo;

            return new QueryRepositoryParamsCarrier(userId, userToken, userEmail, traceInfo, actionTime, repoTypeName, operations, nestingLevel);

        }

        public ISaveEntityParamsCarrier CreateSaveEntityParams(Guid userId, 
                                                                string userToken,
                                                                string userEmail,
                                                                ITraceInfo? traceInfo,
                                                                DateTime actionTime, 
                                                                IEntityInfo entityInfo)
        {
            UserToken = userToken;
            UserEmail = userEmail;
            TraceInfo = traceInfo;

            return new SaveEntityParamsCarrier(userId, userToken, userEmail, traceInfo, actionTime, entityInfo);
        }

        public IDeleteEntityParamsCarrier CreateDeleteEntityParams(Guid userId,
                                                                string userToken,
                                                                string userEmail,
                                                                ITraceInfo? traceInfo,
                                                                DateTime actionTime,
                                                                Guid entityId,
                                                                string entityModelType)
        {
            UserToken = userToken;
            UserEmail = userEmail;
            TraceInfo = traceInfo;

            return new DeleteEntityParamsCarrier(userId, userToken, userEmail, traceInfo, actionTime, entityId, entityModelType);
        }


        public IParametricActionParamsCarrier CreateParametricActionParams(
                                                Guid userId,
                                                string userToken,
                                                string userEmail,
                                                ITraceInfo? traceInfo,
                                                DateTime actionTime,
                                                string repoTypeName,
                                                Guid entityId,
                                                string paramActionName,
                                                object[] values)
        {
            UserToken = userToken;
            UserEmail = userEmail;
            TraceInfo = traceInfo;

            var sValues = EntityMetadata.SerializeParamActionValues(repoTypeName + "." + paramActionName, values);

            return new ParametricActionParamsCarrier(userId,
                                                userToken,
                                                userEmail,
                                                traceInfo,
                                                actionTime,
                                                repoTypeName,
                                                entityId,
                                                paramActionName,
                                                sValues);
        }

        public IServiceActionParamsCarrier CreateServiceActionParams(
                                                Guid userId,
                                                string userToken,
                                                string userEmail,
                                                ITraceInfo? traceInfo,
                                                DateTime actionTime,
                                                string serviceTypeName,
                                                string serviceActionName,
                                                object[] values)
        {
            UserToken = userToken;
            UserEmail = userEmail;
            TraceInfo = traceInfo;

            var sValues = values.Select(x=>x.ToString()).ToArray(); 
            return new ServiceActionParamsCarrier(userId,
                                                userToken, 
                                                userEmail,
                                                traceInfo,
                                                actionTime,
                                                serviceTypeName,
                                                serviceActionName,
                                                sValues);
        }

        public IUnitOfWorkParamsCarrier CreateUnitOfWorkParams(Guid userId,
                                                            string userToken,
                                                            string userEmail,
                                                            ITraceInfo? traceInfo,
                                                            DateTime actionTime,
                                                            IEnumerable<IUoWActInfo> actions)
        {
            UserToken = userToken;
            UserEmail = userEmail;
            TraceInfo = traceInfo;

            return new UnitOfWorkParamsCarrier(userId, userToken, userEmail, traceInfo, actionTime, actions);

        }
    }
}
