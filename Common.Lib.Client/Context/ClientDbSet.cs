using Common.Lib.Authentication;
using Common.Lib.Client;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services;
using Common.Lib.Services.ParamsCarriers;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Common.Lib.Core.Context
{
    public class ClientDbSet<T> : IDbSet<T> where T: Entity, new()
    {
        public int NestingLevel { get; set; } = 0;
        static Guid UserId { get; set; } = Guid.NewGuid(); //todo: implement user auth
        static string UserToken { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        IServiceInvoker ServiceInvoker { get; set; }
        IParamsCarrierFactory ParamsCarrierFactory { get; set; }

        public ClientDbSet(IServiceInvoker serviceInvoker, IParamsCarrierFactory paramsCarrierFactory)
        {
            ServiceInvoker = serviceInvoker;
            ParamsCarrierFactory = paramsCarrierFactory;
        }

        public async Task<ISaveResult<T>> AddAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            var paramsCarrier = ParamsCarrierFactory
                                .CreateSaveEntityParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: trace,
                                        actionTime: DateTime.Now,
                                        entityInfo: entity.GetChanges());

            var response = await ServiceInvoker.AddNewEntityRequestAsync<T>(paramsCarrier);

            return await Task.FromResult(response);
        }

        public async Task<IDeleteResult> DeleteAsync(Guid id, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            var output = new DeleteResult();
            var paramsCarrier = ParamsCarrierFactory
                                .CreateDeleteEntityParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: trace,
                                        actionTime: DateTime.Now,
                                        entityId: id,
                                        entityModelType: typeof(T).FullName);

            var response = await ServiceInvoker.DeleteEntityRequestAsync(paramsCarrier);
            output.IsSuccess = response.IsSuccess;
            output.Message = response.Message;

            return await Task.FromResult(output);
        }

        public async Task<ISaveResult<T>> UpdateAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            //var output = new SaveResult<T>();

            var paramsCarrier = ParamsCarrierFactory
                                .CreateSaveEntityParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: trace,
                                        actionTime: DateTime.Now,
                                        entityInfo: entity.GetChanges());

            var response = await ServiceInvoker.UpdateEntityRequestAsync<T>(paramsCarrier);
            //output.IsSuccess = response.IsSuccess;
            //output.Message = response.Message;

            return await Task.FromResult(response);
        }

        public async Task<QueryResult<T>> FindAsync(Guid id)
        {
            var operations = new List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>>();
            var expressions = new IQueryExpression[1] { EntityById.Create(id) };

            operations.Add(new Tuple<QueryTypes, IExpressionBuilder, ValueTypes>
                                (QueryTypes.Find, expressions.GroupExpressions(), ValueTypes.Bool));

            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            Log.WriteLine("paramsCarrier Nesting Level: " + paramsCarrier.NestingLevel);

            var response = await ServiceInvoker.QueryRepositoryForEntity<T>(paramsCarrier);

            if (response.IsSuccess)
                response.Value?.AssignChildren(response);

            return response;
        }

        public QueryResult<T> Find(Guid id) 
        {
            var output = new QueryResult<T>
            {
                Value = null,
                IsSuccess = false
            };

            output.AddError("no se puede usar el Find sincrono desde el cliente, usar FindAsync a cambio");

            return output;
        }

        public async Task<QueryResult<T>> GetEntityAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = await ServiceInvoker.QueryRepositoryForEntity<T>(paramsCarrier);

            if (response.IsSuccess)
                response.Value?.AssignChildren(response);

            return response;
        }

        public async Task<QueryResult<List<T>>> GetEntitiesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            //Console.WriteLine("ClientDbSet GetEntitiesAsync 1");
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            //Console.WriteLine("ClientDbSet GetEntitiesAsync 2");
            var response = await ServiceInvoker.QueryRepositoryForEntities<T>(paramsCarrier);

            //Log.WriteLine("Total main entities: " + response.Value.Count + " total referenced:" + response.ReferencedEntities.Count);


            //Console.WriteLine("ClientDbSet GetEntitiesAsync 3");
            if (response.IsSuccess)
                response.Value?.DoForeach(e => e.AssignChildren(response));

            //Console.WriteLine("ClientDbSet GetEntitiesAsync 4");
            return response;
        }

        #region  Get Value
        public Task<QueryResult<bool>> GetBoolValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var e = (operations[0].Item2 as ExpressionsGroup).Expressions;
            Log.WriteLine("ClientDbSet GetBoolValueAsync operations[0].expressions.Length: " + e.Length);

            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForBool(paramsCarrier);
            return response;
        }

        public Task<QueryResult<double>> GetDoubleValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForDouble(paramsCarrier);
            return response;
        }

        public Task<QueryResult<int>> GetIntValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForInt(paramsCarrier);
            return response;
        }

        public Task<QueryResult<DateTime>> GetDateTimeValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForDateTime(paramsCarrier);
            return response;
        }
        public Task<QueryResult<Guid>> GetGuidValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForGuid(paramsCarrier);
            return response;
        }

        public Task<QueryResult<string>> GetStringValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForString(paramsCarrier);
            return response;
        }



        #endregion

        public Task<QueryResult<List<TOut>>> GetValuesAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: ClientGlobals.AlternativeUserId,
                                        userEmail: ClientGlobals.AlternativeUserEmail,
                                        traceInfo: null,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForValues<TOut>(paramsCarrier);
            return response;
        }
    }
}
