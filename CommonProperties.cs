using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.TouchThis;
using Klyte.TouchThis.TextureAtlas;
using Klyte.TouchThis.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Klyte.TouchThis.TextureAtlas.TTTCommonTextureAtlas;

namespace Klyte.Commons
{
    public static class CommonProperties 
    {
        public static bool DebugMode => TouchThisToolMod.DebugMode;
        public static string Version => TouchThisToolMod.Version;
        public static string ModName => TouchThisToolMod.Instance.SimpleName;        
        public static object Acronym => "TTT";
    }
}