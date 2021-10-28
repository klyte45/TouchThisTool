using ICities;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System.Collections.Generic;
using System.Reflection;

[assembly: AssemblyVersion("1.0.0.3")]
namespace Klyte.UpgradeUntouchable
{
    public class UpgradeUntouchableMod : BasicIUserMod<UpgradeUntouchableMod, UUController, UUPanel>
    {
        public override string SimpleName => "Upgrade Untouchable";

        public override string Description => "Tool for upgrade untouchable segments without changing building structure.";

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ToolController tc = UnityEngine.Object.FindObjectOfType<ToolController>();
            tc?.gameObject?.AddComponent<UpgradeUntouchableTool>();
        }

        protected override Tuple<string, string> GetButtonLink() => Tuple.New("See feature presentation thread @ Twitter", "https://twitter.com/Klyte45/status/1449112400884600834");

        protected override List<ulong> AutomaticUnsubMods => new List<ulong>() {
           1393797695 //TTT
        };
        protected override bool IsValidLoadMode(ILoading loading) => base.IsValidLoadMode(loading) || loading?.currentMode == AppMode.AssetEditor;
        protected override bool IsValidLoadMode(LoadMode mode) => base.IsValidLoadMode(mode) || mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset;
    }
}