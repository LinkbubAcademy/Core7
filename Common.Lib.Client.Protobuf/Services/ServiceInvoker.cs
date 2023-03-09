using Common.Lib.Core;
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

        public async Task<IActionResult> AddNewEntityRequestAsync(ISaveEntityParamsCarrier paramsCarrier)
        {
            if (paramsCarrier is not SaveEntityParamsCarrier)
            {
                throw new ArgumentNullException($"ISaveEntityParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            return await Channel.RequestAddNewEntityAsync((SaveEntityParamsCarrier)paramsCarrier);
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

            return new QueryResult<TEntity>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = ContextFactory.ReconstructEntity<TEntity>(result.SValue),
                ReferencedEntities = result
                            .SReferencedEntities
                            .ToDictionary(x => Guid.Parse(x.Key),
                                            x => ContextFactory.ReconstructEntity(x.Value))
            };
        }

        public async Task<QueryResult<List<TEntity>>> QueryRepositoryForEntities<TEntity>(IQueryRepositoryParamsCarrier paramsCarrier) where TEntity : Entity, new()
        {
            if (paramsCarrier is not QueryRepositoryParamsCarrier)
            {
                throw new ArgumentNullException($"IQueryRepositoryParamsCarrier paramsCarrier must come " +
                    $"from the proper factory: " +
                    $"(Common.Lib.Services.Protobuf.ParamsCarrierFactory");
            }

            var result = await Channel.QueryRepositoryForEntitiesAsync((QueryRepositoryParamsCarrier)paramsCarrier);

            return new QueryResult<List<TEntity>>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = result.SValue.Select(x => ContextFactory.ReconstructEntity<TEntity>(x)).ToList(),
                ReferencedEntities = result
                            .SReferencedEntities
                            .ToDictionary(x => Guid.Parse(x.Key), 
                                            x => ContextFactory.ReconstructEntity(x.Value))
            };
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

            var result = await Channel.QueryRepositoryForBoolAsync((QueryRepositoryParamsCarrier)paramsCarrier);

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
            
            var parseFunc = EntityInfo.GetParseFunc<TValue>();

            return new QueryResult<List<TValue>>()
            {
                IsSuccess = result.ActionResult.IsSuccess,
                Message = result.ActionResult.Message,
                Value = result.SValue.Select(x => parseFunc(x)).ToList(),
            };
        }        
    }
}
