using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hearthstone
{
    [BepInPlugin("Detalhes.Hearthstone", "Hearthstone", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class Hearthstone : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.Hearthstone";
        Harmony _Harmony = new Harmony("Detalhes.Hearthstone");


        private void Awake()
        {
            _Harmony.PatchAll();
            LoadAssets();
        }

        private void LoadAssets()
        {
            ItemManager.OnVanillaItemsAvailable += AddClonedItems;
        }

        private void AddClonedItems()
        {
            try
            {
                CustomItem CI = new CustomItem("Hearthstone", "YagluthDrop");
                ItemManager.Instance.AddItem(CI);

                ItemDrop itemDrop = CI.ItemDrop;
                itemDrop.m_itemData.m_shared.m_name = "Hearthstone";
                itemDrop.m_itemData.m_shared.m_description = "Go back home!";
                itemDrop.m_itemData.m_shared.m_maxStackSize = 1;
                itemDrop.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Consumable;

                RecipeHearthStone(itemDrop);
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Error while adding cloned item: {ex.Message}");
            }
            finally
            {
                ItemManager.OnVanillaItemsAvailable -= AddClonedItems;
            }
        }

        private void RecipeHearthStone(ItemDrop itemDrop)
        {
            // Create and add a recipe for the copied item
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_Hearthstone";
            recipe.m_item = itemDrop;
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench");
            recipe.m_resources = new Piece.Requirement[]
            {
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Coins"),
                    m_amount = 999
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("GreydwarfEye"),
                    m_amount = 999
                }
            };
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);
        }

        [HarmonyPatch(typeof(Player), "ConsumeItem")]
        public static class ConsumePatch
        {
            private static void Postfix(ItemDrop.ItemData item)
            {
                Debug.Log(item.m_shared.m_name);
                if (item.m_shared.m_name == "Hearthstone")
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Você comeu o fruto do diabo");
                }
            }
        }
    }
}
