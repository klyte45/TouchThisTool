using Klyte.TouchThis;


namespace Klyte.Commons
{
    public static class CommonProperties
    {
        public static bool DebugMode => TouchThisToolMod.DebugMode;
        public static string Version => TouchThisToolMod.Version;
        public static string ModName => TouchThisToolMod.Instance.SimpleName;
        public static string Acronym => "TTT";
        public static string ModIcon => TouchThisToolMod.Instance.IconName;
        public static string ModRootFolder { get; } = TTTController.FOLDER_NAME;
        public static string ModDllRootFolder { get; } = TouchThisToolMod.RootFolder;
    }
}