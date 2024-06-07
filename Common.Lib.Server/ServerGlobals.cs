namespace Common.Lib.Server
{
    public static class ServerGlobals
    {
        public static bool IsMaintenanceModeOn { get; set; }

        public static string MaintenanceModeMessage { get; set; } = string.Empty;

        public static List<string> ServiceActionAllowedDuringMaintenance { get; set; } = ["DeactivateMaintenanceMode", "ResetCachedMemory"];
    }
}
