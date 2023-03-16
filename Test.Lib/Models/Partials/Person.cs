using Common.Lib.Authentication;
using Test.Lib.Authentication;

namespace Test.Lib.Models
{
    public partial class Person
    {
        #region Static Permissions Scopes

        static internal void AddScopes(Dictionary<PersonActions, Dictionary<TestRoles, RolActionMap>> rolActionMaps)
        {
            var readerMap = rolActionMaps[PersonActions.Read][TestRoles.Reader];
            readerMap.ScopeApplication = (user, e) =>
            {
                if (e is not Person person)
                    return false;

                return person.Id == user.Id;
            };
        }

        #endregion
    }
}
