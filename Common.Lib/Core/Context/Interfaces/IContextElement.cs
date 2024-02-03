namespace Common.Lib.Core.Context
{
    public interface IContextElement : IDisposable
    {
        IContextFactory ContextFactory { get; set; }

    }
}
