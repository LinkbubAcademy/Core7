using Common.Lib.Core.Context;
using Common.Lib.Core;
using Common.Lib.Core.Tracking;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Services.ParamsCarriers
{
    public interface IParamsCarrierFactory
    {
        ISaveEntityParamsCarrier CreateSaveEntityParams(Guid userId, string userToken, DateTime actionTime, IEntityInfo entityInfo);
        IDeleteEntityParamsCarrier CreateDeleteEntityParams(Guid userId, string userToken, DateTime actionTime, Guid entityId, string entityModelType);

        IQueryRepositoryParamsCarrier CreateQueryRepositoryParams(
                                            Guid userId,
                                            string userToken,
                                            DateTime actionTime,
                                            string repoTypeName,
                                            List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations,
                                            int nestingLevel);

        IParametricActionParamsCarrier CreateParametricActionParams(
                                                Guid userId,
                                                string userToken,
                                                DateTime actionTime,
                                                string repoTypeName,
                                                Guid entityId,
                                                string paramActionName,
                                                object[] values);

        IUnitOfWorkParamsCarrier CreateUnitOfWorkParams(Guid userId,
                                                            string userToken,
                                                            DateTime actionTime,
                                                            IEnumerable<IUoWActInfo> actions);
    }
}
