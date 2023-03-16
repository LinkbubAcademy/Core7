using Common.Lib.Core;

namespace Common.Lib.Authentication
{
    public class RolActionMap
    {
        public bool AllowAll { get; set; }

        public UserRol UserRol { get; set; }
        public HashSet<int>? FieldsAllowed { get; set; }


        /// <summary>
        /// This a filter applied to the Repository to limit
        /// which data can be accessed by the user
        /// </summary>
        public Func<User, Entity, bool> ScopeApplication { get; set; } = (user, e) => true;

        public RolActionMap(UserRol userRol, bool allowAll = false)
        {
            UserRol = userRol;
            AllowAll = allowAll;
        }
    }
}
