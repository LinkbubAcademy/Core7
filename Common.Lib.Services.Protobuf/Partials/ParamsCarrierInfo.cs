using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class ParamsCarrierInfo : IParamsCarrierInfo
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

        public DateTime ActionTime
        {
            get
            {
                return new DateTime().AddTicks(LActionTime);
            }
            set
            {
                LActionTime = value.Ticks;
            }
        }

        public ParamsCarrierInfo(IParamsCarrierInfo paramsCarrier)
        {
            UserId = paramsCarrier.UserId;
            ActionTime = paramsCarrier.ActionTime;
            UserToken = paramsCarrier.UserToken;
        }
    }
}
