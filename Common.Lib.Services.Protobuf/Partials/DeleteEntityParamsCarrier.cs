using Common.Lib.Core.Tracking;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class DeleteEntityParamsCarrier : IDeleteEntityParamsCarrier
    {
        public DeleteEntityParamsCarrier(Guid userId, string userToken, DateTime actionTime, Guid entityId, string entityModelType)
        {     
            EntityId = entityId;
            EntityModelType = entityModelType;

            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                ActionTime = actionTime
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
