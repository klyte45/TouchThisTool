using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System.Collections;

namespace Klyte.TouchThis
{
    public class TTTController : BaseController<TouchThisToolMod, TTTController>
    {
        public const string FOLDER_NAME = "TouchThisTool";
        public UIButton ToolButton { get; private set; }

        internal void ToggleTool(bool newState, InfoManager.InfoMode viewMode, InfoManager.SubInfoMode submode)
        {
            LogUtils.DoWarnLog($"A[{newState}] viewMode, submode = {viewMode},{submode}");
            LogUtils.DoWarnLog($"B[{newState}] viewMode, submode = {viewMode},{submode}");
            if (newState)
            {
                InfoManager.instance.SetCurrentMode(viewMode, submode);
            }
            else
            {
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, 0);
            }

            StartCoroutine(ToggleTool_Internal(newState));
        }
        private IEnumerator ToggleTool_Internal(bool newState)
        {
            yield return 0;
            TouchThisTool.instance.enabled = newState;
        }

    }

}
