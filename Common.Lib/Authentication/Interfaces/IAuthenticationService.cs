namespace Common.Lib.Authentication
{
    public interface IAuthenticationService
    {
        IEnumerable<UserRol> GetUserRoles();

        void RegisterRol(UserRol rol);

    }
}
