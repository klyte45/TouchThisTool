﻿using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons;
using Klyte.Commons.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Klyte.Commons.Utils.NetAIWrapper;

namespace Klyte.TouchThis
{

    public class TouchThisTool : BasicNetTool<TouchThisTool>
    {
        private enum Mode
        {
            Touch,
            Upgrade
        }


        private Mode m_toolMode = Mode.Upgrade;
        private NetAIWrapper m_upgradeAI;
        private ElevationType m_targetType;
        private ElevationType m_effectiveTargetType;
        private NetAIWrapper m_oldAI;
        private ElevationType m_oldType;
        private string m_cachedInfoCompareText;
        private bool m_mustConfirm;

        private NetTool.ControlPoint[] m_controlPoints = new NetTool.ControlPoint[3];
        private int m_controlPointCount;




        protected override void OnLeftClick()
        {
            LogUtils.DoLog("OnLeftClick");
            if (m_hoverSegment > 0)
            {
                if (m_toolMode == Mode.Touch && (SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0)
                {
                    LogUtils.DoLog("Touching!");
                    var id = new InstanceID
                    {
                        NetSegment = m_hoverSegment
                    };

                    Singleton<NetManager>.instance.m_segments.m_buffer[m_hoverSegment].m_flags &= ~NetSegment.Flags.Untouchable;

                    EffectInfo effectInfo = Singleton<NetManager>.instance.m_properties.m_placementEffect;

                    var spawnArea = new EffectInfo.SpawnArea(new Vector3(), new Vector3(), 0);
                    Singleton<EffectManager>.instance.DispatchEffect(effectInfo, id, spawnArea, Vector3.zero, 0f, 1f, Singleton<AudioManager>.instance.DefaultGroup, 0u, true);
                    LogUtils.DoLog("Touched!");
                }
                else if (m_toolMode == Mode.Upgrade)
                {
                    if (Event.current.control)
                    {
                        if (!(SegmentBuffer[m_hoverSegment].Info.m_netAI is DamAI))
                        {
                            m_upgradeAI = new NetAIWrapper(Singleton<NetManager>.instance.m_segments.m_buffer[m_hoverSegment].Info.m_netAI);
                        }
                    }
                    else if (!(m_upgradeAI is null) && (SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0 && !(m_oldAI == m_upgradeAI && m_oldType == m_effectiveTargetType))
                    {
                        ref NetSegment targetSegment = ref Singleton<NetManager>.instance.m_segments.m_buffer[m_hoverSegment];
                        ref NetNode startNode = ref Singleton<NetManager>.instance.m_nodes.m_buffer[targetSegment.m_startNode];
                        ref NetNode endNode = ref Singleton<NetManager>.instance.m_nodes.m_buffer[targetSegment.m_endNode];



                        Singleton<SimulationManager>.instance.AddAction(CreateNode(false, startNode.m_building, endNode.m_building));
                    }

                }
            }
        }
        protected override void OnRightClick()
        {
            LogUtils.DoLog("OnRightClick");
            if (m_hoverSegment > 0)
            {
                if (m_toolMode == Mode.Upgrade)
                {
                    if ((SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0)
                    {
                        ref NetSegment targetSegment = ref Singleton<NetManager>.instance.m_segments.m_buffer[m_hoverSegment];

                        if (targetSegment.Info.m_forwardVehicleLaneCount != targetSegment.Info.m_backwardVehicleLaneCount)
                        {
                            m_oldAI = new NetAIWrapper(targetSegment.Info.m_netAI);
                            m_oldType = m_oldAI.ToType(targetSegment.Info);
                            UpdateControlPoints(targetSegment.Info);
                            ref NetNode startNode = ref Singleton<NetManager>.instance.m_nodes.m_buffer[targetSegment.m_startNode];
                            ref NetNode endNode = ref Singleton<NetManager>.instance.m_nodes.m_buffer[targetSegment.m_endNode];
                            Singleton<SimulationManager>.instance.AddAction(CreateNode(true, startNode.m_building, endNode.m_building));
                        }
                    }

                }
            }
        }

        internal void SetClassicMode(bool isChecked) => m_toolMode = isChecked ? Mode.Touch : Mode.Upgrade;

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (m_hoverSegment != 0)
            {
                Color toolColor = m_toolMode == Mode.Touch
                    ? (SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0
                        ? SegmentBuffer[m_hoverSegment].Info.m_netAI is DamAI || (m_oldAI == m_upgradeAI && m_oldType == m_effectiveTargetType)
                            ? m_removeColor
                            : m_hoverColor
                        : m_removeColor
                    : Event.current.control
                        ? SegmentBuffer[m_hoverSegment].Info.m_netAI is DamAI
                            ? m_removeColor
                            : Color.cyan
                        : (SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0
                            ? (m_upgradeAI is null)
                                ? m_removeColor
                                : m_hoverColor
                            : m_removeColor;
                RenderOverlayUtils.RenderNetSegmentOverlay(cameraInfo, toolColor, m_hoverSegment);
            }
        }

        public new void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        public override void SimulationStep()
        {
            if (m_toolMode == Mode.Upgrade && m_hoverSegment != 0)
            {
                if (!(m_upgradeAI is null) && ((SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0) && !Event.current.control && !(m_oldAI is null))
                {
                    UpdateControlPoints(m_upgradeAI.RelativeTo(m_targetType == ElevationType.Default ? m_oldType : m_targetType));
                }
            }
        }

        private void UpdateControlPoints(NetInfo targetInfo)
        {
            NetManager instance = Singleton<NetManager>.instance;

            NetTool.ControlPoint controlPoint;
            controlPoint.m_node = instance.m_segments.m_buffer[m_hoverSegment].m_startNode;
            controlPoint.m_segment = 0;
            controlPoint.m_position = instance.m_nodes.m_buffer[controlPoint.m_node].m_position;
            controlPoint.m_direction = instance.m_segments.m_buffer[m_hoverSegment].m_startDirection;
            controlPoint.m_elevation = instance.m_nodes.m_buffer[controlPoint.m_node].m_elevation;
            if (instance.m_nodes.m_buffer[controlPoint.m_node].Info.m_netAI.IsUnderground())
            {
                controlPoint.m_elevation = -controlPoint.m_elevation;
            }
            controlPoint.m_outside = ((instance.m_nodes.m_buffer[controlPoint.m_node].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None);
            NetTool.ControlPoint controlPoint2;
            controlPoint2.m_node = instance.m_segments.m_buffer[m_hoverSegment].m_endNode;
            controlPoint2.m_segment = 0;
            controlPoint2.m_position = instance.m_nodes.m_buffer[controlPoint2.m_node].m_position;
            controlPoint2.m_direction = -instance.m_segments.m_buffer[m_hoverSegment].m_endDirection;
            controlPoint2.m_elevation = instance.m_nodes.m_buffer[controlPoint2.m_node].m_elevation;
            if (instance.m_nodes.m_buffer[controlPoint2.m_node].Info.m_netAI.IsUnderground())
            {
                controlPoint2.m_elevation = -controlPoint2.m_elevation;
            }
            controlPoint2.m_outside = ((instance.m_nodes.m_buffer[controlPoint2.m_node].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None);
            NetTool.ControlPoint controlPoint3;
            controlPoint3.m_node = 0;
            controlPoint3.m_segment = m_hoverSegment;
            controlPoint3.m_position = controlPoint.m_position + (controlPoint.m_direction * (targetInfo.GetMinNodeDistance() + 1f));
            controlPoint3.m_direction = controlPoint.m_direction;
            controlPoint3.m_elevation = Mathf.Lerp(controlPoint.m_elevation, controlPoint2.m_elevation, 0.5f);
            controlPoint3.m_outside = false;
            m_controlPoints[0] = controlPoint;
            m_controlPoints[1] = controlPoint3;
            m_controlPoints[2] = controlPoint2;
            m_controlPointCount = 2;
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (m_toolMode == Mode.Upgrade && m_hoverSegment != 0)
            {
                if (((SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0) || Event.current.control || (m_upgradeAI is null))
                {
                    var text = string.Format(Locale.Get(Event.current.control
                        ? SegmentBuffer[m_hoverSegment].Info.m_netAI is DamAI
                            ? "K45_TTT_NETNOTSUPPORTED"
                            : "K45_TTT_PICKNET_PATTERN"
                        : (m_upgradeAI is null)
                            ? "K45_TTT_PRESSCTRLTOPICK"
                            : SegmentBuffer[m_hoverSegment].Info.m_netAI is DamAI
                                ? "K45_TTT_NETNOTSUPPORTED"
                                : (m_oldAI == m_upgradeAI && m_oldType == m_effectiveTargetType)
                                    ? "K45_TTT_CANNOTUPGRADE_ITSELF"
                                    : "K45_TTT_UPGRADETO_PATTERN"
                        ), Event.current.control
                        ? SegmentBuffer[m_hoverSegment].Info.GetUncheckedLocalizedTitle()
                        : m_upgradeAI?.RelativeTo(m_targetType == ElevationType.Default ? m_oldType : m_targetType)?.GetUncheckedLocalizedTitle());

                    if (((SegmentBuffer[m_hoverSegment].m_flags & NetSegment.Flags.Untouchable) != 0) && !Event.current.control)
                    {
                        var oldInfo = SegmentBuffer[m_hoverSegment].Info;
                        if (oldInfo.m_netAI != m_oldAI?.AI)
                        {
                            m_oldAI = new NetAIWrapper(oldInfo.m_netAI);
                            m_oldType = m_oldAI.ToType(oldInfo);
                            if (!(m_upgradeAI is null))
                            {
                                m_effectiveTargetType = m_targetType == ElevationType.Default ? m_oldType : m_targetType;
                                var targetUpgradeInfo = m_upgradeAI.RelativeTo(m_effectiveTargetType);
                                m_effectiveTargetType = m_upgradeAI.ToType(targetUpgradeInfo);

                                var isSameWidth = oldInfo.m_halfWidth == targetUpgradeInfo.m_halfWidth;
                                var oldHasStop = oldInfo.m_lanes.Where(x => x.m_stopType != 0).Count() > 0;
                                var newHasStop = targetUpgradeInfo.m_lanes.Where(x => x.m_stopType != 0).Count() > 0;
                                var isSameHasStop = oldHasStop == newHasStop;


                                var isSameSegElvType = m_oldType == m_effectiveTargetType;

                                var isSameSubService = oldInfo.m_class.m_subService == targetUpgradeInfo.m_class.m_subService;

                                m_mustConfirm = false;
                                var observations = new List<string>();
                                if (!isSameWidth)
                                {
                                    observations.Add($"<color yellow>{Locale.Get("K45_TTT_DIFFERENTHALFWIDTHS")}: {oldInfo.m_halfWidth.ToString("0.0")} => {targetUpgradeInfo.m_halfWidth.ToString("0.0")} </color>");
                                }
                                if (!isSameHasStop)
                                {
                                    m_mustConfirm = true;
                                    observations.Add($"<color red>{Locale.Get("K45_TTT_DIFFERENTHASSTOP")}: {Locale.Get("K45_TTT_" + (oldHasStop ? "HASSTOP" : "NOSTOP"))} => {Locale.Get("K45_TTT_" + (newHasStop ? "HASSTOP" : "NOSTOP"))}</color>");
                                }
                                if (!isSameSegElvType)
                                {
                                    observations.Add($"<color yellow>{Locale.Get("K45_TTT_DIFFERENTELEVATIONTYPE")}: {Locale.Get("K45_TTT_ELEVATIONTYPE", m_oldType.ToString())} =>  {Locale.Get("K45_TTT_ELEVATIONTYPE", m_effectiveTargetType.ToString())}</color>");
                                }
                                if (!isSameSubService)
                                {
                                    observations.Add($"<color yellow>{Locale.Get("K45_TTT_DIFFERENTSUBSERVICE")}: { oldInfo.m_class.m_subService} =>  {targetUpgradeInfo.m_class.m_subService}</color>");
                                }

                                m_cachedInfoCompareText = string.Join("\n", observations.ToArray());
                            }
                            base.ShowToolInfo(true, (text + "\n" + m_cachedInfoCompareText).Trim(), m_raycastHit);
                        }
                        else
                        {
                            base.ShowToolInfo(true, text, m_raycastHit);
                        }
                    }
                    else
                    {
                        base.ShowToolInfo(true, text, m_raycastHit);
                    }
                }
                else
                {
                    base.ShowToolInfo(false, null, Vector3.zero);
                }
            }
            else
            {
                base.ShowToolInfo(false, null, Vector3.zero);
            }
            TTTPanel.Instance.m_currentlyUpgradingLabel.suffix = m_upgradeAI?.RelativeTo(m_targetType == ElevationType.Default ? m_oldType : m_targetType)?.GetUncheckedLocalizedTitle() ?? Locale.Get("K45_TTT_NONESELECTED");
        }

        private IEnumerator<bool> CreateNode(bool switchDirection, ushort buildingStart, ushort buildingEnd)
        {
            if (buildingStart != 0)
            {
                BuildingManager.instance.ReleaseBuilding(buildingStart);
            }
            if (buildingEnd != 0)
            {
                BuildingManager.instance.ReleaseBuilding(buildingEnd);
            }
            yield return CreateNodeImpl(switchDirection);
            yield break;
        }
        private bool CreateNodeImpl(bool switchDirection)
        {
            NetInfo prefab = switchDirection ? m_oldAI.RelativeTo(m_oldType) : m_upgradeAI?.RelativeTo(m_targetType == ElevationType.Default ? m_oldType : m_targetType);
            if (prefab != null)
            {
                if (m_toolMode == Mode.Upgrade && m_controlPointCount < 2)
                {
                    prefab.m_netAI.UpgradeFailed();
                }
                else
                {

                    NetTool.ControlPoint controlPoint;
                    NetTool.ControlPoint controlPoint2;
                    NetTool.ControlPoint controlPoint3;
                    if (m_controlPointCount == 1)
                    {
                        controlPoint = m_controlPoints[0];
                        controlPoint2 = m_controlPoints[1];
                        controlPoint3 = m_controlPoints[1];
                        controlPoint3.m_node = 0;
                        controlPoint3.m_segment = 0;
                        controlPoint3.m_position = (m_controlPoints[0].m_position + m_controlPoints[1].m_position) * 0.5f;
                        controlPoint3.m_elevation = (m_controlPoints[0].m_elevation + m_controlPoints[1].m_elevation) * 0.5f;
                    }
                    else
                    {
                        controlPoint = m_controlPoints[0];
                        controlPoint3 = m_controlPoints[1];
                        controlPoint2 = m_controlPoints[2];
                    }
                    NetTool.ControlPoint startPoint = controlPoint;
                    NetTool.ControlPoint middlePoint = controlPoint3;
                    NetTool.ControlPoint endPoint = controlPoint2;
                    bool secondaryControlPoints = GetSecondaryControlPoints(prefab, ref startPoint, ref middlePoint, ref endPoint);
                    if (CreateNodeImpl(prefab, false, switchDirection, controlPoint, controlPoint3, controlPoint2))
                    {
                        if (secondaryControlPoints)
                        {
                            CreateNodeImpl(prefab, false, switchDirection, startPoint, middlePoint, endPoint);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CreateNodeImpl(NetInfo info, bool needMoney, bool switchDirection, NetTool.ControlPoint startPoint, NetTool.ControlPoint middlePoint, NetTool.ControlPoint endPoint)
        {

            NetTool.CreateNode(info, startPoint, middlePoint, endPoint, NetTool.m_nodePositionsSimulation, 1000, false, false, true, needMoney, false, switchDirection, 0, out ushort nodeId, out ushort segmentId, out _, out _);
            NetManager instance = Singleton<NetManager>.instance;
            endPoint.m_segment = 0;
            endPoint.m_node = nodeId;
            ushort otherNode = 0;
            if (segmentId != 0)
            {
                otherNode = instance.m_segments.m_buffer[segmentId].GetOtherNode(nodeId);
                instance.m_nodes.m_buffer[nodeId].m_flags |= NetNode.Flags.Untouchable;
                instance.m_nodes.m_buffer[otherNode].m_flags |= NetNode.Flags.Untouchable;
                instance.m_segments.m_buffer[segmentId].m_flags |= NetSegment.Flags.Untouchable;
                if (instance.m_segments.m_buffer[segmentId].m_startNode == nodeId)
                {
                    endPoint.m_direction = -instance.m_segments.m_buffer[segmentId].m_startDirection;
                }
                else if (instance.m_segments.m_buffer[segmentId].m_endNode == nodeId)
                {
                    endPoint.m_direction = -instance.m_segments.m_buffer[segmentId].m_endDirection;
                }
            }
            m_controlPoints[0] = endPoint;
            m_controlPointCount = 1;

            if (info.m_class.m_service == ItemClass.Service.Road)
            {
                Singleton<CoverageManager>.instance.CoverageUpdated(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
            }
            if ((info.m_class.m_service == ItemClass.Service.Road || info.m_class.m_service == ItemClass.Service.PublicTransport || info.m_class.m_service == ItemClass.Service.Beautification) && (info.m_hasForwardVehicleLanes || info.m_hasBackwardVehicleLanes) && (!info.m_hasForwardVehicleLanes || !info.m_hasBackwardVehicleLanes))
            {
                Singleton<NetManager>.instance.m_onewayRoadPlacement.Disable();
            }
            info.m_netAI.UpgradeSucceeded();

            if (info.m_netAI is DamAI damAi)
            {
                instance.m_nodes.m_buffer[nodeId].m_flags |= NetNode.Flags.Middle;
                instance.m_nodes.m_buffer[otherNode].m_flags |= NetNode.Flags.Middle;

            }

            return true;
        }
        private static bool GetSecondaryControlPoints(NetInfo info, ref NetTool.ControlPoint startPoint, ref NetTool.ControlPoint middlePoint, ref NetTool.ControlPoint endPoint)
        {
            ushort num = middlePoint.m_segment;
            if (startPoint.m_segment == num || endPoint.m_segment == num)
            {
                num = 0;
            }
            ushort num2 = 0;
            if (num != 0)
            {
                num2 = DefaultTool.FindSecondarySegment(num);
            }
            if (num2 != 0)
            {
                NetManager instance = Singleton<NetManager>.instance;
                startPoint.m_node = instance.m_segments.m_buffer[num2].m_startNode;
                startPoint.m_segment = 0;
                startPoint.m_position = instance.m_nodes.m_buffer[startPoint.m_node].m_position;
                startPoint.m_direction = instance.m_segments.m_buffer[num2].m_startDirection;
                startPoint.m_elevation = instance.m_nodes.m_buffer[startPoint.m_node].m_elevation;
                if (instance.m_nodes.m_buffer[startPoint.m_node].Info.m_netAI.IsUnderground())
                {
                    startPoint.m_elevation = -startPoint.m_elevation;
                }
                startPoint.m_outside = ((instance.m_nodes.m_buffer[startPoint.m_node].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None);
                endPoint.m_node = instance.m_segments.m_buffer[num2].m_endNode;
                endPoint.m_segment = 0;
                endPoint.m_position = instance.m_nodes.m_buffer[endPoint.m_node].m_position;
                endPoint.m_direction = -instance.m_segments.m_buffer[num2].m_endDirection;
                endPoint.m_elevation = instance.m_nodes.m_buffer[endPoint.m_node].m_elevation;
                if (instance.m_nodes.m_buffer[endPoint.m_node].Info.m_netAI.IsUnderground())
                {
                    endPoint.m_elevation = -endPoint.m_elevation;
                }
                endPoint.m_outside = ((instance.m_nodes.m_buffer[endPoint.m_node].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None);
                if ((instance.m_segments.m_buffer[num2].m_flags & NetSegment.Flags.Collapsed) != NetSegment.Flags.None)
                {
                    info = instance.m_segments.m_buffer[num2].Info;
                }
                middlePoint.m_node = 0;
                middlePoint.m_segment = num2;
                middlePoint.m_position = startPoint.m_position + startPoint.m_direction * (info.GetMinNodeDistance() + 1f);
                middlePoint.m_direction = startPoint.m_direction;
                middlePoint.m_elevation = Mathf.Lerp(startPoint.m_elevation, endPoint.m_elevation, 0.5f);
                middlePoint.m_outside = false;
                return true;
            }
            return false;
        }
    }

}
