using Common.Lib.Authorization;

namespace Test.Lib.Authorization
{
    public class TestAuthorization
    {
        IAuthorizationService _authorizationSvc;

        public TestAuthorization(IAuthorizationService authSvc)
        {
            _authorizationSvc = authSvc;
        }

        public void RegisterSimpleAuth()
        {

        }

        public void RegisterSimpleAuthActions()
        {
            #region Person Actions


            #endregion
        }


        public void RegisterWorkflowAuth()
        {

        }

    }
}
