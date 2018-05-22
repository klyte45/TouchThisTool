using ColossalFramework;
using ColossalFramework.UI;
using Klyte.TouchThis.Utils;
using UnityEngine;

namespace Klyte.TouchThis
{
    internal class TTTController : Singleton<TTTController>
    {
        internal static UITextureAtlas taTTT;
        private UIButton openTTTPanelButton;
        public UIButton ToolButton => openTTTPanelButton;


        public void Start()
        {

            if (taTTT == null)
            {
                taTTT = TTTResourceLoader.instance.CreateTextureAtlas("UI.Images.sprites.png", "TouchThisToolSprites", GameObject.FindObjectOfType<UIView>().FindUIComponent<UIPanel>("InfoPanel").atlas.material, 64, 64, new string[] {
                    "TouchThisIcon","TouchThisIconSmall","ToolbarIconGroup6Hovered","ToolbarIconGroup6Focused"
                });
            }

            UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
            openTTTPanelButton = toolStrip.AddTab();
            this.openTTTPanelButton.size = new Vector2(49f, 49f);
            this.openTTTPanelButton.name = "TouchThisButton";
            this.openTTTPanelButton.tooltip = "Touch This Tool (v" + TouchThisToolMod.version + ")";
            this.openTTTPanelButton.relativePosition = new Vector3(0f, 5f);
            toolStrip.AddTab("TouchThisButton", this.openTTTPanelButton.gameObject, null, null);
            openTTTPanelButton.atlas = taTTT;
            openTTTPanelButton.normalBgSprite = "TouchThisIconSmall";
            openTTTPanelButton.focusedFgSprite = "ToolbarIconGroup6Focused";
            openTTTPanelButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
            UnityEngine.Object.FindObjectOfType<ToolController>().gameObject.AddComponent<TouchThisTool>();
            this.openTTTPanelButton.eventButtonStateChanged += delegate (UIComponent c, UIButton.ButtonState s)
            {
                TouchThisToolMod.instance.showVersionInfoPopup();
                TouchThisTool.instance.enabled = (s == UIButton.ButtonState.Focused);
            };
        }

        public void Awake()
        {

        }

    }

}
