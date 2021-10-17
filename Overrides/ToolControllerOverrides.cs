using Klyte.Commons.Extensions;

namespace Klyte.UpgradeUntouchable.Overrides
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
            if (UpgradeUntouchableTool.instance?.enabled == true && tool != UpgradeUntouchableTool.instance && (UUPanel.Instance?.MainPanel?.isVisible ?? false))
            {
                UUPanel.Instance?.DeselectTool();
            }
            UUPanel.Instance?.UpdatePickerState(tool is NetTool);
        }

    }
}