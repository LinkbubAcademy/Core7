﻿using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Services.Protobuf;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System;

namespace Common.Lib.Client.Services
{
    public class ServiceInvoker : IServiceInvoker
    {
        public IContextFactory ContextFactory { get; set; }
        public string BaseUri { get; set; } = string.Empty;
        CoreServices.CoreServicesClient Channel { get; set; }

        public ServiceInvoker(string uri, IContextFactory contextFactory)
        {
            BaseUri = uri;
            ContextFactory = contextFactory;

            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var channel = GrpcChannel.ForAddress(BaseUri, new GrpcChannelOptions { HttpClient = httpClient });
            Channel = new CoreServices.CoreServicesClient(channel);
        }

        void ActivateMaintenanceMode(string msg)
        {
            ClientGlobals.IsMaintenanceModeOn = true;
            ClientGlobals.MaintenanceModeMessage = msg;
            ClientGlobals.SetViewToMaintenanceModeOnOff?.Invoke();            
        }

        public async Task<ISaveResult<TEntity>> AddNewEntityRequestAsync<TEntity>(ISaveEntityParamsCarrier paramsCarrier) where TEntity : Entity, new()
        {
            if (paramsCarrier is not SaveEntityParamsCarrier)
            {
                throw new ArgumentNullException($"ISaveEntityParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.RequestAddNewEntityAsync((SaveEntityParamsCarrier)paramsCarrier);

            if (result.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.Message);

            var output = new SaveResult<TEntity>
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Value = ContextFactory.ReconstructEntity<TEntity>(result.SValue),
                //ReferencedEntities = result
                //            .SReferencedEntities
                //            .ToDictionary(x => Guid.Parse(x.Key),
                //                            x => ContextFactory.ReconstructEntity(x.Value))
            };

            if (result.Errors.Count() > 0)
                output.AddErrors(result.Errors);
            
            return output;
        }

        public async Task<ISaveResult<TEntity>> UpdateEntityRequestAsync<TEntity>(ISaveEntityParamsCarrier paramsCarrier) where TEntity : Entity, new()
        {
            if (paramsCarrier is not SaveEntityParamsCarrier)
            {
                throw new ArgumentNullException($"ISaveEntityParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.RequestUpdateEntityAsync((SaveEntityParamsCarrier)paramsCarrier);

            if (result.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.Message);

            var output = new SaveResult<TEntity>
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Value = ContextFactory.ReconstructEntity<TEntity>(result.SValue),
                //ReferencedEntities = result
                //            .SReferencedEntities
                //            .ToDictionary(x => Guid.Parse(x.Key),
                //                            x => ContextFactory.ReconstructEntity(x.Value))
            };

            if (result.Errors.Count() > 0)
                output.AddErrors(result.Errors);

            return output;
        }

        public async Task<IDeleteResult> DeleteEntityRequestAsync(IDeleteEntityParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not DeleteEntityParamsCarrier)
            {
                throw new ArgumentNullException($"IDeleteEntityParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var output = await Channel.RequestDeleteEntityAsync((DeleteEntityParamsCarrier)paramsCarrier);

            if (output.IsMaintenanceModeOn)
                ActivateMaintenanceMode(output.Message);

            return output;
        }

        public async Task<IProcessActionResult> RequestServiceActionAsync(IServiceActionParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not ServiceActionParamsCarrier)
            {
                throw new ArgumentNullException($"IParametricActionParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var output = await Channel.RequestServiceActionAsync((ServiceActionParamsCarrier)paramsCarrier,
                                                                    deadline: DateTime.UtcNow.AddSeconds(200));

            if (output.IsMaintenanceModeOn)
                ActivateMaintenanceMode(output.Message);

            Log.WriteLine("RequestServiceActionAsync.IsSuccess " + output.IsSuccess + " serializedValue: " + output.Serialized);
            return output;
        }

        public async Task<IProcessActionResult> RequestParametricActionAsync(IParametricActionParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not ParametricActionParamsCarrier)
            {
                throw new ArgumentNullException($"IParametricActionParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var output = await Channel.RequestParametricActionAsync((ParametricActionParamsCarrier)paramsCarrier);

            if (output.IsMaintenanceModeOn)
                ActivateMaintenanceMode(output.Message);

            Log.WriteLine("RequestParametricActionAsync.IsSuccess " + output.IsSuccess + " serializedValue: " + output.Serialized);
            return output;
        }

        public async Task<QueryResult<TEntity>> QueryRepositoryForEntity<TEntity>(IQueryRepositoryParamsCarrier paramsCarrier) where TEntity : Entity, new()
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForEntityAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            return new QueryResult<TEntity>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = result.ActionResult.IsSuccess && result.SValue != null ? ContextFactory.ReconstructEntity<TEntity>(result.SValue) : default,
                ReferencedEntities = result
                            .SReferencedEntities
                            .ToDictionary(x => Guid.Parse(x.Key),
                                            x => ContextFactory.ReconstructEntity(x.Value))
            };
        }

        public async Task<QueryResult<List<TEntity>>> QueryRepositoryForEntities<TEntity>(IQueryRepositoryParamsCarrier paramsCarrier) where TEntity : Entity, new()
        {
            Log.WriteLine("ServiceInvoker QueryRepositoryForEntities 1");

            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            Log.WriteLine("ServiceInvoker QueryRepositoryForEntities 2");
            try
            {
                var result = await Channel.QueryRepositoryForEntitiesAsync((QueryRepositoryParamsCarrier)paramsCarrier);

                if (result.ActionResult.IsMaintenanceModeOn)
                    ActivateMaintenanceMode(result.ActionResult.Message);

                Log.WriteLine("ServiceInvoker QueryRepositoryForEntities 3");
                var value = result.SValue.Select(x => ContextFactory.ReconstructEntity<TEntity>(x)).ToList();

                return new QueryResult<List<TEntity>>()
                {
                    IsSuccess = result.ActionResult.IsSuccess,
                    Message = result.ActionResult.Message,
                    Value = value,
                    ReferencedEntities = result
                                .SReferencedEntities
                                .ToDictionary(x => Guid.Parse(x.Key),
                                                x => ContextFactory.ReconstructEntity(x.Value))
                };
            }
            catch(Exception e1)
            {
                throw e1;
            }
        }
        
        #region Get Value
        public async Task<QueryResult<bool>> QueryRepositoryForBool(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForBoolAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<bool>();

            return new QueryResult<bool>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = parseFunc(result.SValue)
            };
        }


        public async Task<QueryResult<int>> QueryRepositoryForInt(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForIntAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<int>();

            return new QueryResult<int>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = parseFunc(result.SValue)
            };
        }

