namespace Common.Lib.Core.Context
{
    public interface IModelFactory
    {
        T CreateNewEntity<T>() where T : Entity, new();
    }
}
