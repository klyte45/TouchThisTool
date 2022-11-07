using ColossalFramework.UI;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UpgradeUntouchable.Localization;
using UpgradeUntouchable.Utils;

namespace UpgradeUntouchable.UI
{

    public class UUWindow : GUIOpacityChanging
    {
        protected override bool showOverModals => false;
        protected override bool requireModal => false;
        protected override bool ShowCloseButton => false;
        protected override bool ShowMinimizeButton => true;
        protected override float FontSizeMultiplier => .9f;
        private const float minHeight = 250;
        public static UUWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObjectUtils.CreateElement<UUWindow>(UIView.GetAView().transform);
                    instance.Init(ModInstance.Instance.GeneralName, new Rect(256, 256, 250, minHeight), resizable: false, minSize: new Vector2(100, minHeight), hasTitlebar: true);
                    instance.Visible = false;
                }
                return instance;
            }
        }

        private static UUWindow instance;
        private UpgradeUntouchableTool m_tool;
        private NetTool m_defaultNetTool;
        private bool m_enableNetPickerWindow;

        private GUIStyle m_normalBtn;
        private GUIStyle m_disabledBtn;
        private GUIStyle m_selectedBtn;

        public override void Awake()
        {
            base.Awake();
            m_tool = ToolsModifierControl.toolController.GetComponent<UpgradeUntouchableTool>();
            m_defaultNetTool = ToolsModifierControl.toolController.GetComponents<NetTool>().Where(x => x.GetType() == typeof(NetTool)).First();
        }

        public void FixedUpdate()
        {
            Visible = m_tool.isActiveAndEnabled || (m_enableNetPickerWindow && m_defaultNetTool.isActiveAndEnabled);
        }

        protected override void OnWindowClosed()
        {
            base.OnWindowClosed();
            InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.None);
        }

        protected override void OnWindowOpened()
        {
            base.OnWindowOpened();

            InfoManager.instance.SetCurrentMode(m_currentMode, m_currentSubMode);

        }

        protected override void DrawWindow(Vector2 size)
        {
            InitStyles();

            if (m_tool.isActiveAndEnabled)
            {

                GUIKwyttoCommons.AddToggle(Str.UU_SHOWUNDERGROUNDVIEW, m_currentMode != InfoManager.InfoMode.None, OnUndergroundChange);
                GUIKwyttoCommons.AddToggle(Str.UU_CLASSICTOUCHTHISMODE, m_tool.ToolMode == UpgradeUntouchableTool.UuMode.Touch, m_tool.SetClassicMode);
                if (m_tool.ToolMode == UpgradeUntouchableTool.UuMode.Upgrade)
                {
                    GUIKwyttoCommons.AddToggle(Str.uu_enableNetPickerWindow, ref m_enableNetPickerWindow);
                    GUILayout.Space(10);
                    GUILayout.Label(Str.UU_CURRENTLYUPGRADINGTO);
                    GUILayout.Label(m_tool.CurrentSelection, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                    GUILayout.Space(5);
                    var modesArray = AllModes;
                    for (int i = 0; i < modesArray.Length; i++)
                    {
                        string mode = modesArray[i];
                        if (m_tool.CurrentSelection is null || !m_modesAvailable.Contains(i))
                        {
                            GUILayout.Label(mode, m_disabledBtn);
                            if (!(m_tool.CurrentSelection is null) && i == (int)m_selectedToolMode)
                            {
                                SetElevationMode(0);
                            }
                        }
                        else if ((int)m_selectedToolMode == i)
                        {
                            GUILayout.Label(mode, m_selectedBtn);
                        }
                        else if (GUILayout.Button(mode, m_normalBtn))
                        {
                            SetElevationMode((NetAIWrapper.ElevationType)i);
                        }
                    }
                }
            }
            else
            {
                if (GUILayout.Button(Str.UU_PICKFROMCURRENTNETTOOLITEM))
                {
                    if (m_tool.SetUpgradeTarget(m_defaultNetTool.m_prefab))
                    {
                        ToolsModifierControl.SetTool<UpgradeUntouchableTool>();
                        SetElevationMode(0);
                    }
                }
            }
            GUILayout.Space(1);
            var rect = GUILayoutUtility.GetLastRect();
            if (rect.position == default)
            {
                rect = cachedRect;
            }
            else
            {
                cachedRect = rect;
            }
            windowRect.height = TitleBarHeight + rect.y + 3;
        }

        private Rect cachedRect;

        private void InitStyles()
        {
            if (m_normalBtn is null)
            {
                m_normalBtn = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal =
                    {
                        background = GUIKwyttoCommons.darkGreenTexture,
                        textColor = Color.white
                    },
                    hover =
                    {
                        background = GUIKwyttoCommons.greenTexture,
                        textColor = Color.black
                    }
                };
            }
            if (m_disabledBtn is null)
            {
                m_disabledBtn = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal =
                    {
                        background = GUIKwyttoCommons.blackTexture,
                        textColor = Color.gray
                    },
                };
            }
            if (m_selectedBtn is null)
            {
                m_selectedBtn = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal =
                    {
                        background = GUIKwyttoCommons.whiteTexture,
                        textColor = Color.black
                    },
                };
            }
        }

        private void SetElevationMode(NetAIWrapper.ElevationType i)
        {
            m_selectedToolMode = i;
            m_tool.SetUpgradeMode(m_selectedToolMode);
        }

        private NetAIWrapper.ElevationType m_selectedToolMode = 0;

        private InfoManager.InfoMode m_currentMode = InfoManager.InfoMode.None;
        private InfoManager.SubInfoMode m_currentSubMode = InfoManager.SubInfoMode.None;
        private string[] AllModes => new[]
        {
            Str.UU_NORMALMODE,
            Str.UU_GROUNDMODE,
            Str.UU_ELEVATEDMODE,
            Str.UU_BRIDGEMODE,
            Str.UU_TUNNELMODE
        };

        private void OnUndergroundChange(bool isChecked)
        {
            m_currentMode = isChecked ? InfoManager.InfoMode.Underground : InfoManager.InfoMode.None;
            m_currentSubMode = isChecked ? InfoManager.SubInfoMode.UndergroundTunnels : InfoManager.SubInfoMode.None;
            InfoManager.instance.SetCurrentMode(m_currentMode, m_currentSubMode);
        }

        private readonly HashSet<int> m_modesAvailable = new HashSet<int>();

        public void UpdateAvailabilities(NetAIWrapper aIWrapper)
        {
            m_modesAvailable.Clear();
            m_modesAvailable.Add(0);
            for (int i = 1; i < AllModes.Length; i++)
            {
                m_modesAvailable.Add(i);
            }
        }
    }

}
