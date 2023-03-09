namespace Common.Lib.Core.Context
{    
    public interface IUoWRepository<T> : IRepository<T> where T : Entity, new()
    {
        IUnitOfWork UnitOfWork { get; set; }
    }

}
