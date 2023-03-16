using Common.Lib.Authorization;

namespace Common.Lib.Authentication
{
    public class WorkflowAction
    {
        public Type? EntityType { get; set; }
        public WorkflowStatus? Origin { get; set; }

        public WorkflowStatus? Destination { get; set; }

        public List<UserRol> AllowedRoles { get; set; } = new();
    }
}
