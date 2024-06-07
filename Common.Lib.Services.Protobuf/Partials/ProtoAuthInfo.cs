using Common.Lib.Authentication;

namespace Common.Lib.Services.Protobuf
{
    public partial class ProtoAuthInfo : IAuthInfo
    {
        public Guid UserId
        {
            get
            {
                return Guid.Parse(SUserId);
            }
            set
            {
                SUserId = value.ToString();
            }
        }
        public ProtoAuthInfo(IAuthInfo line)
        {
            UserId = line.UserId;
            UserEmail = line.UserEmail;
            UserToken = line.UserToken;
        }

    }
}
