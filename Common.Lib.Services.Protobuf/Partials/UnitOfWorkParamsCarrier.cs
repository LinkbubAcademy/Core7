using Common.Lib.Core.Context;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Infrastructure;

namespace Common.Lib.Services.Protobuf
{
    public partial class UnitOfWorkParamsCarrier : IUnitOfWorkParamsCarrier
    {
        public UnitOfWorkParamsCarrier(Guid userId,
                                            string userToken,
                                            string userEmail,
                                            ITraceInfo? trace,
                                            DateTime actionTime,
                                            IEnumerable<IUoWActInfo> actions)
        {

            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                UserEmail = userEmail,
                ActionTime = actionTime,
                TraceInfo = trace
            };

            UowActions = actions;
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

        public IEnumerable<IUoWActInfo> UowActions
        {
            get
            {
                return SUowActions.ToList();
            }
            set
            {
                SUowActions.Clear();
                foreach (var action in value)
                    SUowActions.Add(new UowActionInfo(action));
            }
        }

        public ITraceInfo? TraceInfo { get; set; }
    }
}
