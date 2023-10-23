using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Infrastructure;
using Common.Lib.Services;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Context
{
    public class ClientUnitOfWork : IUnitOfWork
    {
        List<UoWActInfo> UowActions { get; set; } = new();

        IParamsCarrierFactory ParamsCarrierFactory { get; set; }
        IServiceInvoker ServiceInvoker { get; set; }

        static Guid UserId { get; set; } = Guid.NewGuid(); //todo: implement user auth
        static string UserToken { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        public ClientUnitOfWork(IServiceInvoker serviceInvoker, IParamsCarrierFactory paramsCarrierFactory)
        {
            ServiceInvoker = serviceInvoker;
            ParamsCarrierFactory = paramsCarrierFactory;
        }

        public void AddEntitySave(Entity entity)
        {
            UowActions.Add(new UoWActInfo()
            {
                Change = entity.GetChanges(),
                ActionInfoType = ActionInfoTypes.Save
            });
        }

        public async Task<IActionResult> CommitAsync()
        {
            Console.WriteLine("ClientUnitOfWork CommitAsync paramsCarrier");
            var paramsCarrier = ParamsCarrierFactory
                                    .CreateUnitOfWorkParams(userId: UserId,
                                        userToken: UserToken,
                                        actionTime: DateTime.Now,
                                        UowActions.Select(x => (IUoWActInfo)x));

            Console.WriteLine("ClientUnitOfWork CommitAsync CommitUnitOfWorkAsync");
            var response = await ServiceInvoker.CommitUnitOfWorkAsync(paramsCarrier);
            return response;
        }

        public void Dispose()
        {
        }
    }
}
