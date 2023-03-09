namespace System
{
    public class ContextFactoryNullException : Exception
    {
        public ContextFactoryNullException(string callerClass, string callerMethod) : 
            base($"Context Factory within {callerClass} and method {callerMethod} is null, use ContextFactory to instantiate the object") 
        {

        }
    }
}
