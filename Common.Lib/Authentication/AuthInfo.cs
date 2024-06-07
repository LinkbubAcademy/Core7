using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Authentication
{
    public class AuthInfo : IAuthInfo
    {
        public Guid UserId { get; set; }

        public string UserToken { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public AuthInfo()
        {

        }

        public AuthInfo(IAuthInfo authInfo)
        {
            UserId = authInfo.UserId;
            UserEmail = authInfo.UserEmail;
            UserToken = authInfo.UserToken;
        }

        public AuthInfo(Guid userId, string userToken, string userEmail)
        {
            UserId = userId;
            UserToken = userToken;
            UserEmail = userEmail;
        }

        public AuthInfo(IParamsCarrierInfo pc)
        {
            UserId = pc.UserId;
            UserToken = pc.UserToken;
            UserEmail = pc.UserEmail;
        }

    }
}
