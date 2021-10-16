using Klyte.UpgradeUntouchable;


namespace Klyte.Commons
{
    public static class CommonProperties
    {
        public static bool DebugMode => UpgradeUntouchableMod.DebugMode;
        public static string Version => UpgradeUntouchableMod.Version;
        public static string ModName => UpgradeUntouchableMod.Instance.SimpleName;
        public static string Acronym => "UU";
        public static string ModIcon => UpgradeUntouchableMod.Instance.IconName;
        public static string ModRootFolder { get; } = UUController.FOLDER_NAME;
        public static string ModDllRootFolder { get; } = UpgradeUntouchableMod.RootFolder;
        public static string[] AssetExtraFileNames { get; } = new string[0];
        public static string[] AssetExtraDirectoryNames { get; } = new string[0];

        public static string GitHubRepoPath { get; } = "klyte45/TouchThisTool";
    }
}