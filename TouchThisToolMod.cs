using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.TouchThis.i18n;
using Klyte.TouchThis.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.1.*")]
namespace Klyte.TouchThis
{
    public class TouchThisToolMod : MonoBehaviour, IUserMod, ILoadingExtension
    {

        public static string minorVersion => majorVersion + "." + typeof(TouchThisToolMod).Assembly.GetName().Version.Build;
        public static string majorVersion => typeof(TouchThisToolMod).Assembly.GetName().Version.Major + "." + typeof(TouchThisToolMod).Assembly.GetName().Version.Minor;
        public static string fullVersion => minorVersion + " r" + typeof(TouchThisToolMod).Assembly.GetName().Version.Revision;
        public static string version
        {
            get {
                if (typeof(TouchThisToolMod).Assembly.GetName().Version.Minor == 0 && typeof(TouchThisToolMod).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(TouchThisToolMod).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(TouchThisToolMod).Assembly.GetName().Version.Build > 0)
                {
                    return minorVersion;
                }
                else
                {
                    return majorVersion;
                }
            }
        }
        public static bool IsKlyteCommonsEnabled()
        {
            if (!m_isKlyteCommonsLoaded)
            {
                try
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var assembly = (from a in assemblies
                                    where a.GetType("Klyte.Commons.KlyteCommonsMod") != null
                                    select a).SingleOrDefault();
                    if (assembly != null)
                    {
                        m_isKlyteCommonsLoaded = true;
                    }
                }
                catch { }
            }
            return m_isKlyteCommonsLoaded;
        }

        private SavedBool m_debugMode;
        public bool needShowPopup;
        private static bool m_isKlyteCommonsLoaded;
        private static bool isLocaleLoaded = false;

        public static bool LocaleLoaded => isLocaleLoaded;

        public static TouchThisToolMod instance;

        public string Name
        {
            get {

                return "Touch This! Tool " + version;
            }
        }

        public string Description
        {
            get { return "Tool for unlocking freeze segments to update and/or delete them. Util for buildings' roads."; }
        }


        public static bool debugMode => instance.m_debugMode.value;

        private SavedString currentSaveVersion => new SavedString("TTTSaveVersion", Settings.gameSettingsFile, "null", true);

        private SavedInt currentLanguageId => new SavedInt("TTTLanguage", Settings.gameSettingsFile, 0, true);

        public void OnCreated(ILoading loading)
        {
        }

        public TouchThisToolMod()
        {
            m_debugMode = new SavedBool("TTTdebugMode", Settings.gameSettingsFile, typeof(TouchThisToolMod).Assembly.GetName().Version.Major == 0, true);
            if (instance != null) { Destroy(instance); }
            instance = this;
        }


