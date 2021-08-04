using BepInEx;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using UnityEngine;

namespace Hearthstone
{
    [BepInPlugin("Detalhes.Hearthstone", "Hearthstone", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class Hearthstone : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.Hearthstone";
        Harmony harmony = new Harmony(PluginGUID);
        public static Hearthstone context;

        public static string modKey;

        private void Awake()
        {
            context = this;
            modKey = "left alt";
            harmony.PatchAll();
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
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_Hearthstone";
            recipe.m_item = itemDrop;
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench");
            recipe.m_resources = new Piece.Requirement[]
            {
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Coins"),
                    m_amount = 250
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Resin"),
                    m_amount = 30
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("BoneFragments"),
                    m_amount = 30
                }
            };
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);
        }

        unsafe public static Vector3 GetHearthStonePosition()
        {
            if (!Player.m_localPlayer.m_knownTexts.ContainsKey("positionX"))
            {
                return Vector3.zero;
            }

            return new Vector3
            {
                x = float.Parse(Player.m_localPlayer.m_knownTexts["positionX"]),
                y = float.Parse(Player.m_localPlayer.m_knownTexts["positionY"]),
                z = float.Parse(Player.m_localPlayer.m_knownTexts["positionZ"])
            };
        }

        unsafe public static void SetHearthStonePosition()
        {
            if (!Player.m_localPlayer.m_knownTexts.ContainsKey("positionX"))
            {
                Player.m_localPlayer.m_knownTexts.Add("positionX", Player.m_localPlayer.transform.position.x.ToString());
            } else
            {
                Player.m_localPlayer.m_knownTexts["positionX"] = Player.m_localPlayer.transform.position.x.ToString();
            }

            if (!Player.m_localPlayer.m_knownTexts.ContainsKey("positionY"))
            {
                Player.m_localPlayer.m_knownTexts.Add("positionY", Player.m_localPlayer.transform.position.y.ToString());
            }
            else
            {
                Player.m_localPlayer.m_knownTexts["positionY"] = Player.m_localPlayer.transform.position.y.ToString();
            }

            if (!Player.m_localPlayer.m_knownTexts.ContainsKey("positionZ"))
            {
                Player.m_localPlayer.m_knownTexts.Add("positionZ", Player.m_localPlayer.transform.position.z.ToString());
            }
            else
            {
                Player.m_localPlayer.m_knownTexts["positionZ"] = Player.m_localPlayer.transform.position.z.ToString();
            }
        }
    }
}
