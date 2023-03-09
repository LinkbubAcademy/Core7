using Common.Lib.Core.Tracking;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class SaveEntityParamsCarrier : ISaveEntityParamsCarrier
    {
        public SaveEntityParamsCarrier(Guid userId, string userToken, DateTime actionTime, IEntityInfo entityInfo)
        {            
            this.EntityInfo = new EntityInfo(entityInfo);
            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                ActionTime = actionTime
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
