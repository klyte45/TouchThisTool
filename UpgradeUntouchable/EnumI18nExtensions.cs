using Kwytto.Localization;
using System;
using System.Linq;
using UpgradeUntouchable.Localization;
using static UpgradeUntouchable.Utils.NetAIWrapper;

namespace UpgradeUntouchable
{
    internal static class EnumI18nExtensions
    {
        public static string ValueToI18n(this Enum variable)
        {
            switch (variable)
            {
                case ElevationType et:
                    switch (et)
                    {
                        case ElevationType.None: return Str.UU_ELEVATIONTYPE__None;
                        case ElevationType.Default: return Str.UU_ELEVATIONTYPE__Default;
                        case ElevationType.Ground: return Str.UU_ELEVATIONTYPE__Ground;
                        case ElevationType.Elevated: return Str.UU_ELEVATIONTYPE__Elevated;
                        case ElevationType.Bridge: return Str.UU_ELEVATIONTYPE__Bridge;
                        case ElevationType.Tunnel: return Str.UU_ELEVATIONTYPE__Tunnel;
                        case ElevationType.Slope: return Str.UU_ELEVATIONTYPE__Slope;
                    }
                    break;
            }
            return variable.ValueToI18nKwytto();
        }

        public static string[] GetAllValuesI18n<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => x.ValueToI18n()).ToArray();
    }
}
