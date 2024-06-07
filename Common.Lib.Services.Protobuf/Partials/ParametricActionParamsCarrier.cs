using Common.Lib.Infrastructure;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class ParametricActionParamsCarrier : IParametricActionParamsCarrier
    {
        public ITraceInfo? TraceInfo { get; set; }
        public ParametricActionParamsCarrier(Guid userId, 
                                                string userToken,
                                                string userEmail,
                                                ITraceInfo? traceInfo,
                                                DateTime actionTime,
                                                string repoTypeName,
                                                Guid entityId,
                                                string paramActionName, 
                                                string[] serializedValues)
        {
            TraceInfo = traceInfo;
            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                UserEmail = userEmail,
                ActionTime = actionTime,
                TraceInfo = traceInfo,
            };

            this.RepositoryType = repoTypeName;
            this.EntityId = entityId;
            this.ParametricActionName = paramActionName;
            this.SerializedValues = serializedValues;
        }

        public Guid EntityId
        {
            get
            {
                return Guid.Parse(this.SEntityId);
            }
            set
            {
                this.SEntityId = value.ToString();
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

        public string[] SerializedValues
        {
            get
            {
                return this.sParams_.ToArray();
            }
            set
            {
                this.sParams_.Clear();
                foreach(var s in value)
                    this.sParams_.Add(s);
            }
        }
    }
}
