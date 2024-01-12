using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Infrastructure;
using Common.Lib.Services.Protobuf;
using Grpc.Core;
using System;

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
            try
            {
                var entity = ContextFactory.ReconstructEntity(paramsCarrier.EntityInfo);
                entity.Id = paramsCarrier.EntityInfo.EntityId;

                var sr1 = entity.SaveAction != null ?
                                await entity.SaveAction() :
                                new Infrastructure.Actions.SaveResult<Entity>() { IsSuccess = false, Message = "Save Action not added" };


                if (sr1.IsSuccess)
                {
                    var logManager = ContextFactory.Resolve<ILogManager>();
                    if (logManager != null)
                    {
                        await logManager.RegisterChangesAsync(paramsCarrier.EntityInfo, "system");
                    }
                }

                return await Task.FromResult(new SaveResult(sr1));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(new SaveResult(false, ex.Message));
            }
        }

        public override async Task<SaveResult> RequestUpdateEntity(SaveEntityParamsCarrier paramsCarrier, ServerCallContext context)
        {
            var qre = await ContextFactory.ReconstructAndUpdateEntity(paramsCarrier.EntityInfo);

            if (qre.IsSuccess)
            {
                var entity = qre.Value;
                entity.IsNew = false;
                entity.Id = paramsCarrier.EntityInfo.EntityId;

                var sr1 = entity.SaveAction != null ?
                                await entity.SaveAction() :
                                new Infrastructure.Actions.SaveResult<Entity>() { IsSuccess = false, Message = "Save Action not added" };
                
                if (sr1.IsSuccess)
                {
                    var logManager = ContextFactory.Resolve<ILogManager>();
                    if (logManager != null)
                    {
                        await logManager.RegisterChangesAsync(paramsCarrier.EntityInfo, "system");
                    }
                }

                return new SaveResult(sr1);
            }
            else
            {
                return await RequestAddNewEntity(paramsCarrier, context);
            }
        }

        public override async Task<DeleteResult> RequestDeleteEntity(DeleteEntityParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.EntityModelType);
            var qrEntity = await repo.FindAsync(paramsCarrier.EntityId);

            var entity = qrEntity.Value;
            if (entity == null)
                return new DeleteResult()
                {
                    IsSuccess = true,
                    Message = "Entity was not found"
                };

            entity.ContextFactory = ContextFactory;

            var dr1 = entity.DeleteAction != null ?
                            await entity.DeleteAction() :
                            new Infrastructure.Actions.DeleteResult() { IsSuccess = false, Message = "Save Action not added" };

            return await Task.FromResult(new DeleteResult(dr1));
        }

        public override async Task<ProcessActionResult> RequestParametricAction(ParametricActionParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var qrEntity = await repo.FindAsync(paramsCarrier.EntityId);

            if (qrEntity.IsSuccess)
            {
                var ent = qrEntity.Value;

                if (ent == null)
                    return new ProcessActionResult() { IsSuccess = false, Message = "Entity not found in repo" };
                ent.ContextFactory = ContextFactory;

                var paramActionId = paramsCarrier.RepositoryType + "." + paramsCarrier.ParametricActionName;
                var values = EntityMetadata.DeserializeParamActionValues(paramActionId, paramsCarrier.SerializedValues);

                var uow = ContextFactory.Resolve<IUnitOfWork>();
                var aResult = await ent.ProcessActionAsync(paramActionId, values, uow);

                ent.ContextFactory = null;

                if (aResult == null)
                    return await Task.FromResult(new ProcessActionResult() { IsSuccess = false, Message="action not registered" });
                if (aResult.IsSuccess)
                {
                    var logManager = ContextFactory.Resolve<ILogManager>();
                    if (logManager != null)
                    {
                        foreach (var action in uow.UowActions.Where(x => x.ActionInfoType == ActionInfoTypes.Save))
                            await logManager.RegisterChangesAsync(action.Change, "system");
                    }                   


                    await uow.CommitAsync();
                    return await Task.FromResult(new ProcessActionResult(aResult));
                }

            }
            return await Task.FromResult(new ProcessActionResult() { IsSuccess = false });
        }

        public override async Task<ProcessActionResult> RequestServiceAction(ServiceActionParamsCarrier paramsCarrier, ServerCallContext context)
        {
            var service = ContextFactory.GetBusinessService(paramsCarrier.ServiceType);
            var uow = ContextFactory.Resolve<IUnitOfWork>();

            var pa1 = await service.CallMethodAsync(paramsCarrier.ServiceActionName, paramsCarrier.SParams.ToArray(), uow);

            if (pa1.IsSuccess)
            {
                await uow.CommitAsync();
                return await Task.FromResult(new ProcessActionResult(pa1));
            }

            return await Task.FromResult(new ProcessActionResult() { IsSuccess = false });
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

        public override async Task<UnitOfWorkResult> RequestUnityOfWorkOperations(UnitOfWorkParamsCarrier paramsCarrier, ServerCallContext context)
        {
            using var uow = ContextFactory.Resolve<IUnitOfWork>();
            var result = await uow.CommitAsync(paramsCarrier.UowActions);

            if(result.IsSuccess)
            {
                var logManager = ContextFactory.Resolve<ILogManager>();
                if (logManager != null)
                {
                    foreach (var action in uow.UowActions.Where(x => x.ActionInfoType == ActionInfoTypes.Save))
                        await logManager.RegisterChangesAsync(action.Change, "system");
                }
            }

            return await Task.FromResult(new UnitOfWorkResult() { IsSuccess = result.IsSuccess, Message = result.Message });
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
