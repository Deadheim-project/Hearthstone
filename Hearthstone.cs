using BepInEx;
using BepInEx.Configuration;
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

        public static ConfigEntry<string> modKey;
        public static ConfigEntry<int> nexusID;
        public static ConfigEntry<int> resinCost;
        public static ConfigEntry<int> coinsCost;
        public static ConfigEntry<int> boneFragmentsCost;
        public static ConfigEntry<bool> allowTeleportWithoutRestriction;

        private void Awake()
        {
            modKey = Config.Bind<string>("General", "ModKey", "left alt", "Modifier key to set hearthstone spawn. Use https://docs.unity3d.com/Manual/ConventionalGameInput.html");
            nexusID = Config.Bind<int>("General", "NexusID", 1417, "Nexus mod ID for updates");

            resinCost = Config.Bind<int>("General", "ResinCost", 10, "Resin cost");
            coinsCost = Config.Bind<int>("General", "CoinsCost", 30, "Coins cost");
            boneFragmentsCost = Config.Bind<int>("General", "BoneFragmentsCost", 10, "Bone Fragments cost");

            allowTeleportWithoutRestriction = Config.Bind<bool>("General", "allowTeleportWithoutRestriction", false, "Allow teleport without restriction");

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
                    m_amount = coinsCost.Value
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Resin"),
                    m_amount = resinCost.Value
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("BoneFragments"),
                    m_amount = boneFragmentsCost.Value
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
            } 
            else
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
