using Common.Lib.Core.Context;
using Common.Lib.Core;
using Common.Lib.Core.Tracking;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure;

namespace Common.Lib.Services.ParamsCarriers
{
    public interface IParamsCarrierFactory
    {
        string UserToken { get; set; }
        string UserEmail { get; set; }

        ISaveEntityParamsCarrier CreateSaveEntityParams(Guid userId, string userToken, string userEmail, ITraceInfo? traceInfo, DateTime actionTime, IEntityInfo entityInfo);
        IDeleteEntityParamsCarrier CreateDeleteEntityParams(Guid userId, string userToken, string userEmail, ITraceInfo? traceInfo, DateTime actionTime, Guid entityId, string entityModelType);

        IQueryRepositoryParamsCarrier CreateQueryRepositoryParams(
                                            Guid userId,
                                            string userToken,
                                            string userEmail, 
                                            ITraceInfo? traceInfo,
                                            DateTime actionTime,
                                            string repoTypeName,
                                            List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations,
                                            int nestingLevel);

        IParametricActionParamsCarrier CreateParametricActionParams(
                                                Guid userId,
                                                string userToken,
                                                string userEmail, 
                                                ITraceInfo? traceInfo,
                                                DateTime actionTime,
                                                string repoTypeName,
                                                Guid entityId,
                                                string paramActionName,
                                                object[] values);
        
        IServiceActionParamsCarrier CreateServiceActionParams(
                                                Guid userId,
                                                string userToken,
                                                string userEmail,
                                                ITraceInfo? traceInfo,
                                                DateTime actionTime,
                                                string serviceTypeName,
                                                string serviceActionName,
                                                object[] values);



        IUnitOfWorkParamsCarrier CreateUnitOfWorkParams(Guid userId,
                                                            string userToken,
                                                            string userEmail,
                                                            ITraceInfo? traceInfo,
                                                            DateTime actionTime,
                                                            IEnumerable<IUoWActInfo> actions);
    }
}
