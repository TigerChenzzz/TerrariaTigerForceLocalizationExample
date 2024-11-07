using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TigerForceLocalizationExample.Content.Items;

public class ExampleSword : ModItem {
    public override void SetDefaults() {
        Item.damage = 50;
        Item.DamageType = DamageClass.Melee;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(silver: 1);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
    }

    private int useType;
    public override void UseAnimation(Player player) {
        if (player.altFunctionUse == 2) {
            AltUseAnimation(player);
            return;
        }
        if (useType == 0) {
        }
        else if (useType == 1) {
            var itemID = Main.rand.Next(ItemLoader.ItemCount);
            player.QuickSpawnItem(null, itemID);
            CombatText.NewText(player.Hitbox, Color.White, $"get an item of id {itemID}!");
        }
        else if (useType == 2) {
            var buffID = Main.rand.Next(BuffLoader.BuffCount);
            player.AddBuff(buffID, 600);
            CombatText.NewText(player.Hitbox, Color.White, $"get a buff of id {buffID}!");
        }
        else {
            CombatText.NewText(player.Hitbox, Color.Red, "Wrong use type!");
        }
    }
    private void AltUseAnimation(Player player) {
        useType += 1;
        useType %= 3;
        CombatText.NewText(player.Hitbox, Color.White, $"use type has changed to {useType}!");
    }
    public override bool AltFunctionUse(Player player) {
        return true;
    }
    public override void SaveData(TagCompound tag) {
        if (useType != 0)
            tag["useType"] = useType;
    }
    public override void LoadData(TagCompound tag) => LoadDataInner(tag);
    private void LoadDataInner(TagCompound tag) {
        if (tag.ContainsKey("useType"))
            useType = tag.GetInt("useType");
    }

    public override void AddRecipes() {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.DirtBlock, 10);
        recipe.AddIngredient(null, "ExampleSword", 1);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}

