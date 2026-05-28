using System;
using System.Collections.Generic;
using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DialogueTweak;

public partial class DialogueTweak : Mod
{
    public override object Call(params object[] args)
    {
        try
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
            }

            if (args.Length == 0)
            {
                throw new ArgumentException("Arguments cannot be empty!");
            }

            if (args[0] is string msg)
            {
                switch (msg)
                {
                    case "ReplaceButtonIcon":
                        {
                            string interactionTypeName = args[1] as string;
                            var iconInfo = new IconInfo(
                                interactionTypeName,
                                AsListOfInt(args[2]),
                                AsFuncString(args[3])
                            );
                            if (args.Length > 4)
                                iconInfo.Available = args[4] as Func<bool>;
                            if (args.Length > 5)
                                iconInfo.Frame = args[5] as Func<Rectangle>;
                            if (args.Length > 6)
                                iconInfo.CustomOffset = args[6] as Func<float>;
                            HandleAssets.IconInfos.Add(iconInfo);
                            return true;
                        }
                    case "OnPostPortraitDraw":
                        {
                            PortraitDrawer.OnPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle>;
                            return true;
                        }
                    case "OnPreNPCPortraitDraw":
                        {
                            PortraitDrawer.OnPreNPCPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, NPC>;
                            return true;
                        }
                    case "OnPostNPCPortraitDraw":
                        {
                            PortraitDrawer.OnPostNPCPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, NPC>;
                            return true;
                        }
                    case "OnPreSignPortraitDraw":
                        {
                            PortraitDrawer.OnPreSignPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, int>;
                            return true;
                        }
                    case "OnPostSignPortraitDraw":
                        {
                            PortraitDrawer.OnPostSignPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, int>;
                            return true;
                        }
                    case "DisablePanelRework":
                        {
                            int npcId = Convert.ToInt32(args[1]);
                            var condition = args[2] as Func<bool>;
                            DialogueTweakSystem.ReworkDisableConditions[npcId] = condition;
                            return true;
                        }
                    default:
                        Logger.Error($"Replacement type \"{msg}\" not found.");
                        return false;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error($"{e.StackTrace} {e.Message}");
        }

        return false;
    }

    private static List<int> AsListOfInt(object data) => data as List<int> ?? [Convert.ToInt32(data)];

    private static Func<string> AsFuncString(object data) => data as Func<string> ?? (() => data as string);
}
