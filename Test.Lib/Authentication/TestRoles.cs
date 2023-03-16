using Common.Lib.Authentication;

namespace Test.Lib.Authentication
{
    public class TestRolesHandler
    {
        #region Roles

        public static readonly UserRol System = new()
        {
            Id = (int)TestRoles.System,
            Code = "System"

        };

        public static readonly UserRol Writer = new()
        {
            Id = (int)TestRoles.Writer,
            Code = "Writer"
        };

        public static readonly UserRol Editor = new()
        {
            Id = (int)TestRoles.Editor,
            Code = "Editor"
        };

        public static readonly UserRol Reader = new()
        {
            Id = (int)TestRoles.Reader,
            Code = "Reader"
        };

        #endregion

        IAuthenticationService AuthorizationSvc { get; set; }

        public TestRolesHandler(IAuthenticationService authSvc)
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

    public enum TestRoles
    {
        System = 1,
        Writer = 2,
        Editor = 3,
        Reader = 4
    }

}
