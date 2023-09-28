using Common.Lib.Authentication;
using Delicapp.Lib.Authentication;

namespace Delicapp.Lib.Models
{
    public partial class Person
    {
        #region Static Permissions Scopes

        static internal void AddScopes(Dictionary<PersonActions, Dictionary<DelicappRoles, RolActionMap>> rolActionMaps)
        {
            var readerMap = rolActionMaps[PersonActions.Read][DelicappRoles.Reader];
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
