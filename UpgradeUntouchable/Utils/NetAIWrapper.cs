using System;
using System.Collections.Generic;
using System.Reflection;

namespace UpgradeUntouchable.Utils
{
    public class NetAIWrapper : IEquatable<NetAIWrapper>
    {
        public NetAI AI { get; }
        private FieldInfo m_elevated;
        private FieldInfo m_bridge;
        private FieldInfo m_slope;
        private FieldInfo m_tunnel;
        private FieldInfo m_invisible;
        private ElevationType m_defaultElevationType;

        public NetAIWrapper(NetAI ai)
        {
            AI = ai;

            try
            {
                m_elevated = AI.GetType().GetField("m_elevatedInfo");
                m_bridge = AI.GetType().GetField("m_bridgeInfo");
                m_slope = AI.GetType().GetField("m_slopeInfo");
                m_tunnel = AI.GetType().GetField("m_tunnelInfo");
                m_invisible = AI.GetType().GetField("m_invisible");
            }
            catch
            {
                m_elevated = null;
                m_bridge = null;
                m_slope = null;
                m_tunnel = null;
                m_invisible = null;
            }
            m_defaultElevationType = ClassifyInfo(Default);
        }

        public bool HasElevation => m_elevated != null && m_bridge != null && m_slope != null && m_tunnel != null;

        public NetInfo Default
        {
            get => AI.m_info;
            set => AI.m_info = value;
        }

        public NetInfo Elevated
        {
            get => HasElevation ? m_elevated.GetValue(AI) as NetInfo : m_defaultElevationType == ElevationType.Elevated ? Default : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_elevated.SetValue(AI, value);
            }
        }

        public NetInfo Bridge
        {
            get => HasElevation ? m_bridge.GetValue(AI) as NetInfo : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_bridge.SetValue(AI, value);
            }
        }

        public NetInfo Slope
        {
            get => HasElevation ? m_slope.GetValue(AI) as NetInfo : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_slope.SetValue(AI, value);
            }
        }

        public NetInfo Tunnel
        {
            get => HasElevation ? m_tunnel.GetValue(AI) as NetInfo : m_defaultElevationType == ElevationType.Tunnel ? Default : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_tunnel.SetValue(AI, value);
            }
        }


        public bool IsInvisible() => m_invisible != null ? (bool)m_invisible.GetValue(AI) : false;


        public enum ElevationType
        {
            None = -1,
            Default,
            Ground,
            Elevated,
            Bridge,
            Tunnel,
            Slope
        }

        internal NetInfo RelativeTo(ElevationType elevationType, bool strict = false)
        {
            if (!HasElevation)
            {
                return elevationType == m_defaultElevationType || elevationType == ElevationType.Default ? Default : null;
            }
            switch (elevationType == ElevationType.Default ? m_defaultElevationType : elevationType)
            {
                default:
                case ElevationType.None:
                    return null;
                case ElevationType.Ground:
                    return m_defaultElevationType == ElevationType.Ground || !strict ? Default : null;
                case ElevationType.Tunnel:
                    return Tunnel ?? (strict ? null : Default);
                case ElevationType.Elevated:
                    return Elevated ?? (strict ? null : Default);
                case ElevationType.Bridge:
                    return Bridge ?? (strict ? null : Default);
                case ElevationType.Slope:
                    return Slope ?? (strict ? null : Default);
            }
        }

        internal ElevationType ToType(NetInfo oldInfo)
        {
            if (!HasElevation)
            {
                return ClassifyInfo(oldInfo);
            }

            if (oldInfo == Default)
            {
                return ElevationType.Ground;
            }
            if (oldInfo == Tunnel)
            {
                return ElevationType.Tunnel;
            }
            if (oldInfo == Elevated)
            {
                return ElevationType.Elevated;
            }
            if (oldInfo == Bridge)
            {
                return ElevationType.Bridge;
            }
            if (oldInfo == Slope)
            {
                return ElevationType.Slope;
            }
            return ElevationType.None;
        }

        private ElevationType ClassifyInfo(NetInfo oldInfo) => oldInfo.m_netAI.IsUnderground() ? ElevationType.Tunnel : oldInfo.m_clipTerrain ? ElevationType.Ground : ElevationType.Elevated;


        public override bool Equals(object obj) => Equals(obj as NetAIWrapper);
        public bool Equals(NetAIWrapper other) => other != null && EqualityComparer<NetAI>.Default.Equals(AI, other.AI);
        public override int GetHashCode() => 639214903 + EqualityComparer<NetAI>.Default.GetHashCode(AI);
        public static bool operator ==(NetAIWrapper left, NetAIWrapper right) => EqualityComparer<NetAIWrapper>.Default.Equals(left, right);
        public static bool operator !=(NetAIWrapper left, NetAIWrapper right) => !(left == right);
    }
}