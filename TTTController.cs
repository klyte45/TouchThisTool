using ColossalFramework.UI;
using Klyte.TouchThis.TextureAtlas;
using System.Collections;
using UnityEngine;

namespace Klyte.TouchThis
{
    public class TTTController : MonoBehaviour
    {
        public UIButton ToolButton { get; private set; }

        public void Start()
        {

            UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
            ToolButton = toolStrip.AddTab();
            ToolButton.size = new Vector2(49f, 49f);
            ToolButton.name = "TouchThisButton";
            ToolButton.tooltip = "Touch This Tool (v" + TouchThisToolMod.Version + ")";
            ToolButton.relativePosition = new Vector3(0f, 5f);
            toolStrip.AddTab("TouchThisButton", ToolButton.gameObject, null, null);
            ToolButton.atlas = TTTCommonTextureAtlas.instance.Atlas;
            ToolButton.normalBgSprite = "TouchThisIconSmall";
            ToolButton.focusedFgSprite = "ToolbarIconGroup6Focused";
            ToolButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
            FindObjectOfType<ToolController>().gameObject.AddComponent<TouchThisTool>();
            ToolButton.eventButtonStateChanged += delegate (UIComponent c, UIButton.ButtonState s)
            {
                TouchThisToolMod.Instance.ShowVersionInfoPopup();
                StartCoroutine(ToggleTool());
            };
        }

        private IEnumerator ToggleTool()
        {
            yield return 0;
            InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.None);
            TouchThisTool.instance.enabled = (ToolButton.state == UIButton.ButtonState.Focused);
        }

    }

}
