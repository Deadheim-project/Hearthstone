using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ItemManager;
using ServerSync;
using UnityEngine;

namespace Hearthstone
{
	[BepInPlugin("Detalhes.Hearthstone", "Hearthstone", "2.0.3")]
	public class Hearthstone : BaseUnityPlugin
	{
		private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
		{
			ConfigEntry<T> configEntry = base.Config.Bind<T>(group, name, value, description);
			SyncedConfigEntry<T> syncedConfigEntry = this.configSync.AddConfigEntry<T>(configEntry);
			syncedConfigEntry.SynchronizedConfig = synchronizedSetting;
			return configEntry;
		}

		private ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true)
		{
			return this.config<T>(group, name, value, new ConfigDescription(description, null, Array.Empty<object>()), synchronizedSetting);
		}

		private void Awake()
		{
			Hearthstone.allowTeleportWithoutRestriction = this.config<bool>("General", "allowTeleportWithoutRestriction", false, "Allow teleport without restriction", true);
			Item item = new Item("hearthstone", "Hearthstone", "assets");
			item.RequiredItems.Add("Crystal", 3);
			item.RequiredItems.Add("Coins", 30);
			item.RequiredItems.Add("BoneFragments", 20);
			item.Crafting.Add(CraftingTable.ArtisanTable, 1);
			this.harmony.PatchAll();
		}

		private void Update()
		{
			Player localPlayer = Player.m_localPlayer;
			bool flag = Player.m_localPlayer == null;
			if (!flag)
			{
				bool flag2 = localPlayer.m_hovering;
				if (flag2)
				{
					Interactable componentInParent = localPlayer.m_hovering.GetComponentInParent<Interactable>();
					bool flag3 = componentInParent != null;
					if (flag3)
					{
						bool flag4 = componentInParent is Bed;
						if (flag4)
						{
							Bed bed = (Bed)componentInParent;
							bool flag5 = bed.IsMine();
							if (flag5)
							{
								bool keyDown = Input.GetKeyDown(KeyCode.P);
								if (keyDown)
								{
									Hearthstone.SetHearthStonePosition();
									Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Here is your new Hearthstone spawn", 0, null);
								}
							}
						}
					}
				}
			}
		}

		public static Vector3 GetHearthStonePosition()
		{
			bool flag = !Player.m_localPlayer.m_knownTexts.ContainsKey("positionX");
			Vector3 result;
			if (flag)
			{
				result = Vector3.zero;
			}
			else
			{
				Vector3 vector = default(Vector3);
				vector.x = float.Parse(Player.m_localPlayer.m_knownTexts["positionX"]);
				vector.y = float.Parse(Player.m_localPlayer.m_knownTexts["positionY"]);
				vector.z = float.Parse(Player.m_localPlayer.m_knownTexts["positionZ"]);
				result = vector;
			}
			return result;
		}

		public static void SetHearthStonePosition()
		{
			bool flag = !Player.m_localPlayer.m_knownTexts.ContainsKey("positionX");
			if (flag)
			{
				Dictionary<string, string> knownTexts = Player.m_localPlayer.m_knownTexts;
				string key = "positionX";
				Vector3 position = Player.m_localPlayer.transform.position;
				knownTexts.Add(key, position.x.ToString());
			}
			else
			{
				Dictionary<string, string> knownTexts2 = Player.m_localPlayer.m_knownTexts;
				string key2 = "positionX";
				Vector3 position = Player.m_localPlayer.transform.position;
				knownTexts2[key2] = position.x.ToString();
			}
			bool flag2 = !Player.m_localPlayer.m_knownTexts.ContainsKey("positionY");
			if (flag2)
			{
				Dictionary<string, string> knownTexts3 = Player.m_localPlayer.m_knownTexts;
				string key3 = "positionY";
				Vector3 position = Player.m_localPlayer.transform.position;
				knownTexts3.Add(key3, position.y.ToString());
			}
			else
			{
				Dictionary<string, string> knownTexts4 = Player.m_localPlayer.m_knownTexts;
				string key4 = "positionY";
				Vector3 position = Player.m_localPlayer.transform.position;
				knownTexts4[key4] = position.y.ToString();
			}
			bool flag3 = !Player.m_localPlayer.m_knownTexts.ContainsKey("positionZ");
			if (flag3)
			{
				Dictionary<string, string> knownTexts5 = Player.m_localPlayer.m_knownTexts;
				string key5 = "positionZ";
				Vector3 position = Player.m_localPlayer.transform.position;
				knownTexts5.Add(key5, position.z.ToString());
			}
			else
			{
				Dictionary<string, string> knownTexts6 = Player.m_localPlayer.m_knownTexts;
				string key6 = "positionZ";
				Vector3 position = Player.m_localPlayer.transform.position;
				knownTexts6[key6] = position.z.ToString();
			}
		}

		public const string PluginGUID = "Detalhes.Hearthstone";

		public const string Version = "2.0.1";

		private Harmony harmony = new Harmony("Detalhes.Hearthstone");

		public static ConfigEntry<string> item1;

		public static ConfigEntry<string> item2;

		public static ConfigEntry<string> item3;

		public static ConfigEntry<int> itemCost1;

		public static ConfigEntry<int> itemCost2;

		public static ConfigEntry<int> itemCost3;

		public static ConfigEntry<bool> allowTeleportWithoutRestriction;

		private ConfigSync configSync = new ConfigSync("Detalhes.Hearthstone")
		{
			DisplayName = "Hearthstone",
			CurrentVersion = "2.0.1",
			MinimumRequiredVersion = "2.0.1"
		};
	}
}
