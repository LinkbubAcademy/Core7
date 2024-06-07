using Common.Lib.Authentication;
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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                return new SaveResult()
                {
                    IsSuccess = false,
                    IsMaintenanceModeOn = true,
                    Message = ServerGlobals.MaintenanceModeMessage
                };
            }

            try
            {
                var entity = ContextFactory.ReconstructEntity(paramsCarrier.EntityInfo);
                entity.Id = paramsCarrier.EntityInfo.EntityId;

                var sr1 = entity.SaveAction != null ?
                                await entity.SaveAction(new AuthInfo(paramsCarrier), paramsCarrier.ServiceInfo.TraceInfo) :
                                new Infrastructure.Actions.SaveResult<Entity>() { IsSuccess = false, Message = "Save Action not added" };

                if (sr1.IsSuccess)
                {
                    var logManager = ContextFactory.Resolve<ILogManager>();
                    if (logManager != null)
                    {
                        await logManager.RegisterChangesAsync(paramsCarrier.EntityInfo, paramsCarrier.UserToken);
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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                return new SaveResult()
                {
                    IsSuccess = false,
                    IsMaintenanceModeOn = true,
                    Message = ServerGlobals.MaintenanceModeMessage
                };
            }

            var qre = await ContextFactory.ReconstructAndUpdateEntity(paramsCarrier.EntityInfo);

            if (qre.IsSuccess)
            {
                var entity = qre.Value;
                entity.IsNew = false;
                entity.Id = paramsCarrier.EntityInfo.EntityId;

                var sr1 = entity.SaveAction != null ?
                                await entity.SaveAction(new AuthInfo(paramsCarrier), paramsCarrier.ServiceInfo.TraceInfo) :
                                new Infrastructure.Actions.SaveResult<Entity>() { IsSuccess = false, Message = "Save Action not added" };
                
                if (sr1.IsSuccess)
                {
                    var logManager = ContextFactory.Resolve<ILogManager>();
                    if (logManager != null)
                    {
                        await logManager.RegisterChangesAsync(paramsCarrier.EntityInfo, paramsCarrier.UserToken);
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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                return new DeleteResult()
                {
                    IsSuccess = false,
                    IsMaintenanceModeOn = true,
                    Message = ServerGlobals.MaintenanceModeMessage
                };
            }

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
                            await entity.DeleteAction(new AuthInfo(paramsCarrier), paramsCarrier.TraceInfo) :
                            new Infrastructure.Actions.DeleteResult() { IsSuccess = false, Message = "Save Action not added" };

            return await Task.FromResult(new DeleteResult(dr1));
        }

        public override async Task<ProcessActionResult> RequestParametricAction(ParametricActionParamsCarrier paramsCarrier, ServerCallContext context)
        {
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                return new ProcessActionResult()
                {
                    IsSuccess = false,
                    IsMaintenanceModeOn = true,
                    Message = ServerGlobals.MaintenanceModeMessage
                };
            }

            var authInfo = new AuthInfo(paramsCarrier);

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
                    var uowLog = ContextFactory.Resolve<IUnitOfWork>();
                    if (logManager != null)
                    {
                        foreach (var action in uow.UowActions.Where(x => x.ActionInfoType == ActionInfoTypes.Save).ToList())
                            logManager.RegisterChanges(uowLog, action.Change, "system");
                    }

                    var sr1 = await uow.CommitAsync(info: authInfo);
                    if(sr1.IsSuccess)
                    {
                        var sr2 = await uowLog.CommitAsync();
                    }
                    
                    return await Task.FromResult(new ProcessActionResult(aResult));
                }

            }
            return await Task.FromResult(new ProcessActionResult() { IsSuccess = false });
        }

        public override async Task<ProcessActionResult> RequestServiceAction(ServiceActionParamsCarrier paramsCarrier, ServerCallContext context)
        {
            if (ServerGlobals.IsMaintenanceModeOn && 
                !ServerGlobals.ServiceActionAllowedDuringMaintenance.Contains(paramsCarrier.ServiceActionName))
            {
                return new ProcessActionResult()
                {
                    IsSuccess = false,
                    IsMaintenanceModeOn = true,
                    Message = ServerGlobals.MaintenanceModeMessage
                };
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var output = new QueryEntityResult();

                output.ActionResult.IsSuccess = false;
                output.ActionResult.IsMaintenanceModeOn = true;
                output.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return output;
            }

            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetEntityRequest(paramsCarrier.Operations);

            return new QueryEntityResult(result);
        }

        public override async Task<QueryEntitiesResult> QueryRepositoryForEntities(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var output = new QueryEntitiesResult();

                output.ActionResult.IsSuccess = false;
                output.ActionResult.IsMaintenanceModeOn = true;
                output.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return output;
            }

            try
            {
                using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
                var result = await repo
                                    .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                    .ExecuteGetEntitiesRequest(paramsCarrier.Operations);

                return new QueryEntitiesResult(result);
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        public override async Task<UnitOfWorkResult> RequestUnityOfWorkOperations(UnitOfWorkParamsCarrier paramsCarrier, ServerCallContext context)
        {
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var output = new UnitOfWorkResult
                {
                    IsSuccess = false,
                    IsMaintenanceModeOn = true,
                    Message = ServerGlobals.MaintenanceModeMessage
                };

                return output;
            }

            using var uow = ContextFactory.Resolve<IUnitOfWork>();

            var authInfo = new AuthInfo(paramsCarrier);

            var sr1 = await uow.CommitAsync(paramsCarrier.UowActions, authInfo, paramsCarrier.ServiceInfo.TraceInfo);

            if (Log.IsLogActive && paramsCarrier.ServiceInfo.TraceInfo != null)
            {
                Log.AddTrace(paramsCarrier.ServiceInfo.TraceInfo);
            }

            if (sr1.IsSuccess)
            {
                var logManager = ContextFactory.Resolve<ILogManager>();
                using var uowLog = ContextFactory.Resolve<IUnitOfWork>();

                if (logManager != null)
                {
                    foreach (var action in uow.UowActions.Where(x => x.ActionInfoType == ActionInfoTypes.Save))
                        logManager.RegisterChanges(uowLog, action.Change, "system");

                    await uowLog.CommitAsync(null, authInfo);
                }
            }

            return await Task.FromResult(new UnitOfWorkResult() { IsSuccess = sr1.IsSuccess, Message = sr1.Message, Errors = sr1.Errors });
        }

        #region Get Value

        public override async Task<QueryValueResult> QueryRepositoryForBool(QueryRepositoryParamsCarrier paramsCarrier, ServerCallContext context)
        {
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValueResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValueResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValueResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValueResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValueResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValueResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

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
            if (ServerGlobals.IsMaintenanceModeOn)
            {
                var o = new QueryValuesResult();

                o.ActionResult.IsSuccess = false;
                o.ActionResult.IsMaintenanceModeOn = true;
                o.ActionResult.Message = ServerGlobals.MaintenanceModeMessage;

                return o;
            }

            using var repo = ContextFactory.GetRepository(paramsCarrier.RepositoryType);
            var result = await repo
                                .DeclareChildrenPolicy(paramsCarrier.NestingLevel)
                                .ExecuteGetValuesRequest(paramsCarrier.Operations);

            return new QueryValuesResult(result);
        }
    }
}
