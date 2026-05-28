using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace DialogueTweak.Tests;

public class TestGlobalNPC : GlobalNPC
{
    public override bool IsLoadingEnabled(Mod mod) => false;

    public override void RegisterChatButtons(NPC npc, NPCInteractionList interactions)
    {
        if (npc.type != NPCID.Guide)
            return;

        // Test button 1: A textless button with only icon
        interactions.Append(new TestTextlessButton());

        // Test button 2: An extra button interleaved between native ones
        // Insert after the native GuideReverseCrafting button
        var reverseCrafting = interactions.Interactions.OfType<NPCInteractions.Actions.GuideTip>()
            .FirstOrDefault();
        if (reverseCrafting != null)
        {
            interactions.InsertAfter(new TestWeatherButton(), reverseCrafting);
        }

        // Test button 3: A conditional button that only shows sometimes
        interactions.Append(new TestConditionalButton());
    }

    public override void SetStaticDefaults() {
        // Replace the TestWeatherButton icon with a random item texture that changes every second
        // The following code equals to:
        //     DialogueTweakHelper.ReplaceButtonIcon("TestWeatherButton", NPCID.Guide, () => ...) 
        DialogueTweak.Instance.Call(
            "ReplaceButtonIcon",
            "TestWeatherButton",
            new List<int> { NPCID.Guide },
            new Func<string>(() =>
            {
                // Pick a random item from common item IDs
                int[] items = [
                    ItemID.WoodenSword, ItemID.CopperPickaxe, ItemID.HealingPotion,
                    ItemID.Torch, ItemID.IronHelmet, ItemID.GoldCoin, ItemID.Bomb,
                    ItemID.LifeCrystal, ItemID.ManaCrystal, ItemID.HermesBoots
                ];
                var random = new UnifiedRandom((int)Main.GameUpdateCount / 60);
                int randomItemId = random.NextFromList(items);
                return $"Terraria/Images/Item_{randomItemId}";
            })
        );
    }
}

/// <summary>
/// A test button that sets a custom chat message. Demonstrates a simple interaction.
/// </summary>
public class TestTextlessButton : NPCInteraction
{
    public override bool Condition() => TalkNPCType == NPCID.Guide;

    public override string GetText() => "";

    public override void Interact()
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
        Main.npcChatText = "Hello from DialogueTweak test button!";
        Main.DoNPCPortraitHop();
    }
}

/// <summary>
/// A test button inserted between native GuideTip and GuideReverseCrafting.
/// Demonstrates that mod buttons respect the registration order.
/// </summary>
public class TestWeatherButton : NPCInteraction
{
    public override bool ShowExcalmation => Main.raining;

    public override bool Condition() => TalkNPCType == NPCID.Guide;

    public override string GetText()
    {
        if (Main.raining)
            return "Test: Weather...";
        if (Main.IsItDay())
            return "Test: Weather!";
        return "Test: Weather.";
    }

    public override void Interact()
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
        Main.npcChatText = Main.raining
            ? "Better bring an umbrella!"
            : "What a beautiful day!";
        Main.DoNPCPortraitHop();
    }
}

/// <summary>
/// A conditional button that only appears when the player has low health.
/// Demonstrates dynamic availability.
/// </summary>
public class TestConditionalButton : NPCInteraction
{
    public override bool Condition()
    {
        return Main.mouseX > Main.screenWidth / 2f && Main.mouseY < Main.screenHeight / 2f;
    }

    public override string GetText() => "Test: Secret Button";

    public override void Interact()
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
        Main.npcChatText = "You have discovered a secret button!";
        Main.DoNPCPortraitHop();
    }
}
