using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Extensions;
using Klyte.Commons.Interfaces;
using Klyte.Commons.UI.SpriteNames;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.UpgradeUntouchable
{
    public class UUPanel : BasicKPanel<UpgradeUntouchableMod, UUController, UUPanel>
    {
        public override float PanelWidth => 500;

        public override float PanelHeight => 260;

        private InfoManager.InfoMode m_currentMode = InfoManager.InfoMode.None;
        private InfoManager.SubInfoMode m_currentSubMode = InfoManager.SubInfoMode.None;

        private UILabel m_currentlyUpgradingLabel;
        private UITabstrip m_modes;
        private UIButton m_getFromNetTool;

        internal string CurrentDisplayingNet { get => m_currentlyUpgradingLabel.suffix; set => m_currentlyUpgradingLabel.suffix = value; }

        protected override void AwakeActions()
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel layoutPanel, MainPanel.transform, "LayoutPanel", new Vector4(0, 40, PanelWidth, PanelHeight - 40));
            layoutPanel.padding = new RectOffset(8, 8, 10, 10);
            layoutPanel.autoLayout = true;
            layoutPanel.autoLayoutDirection = LayoutDirection.Vertical;
            layoutPanel.autoLayoutPadding = new RectOffset(0, 0, 10, 10);
            var uiHelper = new UIHelperExtension(layoutPanel);

            KlyteMonoUtils.LimitWidthAndBox(uiHelper.AddCheckboxLocale("K45_UU_SHOWUNDERGROUNDVIEW", false, OnUndergroundChange).label, PanelWidth - 40); ;
            var classicMode = uiHelper.AddCheckboxLocale("K45_UU_CLASSICTOUCHTHISMODE", false, OnClassicModeChanged);
            classicMode.label.processMarkup = true;
            KlyteMonoUtils.LimitWidthAndBox(classicMode.label, PanelWidth - 10);



            m_currentlyUpgradingLabel = UIHelperExtension.AddLabel(uiHelper.Self, "\n\t", PanelWidth - 50, out UIPanel containerUpgrading);
            m_currentlyUpgradingLabel.prefix = Locale.Get("K45_UU_CURRENTLYUPGRADINGTO");
            KlyteMonoUtils.CreateUIElement(out m_getFromNetTool, containerUpgrading.transform, "getFromNetTool", new Vector4(containerUpgrading.width, 18, 36, 36));
            KlyteMonoUtils.InitButton(m_getFromNetTool, false, "OptionBase");
            m_getFromNetTool.normalFgSprite = KlyteResourceLoader.GetDefaultSpriteNameFor(CommonsSpriteNames.K45_Dropper);
            m_getFromNetTool.eventClicked += (x, y) =>
            {
                var netTool = FindObjectOfType<NetTool>();
                UpgradeUntouchableTool.instance.SetUpgradeTarget(netTool.m_prefab);
                m_modes.selectedIndex = 0;
            };
            m_getFromNetTool.tooltipLocaleID = "K45_UU_PICKFROMCURRENTNETTOOLITEM";

            KlyteMonoUtils.CreateUIElement(out m_modes, layoutPanel.transform, "modePanel", new Vector4(0, 0, PanelWidth - 10, 36));


            foreach (var btnName in new string[]
            {
                "K45_UU_NormalMode",
                "K45_UU_GroundMode",
                "K45_UU_ElevatedMode",
                "K45_UU_BridgeMode",
                "K45_UU_TunnelMode"
            })
            {
                UIButton btn;
                btn = m_modes.AddTab("");
                KlyteMonoUtils.InitButton(btn, false, "OptionBase");
                KlyteMonoUtils.InitButtonFg(btn, false, btnName);
                btn.tooltipLocaleID = btnName.ToUpper();
                btn.name = btnName;
            }
            m_modes.startSelectedIndex = -1;
            m_modes.eventSelectedIndexChanged += (w, x) => ResetTool();
            UpdateAvailabilities(null);

            UIHelperExtension.AddLabel(uiHelper.Self, Locale.Get("K45_UU_TIP_PRESSCTRLTOPICK"), PanelWidth - 50).textScale = 0.8f;
        }

        public void UpdatePickerState(bool state)
        {
            if (state)
            {
                m_getFromNetTool.Enable();
            }
            else
            {
                m_getFromNetTool.Disable();
            }
        }

        public void DeselectTool() => m_modes.selectedIndex = -1;

        public void UpdateAvailabilities(NetAIWrapper aIWrapper)
        {
            for (int i = 1; i < m_modes.tabCount; i++)
            {
                if (aIWrapper?.RelativeTo((NetAIWrapper.ElevationType)i, true) is null)
                {
                    m_modes.tabs[i].Disable();
                    if (m_modes.selectedIndex == i)
                    {
                        m_modes.selectedIndex = 0;
                    }
                }
                else
                {
                    m_modes.tabs[i].Enable();
                }
            }
        }

        private void ResetTool()
        {
            if (m_modes.selectedIndex >= 0)
            {
                UpgradeUntouchableMod.Controller?.ToggleTool(true, m_currentMode, m_currentSubMode);
                UpgradeUntouchableTool.instance.SetUpgradeMode((NetAIWrapper.ElevationType)m_modes.selectedIndex);
            }
        }

        private void OnUndergroundChange(bool isChecked)
        {
            m_currentMode = isChecked ? InfoManager.InfoMode.Underground : InfoManager.InfoMode.None;
            m_currentSubMode = isChecked ? InfoManager.SubInfoMode.UndergroundTunnels : InfoManager.SubInfoMode.None;
        }
        private void OnClassicModeChanged(bool isChecked)
        {
            UpgradeUntouchableTool.instance.SetClassicMode(isChecked);
            m_currentlyUpgradingLabel.isVisible = !isChecked;
            m_modes.isVisible = !isChecked;
            m_modes.selectedIndex = 0;
        }

    }
}