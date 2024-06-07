using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class SaveEntityParamsCarrier : ISaveEntityParamsCarrier
    {
        public ITraceInfo? TraceInfo { get; set; }
        public SaveEntityParamsCarrier(Guid userId, string userToken, string userEmail, ITraceInfo? traceInfo, DateTime actionTime, IEntityInfo entityInfo)
        {
            TraceInfo = traceInfo;
            this.EntityInfo = new EntityInfo(entityInfo);
            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                UserEmail = userEmail,
                ActionTime = actionTime,
                TraceInfo = traceInfo,
            };
        }

        public Guid UserId
        {
            get
            {
                return this.ServiceInfo.UserId;
            }
            set
            {
               this.ServiceInfo.UserId = value;
            }
        }
        public string UserToken
        {
            get
            {
                return this.ServiceInfo.UserToken;
            }
            set
            {
                this.ServiceInfo.UserToken = value;
            }
        }

        public string UserEmail
        {
            get
            {
                return this.ServiceInfo.UserEmail;
            }
            set
            {
                this.ServiceInfo.UserEmail = value;
            }
        }

        public DateTime ActionTime

        {
            get
            {
                return this.ServiceInfo.ActionTime;
            }
            set
            {
                this.ServiceInfo.ActionTime = value;
            }
        }

        IEntityInfo ISaveEntityParamsCarrier.EntityInfo
        {
            get
            {
                return EntityInfo; 
            }
            set
            {
                EntityInfo = new EntityInfo(value);
            }
        }
    }
}
