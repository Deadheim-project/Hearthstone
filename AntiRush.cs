using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BossControll
{
  [BepInPlugin("EnderBombz_Holanda.AntiRush", "BossControll", "1.0.0")]
  [BepInProcess("valheim.exe")]
  public class AntiRush : BaseUnityPlugin
  {
    private readonly Harmony harmony = new Harmony("EnderBombz_Holanda.BossControll");
    public static int realDay = 31;
    public static int currentDay;
    public static string[] bosses = new string[5]
    {
      "$piece_offerbowl_eikthyr",
      "$prop_eldersummoningbowl_name",
      "$piece_offerbowl_bonemass",
      "$prop_dragonsummoningbowl_name",
      "$piece_offerbowl_yagluth"
    };
    public static ConfigEntry<bool> Real;
    public static ConfigEntry<int> EikthyrInvokeDay;
    public static ConfigEntry<int> EikthyrItemAmount;
    public static ConfigEntry<int> ElderInvokeDay;
    public static ConfigEntry<int> ElderItemAmount;
    public static ConfigEntry<int> BoneMassInvokeDay;
    public static ConfigEntry<int> BoneMassItemAmount;
    public static ConfigEntry<int> ModerInvokeDay;
    public static ConfigEntry<int> YagluthInvokeDay;
    public static List<AntiRush.ControlBossConfig> bossList;

    private void Awake()
    {
      ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "bossControll_config.cfg"), true);
      AntiRush.Real = (ConfigEntry<bool>) configFile.Bind<bool>("Real", "realLife", false, "This option changes the calculation method, one day in real life, there are 31 days in the game, turn false this option if you want the normal game days.'");
      AntiRush.EikthyrInvokeDay = (ConfigEntry<int>) configFile.Bind<int>("EikthyrInvokeDay", "spawnDay",  1, "Eikthyr day to be invoked ");
      AntiRush.EikthyrItemAmount = (ConfigEntry<int>) configFile.Bind<int>("EikthyrItemAmount", "amountItem",  2, "Amout of items to invoke");
      AntiRush.ElderInvokeDay = (ConfigEntry<int>) configFile.Bind<int>("ElderInvokeDay", "spawnDay",  5, "Elder day to be invoked ");
      AntiRush.ElderItemAmount = (ConfigEntry<int>) configFile.Bind<int>("ElderItemAmount", "amountItem", 3, "Amout of items to invoke");
      AntiRush.BoneMassInvokeDay = (ConfigEntry<int>) configFile.Bind<int>("BoneMassInvokeDay", "spawnDay", 20, "BoneMass day to be invoked");
      AntiRush.BoneMassItemAmount = (ConfigEntry<int>) configFile.Bind<int>("BoneMassItemAmount", "amountItem",  3, "Amout of items to invoke");
      AntiRush.ModerInvokeDay = (ConfigEntry<int>) configFile.Bind<int>("ModerInvokeDay", "spawnDay",  30, "Moder day to be invoked");
      AntiRush.YagluthInvokeDay = (ConfigEntry<int>) configFile.Bind<int>("YagluthInvokeDay", "spawnDay",  50, "Yagluth day to be invoked ");
      AntiRush.bossList = new List<AntiRush.ControlBossConfig>()
      {
        new AntiRush.ControlBossConfig()
        {
          NameTranslate = "Eikthyr",
          PlaceName = "$piece_offerbowl_eikthyr",
          Days = AntiRush.EikthyrInvokeDay.Value,
          ItemAmount = AntiRush.EikthyrItemAmount.Value
        },
        new AntiRush.ControlBossConfig()
        {
          NameTranslate = "Ancião",
          PlaceName = "$prop_eldersummoningbowl_name",
          Days = AntiRush.ElderInvokeDay.Value,
          ItemAmount = AntiRush.ElderItemAmount.Value
        },
        new AntiRush.ControlBossConfig()
        {
          NameTranslate = "Massa Óssea",
          PlaceName = "$piece_offerbowl_bonemass",
          Days = AntiRush.BoneMassInvokeDay.Value,
          ItemAmount = AntiRush.BoneMassItemAmount.Value
        },
        new AntiRush.ControlBossConfig()
        {
          NameTranslate = "Moder",
          PlaceName = "$prop_dragonsummoningbowl_name",
          Days = AntiRush.ModerInvokeDay.Value,
          ItemAmount = 0
        },
        new AntiRush.ControlBossConfig()
        {
          NameTranslate = "Yagluth",
          PlaceName = "$piece_offerbowl_yagluth",
          Days = AntiRush.YagluthInvokeDay.Value,
          ItemAmount = 0
        }
      };
      this.harmony.PatchAll();
    }

    public static bool isBossEnabled(string bossPlace, OfferingBowl __instance, Humanoid user)
    {
      foreach (AntiRush.ControlBossConfig boss in AntiRush.bossList)
      {
        if (AntiRush.Real.Value)
        {
          int num = AntiRush.realDay * boss.Days;
          if (AntiRush.currentDay < num && bossPlace == boss.PlaceName)
          {
            Debug.Log((object) "Yes he is entering in exeption");
            ((Character) user).Message((MessageHud.MessageType) 2, string.Format("O {0} só pode ser invocado em {1} / {2} dias!", (object) boss.NameTranslate, (object) AntiRush.currentDay, (object) num), 0, (Sprite) null);
            return false;
          }
        }
        else if (AntiRush.currentDay < boss.Days && bossPlace == boss.PlaceName)
        {
          Debug.Log((object) "Yes he is entering in exeption");
          ((Character) user).Message((MessageHud.MessageType) 2, string.Format("O {0} só pode ser invocado em {1} / {2} dias!", (object) boss.NameTranslate, (object) AntiRush.currentDay, (object) boss.Days), 0, (Sprite) null);
          return false;
        }
      }
      return true;
    }

    public class ControlBossConfig
    {
      public string NameTranslate { get; set; }

      public int Days { get; set; }

      public string PlaceName { get; set; }

      public int ItemAmount { get; set; }

      public ControlBossConfig()
      {
      }

      public ControlBossConfig(string name) => this.PlaceName = name;
    }

    [HarmonyPatch(typeof (OfferingBowl), "Interact")]
    public static class AntiRushInteraction_patch
    {
      private static bool Prefix(OfferingBowl __instance, Humanoid user)
      {
        foreach (AntiRush.ControlBossConfig boss in AntiRush.bossList)
        {
          if ((string) __instance.m_name == boss.PlaceName && boss.ItemAmount > 0)
            __instance.m_bossItems = boss.ItemAmount;
        }
        AntiRush.currentDay = EnvMan.instance.GetDay(ZNet.instance.GetTimeSeconds());
        Debug.Log((object) "Interact debugging...");
        Debug.Log((object) string.Format("Current day is: {0}", (object) AntiRush.currentDay));
        Debug.Log((object) ("Current boss is: " + Localization.instance.Localize((string) __instance.m_name)));
        Debug.Log((object) ("Current boss altar name: " + (string) __instance.m_name));
        return AntiRush.isBossEnabled((string) __instance.m_name, __instance, user);
      }
    }

    [HarmonyPatch(typeof (OfferingBowl), "UseItem")]
    public static class AntiRushUseItem_patch
    {
      private static bool Prefix(OfferingBowl __instance, Humanoid user, ItemDrop.ItemData item)
      {
        foreach (AntiRush.ControlBossConfig boss in AntiRush.bossList)
        {
          if ((string) __instance.m_name == boss.PlaceName && boss.ItemAmount > 0)
            __instance.m_bossItems = boss.ItemAmount;
        }
        AntiRush.currentDay = EnvMan.instance.GetDay(ZNet.instance.GetTimeSeconds());
        Debug.Log((object) "UseItem debugging...");
        Debug.Log((object) string.Format("{0}<{1} && {2}=={3}?", (object) AntiRush.currentDay, (object) AntiRush.realDay, (object) __instance.m_name, (object) AntiRush.bosses[0]));
        return AntiRush.isBossEnabled((string) __instance.m_name, __instance, user);
      }
    }
  }
}
