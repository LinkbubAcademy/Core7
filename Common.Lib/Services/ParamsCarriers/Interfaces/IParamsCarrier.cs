namespace Common.Lib.Services.ParamsCarriers
{
    public interface IParamsCarrierInfo
    {
        Guid UserId { get; set; }
        string UserToken { get; set; }
        DateTime ActionTime { get; set; }

    }


    public interface IDeleteEntity
    {

    }

    public interface IQueryRepository
    {

    }

    public interface IUowSave
    {

    }

    public interface IBusinessServiceCall
    {

    }

}
