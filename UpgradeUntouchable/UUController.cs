using Kwytto.Interfaces;
using Kwytto.Utils;
using UpgradeUntouchable.UI;

namespace UpgradeUntouchable
{
    public class UUController : BaseController<ModInstance, UUController>
    {
        protected void Awake()
        {
            ToolsModifierControl.toolController.AddExtraToolToController<UpgradeUntouchableTool>();
            _ = UUWindow.Instance;
        }

    }

}
