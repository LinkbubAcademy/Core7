using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Client.UI
{
    public class BaseViewModel
    {
        public IContextFactory ContextFactory { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static AndOperation And
        {
            get
            {
                return new();
            }
        }
        public static OrOperation Or
        {
            get
            {
                return new();
            }
        }

        public BaseViewModel(IContextFactory contextFactory) 
        {
            ContextFactory = contextFactory;
        }
    }
}
