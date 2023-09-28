using Common.Lib.Authentication;

namespace Delicapp.Lib.Authentication
{
    public class DelicappRolesHandler
    {
        #region Roles

        public static readonly UserRol System = new()
        {
            Id = (int)DelicappRoles.System,
            Code = "System"

        };

        public static readonly UserRol Writer = new()
        {
            Id = (int)DelicappRoles.Writer,
            Code = "Writer"
        };

        public static readonly UserRol Editor = new()
        {
            Id = (int)DelicappRoles.Editor,
            Code = "Editor"
        };

        public static readonly UserRol Reader = new()
        {
            Id = (int)DelicappRoles.Reader,
            Code = "Reader"
        };

        #endregion

        IAuthenticationService AuthorizationSvc { get; set; }

        public DelicappRolesHandler(IAuthenticationService authSvc)
        {
            AuthorizationSvc = authSvc;
        }

        public void RegisterRoles()
        {
            AuthorizationSvc.RegisterRol(System);
            AuthorizationSvc.RegisterRol(Writer);
            AuthorizationSvc.RegisterRol(Editor);
            AuthorizationSvc.RegisterRol(Reader);
        }
    }

    public enum DelicappRoles
    {
        System = 1,
        Writer = 2,
        Editor = 3,
        Reader = 4
    }

}
