using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class DeleteEntityParamsCarrier : IDeleteEntityParamsCarrier
    {
        public ITraceInfo? TraceInfo { get; set; }

        public DeleteEntityParamsCarrier(Guid userId, string userToken, string userEmail, ITraceInfo? traceInfo, DateTime actionTime, Guid entityId, string entityModelType)
        {     
            EntityId = entityId;
            EntityModelType = entityModelType;
            TraceInfo = traceInfo;

            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                ActionTime = actionTime,
                TraceInfo = traceInfo,
            };
        }
        public Guid EntityId
        {
            get
            {
                return Guid.Parse(SEntityId);
            }
            set
            {
                SEntityId = value.ToString();
            }
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
    }
}
