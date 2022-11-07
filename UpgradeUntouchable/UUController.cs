using Kwytto.Interfaces;
using Kwytto.Utils;

namespace UpgradeUntouchable
{
    public class UUController : BaseController<UpgradeUntouchableMod, UUController>
    {
        protected void Awake()
        {
            ToolsModifierControl.toolController.AddExtraToolToController<UpgradeUntouchableTool>();
        }

    }

}
