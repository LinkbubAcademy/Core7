namespace Common.Lib.Authentication
{
    public class ModelRolesMap
    {
        public Dictionary<int, RoleMap>? RoleMaps { get; set; }

        public bool IsFieldAllowOnCreate(int role, int metadata)
        {
            if (RoleMaps == null)
                return true;

            if (RoleMaps.TryGetValue(role, out RoleMap? map))
                return map != null && map.IsFieldAllowOnCreate(metadata);

            return false;
        }

        public bool IsFieldAllowOnUpdate(int role, int metadata)
        {
            if (RoleMaps == null)
                return true;

            if (RoleMaps.TryGetValue(role, out RoleMap? map))
                return map != null && map.IsFieldAllowOnUpdate(metadata);

            return false;
        }
    }

    public class RoleMap
    {
        public bool AllowAllOnCreate { get; set; }

        public bool AllowAllOnUpdate { get; set; }

        public HashSet<int>? FieldsAllowedOnCreate { get; set; }
        public HashSet<int>? FieldsAllowedOnUpdate { get; set; }

        public bool IsFieldAllowOnCreate(int metadata)
        {
            if (AllowAllOnCreate)
                return true;

            if (FieldsAllowedOnCreate == null)
                return false;

            return FieldsAllowedOnCreate.Contains(metadata);
        }

        public bool IsFieldAllowOnUpdate(int metadata)
        {
            if (AllowAllOnUpdate)
                return true;

            if (FieldsAllowedOnUpdate == null)
                return false;

            return FieldsAllowedOnUpdate.Contains(metadata);
        }
    }
}
