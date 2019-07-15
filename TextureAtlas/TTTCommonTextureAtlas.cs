using Klyte.Commons.Interfaces;
using Klyte.TouchThis.Utils;
using static Klyte.TouchThis.TextureAtlas.TTTCommonTextureAtlas;

namespace Klyte.TouchThis.TextureAtlas
{
    public class TTTCommonTextureAtlas : TextureAtlasDescriptor<TTTCommonTextureAtlas, TTTResourceLoader, SpriteNames>
    {
        protected override string ResourceName => "UI.Images.sprites.png";
        protected override string CommonName => "TouchThisSprites";
        public enum SpriteNames
        {
            TouchThisIcon, TouchThisIconSmall, ToolbarIconGroup6Hovered, ToolbarIconGroup6Focused
        }
    }
}
