using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.TouchThis.i18n;
using Klyte.TouchThis.TextureAtlas;
using Klyte.TouchThis.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.2.9999")]
namespace Klyte.TouchThis
{
    public class TouchThisToolMod : BasicIUserMod<TouchThisToolMod, TTTLocaleUtils, TTTResourceLoader, TTTController, TTTCommonTextureAtlas, UICustomControl>
    {
        public TouchThisToolMod()
        {
            Construct();
        }

        public override string SimpleName => "Touch This! Tool";

        public override string Description => "Tool for unlocking freeze segments to update and/or delete them. Util for buildings' roads.";

        public override void doErrorLog(string fmt, params object[] args)
        {
            TTTUtils.doErrorLog(fmt, args);
        }

        public override void doLog(string fmt, params object[] args)
        {
            TTTUtils.doLog(fmt, args);
        }

        public override void LoadSettings()
        {
        }

        public override void TopSettingsUI(UIHelperExtension ext)
        {
        }
    }
}