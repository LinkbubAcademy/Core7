using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public interface IServerRepository : IRepository
    {
        IServerRepository DeclareChildrenPolicy(int nestingLevel);
        Task<QueryResult<Entity>> ExecuteGetEntityRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<List<Entity>>> ExecuteGetEntitiesRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<bool>> ExecuteGetBoolValueRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<int>> ExecuteGetIntValueRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<double>> ExecuteGetDoubleValueRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<string>> ExecuteGetStringValueRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<DateTime>> ExecuteGetDateTimeValueRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<Guid>> ExecuteGetGuidValueRequest(IEnumerable<IQueryOperationInfo> operations);
        Task<QueryResult<List<object>>> ExecuteGetValuesRequest(IEnumerable<IQueryOperationInfo> operations);
    }
}
