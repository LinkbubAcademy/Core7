using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public interface IDbSet<T> : ICrudHandler<T> where T : Entity, new()
    {
        int NestingLevel { get; set; }

        Task<QueryResult<T>> FindAsync(Guid id);

        Task<QueryResult<T>> GetEntityAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);

        Task<QueryResult<List<T>>> GetEntitiesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);

        Task<QueryResult<bool>> GetBoolValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);
        Task<QueryResult<double>> GetDoubleValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);
        Task<QueryResult<int>> GetIntValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);        
        Task<QueryResult<string>> GetStringValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);
        Task<QueryResult<DateTime>> GetDateTimeValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);
        Task<QueryResult<Guid>> GetGuidValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);

        //Task<QueryResult<string>> GetStringValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<Guid>> GetGuidValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<byte[]>> GetBytesValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<TOut>> GetValueAsync<TValue, TOut>(List<Tuple<QueryTypes, object, ValueTypes>> Operations);
        Task<QueryResult<List<TOut>>> GetValuesAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations);

        //Task<QueryResult<List<bool>>> GetBoolValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<List<double>>> GetDoubleValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<List<int>>> GetIntValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<List<string>>> GetStringValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<List<DateTime>>> GetDateTimeValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<List<Guid>>> GetGuidValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
        //Task<QueryResult<List<byte[]>>> GetBytesValuesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations);
    }
}
