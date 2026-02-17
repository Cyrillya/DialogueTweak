using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ReLogic.Content;

using Terraria;

namespace DialogueTweak.Port;

// 来自 1.4.5 的 NPCID.cs 的代码
internal class NewSourceCode
{
    public static Dictionary<int, NPCPortraitProvider> NPCPortraits = new Dictionary<int, NPCPortraitProvider>
        {
            {
                369,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Angler_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Angler"))
            },
            {
                19,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_ArmsDealer_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_ArmsDealer"))
            },
            {
                54,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Clothier_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Clothier"))
            },
            {
                209,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cyborg_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cyborg"))
            },
            {
                38,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Demolitionist_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Demolitionist"))
            },
            {
                20,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dryad_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dryad"))
            },
            {
                207,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_DyeTrader_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_DyeTrader"))
            },
            {
                107,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_GoblinTinkerer_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_GoblinTinkerer"))
            },
            {
                588,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Golfer_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Golfer"))
            },
            {
                22,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Guide_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Guide"))
            },
            {
                124,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Mechanic_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Mechanic"))
            },
            {
                17,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Merchant_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Merchant"))
            },
            {
                18,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Nurse_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Nurse"))
            },
            {
                37,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_OldMan_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_OldMan"))
            },
            {
                227,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Painter_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Painter"))
            },
            {
                208,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_PartyGirl_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_PartyGirl"))
            },
            {
                229,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Pirate_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Pirate"))
            },
            {
                663,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Princess_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Princess"))
            },
            {
                142,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Santa_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Santa"))
            },
            {
                453,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_SkeletonMerchant_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SkeletonMerchant"))
            },
            {
                178,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Steampunker_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Steampunker"))
            },
            {
                353,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Stylist_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Stylist"))
            },
            {
                550,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Tavernkeep_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Tavernkeep"))
            },
            {
                441,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_TaxCollector_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_TaxCollector"))
            },
            {
                368,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_TravellingMerchant_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_TravellingMerchant"))
            },
            {
                160,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Truffle_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Truffle"))
            },
            {
                228,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_WitchDoctor_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_WitchDoctor"))
            },
            {
                108,
                PrioritizedPortrait().With(ShimmeredPortraitCondition, BasicPortrait("Images/TownNPCs/Portraits/Portrait_Wizard_shimmer")).Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Wizard"))
            },
            {
                633,
                PrioritizedPortrait().With(() => ShimmeredPortraitCondition() && !ShouldBestiaryGirlBeLycantrope(), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Zoologista_shimmer")).With(() => ShimmeredPortraitCondition() && ShouldBestiaryGirlBeLycantrope(), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Zoologistb_shimmer")).With(() => !ShimmeredPortraitCondition() && ShouldBestiaryGirlBeLycantrope(), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Zoologistb"))
                    .Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Zoologista"))
            },
            {
                680,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeClumsy"))
            },
            {
                678,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeCool"))
            },
            {
                681,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeDiva"))
            },
            {
                679,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeElder"))
            },
            {
                683,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeMystic"))
            },
            {
                670,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeNerdy"))
            },
            {
                684,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeSquire"))
            },
            {
                682,
                PrioritizedPortrait().Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_SlimeSurly"))
            },
            {
                638,
                PrioritizedPortrait().With(VariantPortraitCondition(0), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_Labrador")).With(VariantPortraitCondition(1), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_PitBull")).With(VariantPortraitCondition(2), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_Beagle"))
                    .With(VariantPortraitCondition(3), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_Corgi"))
                    .With(VariantPortraitCondition(4), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_Dalmatian"))
                    .With(VariantPortraitCondition(5), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_Husky"))
                    .Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Dog_Labrador"))
            },
            {
                637,
                PrioritizedPortrait().With(VariantPortraitCondition(0), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_Siamese")).With(VariantPortraitCondition(1), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_Black")).With(VariantPortraitCondition(2), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_Orangetabby"))
                    .With(VariantPortraitCondition(3), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_RussianBlue"))
                    .With(VariantPortraitCondition(4), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_Silver"))
                    .With(VariantPortraitCondition(5), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_White"))
                    .Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Cat_Black"))
            },
            {
                656,
                PrioritizedPortrait().With(VariantPortraitCondition(0), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_White")).With(VariantPortraitCondition(1), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_Angora")).With(VariantPortraitCondition(2), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_Dutch"))
                    .With(VariantPortraitCondition(3), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_Flemish"))
                    .With(VariantPortraitCondition(4), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_Lop"))
                    .With(VariantPortraitCondition(5), BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_Silver"))
                    .Default(BasicPortrait("Images/TownNPCs/Portraits/Portrait_Bunny_Angora"))
            }
        };

    public static Dictionary<int, Vector2> NPCPortraitsCloseUpOffsets = new Dictionary<int, Vector2>
        {
            {
                17,
                new Vector2(6f, 0f)
            },
            {
                18,
                new Vector2(-6f, 0f)
            },
            {
                19,
                new Vector2(0f, 0f)
            },
            {
                20,
                new Vector2(0f, 0f)
            },
            {
                22,
                new Vector2(0f, 0f)
            },
            {
                38,
                new Vector2(0f, -6f)
            },
            {
                54,
                new Vector2(6f, 0f)
            },
            {
                37,
                new Vector2(0f, 0f)
            },
            {
                107,
                new Vector2(0f, 0f)
            },
            {
                108,
                new Vector2(6f, 4f)
            },
            {
                124,
                new Vector2(-6f, 4f)
            },
            {
                142,
                new Vector2(3f, 6f)
            },
            {
                160,
                new Vector2(-6f, 18f)
            },
            {
                178,
                new Vector2(-6f, 12f)
            },
            {
                207,
                new Vector2(0f, 0f)
            },
            {
                208,
                new Vector2(-12f, 0f)
            },
            {
                209,
                new Vector2(0f, 6f)
            },
            {
                227,
                new Vector2(-3f, 0f)
            },
            {
                228,
                new Vector2(9f, 6f)
            },
            {
                229,
                new Vector2(0f, 0f)
            },
            {
                353,
                new Vector2(-6f, 6f)
            },
            {
                368,
                new Vector2(0f, 6f)
            },
            {
                369,
                new Vector2(-6f, -6f)
            },
            {
                441,
                new Vector2(-9f, 0f)
            },
            {
                453,
                new Vector2(-12f, 0f)
            },
            {
                550,
                new Vector2(15f, 12f)
            },
            {
                588,
                new Vector2(3f, 0f)
            },
            {
                633,
                new Vector2(-3f, 0f)
            },
            {
                663,
                new Vector2(0f, -6f)
            },
            {
                637,
                new Vector2(-15f, 8f)
            },
            {
                638,
                new Vector2(-24f, 12f)
            },
            {
                656,
                new Vector2(0f, 0f)
            },
            {
                684,
                new Vector2(-3f, 2f)
            },
            {
                670,
                new Vector2(0f, 2f)
            },
            {
                678,
                new Vector2(-3f, 2f)
            },
            {
                679,
                new Vector2(0f, 2f)
            },
            {
                680,
                new Vector2(-6f, 2f)
            },
            {
                681,
                new Vector2(0f, 2f)
            },
            {
                683,
                new Vector2(-6f, 2f)
            },
            {
                682,
                new Vector2(-3f, 2f)
            }
        };

    public static Dictionary<int, Vector2> NPCPortraitsFullBodyRetroOffsets = new Dictionary<int, Vector2>
        {
            {
                18,
                new Vector2(-3f, 0f)
            },
            {
                20,
                new Vector2(-4f, 0f)
            },
            {
                124,
                new Vector2(-6f, 2f)
            },
            {
                178,
                new Vector2(-4f, 0f)
            },
            {
                208,
                new Vector2(-4f, 0f)
            },
            {
                227,
                new Vector2(6f, 0f)
            },
            {
                353,
                new Vector2(-4f, 0f)
            },
            {
                369,
                new Vector2(-4f, 0f)
            },
            {
                441,
                new Vector2(-10f, 0f)
            },
            {
                588,
                new Vector2(-2f, 0f)
            },
            {
                633,
                new Vector2(-6f, 0f)
            },
            {
                637,
                new Vector2(-8f, 0f)
            },
            {
                638,
                new Vector2(-8f, 0f)
            },
            {
                684,
                new Vector2(-12f, 0f)
            },
            {
                670,
                new Vector2(-6f, 0f)
            },
            {
                678,
                new Vector2(-8f, 0f)
            },
            {
                679,
                new Vector2(-6f, 0f)
            },
            {
                680,
                new Vector2(-10f, 0f)
            },
            {
                681,
                new Vector2(-6f, 0f)
            },
            {
                683,
                new Vector2(-6f, 0f)
            },
            {
                682,
                new Vector2(-6f, 0f)
            }
        };

    public static bool ShouldBestiaryGirlBeLycantrope()
    {
        if (!Main.bloodMoon || Main.dayTime)
        {
            if (Main.moonPhase == 0)
            {
                return !Main.dayTime;
            }
            return false;
        }
        return true;
    }

    public static NPCPortraitSelector PrioritizedPortrait()
    {
        return new NPCPortraitSelector();
    }

    public static BasicNPCPortrait BasicPortrait(string texturePath)
    {
        return new BasicNPCPortrait(texturePath);
    }

    public static NPCPortraitSelector.SelectionCondition VariantPortraitCondition(int variantIndex)
    {
        return new NPCVariantChecker(variantIndex).Fits;
    }

    public static bool ShimmeredPortraitCondition()
    {
        int talkNPC = Main.LocalPlayer.talkNPC;
        if (talkNPC < 0 || talkNPC >= Main.maxNPCs)
        {
            return false;
        }
        return Main.npc[talkNPC].IsShimmerVariant;
    }

    public interface NPCPortraitProvider
    {
        void GetDrawData(out Texture2D texture, out Rectangle drawFrame);
    }

    public class BasicNPCPortrait : NPCPortraitProvider
    {
        public string TexturePath;

        public int HorizontalFrames;

        public int VerticalFrames;

        public int PaddingX;

        public int PaddingY;

        public int FrameX;

        public int FrameY;

        private Asset<Texture2D> _image;

        public BasicNPCPortrait(string texturePath)
        {
            TexturePath = texturePath;
            HorizontalFrames = (VerticalFrames = 1);
            PaddingX = (PaddingY = 0);
            FrameX = (FrameY = 0);
        }

        public virtual void GetDrawData(out Texture2D texture, out Rectangle drawFrame)
        {
            if (_image == null)
            {
                // _image = Main.Assets.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad);
                _image = NewAssetsHandler.Request(TexturePath);
            }
            texture = null;
            if (_image.IsLoaded)
            {
                texture = _image.Value;
            }
            drawFrame = texture.Frame(HorizontalFrames, VerticalFrames, FrameX, FrameY, PaddingX, PaddingY);
        }
    }

    public class NPCPortraitSelector : NPCPortraitProvider
    {
        public delegate bool SelectionCondition();

        public struct Entry
        {
            public SelectionCondition Condition;

            public NPCPortraitProvider Portrait;
        }

        private List<Entry> _entries = new List<Entry>();

        public NPCPortraitSelector With(SelectionCondition condition, NPCPortraitProvider portrait)
        {
            _entries.Add(new Entry
            {
                Condition = condition,
                Portrait = portrait
            });
            return this;
        }

        public NPCPortraitSelector Default(NPCPortraitProvider portrait)
        {
            _entries.Add(new Entry
            {
                Condition = () => true,
                Portrait = portrait
            });
            return this;
        }

        public void GetDrawData(out Texture2D texture, out Rectangle drawFrame)
        {
            texture = null;
            drawFrame = default(Rectangle);
            GetFirstValidatedEntry()?.GetDrawData(out texture, out drawFrame);
        }

        private NPCPortraitProvider GetFirstValidatedEntry()
        {
            foreach (Entry entry in _entries)
            {
                if (entry.Condition())
                {
                    return entry.Portrait;
                }
            }
            return null;
        }
    }

    public class NPCVariantChecker
    {
        public int VariantToCheck;

        public NPCVariantChecker(int variantToCheck)
        {
            VariantToCheck = variantToCheck;
        }

        public bool Fits()
        {
            int talkNPC = Main.LocalPlayer.talkNPC;
            if (talkNPC < 0 || talkNPC >= Main.maxNPCs)
            {
                return false;
            }
            return Main.npc[talkNPC].townNpcVariationIndex == VariantToCheck;
        }
    }
}
