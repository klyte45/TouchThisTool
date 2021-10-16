using ICities;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System.Reflection;

[assembly: AssemblyVersion("0.0.0.*")]
namespace Klyte.UpgradeUntouchable
{
    public class TouchThisToolMod : BasicIUserMod<TouchThisToolMod, TTTController, TTTPanel>
    {
        public override string SimpleName => "Upgrade Untouchable";

        public override string Description => "Tool for upgrade frozen segments without changing building structure.";

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ToolController tc = UnityEngine.Object.FindObjectOfType<ToolController>();
            tc?.gameObject?.AddComponent<TouchThisTool>();
        }

        protected override Tuple<string, string> GetButtonLink() => Tuple.New("See feature presentation thread @ Twitter", "https://twitter.com/Klyte45/status/1449112400884600834"); 
    }
}