        public async Task<QueryResult<double>> QueryRepositoryForDouble(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForDoubleAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<double>();

            return new QueryResult<double>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = parseFunc(result.SValue)
            };
        }

        public async Task<QueryResult<string>> QueryRepositoryForString(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForDoubleAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<string>();

            return new QueryResult<string>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = parseFunc(result.SValue)
            };
        }

        public async Task<QueryResult<DateTime>> QueryRepositoryForDateTime(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForDateTimeAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<DateTime>();

            return new QueryResult<DateTime>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = parseFunc(result.SValue)
            };
        }


        public async Task<QueryResult<Guid>> QueryRepositoryForGuid(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForGuidAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<Guid>();

            return new QueryResult<Guid>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = parseFunc(result.SValue)
            };
        }

        #endregion

        public async Task<QueryResult<List<TValue>>> QueryRepositoryForValues<TValue>(IQueryRepositoryParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForValuesAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            if (result.ActionResult.IsMaintenanceModeOn)
                ActivateMaintenanceMode(result.ActionResult.Message);

            var parseFunc = EntityInfo.GetParseFunc<TValue>();

            return new QueryResult<List<TValue>>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = result.SValue.Select(x => parseFunc(x)).ToList(),
            };
        }

        public async Task<IActionResult> CommitUnitOfWorkAsync(IUnitOfWorkParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not UnitOfWorkParamsCarrier)
            {
                throw new ArgumentNullException($"IParametricActionParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            Log.WriteLine(msg: $"actions {paramsCarrier.UowActions.Count()}");
            var output = await Channel.RequestUnityOfWorkOperationsAsync((UnitOfWorkParamsCarrier)paramsCarrier);

            if (output.IsMaintenanceModeOn)
                ActivateMaintenanceMode(output.Message);

            return output;
        }
    }
}
