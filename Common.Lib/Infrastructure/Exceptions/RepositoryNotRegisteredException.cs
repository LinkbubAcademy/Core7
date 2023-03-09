namespace System
{
    public class RepositoryNotRegisteredException : Exception
    {
        public RepositoryNotRegisteredException(Type repositoryType) : 
            base($"Repository for {repositoryType.Name} is not registered") 
        {

        }
    }
}
