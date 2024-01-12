using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class ServiceActionParamsCarrier : IServiceActionParamsCarrier
    {
        public ServiceActionParamsCarrier(Guid userId, 
                                                string userToken, 
                                                DateTime actionTime,
                                                string serviceTypeName,
                                                string serviceActionName, 
                                                string[] serializedValues)
        {
            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                ActionTime = actionTime
            };
            this.ServiceType = serviceTypeName;
            this.ServiceActionName = serviceActionName;
            this.SerializedValues = serializedValues;
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
