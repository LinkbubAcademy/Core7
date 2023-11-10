namespace Common.Lib.Core.Context
{
    public interface ILogHandler<T> where T : Entity, new()
    {
        void Log(Entity e);
    }
}
