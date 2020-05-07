using Klyte.Commons.Extensors;

namespace Klyte.TouchThis.Overrides
{
    public class ToolControllerOverrides : Redirector, IRedirectable
    {
        public void Awake()
        {
            System.Reflection.MethodInfo setTool = typeof(ToolController).GetMethod("SetTool", RedirectorUtils.allFlags);
            System.Reflection.MethodInfo OnToolChanged = GetType().GetMethod("OnToolChanged", RedirectorUtils.allFlags);
            AddRedirect(setTool, OnToolChanged);
        }

        public static void OnToolChanged(ref ToolBase tool)
        {
            if (TouchThisTool.instance?.enabled == true && tool != TouchThisTool.instance && TTTPanel.Instance.MainPanel.isVisible)
            {
                TouchThisToolMod.Instance.UnselectTab();
            }
        }

    }
}