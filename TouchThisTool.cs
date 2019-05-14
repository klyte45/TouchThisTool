using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using Klyte.Commons;
using Klyte.TouchThis.Utils;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Klyte.TouchThis
{

    public class TouchThisTool : BasicNetTool<TouchThisTool>
    {

        protected override void OnLeftClick()
        {
            TTTUtils.doLog("OnLeftClick");
            if (m_hoverSegment > 0 && (segmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0)
            {
                TTTUtils.doLog("Touching!");
                InstanceID id = new InstanceID
                {
                    NetSegment = m_hoverSegment
                };

                Singleton<NetManager>.instance.m_segments.m_buffer[m_hoverSegment].m_flags &= ~NetSegment.Flags.Untouchable;

                var effectInfo = Singleton<NetManager>.instance.m_properties.m_placementEffect;

                EffectInfo.SpawnArea spawnArea = new EffectInfo.SpawnArea(new Vector3(), new Vector3(), 0);
                Singleton<EffectManager>.instance.DispatchEffect(effectInfo, id, spawnArea, Vector3.zero, 0f, 1f, Singleton<AudioManager>.instance.DefaultGroup, 0u, true);
                TTTUtils.doLog("Touched!");
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {

            if (m_hoverSegment != 0)
            {
                Color toolColor = (segmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0 ? m_hoverColor : m_removeColor;
                RenderOverlay(cameraInfo, toolColor, m_despawnColor, m_hoverSegment);
                return;
            }
        }
    }

}
