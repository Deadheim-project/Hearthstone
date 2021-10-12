using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using UnityEngine;

namespace Hearthstone
{
    [BepInPlugin("Detalhes.Hearthstone", "Hearthstone", "1.0.2")]
    [BepInProcess("valheim.exe")]
    public class Hearthstone : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.Hearthstone";
        Harmony harmony = new Harmony(PluginGUID);

        public static ConfigEntry<string> item1;
        public static ConfigEntry<string> item2;
        public static ConfigEntry<string> item3;
        public static ConfigEntry<int> itemCost1;
        public static ConfigEntry<int> itemCost2;
        public static ConfigEntry<int> itemCost3;
        public static ConfigEntry<bool> allowTeleportWithoutRestriction;

        private void Awake()
        {

            item1 = Config.Bind<string>("General", "RecipeItem1", "BoneFragments", "Recipe item 1");
            item2 = Config.Bind<string>("General", "RecipeItem2", "Coins", "Recipe item 2");
            item3 = Config.Bind<string>("General", "RecipeItem3", "Crystal", "Recipe item 3");

            itemCost1 = Config.Bind<int>("General", "itemCost1", 10, "Recipe item 1 cost");
            itemCost2 = Config.Bind<int>("General", "Itemcost2", 30, "Recipe item 2 cost");
            itemCost3 = Config.Bind<int>("General", "itemCost3", 3, "Recipe item 3 cost");

            allowTeleportWithoutRestriction = Config.Bind<bool>("General", "allowTeleportWithoutRestriction", false, "Allow teleport without restriction");

            harmony.PatchAll();
            LoadAssets();
        }

        private void Update()
        {
            Player __instance = Player.m_localPlayer;

            if (Player.m_localPlayer is null) return;

            if (__instance.m_hovering)
            {
                Interactable componentInParent = __instance.m_hovering.GetComponentInParent<Interactable>();
                if (componentInParent != null)
                {
                    if (componentInParent is Bed)
                    {
                        Bed bed = (Bed)componentInParent;

                        if (bed.IsMine())
                        {
                            if (Input.GetKeyDown(KeyCode.P))
                            {
                                Hearthstone.SetHearthStonePosition();
                                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Here is your new Hearthstone spawn", 0, null);
                            }
                        }
                    }
                }
            }
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
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>(item1.Value),
                    m_amount = itemCost1.Value
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>(item2.Value),
                    m_amount = itemCost2.Value
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>(item3.Value),
                    m_amount = itemCost3.Value
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
