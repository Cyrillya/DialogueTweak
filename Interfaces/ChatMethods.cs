using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;

namespace DialogueTweak.Interfaces;

// 这里基本上就是垃圾堆，都是原版代码（写死了，必须要自己新写一个自己执行的那种）
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

    // 自己写的控制Shop和Extra的贴图
    public static void HandleButtonIcon(int i, out Asset<Texture2D> shop, out Rectangle shopFrame,
        out Func<float> shopCustomOffset, out Asset<Texture2D> extra, out Rectangle extraFrame,
        out Func<float> extraCustomOffset) {
        shopCustomOffset = null;
        extraCustomOffset = null;
        Rectangle? shopFrameOverride = null;
        Rectangle? extraFrameOverride = null;

        // -1即标牌绘制
        if (i == -1) {
            extra = null;
            extraFrame = default;
            shop = ModAsset.Icon_Edit;
            shopFrame = shop.Frame();
            foreach (var info in from a in HandleAssets.IconInfos
                     where a.NPCTypes.Contains(-1) && a.Available() && a.Texture != "" && !a.IsSpecialIcon
                     select a) {
                if (info.IconType == IconType.Shop) {
                    // 这里head传-1没问题，会被GetHeadOrDefaultIcon处理
                    info.GetIconParameters(out shopCustomOffset, out shop, out shopFrameOverride, -1);
                }

                if (info.IconType == IconType.Extra) {
                    info.GetIconParameters(out extraCustomOffset, out extra, out extraFrameOverride, -1);
                }
            }

            shopFrame = shopFrameOverride ?? shop.Frame();
            extraFrame = extraFrameOverride ?? extra?.Frame() ?? default;
            return;
        }

        var npc = Main.npc[Main.LocalPlayer.talkNPC];
        int type = npc.type;
        int head = !TownNPCProfiles.Instance.GetProfile(npc, out var profile)
            ? NPC.TypeToDefaultHeadIndex(type)
            : profile.GetHeadTextureIndex(npc);

        // Shop Default Icon
        shop = ModAsset.Icon_Default;
        if (NPCID.Sets.IsTownPet[type]) {
            shop = GetHeadOrDefaultIcon(head);
            if (shop == ModAsset.Icon_Default_Extra)
                shop = GetHeadOrDefaultIcon(NPC.TypeToDefaultHeadIndex(type));
        }

        // Extra Default Icon
        extra = GetHeadOrDefaultIcon(head);

        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(type) && a.Available() && a.Texture != ""
                 select a) {
            if (info.IconType == IconType.Shop) {
                info.GetIconParameters(out shopCustomOffset, out shop, out shopFrameOverride, head);
            }

            if (info.IconType == IconType.Extra) {
                info.GetIconParameters(out extraCustomOffset, out extra, out extraFrameOverride, head);
            }
        }

        shopFrame = shopFrameOverride ?? shop.Frame();
        extraFrame = extraFrameOverride ?? extra.Frame();
    }

    // 第二按钮被按下的行动
    public static void HandleExtraButtonClicled(NPC talkNPC) {
        if (!NPCLoader.PreChatButtonClicked(false))
            return;

        NPCLoader.OnChatButtonClicked(false);
        if (talkNPC.type == NPCID.Dryad) {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.npcChatText = Lang.GetDryadWorldStatusDialog(out var worldIsEntirelyPure);
            if (Main.CanDryadPlayStardewAnimation(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC])) {
                NPC.PreventJojaColaDialog = true;
                NPC.RerollDryadText = 2;
                Main.LocalPlayer.ConsumeItem(5275, reverseOrder: true);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(144);
                else
                    NPC.HaveDryadDoStardewAnimation();

                Main.npcChatText = Language.GetTextValue("StardewTalk.PlayerGivesCola");
            }
            else if (worldIsEntirelyPure) {
                AchievementsHelper.HandleSpecialEvent(Main.LocalPlayer,
                    AchievementHelperID.Special.PurifyEntireWorld);
            }
        }

        else if (talkNPC.type == NPCID.Guide) {
            Main.playerInventory = true;
            Main.npcChatText = "";
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.InGuideCraftMenu = true;
            UILinkPointNavigator.GoToDefaultPage();
        }
        else if (talkNPC.type == NPCID.GoblinTinkerer) {
            Main.playerInventory = true;
            Main.npcChatText = "";
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.InReforgeMenu = true;
            UILinkPointNavigator.GoToDefaultPage();
        }
        else if (talkNPC.type == NPCID.Stylist) {
            Main.OpenHairWindow();
        }
        else if (talkNPC.type == NPCID.DyeTrader) {
            Main.npcChatCornerItem = 0;
            SoundEngine.PlaySound(SoundID.MenuTick);
            bool gotDye = false;
            int num28 = Main.LocalPlayer.FindItem(ItemID.Sets.ExoticPlantsForDyeTrade);
            if (num28 != -1) {
                Main.LocalPlayer.inventory[num28].stack--;
                if (Main.LocalPlayer.inventory[num28].stack <= 0)
                    Main.LocalPlayer.inventory[num28] = new Item();

                gotDye = true;
                SoundEngine.PlaySound(SoundID.Chat);
                Main.LocalPlayer.GetDyeTraderReward(talkNPC);
            }

            Main.npcChatText = Lang.DyeTraderQuestChat(gotDye);
        }
        else if (talkNPC.type == NPCID.DD2Bartender) {
            SoundEngine.PlaySound(SoundID.MenuTick);
            HelpText();
            Main.npcChatText = Lang.BartenderHelpText(talkNPC);
        }
        else if (talkNPC.type == NPCID.PartyGirl) {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.npcChatText = Language.GetTextValue("PartyGirlSpecialText.Music" + Main.rand.Next(1, 4));
            Main.swapMusic = !Main.swapMusic;
        }
        else if (talkNPC.type == NPCID.Painter) {
            OpenShop(25);
        }
    }

    // NPC对话选项显示字样
    public static void HandleFocusText(ref string focusText, ref string focusText2, ref int money) {
        // 标牌直接特判就行了
        if (Main.LocalPlayer.sign > -1) {
            focusText = (!Main.editSign) ? Lang.inter[48].Value : Lang.inter[47].Value;
            return;
        }

        NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];

        int num4 = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
        for (int j = 0; j < Player.MaxBuffs; j++) {
            int num5 = Main.LocalPlayer.buffType[j];
            if (Main.debuff[num5] && Main.LocalPlayer.buffTime[j] > 60 &&
                (num5 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num5]))
                num4 += 100;
        }

        int health = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
        bool removeDebuffs = true;
        if (NPC.downedGolemBoss)
            num4 *= 200;
        else if (NPC.downedPlantBoss)
            num4 *= 150;
        else if (NPC.downedMechBossAny)
            num4 *= 100;
        else if (Main.hardMode)
            num4 *= 60;
        else if (NPC.downedBoss3 || NPC.downedQueenBee)
            num4 *= 25;
        else if (NPC.downedBoss2)
            num4 *= 10;
        else if (NPC.downedBoss1)
            num4 *= 3;

        if (Main.expertMode)
            num4 *= 2;

        num4 = (int) ((double) num4 * Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);
        if (Main.LocalPlayer.sign > -1) {
            focusText = (!Main.editSign)
                ? Language.GetText("LegacyInterface.48").Value
                : Language.GetText("LegacyInterface.47").Value;
        }
        else if (talkNPC.type == NPCID.Dryad) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            focusText2 = Language.GetText("LegacyInterface.49").Value;
            if (Main.CanDryadPlayStardewAnimation(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC]))
                focusText2 = Language.GetTextValue("StardewTalk.GiveColaButtonText");
        }
        else if (NPCID.Sets.IsTownPet[talkNPC.type]) {
            focusText = Language.GetTextValue("UI.PetTheAnimal");
        }
        else if (talkNPC.type == NPCID.DyeTrader) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            focusText2 = Language.GetText("LegacyInterface.107").Value;
        }
        else if (talkNPC.type == NPCID.SkeletonMerchant) {
            focusText = Language.GetText("LegacyInterface.28").Value;
        }
        else if (talkNPC.type == NPCID.DD2Bartender) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            focusText2 = Language.GetTextValue("UI.BartenderHelp");
        }
        else if (talkNPC.type == NPCID.Golfer) {
            focusText = Language.GetText("LegacyInterface.28").Value;
        }
        else if (talkNPC.type == NPCID.BestiaryGirl) {
            focusText = Language.GetText("LegacyInterface.28").Value;
        }
        else if (talkNPC.type == NPCID.Princess) {
            focusText = Language.GetText("LegacyInterface.28").Value;
        }
        else if (talkNPC.type == NPCID.Stylist) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            focusText2 = Language.GetTextValue("GameUI.HairStyle");
        }
        else if (talkNPC.type == NPCID.TravellingMerchant) {
            focusText = Language.GetText("LegacyInterface.28").Value;
        }
        else if (talkNPC.type == NPCID.Angler) {
            focusText = Language.GetText("LegacyInterface.64").Value;
        }
        else if (talkNPC.type == NPCID.PartyGirl) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            focusText2 = Language.GetTextValue("GameUI.Music");
        }
        else if (talkNPC.type is NPCID.Painter) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            focusText2 = Language.GetTextValue("GameUI.PainterDecor");
        }
        else if (talkNPC.type is NPCID.Merchant or NPCID.ArmsDealer or NPCID.Demolitionist or NPCID.Clothier
                 or NPCID.GoblinTinkerer or NPCID.Wizard or NPCID.Mechanic or NPCID.SantaClaus or NPCID.Truffle
                 or NPCID.Steampunker or NPCID.DyeTrader or NPCID.Cyborg or NPCID.WitchDoctor or NPCID.Pirate) {
            focusText = Language.GetText("LegacyInterface.28").Value;
            if (talkNPC.type == NPCID.GoblinTinkerer)
                focusText2 = Language.GetText("LegacyInterface.19").Value;
        }
        else if (talkNPC.type == NPCID.OldMan) {
            if (!Main.dayTime)
                focusText = Language.GetText("LegacyInterface.50").Value;
        }
        else if (talkNPC.type == NPCID.Guide) {
            focusText = Language.GetText("LegacyInterface.51").Value;
            focusText2 = Language.GetText("LegacyInterface.25").Value;
        }
        else if (talkNPC.type == NPCID.TaxCollector) {
            focusText = Language.GetText("LegacyInterface.89").Value;
            if (Main.LocalPlayer.taxMoney > 0) {
                money = Main.LocalPlayer.taxMoney;
                money = (int) ((double) money / Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);
            }
        }
        else if (talkNPC.type == NPCID.Nurse) {
            int num15 = num4;

            PlayerLoader.ModifyNursePrice(Main.LocalPlayer, talkNPC, health, removeDebuffs, ref num15);

            money = num15;
            focusText = Language.GetText("LegacyInterface.54").Value;
        }

        NPCLoader.SetChatButtons(ref focusText, ref focusText2);
    }

    // 第一按钮被按下的行动
    public static void HandleShop(NPC talkNPC) {
        if (NPCID.Sets.IsTownPet[talkNPC.type]) {
            Main.LocalPlayer.PetAnimal(talkNPC.whoAmI);
            return;
        }

        if (!NPCLoader.PreChatButtonClicked(true))
            return;

        NPCLoader.OnChatButtonClicked(true);

        switch (talkNPC.type) {
            case NPCID.Angler: {
                Main.npcChatCornerItem = 0;
                SoundEngine.PlaySound(SoundID.MenuTick);
                bool flag3 = false;
                if (!Main.anglerQuestFinished && !Main.anglerWhoFinishedToday.Contains(Main.LocalPlayer.name)) {
                    int num19 = Main.LocalPlayer.FindItem(Main.anglerQuestItemNetIDs[Main.anglerQuest]);
                    if (num19 != -1) {
                        Main.LocalPlayer.inventory[num19].stack--;
                        if (Main.LocalPlayer.inventory[num19].stack <= 0)
                            Main.LocalPlayer.inventory[num19] = new Item();

                        flag3 = true;
                        SoundEngine.PlaySound(SoundID.Chat);
                        Main.LocalPlayer.anglerQuestsFinished++;
                        Main.LocalPlayer.GetAnglerReward(talkNPC, Main.anglerQuestItemNetIDs[Main.anglerQuest]);
                    }
                }

                Main.npcChatText = Lang.AnglerQuestChat(flag3);
                if (flag3) {
                    Main.anglerQuestFinished = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.AnglerQuestFinished);
                    else
                        Main.anglerWhoFinishedToday.Add(Main.LocalPlayer.name);

                    AchievementsHelper.HandleAnglerService();
                }

                break;
            }
            case NPCID.OldMan:
                if (Main.netMode == NetmodeID.SinglePlayer)
                    NPC.SpawnSkeletron(Main.myPlayer);
                else
                    NetMessage.SendData(MessageID.MiscDataSync, -1, -1, null, Main.myPlayer, 1f);

                Main.npcChatText = "";
                break;
            case NPCID.Guide:
                SoundEngine.PlaySound(SoundID.MenuTick);
                HelpText();
                break;
            case NPCID.TaxCollector: {
                if (Main.LocalPlayer.taxMoney > 0) {
                    int taxMoney3 = Main.LocalPlayer.taxMoney;
                    taxMoney3 =
                        (int) ((double) taxMoney3 / Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);
                    while (taxMoney3 > 0) {
                        EntitySource_Gift source = new EntitySource_Gift(talkNPC);
                        if (taxMoney3 > 1000000) {
                            int num20 = taxMoney3 / 1000000;
                            taxMoney3 -= 1000000 * num20;
                            int number = Item.NewItem(source, (int) Main.LocalPlayer.position.X,
                                (int) Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height,
                                74, num20);
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);

                            continue;
                        }

                        if (taxMoney3 > 10000) {
                            int num21 = taxMoney3 / 10000;
                            taxMoney3 -= 10000 * num21;
                            int number2 = Item.NewItem(source, (int) Main.LocalPlayer.position.X,
                                (int) Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height,
                                73, num21);
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number2, 1f);

                            continue;
                        }

                        if (taxMoney3 > 100) {
                            int num22 = taxMoney3 / 100;
                            taxMoney3 -= 100 * num22;
                            int number3 = Item.NewItem(source, (int) Main.LocalPlayer.position.X,
                                (int) Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height,
                                72, num22);
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number3, 1f);

                            continue;
                        }

                        int num23 = taxMoney3;
                        if (num23 < 1)
                            num23 = 1;

                        taxMoney3 -= num23;
                        int number4 = Item.NewItem(source, (int) Main.LocalPlayer.position.X,
                            (int) Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height, 71,
                            num23);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number4, 1f);
                    }

                    Main.npcChatText = GetChatDialog(Main.rand.Next(380, 382));
                    Main.LocalPlayer.taxMoney = 0;
                }
                else {
                    Main.npcChatText = GetChatDialog(Main.rand.Next(390, 401));
                }

                break;
            }
            case NPCID.Nurse: {
                SoundEngine.PlaySound(SoundID.MenuTick);
                int num4 = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
                for (int j = 0; j < Player.MaxBuffs; j++) {
                    int num5 = Main.LocalPlayer.buffType[j];
                    if (Main.debuff[num5] && Main.LocalPlayer.buffTime[j] > 60 &&
                        (num5 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num5]))
                        num4 += 100;
                }

                if (NPC.downedGolemBoss)
                    num4 *= 200;
                else if (NPC.downedPlantBoss)
                    num4 *= 150;
                else if (NPC.downedMechBossAny)
                    num4 *= 100;
                else if (Main.hardMode)
                    num4 *= 60;
                else if (NPC.downedBoss3 || NPC.downedQueenBee)
                    num4 *= 25;
                else if (NPC.downedBoss2)
                    num4 *= 10;
                else if (NPC.downedBoss1)
                    num4 *= 3;

                if (Main.expertMode)
                    num4 *= 2;
                int health = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
                bool removeDebuffs = true;
                string reason = Language.GetTextValue("tModLoader.DefaultNurseCantHealChat");
                bool canHeal = PlayerLoader.ModifyNurseHeal(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC],
                    ref health, ref removeDebuffs, ref reason);
                PlayerLoader.ModifyNursePrice(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], health,
                    removeDebuffs, ref num4);
                if (num4 > 0) {
                    if (!canHeal) {
                        Main.npcChatText = reason;
                        return;
                    }

                    if (Main.LocalPlayer.BuyItem(num4)) {
                        AchievementsHelper.HandleNurseService(num4);
                        SoundEngine.PlaySound(SoundID.Item4);
                        Main.LocalPlayer.HealEffect(health, true);
                        if ((double) Main.LocalPlayer.statLife < (double) Main.LocalPlayer.statLifeMax2 * 0.25)
                            Main.npcChatText = GetChatDialog(227);
                        else if ((double) Main.LocalPlayer.statLife < (double) Main.LocalPlayer.statLifeMax2 * 0.5)
                            Main.npcChatText = GetChatDialog(228);
                        else if ((double) Main.LocalPlayer.statLife < (double) Main.LocalPlayer.statLifeMax2 * 0.75)
                            Main.npcChatText = GetChatDialog(229);
                        else
                            Main.npcChatText = GetChatDialog(230);

                        Main.LocalPlayer.statLife += health;

                        if (!removeDebuffs) // no indent for better patching
                            goto SkipDebuffRemoval;

                        for (int l = 0; l < Player.MaxBuffs; l++) {
                            int num24 = Main.LocalPlayer.buffType[l];
                            if (Main.debuff[num24] && Main.LocalPlayer.buffTime[l] > 0 &&
                                (num24 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num24])) {
                                Main.LocalPlayer.DelBuff(l);
                                l = -1;
                            }
                        }

                        SkipDebuffRemoval:
                        PlayerLoader.PostNurseHeal(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], health,
                            removeDebuffs, num4);
                    }
                    else {
                        int num25 = Main.rand.Next(3);
                        if (num25 == 0)
                            Main.npcChatText = GetChatDialog(52);

                        if (num25 == 1)
                            Main.npcChatText = GetChatDialog(53);

                        if (num25 == 2)
                            Main.npcChatText = GetChatDialog(54);
                    }
                }
                else {
                    int num26 = Main.rand.Next(3);
                    if (!ChildSafety.Disabled)
                        num26 = Main.rand.Next(1, 3);

                    switch (num26) {
                        case 0:
                            Main.npcChatText = GetChatDialog(55);
                            break;
                        case 1:
                            Main.npcChatText = GetChatDialog(56);
                            break;
                        case 2:
                            Main.npcChatText = GetChatDialog(57);
                            break;
                    }
                }

                break;
            }
            case NPCID.Merchant:
                OpenShop(1);
                break;
            case NPCID.ArmsDealer:
                OpenShop(2);
                break;
            case NPCID.Dryad:
                OpenShop(3);
                break;
            case NPCID.Demolitionist:
                OpenShop(4);
                break;
            case NPCID.Clothier:
                OpenShop(5);
                break;
            case NPCID.GoblinTinkerer:
                OpenShop(6);
                break;
            case NPCID.Wizard:
                OpenShop(7);
                break;
            case NPCID.Mechanic:
                OpenShop(8);
                break;
            case NPCID.SantaClaus:
                OpenShop(9);
                break;
            case NPCID.Truffle:
                OpenShop(10);
                break;
            case NPCID.Steampunker:
                OpenShop(11);
                break;
            case NPCID.DyeTrader:
                OpenShop(12);
                break;
            case NPCID.PartyGirl:
                OpenShop(13);
                break;
            case NPCID.Cyborg:
                OpenShop(14);
                break;
            case NPCID.Painter:
                OpenShop(15);
                break;
            case NPCID.WitchDoctor:
                OpenShop(16);
                break;
            case NPCID.Pirate:
                OpenShop(17);
                break;
            case NPCID.Stylist:
                OpenShop(18);
                break;
            case NPCID.TravellingMerchant:
                OpenShop(19);
                break;
            case NPCID.SkeletonMerchant:
                OpenShop(20);
                break;
            case NPCID.DD2Bartender:
                OpenShop(21);
                break;
            case NPCID.Golfer:
                OpenShop(22);
                break;
            case NPCID.BestiaryGirl:
                OpenShop(23);
                break;
            case NPCID.Princess:
                OpenShop(24);
                break;
        }
    }

    // 打开商店
    public static void OpenShop(int shopIndex) {
        var targetMethod = Main.instance.GetType().GetMethod("OpenShop",
            BindingFlags.Instance | BindingFlags.NonPublic, new Type[] {typeof(int)});
        targetMethod.Invoke(Main.instance, new object[] {shopIndex});
    }

    internal static void HelpText() {
        var targetMethod = Main.instance.GetType()
            .GetMethod("HelpText", BindingFlags.Static | BindingFlags.NonPublic);
        targetMethod.Invoke(Main.instance, null);
    }

    public static string GetChatDialog(int l) =>
        Language.GetTextValueWith($"LegacyDialog.{l}", Lang.CreateDialogSubstitutionObject());

    public static Asset<Texture2D> GetHeadOrDefaultIcon(int head) {
        if (head > 0 && head < NPCHeadLoader.NPCHeadCount && !NPCHeadID.Sets.CannotBeDrawnInHousingUI[head]) {
            return TextureAssets.NpcHead[head];
        }

        return ModAsset.Icon_Default_Extra;
    }
}