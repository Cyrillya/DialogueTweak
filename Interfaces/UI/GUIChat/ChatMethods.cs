using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;

namespace DialogueTweak.Interfaces.UI.GUIChat
{
    // 这里基本上就是垃圾堆，都是原版代码（写死了，必须要自己新写一个自己执行的那种）
    public class ChatMethods
    {
        static int myPlayer => Main.myPlayer;

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
                default:
                    return 0.8f;
            }
        }

        // 自己写的控制Shop和Extra的贴图
        public static void HandleShopTexture(int i, ref Texture2D Shop, ref Texture2D Extra) {
            if (i == -1) { // -1是标牌
                Shop = DialogueTweak.EditIcon;
                return;
            }
            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            int type = npc.type;
            // Shop
            Shop = DialogueTweak.DefaultIcon;

            // Extra
            int head = NPC.TypeToHeadIndex(type);
            if (head > 0 && head < Main.npcHeadTexture.Length) {
                Extra = Main.npcHeadTexture[head];
            }

            // Special
            switch (type) {
                case NPCID.Guide:
                    Shop = DialogueTweak.HelpIcon;
                    Extra = DialogueTweak.HammerIcon;
                    break;
                case NPCID.OldMan:
                    Shop = DialogueTweak.OldManIcon;
                    break;
                case NPCID.TaxCollector:
                case NPCID.Angler:
                case NPCID.Nurse:
                    Shop = Main.npcHeadTexture[head];
                    break;
            }
        }

        // 1.3给自动暂停特判的在绘制页面拯救NPC
        public static void SaveNPC() {
            if (Main.netMode == NetmodeID.SinglePlayer && Main.autoPause && Main.LocalPlayer.talkNPC >= 0) {
                var npc = Main.npc[Main.LocalPlayer.talkNPC];
                if (npc.type == NPCID.BoundGoblin) {
                    npc.Transform(NPCID.GoblinTinkerer);
                }
                if (npc.type == NPCID.BoundWizard) {
                    npc.Transform(NPCID.Wizard);
                }
                if (npc.type == NPCID.BoundMechanic) {
                    npc.Transform(NPCID.Mechanic);
                }
                if (npc.type == NPCID.WebbedStylist) {
                    npc.Transform(NPCID.Stylist);
                }
                if (npc.type == NPCID.SleepingAngler) {
                    npc.Transform(NPCID.Angler);
                }
                if (npc.type == NPCID.BartenderUnconscious) {
                    npc.Transform(NPCID.DD2Bartender);
                }
            }
        }

        // 根据标牌type获取对应物品type，原版是直接NewItem所以这里只能特判了
        public static void GetSignItemType(int x, int y, ushort tileType, out int dropType) {
            dropType = 171;
            switch (tileType) {
                case 85: {
                        int frameX = Main.tile[x, y].frameX / 18;
                        int style = 0;
                        while (frameX > 1) {
                            frameX -= 2;
                            style++;
                        }
                        int type2 = 321;
                        if (style >= 6 && style <= 10)
                            type2 = 3229 + style - 6;
                        else if (style >= 1 && style <= 5)
                            type2 = 1173 + style - 1;

                        dropType = type2;

                        break;
                    }
                case 395:
                    dropType = 3270;
                    break;
                case 425:
                    dropType = 3617;
                    break;
                case 573:
                    dropType = 4710;
                    break;
                case 511:
                    dropType = 4320;
                    break;
                case 510:
                    dropType = 4319;
                    break;
            }
        }

        // 第二按钮被按下的行动
        public static void HandleExtraButtonClicled(int npcType) {
            if (!NPCLoader.PreChatButtonClicked(false))
                return;

            NPCLoader.OnChatButtonClicked(false);
            if (npcType == 20) {
                Main.PlaySound(SoundID.MenuTick);
                Main.npcChatText = Lang.GetDryadWorldStatusDialog();
            }

            else if (npcType == 22) {
                Main.playerInventory = true;
                Main.npcChatText = "";
                Main.PlaySound(SoundID.MenuTick);
                Main.InGuideCraftMenu = true;
                UILinkPointNavigator.GoToDefaultPage();
            }
            else if (npcType == 107) {
                Main.playerInventory = true;
                Main.npcChatText = "";
                Main.PlaySound(SoundID.MenuTick);
                Main.InReforgeMenu = true;
                UILinkPointNavigator.GoToDefaultPage();
            }
            else if (npcType == 353) {
                Main.OpenHairWindow();
            }
            else if (npcType == 207) {
                Main.npcChatCornerItem = 0;
                Main.PlaySound(SoundID.MenuTick);
                bool gotDye = false;
                int num28 = Main.LocalPlayer.FindItem(ItemID.Sets.ExoticPlantsForDyeTrade);
                if (num28 != -1) {
                    Main.LocalPlayer.inventory[num28].stack--;
                    if (Main.LocalPlayer.inventory[num28].stack <= 0)
                        Main.LocalPlayer.inventory[num28] = new Item();

                    gotDye = true;
                    Main.PlaySound(SoundID.Chat);
                    Main.LocalPlayer.GetDyeTraderReward();
                }

                Main.npcChatText = Lang.DyeTraderQuestChat(gotDye);
            }
            else if (npcType == NPCID.DD2Bartender) {
                Main.PlaySound(SoundID.MenuTick);
                HelpText();
                Main.npcChatText = Lang.BartenderHelpText(Main.npc[Main.LocalPlayer.talkNPC]);
            }
        }

        // NPC对话选项显示字样
        public static void HandleFocusText(ref string focusText, ref string focusText2, ref Color textColor, out int money) {
            textColor = new Color(Main.mouseTextColor, (int)((double)Main.mouseTextColor / 1.1), Main.mouseTextColor / 2, Main.mouseTextColor);
            money = 0;
            // 标牌直接特判就行了
            if (Main.LocalPlayer.sign > -1) {
                focusText = (!Main.editSign) ? Language.GetText("LegacyInterface.48").Value : Language.GetText("LegacyInterface.47").Value;
                return;
            }

            NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];

            if (Main.LocalPlayer.sign > -1) {
                focusText = (!Main.editSign) ? Language.GetText("LegacyInterface.48").Value : Language.GetText("LegacyInterface.47").Value;
            }
            else if (talkNPC.type == NPCID.Dryad) {
                focusText = Language.GetText("LegacyInterface.28").Value;
                focusText2 = Language.GetText("LegacyInterface.49").Value;
            }
            else if (talkNPC.type == NPCID.DyeTrader) {
                focusText = Language.GetText("LegacyInterface.28").Value;
                if (Main.hardMode)
                    focusText2 = Language.GetText("LegacyInterface.107").Value;
            }
            else if (talkNPC.type == NPCID.SkeletonMerchant) {
                focusText = Language.GetText("LegacyInterface.28").Value;
            }
            else if (talkNPC.type == NPCID.DD2Bartender) {
                focusText = Language.GetText("LegacyInterface.28").Value;
                focusText2 = Language.GetTextValue("UI.BartenderHelp");
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
            }
            else if (talkNPC.type == NPCID.Merchant || talkNPC.type == NPCID.ArmsDealer || talkNPC.type == NPCID.Demolitionist || talkNPC.type == NPCID.Clothier || talkNPC.type == NPCID.GoblinTinkerer || talkNPC.type == NPCID.Wizard || talkNPC.type == NPCID.Mechanic || talkNPC.type == NPCID.SantaClaus || talkNPC.type == NPCID.Truffle || talkNPC.type == NPCID.Steampunker || talkNPC.type == NPCID.DyeTrader || talkNPC.type == NPCID.Cyborg || talkNPC.type == NPCID.Painter || talkNPC.type == NPCID.WitchDoctor || talkNPC.type == NPCID.Pirate) {
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
                }
            }
            else if (talkNPC.type == NPCID.Nurse) {
                money = Main.player[Main.myPlayer].statLifeMax2 - Main.player[Main.myPlayer].statLife;
                for (int j = 0; j < Player.MaxBuffs; j++) {
                    int type = Main.player[Main.myPlayer].buffType[j];
                    if (Main.debuff[type] && Main.player[Main.myPlayer].buffTime[j] > 5 && BuffLoader.CanBeCleared(type)) {
                        money += 1000;
                    }
                }
                int health = Main.player[Main.myPlayer].statLifeMax2 - Main.player[Main.myPlayer].statLife;

                if (money > 0 && money < 1)
                    money = 1;

                PlayerHooks.ModifyNursePrice(Main.LocalPlayer, talkNPC, health, true, ref money);

                focusText = Language.GetText("LegacyInterface.54").Value;
            }

            NPCLoader.SetChatButtons(ref focusText, ref focusText2);
        }

        // 第一按钮被按下的行动
        public static void HandleShop(int npcType) {
            if (!NPCLoader.PreChatButtonClicked(true))
                return;

            NPCLoader.OnChatButtonClicked(true);

            ChatMethods methods = new ChatMethods();
            switch (npcType) {
                case NPCID.Angler: {
                        Main.npcChatCornerItem = 0;
                        Main.PlaySound(SoundID.MenuTick);
                        bool flag3 = false;
                        if (!Main.anglerQuestFinished && !Main.anglerWhoFinishedToday.Contains(Main.LocalPlayer.name)) {
                            int num19 = Main.LocalPlayer.FindItem(Main.anglerQuestItemNetIDs[Main.anglerQuest]);
                            if (num19 != -1) {
                                Main.LocalPlayer.inventory[num19].stack--;
                                if (Main.LocalPlayer.inventory[num19].stack <= 0)
                                    Main.LocalPlayer.inventory[num19] = new Item();

                                flag3 = true;
                                Main.PlaySound(SoundID.Chat);
                                Main.LocalPlayer.anglerQuestsFinished++;
                                Main.LocalPlayer.GetAnglerReward();
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
                        NPC.SpawnSkeletron();
                    else
                        NetMessage.SendData(MessageID.Assorted1, -1, -1, null, Main.myPlayer, 1f, 0f, 0f, 0, 0, 0);

                    Main.npcChatText = "";
                    break;
                case NPCID.Guide:
                    Main.PlaySound(SoundID.MenuTick);
                    HelpText();
                    break;
                case NPCID.TaxCollector: {
                        if (Main.LocalPlayer.taxMoney > 0) {
                            int taxMoney3 = Main.LocalPlayer.taxMoney;
                            while (taxMoney3 > 0) {
                                if (taxMoney3 > 1000000) {
                                    int num20 = taxMoney3 / 1000000;
                                    taxMoney3 -= 1000000 * num20;
                                    int number = Item.NewItem((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height, 74, num20);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);

                                    continue;
                                }

                                if (taxMoney3 > 10000) {
                                    int num21 = taxMoney3 / 10000;
                                    taxMoney3 -= 10000 * num21;
                                    int number2 = Item.NewItem((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height, 73, num21);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number2, 1f);

                                    continue;
                                }

                                if (taxMoney3 > 100) {
                                    int num22 = taxMoney3 / 100;
                                    taxMoney3 -= 100 * num22;
                                    int number3 = Item.NewItem((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height, 72, num22);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number3, 1f);

                                    continue;
                                }

                                int num23 = taxMoney3;
                                if (num23 < 1)
                                    num23 = 1;

                                taxMoney3 -= num23;
                                int number4 = Item.NewItem((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, Main.LocalPlayer.width, Main.LocalPlayer.height, 71, num23);
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
                        Main.PlaySound(SoundID.MenuTick);
                        int num4 = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
                        for (int j = 0; j < Player.MaxBuffs; j++) {
                            int type = Main.LocalPlayer.buffType[j];
                            if (Main.debuff[type] && Main.LocalPlayer.buffTime[j] > 5 && BuffLoader.CanBeCleared(type)) {
                                num4 += 1000;
                            }
                        }
                        int health = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
                        bool removeDebuffs = true;
                        string reason = Language.GetTextValue("tModLoader.DefaultNurseCantHealChat");
                        bool canHeal = PlayerHooks.ModifyNurseHeal(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], ref health, ref removeDebuffs, ref reason);
                        PlayerHooks.ModifyNursePrice(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], health, removeDebuffs, ref num4);
                        if (num4 > 0) {
                            if (!canHeal) {
                                Main.npcChatText = reason;
                                return;
                            }

                            if (Main.LocalPlayer.BuyItem(num4)) {
                                AchievementsHelper.HandleNurseService(num4);
                                Main.PlaySound(SoundID.Item4);
                                Main.LocalPlayer.HealEffect(health, true);
                                if ((double)Main.LocalPlayer.statLife < (double)Main.LocalPlayer.statLifeMax2 * 0.25)
                                    Main.npcChatText = GetChatDialog(227);
                                else if ((double)Main.LocalPlayer.statLife < (double)Main.LocalPlayer.statLifeMax2 * 0.5)
                                    Main.npcChatText = GetChatDialog(228);
                                else if ((double)Main.LocalPlayer.statLife < (double)Main.LocalPlayer.statLifeMax2 * 0.75)
                                    Main.npcChatText = GetChatDialog(229);
                                else
                                    Main.npcChatText = GetChatDialog(230);

                                Main.LocalPlayer.statLife += health;
                                if (removeDebuffs) {
                                    for (int l = 0; l < Player.MaxBuffs; l++) {
                                        int num25 = Main.player[Main.myPlayer].buffType[l];
                                        if (Main.debuff[num25] && Main.player[Main.myPlayer].buffTime[l] > 0 && BuffLoader.CanBeCleared(num25)) {
                                            Main.player[Main.myPlayer].DelBuff(l);
                                            l = -1;
                                        }
                                    }
                                }
                                PlayerHooks.PostNurseHeal(Main.player[Main.myPlayer], Main.npc[Main.player[Main.myPlayer].talkNPC], health, removeDebuffs, num4);
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
                    methods.OpenShop(1);
                    break;
                case NPCID.ArmsDealer:
                    methods.OpenShop(2);
                    break;
                case NPCID.Dryad:
                    methods.OpenShop(3);
                    break;
                case NPCID.Demolitionist:
                    methods.OpenShop(4);
                    break;
                case NPCID.Clothier:
                    methods.OpenShop(5);
                    break;
                case NPCID.GoblinTinkerer:
                    methods.OpenShop(6);
                    break;
                case NPCID.Wizard:
                    methods.OpenShop(7);
                    break;
                case NPCID.Mechanic:
                    methods.OpenShop(8);
                    break;
                case NPCID.SantaClaus:
                    methods.OpenShop(9);
                    break;
                case NPCID.Truffle:
                    methods.OpenShop(10);
                    break;
                case NPCID.Steampunker:
                    methods.OpenShop(11);
                    break;
                case NPCID.DyeTrader:
                    methods.OpenShop(12);
                    break;
                case NPCID.PartyGirl:
                    methods.OpenShop(13);
                    break;
                case NPCID.Cyborg:
                    methods.OpenShop(14);
                    break;
                case NPCID.Painter:
                    methods.OpenShop(15);
                    break;
                case NPCID.WitchDoctor:
                    methods.OpenShop(16);
                    break;
                case NPCID.Pirate:
                    methods.OpenShop(17);
                    break;
                case NPCID.Stylist:
                    methods.OpenShop(18);
                    break;
                case NPCID.TravellingMerchant:
                    methods.OpenShop(19);
                    break;
                case NPCID.SkeletonMerchant:
                    methods.OpenShop(20);
                    break;
                case NPCID.DD2Bartender:
                    methods.OpenShop(21);
                    break;
            }
        }

        // 打开商店
        public void OpenShop(int shopIndex) {
            Main.playerInventory = true;
            Main.npcChatText = "";
            Main.npcShop = shopIndex;
            Main.instance.shop[Main.npcShop].SetupShop(Main.npcShop);
            Main.PlaySound(SoundID.MenuTick);
        }

        // 对于中文有优化的换行，自创
        public static string[] WordwrapString(string text, DynamicSpriteFont font, int maxWidth, int maxLines, out int lineAmount) {
            string[] array = new string[maxLines];
            int num = 0;
            // 这里用for给list2添加，是大概是为了一个\n的换行
            // list2似乎是精确到了每个单词
            List<string> list = new List<string>(text.Split('\n'));
            List<string> list2 = new List<string>(list[0].Split(' '));
            for (int i = 1; i < list.Count && i < maxLines; i++) {
                //Main.NewText(list[i]);
                list2.Add("\n");
                list2.AddRange(list[i].Split(' '));
            }

            bool flag = true;
            while (list2.Count > 0) {
                string text2 = list2[0];
                string str = " ";
                if (list2.Count == 1)
                    str = "";

                if (text2 == "\n") {
                    array[num++] += text2;
                    flag = true;
                    if (num >= maxLines)
                        break;

                    list2.RemoveAt(0);
                }
                else if (flag) {
                    if (font.MeasureString(text2).X > (float)maxWidth) {
                        string str2 = text2[0].ToString() ?? "";
                        int num2 = 1;
                        // 如果是以中文结尾(text2[num2]为中文)，那么不需要"-"
                        string hyphen = UseHyphen(text2[num2 - 1]);
                        // 添加文本阶段————文字长度还未触行末
                        while (font.MeasureString(str2 + text2[num2] + hyphen).X <= (float)maxWidth) {
                            str2 += text2[num2++];
                            // 如果下一个索引是英文，上一个是中文
                            if (!IsChineseOrSymbols(text2[num2]) && IsChineseOrSymbols(text2[num2 - 1])) {
                                // 获取单词
                                int count = 1;
                                string word = "" + text2[num2];
                                while (num2 + count < text2.Length && !IsChineseOrSymbols(text2[num2 + count])) {
                                    word += text2[num2 + count];
                                    count++;
                                }
                                // 获取单词所占空间，如果超限，直接break，让单词换行
                                if (font.MeasureString(str2 + word).X > (float)maxWidth) {
                                    break;
                                }
                            }
                            // 每次都更新一下，因为选取的字符是不断变动的
                            hyphen = UseHyphen(text2[num2 - 1]);
                        }
                        // 换行代码
                        // 如果下一个索引是中文符号，上一个不是
                        if (ChineseSymbols.Exists(t => t == text2[num2]) && !ChineseSymbols.Exists(t => t == text2[num2 - 1])) {
                            // 加上这个符号，并把指针向后移动
                            str2 += text2[num2++];
                            hyphen = "";
                            // 下一个字符是不显示的字符（如换行\n等）
                            while (num2 < text2.Length && font.MeasureString(text2[num2].ToString()).X <= 0) {
                                str2 += text2[num2++];
                            }
                        }

                        str2 += hyphen;
                        array[num++] = str2;
                        if (num >= maxLines)
                            break;

                        list2.RemoveAt(0); // 清空list2，方便下一行代码的应用
                        if (num2 < text2.Length) list2.Insert(0, text2.Substring(num2)); // 将剩余文本重新放回到list2（待处理文本区）内
                    }
                    else {
                        ref string reference = ref array[num];
                        reference = reference + text2 + str;
                        flag = false;
                        list2.RemoveAt(0);
                    }
                }
                else if (font.MeasureString(array[num] + text2).X > (float)maxWidth) {
                    num++;
                    if (num >= maxLines)
                        break;

                    flag = true;
                }
                else {
                    ref string reference2 = ref array[num];
                    reference2 = reference2 + text2 + str;
                    flag = false;
                    list2.RemoveAt(0);
                }
            }

            lineAmount = num;
            if (lineAmount == maxLines)
                lineAmount--;

            return array;
        }

        // 检测字符是否为中文

        public static readonly List<char> ChineseSymbols = new List<char>() {
            '–', '—', '‘', '’', '“', '”',
            '…', '、', '。', '〈', '〉', '《',
            '》', '「', '」', '『', '』', '【',
            '】', '〔', '〕', '！', '（', '）',
            '，', '．', '：', '；', '？'
        };
        // https://www.qqxiuzi.cn/zh/hanzi-unicode-bianma.php 汉字Unicode编码范围表
        public static bool IsChinese(char a) => (a >= 0x4E00 && a <= 0x9FEF);
        public static bool IsChineseOrSymbols(char a) => IsChinese(a) || ChineseSymbols.Exists(t => t == a);

        public static string UseHyphen(char a) => (char.IsLetter(a) && !IsChineseOrSymbols(a)) ? "-" : "";

        internal static void HelpText() {
            var targetMethod = Main.instance.GetType().GetMethod("HelpText", BindingFlags.Static | BindingFlags.NonPublic);
            targetMethod.Invoke(Main.instance, null);
        }

        public static string GetChatDialog(int l) => Language.GetTextValueWith($"LegacyDialog.{l}", Lang.CreateDialogSubstitutionObject());
    }
}
