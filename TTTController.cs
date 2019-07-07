using ColossalFramework;
using ColossalFramework.UI;
using Klyte.TouchThis.TextureAtlas;
using Klyte.TouchThis.Utils;
using UnityEngine;

namespace Klyte.TouchThis
{
    public class TTTController : MonoBehaviour
    {
        private UIButton m_openTTTPanelButton;
        public UIButton ToolButton => m_openTTTPanelButton;

        public void Start()
        {

            UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
            m_openTTTPanelButton = toolStrip.AddTab();
            this.m_openTTTPanelButton.size = new Vector2(49f, 49f);
            this.m_openTTTPanelButton.name = "TouchThisButton";
            this.m_openTTTPanelButton.tooltip = "Touch This Tool (v" + TouchThisToolMod.Version + ")";
            this.m_openTTTPanelButton.relativePosition = new Vector3(0f, 5f);
            toolStrip.AddTab("TouchThisButton", this.m_openTTTPanelButton.gameObject, null, null);
            m_openTTTPanelButton.atlas = TTTCommonTextureAtlas.instance.Atlas;
            m_openTTTPanelButton.normalBgSprite = "TouchThisIconSmall";
            m_openTTTPanelButton.focusedFgSprite = "ToolbarIconGroup6Focused";
            m_openTTTPanelButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
            FindObjectOfType<ToolController>().gameObject.AddComponent<TouchThisTool>();
            this.m_openTTTPanelButton.eventButtonStateChanged += delegate (UIComponent c, UIButton.ButtonState s)
            {
                TouchThisToolMod.Instance.ShowVersionInfoPopup();
                TouchThisTool.instance.enabled = (s == UIButton.ButtonState.Focused);
            };
        }

    }

}
