using Common.Lib.Infrastructure;
using Common.Lib.Services;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Core.Context
{
    public class ClientUnitOfWork : UnitOfWork
    {
        IParamsCarrierFactory ParamsCarrierFactory { get; set; }
        IServiceInvoker ServiceInvoker { get; set; }

        static Guid UserId { get; set; } = Guid.NewGuid(); //todo: implement user auth

        static string UserToken { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        public ClientUnitOfWork(IServiceInvoker serviceInvoker, IParamsCarrierFactory paramsCarrierFactory)
        {
            ServiceInvoker = serviceInvoker;
            ParamsCarrierFactory = paramsCarrierFactory;
        }

        public override async Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = null)
        {
            Log.WriteLine("ClientUnitOfWork CommitAsync paramsCarrier");
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateUnitOfWorkParams(userId: UserId,
                                        userToken: UserToken,
                                        actionTime: DateTime.Now,
                                        UowActions.Select(x => (IUoWActInfo)x));

            Log.WriteLine("ClientUnitOfWork CommitAsync CommitUnitOfWorkAsync");
            var response = await ServiceInvoker.CommitUnitOfWorkAsync(paramsCarrier);
            return response;
        }
    }
}
