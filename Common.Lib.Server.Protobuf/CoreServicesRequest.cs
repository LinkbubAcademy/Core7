using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Services.Protobuf;
using Grpc.Core;

namespace Common.Lib.Server.Protobuf
{
    public class CoreServicesRequest : CoreServices.CoreServicesBase
    {
        IContextFactory ContextFactory { get; set; }

        public CoreServicesRequest(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        public override async Task<SaveResult> RequestAddNewEntity(SaveEntityParamsCarrier paramsCarrier, ServerCallContext context)
        {
            var entity = ContextFactory.ReconstructEntity(paramsCarrier.EntityInfo);
            entity.Id = paramsCarrier.EntityInfo.EntityId;

            var sr1 = entity.SaveAction != null ? 
                            await entity.SaveAction() : 
                            new Infrastructure.Actions.SaveResult() { IsSuccess = false, Message = "Save Action not added" };
            
            return await Task.FromResult(new SaveResult(sr1));
        }

        public override async Task<ActionResult> RequestParametricAction(ParametricActionParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var qrEntity = await repo.FindAsync(paramsCarrier.EntityId);

            if (qrEntity.IsSuccess)
            {
                var ent = qrEntity.Value;

                if (ent == null)
                    return new ActionResult() { IsSuccess = false, Message = "Entity not found in repo" };
                ent.ContextFactory = ContextFactory;

                var paramActionId = paramsCarrier.RepositoryType + "." + paramsCarrier.ParametricActionName;
                var values = EntityMetadata.DeserializeParamActionValues(paramActionId, paramsCarrier.SerializedValues);

                var aResult = await ent.ProcessActionAsync(paramActionId, values);

                ent.ContextFactory = null;

                if (aResult == null)
                    return await Task.FromResult(new ActionResult() { IsSuccess = false, Message="action not registered" });

                return await Task.FromResult(new ActionResult() { IsSuccess = aResult.IsSuccess });
            }
            return await Task.FromResult(new ActionResult() { IsSuccess =false });
        }

        public override async Task<QueryEntityResult> QueryRepositoryForEntity(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetEntityRequest(paramsCarrier.Operations);

            return new QueryEntityResult(result);
        }

        public override async Task<QueryEntitiesResult> QueryRepositoryForEntities(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetEntitiesRequest(paramsCarrier.Operations);

            return new QueryEntitiesResult(result);
        }

        #region Get Value

        public override async Task<QueryValueResult> QueryRepositoryForBool(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetBoolValueRequest(paramsCarrier.Operations);

            var output = new QueryValueResult();
            output.AssignResult(result);

            return output;
        }
        public override async Task<QueryValueResult> QueryRepositoryForInt(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetIntValueRequest(paramsCarrier.Operations);

            var output = new QueryValueResult();
            output.AssignResult(result);

            return output;
        }

        public override async Task<QueryValueResult> QueryRepositoryForDouble(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetDoubleValueRequest(paramsCarrier.Operations);

            var output = new QueryValueResult();
            output.AssignResult(result);

            return output;
        }


        public override async Task<QueryValueResult> QueryRepositoryForString(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetStringValueRequest(paramsCarrier.Operations);

            var output = new QueryValueResult();
            output.AssignResult(result);

            return output;
        }

        public override async Task<QueryValueResult> QueryRepositoryForDateTime(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetDateTimeValueRequest(paramsCarrier.Operations);

            var output = new QueryValueResult();
            output.AssignResult(result);

            return output;
        }

        public override async Task<QueryValueResult> QueryRepositoryForGuid(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetGuidValueRequest(paramsCarrier.Operations);

            var output = new QueryValueResult();
            output.AssignResult(result);

            return output;
        }

        #endregion

        public override async Task<QueryValuesResult> QueryRepositoryForValues(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetValuesRequest(paramsCarrier.Operations);

            return new QueryValuesResult(result);
        }
    }
}
