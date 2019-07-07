using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.TouchThis.TextureAtlas;
using Klyte.TouchThis.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Klyte.TouchThis.TextureAtlas.TTTCommonTextureAtlas;

[assembly: AssemblyVersion("3.0.0.0")]
namespace Klyte.TouchThis
{
    public class TouchThisToolMod : BasicIUserMod<TouchThisToolMod, TTTResourceLoader, TTTController, TTTCommonTextureAtlas, UICustomControl, SpriteNames>
    {
        public TouchThisToolMod()
        {
            Construct();
        }

        public override string SimpleName => "Touch This! Tool";

        public override string Description => "Tool for unlocking freeze segments to update and/or delete them. Util for buildings' roads.";

        public override void DoErrorLog(string fmt, params object[] args)
        {
            LogUtils.DoErrorLog(fmt, args);
        }

        public override void DoLog(string fmt, params object[] args)
        {
            LogUtils.DoLog(fmt, args);
        }

        public override void LoadSettings()
        {
        }

        public override void TopSettingsUI(UIHelperExtension ext)
        {
        }
    }
}