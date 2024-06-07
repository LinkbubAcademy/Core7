using Common.Lib.Infrastructure;
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

        public ITraceInfo? TraceInfo
        {
            get
            {
                if (STraceInfo.IsNull)
                    return null;

                return STraceInfo;
            }
            set
            {
                STraceInfo = new ProtoTraceInfo(value);
            }
        }

        public ParamsCarrierInfo(IParamsCarrierInfo paramsCarrier)
        {
            UserId = paramsCarrier.UserId;
            ActionTime = paramsCarrier.ActionTime;
            UserToken = paramsCarrier.UserToken;
            TraceInfo = paramsCarrier.TraceInfo;
        }
    }
}
