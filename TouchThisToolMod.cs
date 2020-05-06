using Klyte.Commons.Interfaces;
using System.Reflection;

[assembly: AssemblyVersion("4.0.0.0")]
namespace Klyte.TouchThis
{
    public class TouchThisToolMod : BasicIUserMod<TouchThisToolMod, TTTController, TTTPanel>
    {
        public override string SimpleName => "Touch This! Tool";

        public override string Description => "Tool for unlocking freeze segments to update and/or delete them. Util for buildings' roads.";

        protected override void OnLevelLoadingInternal()
        {
            ToolController tc = UnityEngine.Object.FindObjectOfType<ToolController>();
            tc.gameObject.AddComponent<TouchThisTool>();
        }
    }
}