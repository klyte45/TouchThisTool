using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.TouchThis.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Klyte.TouchThis.TextureAtlas.TTTCommonTextureAtlas;

namespace Klyte.TouchThis.TextureAtlas
{
    public class TTTCommonTextureAtlas : TextureAtlasDescriptor<TTTCommonTextureAtlas, TTTResourceLoader, SpriteNames>
    {
        protected override string ResourceName => "UI.Images.sprites.png";
        protected override string CommonName => "TouchThisSprites";
        public enum SpriteNames  {
                    TouchThisIcon,TouchThisIconSmall,ToolbarIconGroup6Hovered,ToolbarIconGroup6Focused
                }
    }
}
