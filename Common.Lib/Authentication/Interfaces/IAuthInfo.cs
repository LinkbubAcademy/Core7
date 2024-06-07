
namespace Common.Lib.Authentication
{
    public interface IAuthInfo
    {
        string UserEmail { get; set; }
        Guid UserId { get; set; }
        string UserToken { get; set; }
    }
}