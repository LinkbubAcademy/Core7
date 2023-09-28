using Common.Lib.Authorization;

namespace Delicapp.Lib.Authorization
{
    public class DelicappAuthorization
    {
        IAuthorizationService _authorizationSvc;

        public DelicappAuthorization(IAuthorizationService authSvc)
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
