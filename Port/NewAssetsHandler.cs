using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using ReLogic.Content;

using Terraria.ModLoader;

namespace DialogueTweak.Port;

internal class NewAssetsHandler
{
    // 请求1.4.5的资源
    internal static Asset<Texture2D> Request(string path, AssetRequestMode mode = AssetRequestMode.ImmediateLoad)
    {
        string pathVanilla = $"Terraria/{path}";
        string pathMod = $"DialogueTweak/Port/{path}";
        if (ModContent.RequestIfExists(pathVanilla, out Asset<Texture2D> asset, mode))
            return asset;

        return ModContent.Request<Texture2D>(pathMod, mode);
    }
}