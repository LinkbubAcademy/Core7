namespace Common.Lib.Core
{
    /// <summary>
    /// This class represent a composed value eg. address or email
    /// </summary>
    public class ComplexType
    {
        public static string Splitter = "~";

        public static string ListSplitter = "¬";

        public Action<string> Setter;

        public virtual void Store()
        {

        }
    }
}
