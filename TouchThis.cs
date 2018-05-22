using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace Klyte.TouchThis
{
    public class TouchThis : IThreadingExtension, IUserMod
    {

        public static string version = "1.1";
        public static TouchThis instance;
        public KeyBinding key = new KeyBinding(KeyCode.LeftShift, KeyCode.Alpha0, KeyCode.None);

        public string Name
        {
            get
            {

                return "Touch This! " + version;
            }
        }

        public string Description
        {
            get { return "Unlock freeze segments to update and/or delete them. Util for ports roads"; }
        }

        public void OnCreated(ILoading loading)
        {
        }

        public TouchThis()
        {
            instance = this;
        }


        public void OnSettingsUI(UIHelper helperDefault)
        {
            helperDefault.AddButton("Touch all! (Shift + 0)", delegate ()
            {
                TouchAll();
            });

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


        public void OnCreated(IThreading threading)
        {
        }

        public void OnReleased()
        {
        }

        public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (this.key.isPressed())
            {
                int count = TouchAll();
                if (count > 0)
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
                                string title = "Touched!";
                                string text = string.Format("{0} segments was unlocked. Now touch them all!", count);
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
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }

        public void OnBeforeSimulationTick()
        {
        }

        public void OnBeforeSimulationFrame()
        {
        }

        public void OnAfterSimulationFrame()
        {
        }

        public void OnAfterSimulationTick()
        {
        }
    }
}