        public void OnSettingsUI(UIHelper helperDefault)
        {
            UIHelperExtension helper = new UIHelperExtension(helperDefault);
            void ev()
            {
                foreach (Transform child in helper.self.transform)
                {
                    GameObject.Destroy(child?.gameObject);
                }

                helper.self.eventVisibilityChanged += delegate (UIComponent component, bool b)
                {
                    if (b)
                    {
                        showVersionInfoPopup();
                    }
                };

                UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("TTT_BETAS_EXTRA_INFO"));
                group9.AddDropdownLocalized("TTT_MOD_LANG", TTTLocaleUtils.getLanguageIndex(), currentLanguageId.value, delegate (int idx)
                {
                    currentLanguageId.value = idx;
                    loadTTTLocale(true);
                });
                group9.AddCheckbox(Locale.Get("TTT_DEBUG_MODE"), m_debugMode.value, delegate (bool val) { m_debugMode.value = val; });
                group9.AddLabel("Version: " + fullVersion);
                group9.AddLabel(Locale.Get("TTT_ORIGINAL_TLM_VERSION") + " " + string.Join(".", TTTResourceLoader.instance.loadResourceString("TLMVersion.txt").Split(".".ToCharArray()).Take(3).ToArray()));
                group9.AddButton(Locale.Get("TTT_RELEASE_NOTES"), delegate ()
                {
                    showVersionInfoPopup(true);
                });
                //UILabel lblResult = null;
                //group9.AddButton("TEST SUBTYPE", delegate ()
                // {
                //     var subclasses = TTTUtils.GetSubtypesRecursive(typeof(BasicBuildingAIOverrides<,>), typeof(BasicBuildingAIOverrides<,>));
                //     lblResult.text = string.Format("GetOverride pré - subclasses:\r\n\t{0}", string.Join("\r\n\t", subclasses?.Select(x => x.ToString())?.ToArray() ?? new string[0]));
                // });
                //lblResult = group9.AddLabel("XXX1");
                //string testString = "";
                //for (int i = 0; i < 360; i++)
                //{
                //    var angle = Vector2.zero.GetAngleToPoint(new Vector2(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad)));
                //    testString += $"{i:n0}° => {angle:n1} ({Mathf.Sin(i * Mathf.Deg2Rad):n3}, {Mathf.Cos(i * Mathf.Deg2Rad):n3})\n";
                //}
                //group9.AddLabel("TST:\n" + testString);
                TTTUtils.doLog("End Loading Options");
            }
            if (IsKlyteCommonsEnabled())
            {
                loadTTTLocale(false);
                ev();
            }
            else
            {
                eventOnLoadLocaleEnd = null;
                eventOnLoadLocaleEnd += ev;
            }

        }

        private delegate void OnLocaleLoadedFirstTime();
        private event OnLocaleLoadedFirstTime eventOnLoadLocaleEnd;

        public void autoLoadTTTLocale()
        {
            if (currentLanguageId.value == 0)
            {
                loadTTTLocale(false);
            }
        }
        public void loadTTTLocale(bool force)
        {
            if (SingletonLite<LocaleManager>.exists && IsKlyteCommonsEnabled())
            {
                TTTLocaleUtils.loadLocale(currentLanguageId.value == 0 ? SingletonLite<LocaleManager>.instance.language : TTTLocaleUtils.getSelectedLocaleByIndex(currentLanguageId.value), force);
                if (!isLocaleLoaded)
                {
                    isLocaleLoaded = true;
                    eventOnLoadLocaleEnd?.Invoke();
                }
            }
        }

        public static int TouchAll()
        {
            if (Singleton<NetManager>.instance)
            {
                int count = 0;
                for (int i = 0; i < Singleton<NetManager>.instance.m_segments.m_buffer.Length; i++)
                {
                    if (Singleton<NetManager>.instance.m_segments.m_buffer[i].m_middlePosition != Vector3.zero
                    && (Singleton<NetManager>.instance.m_segments.m_buffer[i].m_flags & NetSegment.Flags.Untouchable) != NetSegment.Flags.None
                    && (Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_laneTypes & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.PublicTransport | NetInfo.LaneType.TransportVehicle | NetInfo.LaneType.CargoVehicle)) != NetInfo.LaneType.None
                    && (
                    Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_service == ItemClass.Service.Road
                    || Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_subService == ItemClass.SubService.PublicTransportBus
                    || Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_subService == ItemClass.SubService.PublicTransportTrain
                    || Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_subService == ItemClass.SubService.PublicTransportTram
                    || Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_subService == ItemClass.SubService.PublicTransportTaxi
                    || Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_subService == ItemClass.SubService.PublicTransportPlane
                    || Singleton<NetManager>.instance.m_segments.m_buffer[i].Info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro
                    ))
                    {
                        count++;
                        Singleton<NetManager>.instance.m_segments.m_buffer[i].m_flags &= ~NetSegment.Flags.Untouchable;
                    }
                }
                return count;
            }
            return -1;
        }

        public bool showVersionInfoPopup(bool force = false)
        {
            if (needShowPopup || force)
            {
                try
                {
                    UIComponent uIComponent = UIView.library.ShowModal("ExceptionPanel");
                    if (uIComponent != null)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        BindPropertyByKey component = uIComponent.GetComponent<BindPropertyByKey>();
                        if (component != null)
                        {
                            string title = "\"Touch This Tool!\" v" + version;
                            string notes = TTTResourceLoader.instance.loadResourceString("UI.VersionNotes.txt");
                            string text = "\"Touch This Tool!\" was updated! Release notes:\r\n\r\n" + notes;
                            string img = "IconMessage";
                            component.SetProperties(TooltipHelper.Format(new string[]
                            {
                            "title",
                            title,
                            "message",
                            text,
                            "img",
                            img
                            }));
                            needShowPopup = false;
                            currentSaveVersion.value = fullVersion;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        TTTUtils.doLog("PANEL NOT FOUND!!!!");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    TTTUtils.doErrorLog("showVersionInfoPopup ERROR {0} {1}", e.GetType(), e.Message);
                }
            }
            return false;
        }

        public void OnReleased()
        {

        }

        public void OnLevelLoaded(LoadMode mode)
        {
            if (!IsKlyteCommonsEnabled())
            {
                throw new Exception("Touch This Tool requires Klyte Commons active!");
            }
            TTTUtils.doLog("LEVEL LOAD");
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                TTTUtils.doLog("NOT GAME ({0})", mode);
                return;
            }


            TTTController.instance.Awake();
        }

        public void OnLevelUnloading()
        {
        }
    }
}