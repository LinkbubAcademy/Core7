using Common.Lib.Core.Context;
using Common.Lib.Core;
using Common.Lib.Core.Expressions;
using Common.Lib.Core.Tracking;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryRepositoryParamsCarrier : IQueryRepositoryParamsCarrier
    {
        public IEnumerable<IQueryOperationInfo> Operations
        {
            get
            {
                return this.SOperations;
            }
        }
        public QueryRepositoryParamsCarrier(Guid userId, 
                                            string userToken, 
                                            DateTime actionTime, 
                                            string repoTypeName, 
                                            List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> operations, 
                                            int nestingLevel)
        {
            this.RepositoryType = repoTypeName;
            this.NestingLevel = nestingLevel;
            this.ServiceInfo = new ParamsCarrierInfo()
            {
                UserId = userId,
                UserToken = userToken,
                ActionTime = actionTime
            };

            foreach (var operation in operations)
            {
                SOperations.Add(new QueryOperationInfo((int)operation.Item1, operation.Item2));
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
