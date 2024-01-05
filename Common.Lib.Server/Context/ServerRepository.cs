using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Server.Context
{
    public class ServerRepository<T> : GenericRepository<T>, IServerRepository where T : Entity, new()
    {
        public ServerRepository(IWorkflowManager wfm, IContextFactory contextFactory) 
            : base(wfm, contextFactory)
        {

        }

        public new IServerRepository DeclareChildrenPolicy(int nestingLevel)
        {
            this.DbSet.NestingLevel = nestingLevel < 0 ? 0 : nestingLevel;
            return this;
        }

        #region Get value

        public Task<QueryResult<bool>> ExecuteGetBoolValueRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetBoolValueAsync(GetOperations(operations));
        }

        public Task<QueryResult<DateTime>> ExecuteGetDateTimeValueRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetDateTimeValueAsync(GetOperations(operations));
        }

        public Task<QueryResult<double>> ExecuteGetDoubleValueRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetDoubleValueAsync(GetOperations(operations));
        }
        public Task<QueryResult<Guid>> ExecuteGetGuidValueRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetGuidValueAsync(GetOperations(operations));
        }

        public Task<QueryResult<int>> ExecuteGetIntValueRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetIntValueAsync(GetOperations(operations));
        }

        public Task<QueryResult<string>> ExecuteGetStringValueRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetStringValueAsync(GetOperations(operations));
        }


        #endregion

        public async Task<QueryResult<List<Entity>>> ExecuteGetEntitiesRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            var qr1 = await DbSet.GetEntitiesAsync(GetOperations(operations));
            var refEnts = new Dictionary<Guid, Entity>();

            if (qr1.Value != null)
            {
                refEnts = new();
                foreach (var entity in qr1.Value)
                {
                    entity.ContextFactory = ContextFactory;
                    await entity.IncludeChildren(refEnts, this.DbSet.NestingLevel);
                }
            }

            return new QueryResult<List<Entity>>()
            {
                IsSuccess = qr1.IsSuccess,
                Message = qr1.Message,
                Value = qr1.Value != null ?
                            qr1.Value.Select(x => x as Entity).ToList() :
                            new List<Entity>(),
                ReferencedEntities = refEnts
            };
        }

        public async Task<QueryResult<Entity>> ExecuteGetEntityRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            var qr1 = await DbSet.GetEntityAsync(GetOperations(operations));

            var refEnts = new Dictionary<Guid, Entity>();

            if (qr1.Value != null)
            {
                refEnts = new();
                qr1.Value.ContextFactory = ContextFactory;
                await qr1.Value.IncludeChildren(refEnts, this.DbSet.NestingLevel);
            }

            return new QueryResult<Entity>()
            {
                IsSuccess = qr1.IsSuccess,
                Message = qr1.Message,
                Value = qr1.Value,
                ReferencedEntities = refEnts
            };
        }
                
        public Task<QueryResult<List<object>>> ExecuteGetValuesRequest(IEnumerable<IQueryOperationInfo> operations)
        {
            return DbSet.GetValuesAsync<object>(GetOperations(operations));
        }

        List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> GetOperations(IEnumerable<IQueryOperationInfo> operationsInfo)
        {
            var output = new List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>>();

            foreach (var operationInfo in operationsInfo)
            {
                IExpressionBuilder? expBuilder = null;
                var queryType = (QueryTypes)operationInfo.QueryType;
                
                switch (queryType)
                {
                    case QueryTypes.Where:
                    case QueryTypes.Any:
                    case QueryTypes.FirstOrDefault:
                    case QueryTypes.LastOrDefault:
                    case QueryTypes.Count:
                    case QueryTypes.All:
                    case QueryTypes.None:
                    case QueryTypes.Find:
                        expBuilder = operationInfo
                                            .Expressions
                                            .ToQueryEpxressions()
                                            .GroupExpressions();
                        break;
                    case QueryTypes.OrderBy:
                    case QueryTypes.OrderByDesc:
                    case QueryTypes.Select:
                    case QueryTypes.SelectOne:
                    case QueryTypes.SelectMany:
                    case QueryTypes.Distinct:
                    case QueryTypes.Max:
                    case QueryTypes.Sum:
                    case QueryTypes.Min:
                    case QueryTypes.Avg:
                        var expList = operationInfo.Expressions.ToArray();
                        expBuilder = expList[0].ToPropertySelector();
                        break;
                    default:
                        break;
                }

                output.Add(new Tuple<QueryTypes, IExpressionBuilder, ValueTypes>(
                            queryType,
                            expBuilder,
                            ValueTypes.Bool));
            }

            return output;
              }

        async Task<QueryResult<Entity>> IServerRepository.FindAsync(Guid id)
        {
            var qr = await DbSet.FindAsync(id);

            var output = new QueryResult<Entity>()
            {
                IsSuccess = qr.IsSuccess,
                Value = qr.Value
            };
            return output;
        }
    }
}
