using ChebsThrownWeapons.Pickables;
using HarmonyLib;
using Jotunn;

namespace ChebsThrownWeapons.Patches
{
    [HarmonyPatch(typeof(Pickable))]
    public class PickablePatches
    {
        [HarmonyPatch(nameof(Pickable.Awake))]
        [HarmonyPrefix]
        static void AwakePrefix(Pickable __instance)
        {
            if (__instance.name.StartsWith(
                    ThrownWeaponsPickable.PickablePrefabName.Substring(0,
                        ThrownWeaponsPickable.PickablePrefabName.Length - 7)))
            {
                __instance.gameObject.AddComponent<ThrownWeaponsPickable>();
            }
        }
        
        [HarmonyPatch(nameof(Pickable.Interact))]
        [HarmonyPrefix]
         static bool Interact(Pickable __instance, Humanoid character, bool repeat, bool alt)
         {
             if (!__instance.TryGetComponent(out ThrownWeaponsPickable _)) return true; // permit base method completion

             var playerId = Game.instance.GetPlayerProfile().GetPlayerID();

             var player = Player.GetPlayer(playerId);
             if (player == null)
             {
                 Logger.LogError("ThrownWeapons: Failed to get player");
                 return false; // deny base method completion
             }

             if (player.GetSkillLevel(Skills.SkillType.Swords) < ChebsThrownWeapons.SwordSkillRequired.Value)
             {
                 character.Message(MessageHud.MessageType.Center, "$chebgonaz_swordinthestone_unworthy");
                 return false; // deny base method completion
             }

             return true; // permit base method completion
         }
    }
}