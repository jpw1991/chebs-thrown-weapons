using ChebsThrownWeapons.Items;
using HarmonyLib;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsThrownWeapons.Patches
{
    [HarmonyPatch(typeof(Attack))]
    public class AttackPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Attack.FireProjectileBurst))]
        static void FireProjectileBurst(Attack __instance)
        {
            // raise height of javelin projectile's spawn
            var lastProjectile = __instance.m_weapon.m_lastProjectile;
            if (lastProjectile != null &&
                lastProjectile.name.StartsWith("ChebGonaz_JavelinProjectile"))
            {
                Logger.LogInfo("Javelin projectile spawning");
                __instance.m_weapon.m_lastProjectile.transform.position +=
                    new Vector3(0, JavelinItem.ProjectileSpawnHeight.Value);
            }
        }
    }
}