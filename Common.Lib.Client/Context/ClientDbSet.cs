using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services;
using Common.Lib.Services.ParamsCarriers;

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

        public async Task<ActionResult> AddAsync(T entity)
        {
            var output = new QueryResult<T>();

            var paramsCarrier = ParamsCarrierFactory
                                .CreateSaveEntityParams(
                                        userId: UserId,
                                        userToken: Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                                        actionTime: DateTime.Now,
                                        entityInfo: entity.GetChanges());

            var response = await ServiceInvoker.AddNewEntityRequestAsync(paramsCarrier);
            output.IsSuccess = response.IsSuccess;
            output.Message = response.Message;

            return await Task.FromResult(output);
        }

        public Task<ActionResult> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<QueryResult<T>> FindAsync(Guid id)
        {
            var output = new QueryResult<T>();

            return await Task.FromResult(output);
        }

        public async Task<QueryResult<T>> GetEntityAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
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
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = await ServiceInvoker.QueryRepositoryForEntities<T>(paramsCarrier);

            if (response.IsSuccess)
                response.Value?.DoForeach(e => e.AssignChildren(response));

            return response;
        }

        #region  Get Value
        public Task<QueryResult<bool>> GetBoolValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations)
        {
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateQueryRepositoryParams(
                                        userId: UserId,
                                        userToken: UserToken,
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
                                        userToken: UserToken,
                                        actionTime: DateTime.Now,
                                        repoTypeName: typeof(T).FullName,
                                        operations: operations,
                                        nestingLevel: NestingLevel);

            var response = ServiceInvoker.QueryRepositoryForValues<TOut>(paramsCarrier);
            return response;
        }
    }
}
