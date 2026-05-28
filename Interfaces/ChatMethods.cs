using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak.Interfaces;

public static class ChatMethods
{
    public static void GetNPCPreferenceSorted(this NPC npc, out List<NPCPreferenceTrait> NPCPreferences,
        out List<BiomePreferenceListTrait> BiomePreferences) {
        var personalityProfile = Main.ShopHelper._database.GetOrCreateProfileByNPCID(npc.type);
        var shopModifiers = personalityProfile.ShopModifiers;
        // NPC和环境偏好表
        List<IShopPersonalityTrait> INPCPreferences = shopModifiers.Where(t => t is NPCPreferenceTrait).ToList();
        List<IShopPersonalityTrait> IBiomePreferences =
            shopModifiers.Where(t => t is BiomePreferenceListTrait).ToList();
        NPCPreferences = null;
        if (INPCPreferences != null) {
            NPCPreferences = INPCPreferences.ConvertAll(t => t as NPCPreferenceTrait);
            NPCPreferences.Sort(NPCLevelComparison);
        }

        BiomePreferences = null;
        if (IBiomePreferences != null) {
            BiomePreferences = IBiomePreferences.ConvertAll(t => t as BiomePreferenceListTrait);
            BiomePreferences.ForEach(t => t.Preferences.Sort(BiomeLevelComparison));
        }

        static int NPCLevelComparison(NPCPreferenceTrait p1, NPCPreferenceTrait p2) => p2.Level.CompareTo(p1.Level);

        static int BiomeLevelComparison(BiomePreferenceListTrait.BiomePreference p1,
            BiomePreferenceListTrait.BiomePreference p2) => p2.Affection.CompareTo(p1.Affection);
    }

    // 自己写的控制NPC语速（实际效果进游戏看
    public static float HandleSpeakingRate(int npcType) {
        switch (npcType) {
            case NPCID.None:
                return 1.0f;
            case NPCID.Merchant:
                return 0.5f;
            case NPCID.Nurse:
                return 0.6f;
            case NPCID.ArmsDealer:
                return 0.6f;
            case NPCID.Dryad:
                return 0.7f;
            case NPCID.Guide:
                return 1.0f;
            case NPCID.Demolitionist:
                return 0.9f;
            case NPCID.Clothier:
                return 0.7f;
            case NPCID.GoblinTinkerer:
                return 0.8f;
            case NPCID.Wizard:
                return 0.7f;
            case NPCID.Mechanic:
                return 0.8f;
            case NPCID.SantaClaus:
                return 0.6f;
            case NPCID.Truffle:
                return 0.9f;
            case NPCID.Steampunker:
                return 0.9f;
            case NPCID.DyeTrader:
                return 0.8f;
            case NPCID.PartyGirl:
                return 1.1f;
            case NPCID.Cyborg:
                return 1.0f;
            case NPCID.Painter:
                return 0.9f;
            case NPCID.WitchDoctor:
                return 0.7f;
            case NPCID.Pirate:
                return 0.8f;
            case NPCID.Stylist:
                return 1.0f;
            case NPCID.TravellingMerchant:
                return 0.8f;
            case NPCID.Angler:
                return 1.1f;
            case NPCID.TaxCollector:
                return 0.7f;
            case NPCID.DD2Bartender:
                return 0.8f;
            case NPCID.Golfer:
                return 0.9f;
            case NPCID.BestiaryGirl:
                return 0.9f;
            case NPCID.Princess:
                return 0.8f;
            default:
                return 0.8f;
        }
    }

    public static void ResolveInteractionIcon(NPCInteraction interaction, int npcType, int head,
        out Asset<Texture2D> texture, out Rectangle frame, out Func<float> customOffset) {
        customOffset = null;
        Rectangle? frameOverride = null;

        if (interaction is NPCInteractions.Actions.OpenShop) {
            texture = ModAsset.Icon_Default;
            if (NPCID.Sets.IsTownPet[npcType]) {
                texture = GetHeadOrDefaultIcon(head);
                if (texture == ModAsset.Icon_Default_Extra)
                    texture = GetHeadOrDefaultIcon(NPC.TypeToDefaultHeadIndex(npcType));
            }
        }
        else if (interaction is NPCInteractions.Actions.OpenSign) {
            texture = ModAsset.Icon_Edit;
        }
        else {
            texture = GetHeadOrDefaultIcon(head);
        }

        string typeName = interaction.GetType().Name;
        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(npcType) && a.Available() && a.Texture != ""
                       && a.InteractionTypeName == typeName
                 select a) {
            info.GetIconParameters(out customOffset, out texture, out frameOverride, head);
        }

        frame = frameOverride ?? texture.Frame();
    }

    public static Asset<Texture2D> GetHeadOrDefaultIcon(int head) {
        if (head > 0 && head < NPCHeadLoader.NPCHeadCount && !NPCHeadID.Sets.CannotBeDrawnInHousingUI[head]) {
            return TextureAssets.NpcHead[head];
        }

        return ModAsset.Icon_Default_Extra;
    }
}