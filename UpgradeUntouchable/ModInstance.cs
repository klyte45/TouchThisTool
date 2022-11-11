using ICities;
using Kwytto.Interfaces;
using Kwytto.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UpgradeUntouchable.Localization;

[assembly: AssemblyVersion("1.1.0.1")]
namespace UpgradeUntouchable
{
    public class ModInstance : BasicIUserMod<ModInstance, UUController>
    {
        public override string GitHubRepoPath { get; } = "klyte45/TouchThisTool";
        public override string SimpleName => "Upgrade Untouchable";

        public override string Description => "Tool for upgrade untouchable segments without changing building structure.";
        protected override List<ulong> AutomaticUnsubMods => new List<ulong>() {
           1393797695 //TTT
        };

        public override string SafeName => "UpgradeUntouchable";

        public override string Acronym => "UU";

        public override Color ModColor { get; } = ColorExtensions.FromRGB("5a2e63");

        protected override bool IsValidLoadMode(ILoading loading) => base.IsValidLoadMode(loading) || loading?.currentMode == AppMode.AssetEditor;
        protected override bool IsValidLoadMode(LoadMode mode) => base.IsValidLoadMode(mode) || mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset;

        protected override void SetLocaleCulture(CultureInfo culture)
        {
            Str.Culture = culture;
        }

        private IUUIButtonContainerPlaceholder[] cachedUUI;
        public override IUUIButtonContainerPlaceholder[] UUIButtons => cachedUUI ?? (cachedUUI = new IUUIButtonContainerPlaceholder[]
        {
            new UUIToolButtonContainerPlaceholder(
                buttonName : SimpleName,
                iconPath : "ModIcon",
                tooltip : SimpleName,
                toolGetter : () => ToolsModifierControl.toolController.GetComponent<UpgradeUntouchableTool>()
            )
        });
    }
}