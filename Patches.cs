using HarmonyLib;
using UnityEngine;

namespace Hearthstone
{
    class Patches
    {
        [HarmonyPatch(typeof(Player), "ConsumeItem")]
        public static class ConsumePatch
        {
            private static bool Prefix(ItemDrop.ItemData item)
            {
                if (item.m_shared.m_name == "Hearthstone")
                {
                    if (!Player.m_localPlayer.IsTeleportable() && !Hearthstone.allowTeleportWithoutRestriction.Value)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You can't teleport carrying those items");
                        return false;
                    }

                    Vector3 teleportPosition = Hearthstone.GetHearthStonePosition();

                    if (teleportPosition == Vector3.zero)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You need to set hearthstone spawn point");
                        return false;
                    }

                    Player.m_localPlayer.TeleportTo(teleportPosition, Player.m_localPlayer.transform.rotation, true);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(Bed), "GetHoverText")]
        static class Bed_GetHoverText_Patch
        {
            static void Postfix(Bed __instance, ref string __result, ZNetView ___m_nview)
            {
                if (__instance.IsMine() && (___m_nview.GetZDO().GetLong("owner", 0L) != 0) || Traverse.Create(__instance).Method("IsCurrent").GetValue<bool>())
                {
                    __result += Localization.instance.Localize($"\n[{Hearthstone.modKey.Value}+<color=yellow><b>$KEY_Use</b></color>] Set hearthstone");
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(Bed), "Interact")]
        static class Bed_Interact_Patch
        {
            static bool Prefix(Bed __instance, Humanoid human, bool repeat, ref bool __result, ZNetView ___m_nview)
            {                
                if (__instance.IsMine() && Input.GetKey(Hearthstone.modKey.Value.ToLower()) && (___m_nview.GetZDO().GetLong("owner", 0L) != 0))
                {
                    Hearthstone.SetHearthStonePosition();
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Here is your new hearthstone spawn");
                    return false;
                }

                return true;
            }
        }
    }
}
