using ChebsThrownWeapons.Items;
using ChebsThrownWeapons.Items.Axes;
using ChebsThrownWeapons.Items.Javelins;
using ChebsThrownWeapons.Items.Shurikens;
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
            if (lastProjectile == null) return;
            
            if (lastProjectile.name.StartsWith("ChebGonaz_ShurikenProjectile"))
            {
                __instance.m_weapon.m_lastProjectile.transform.position +=
                    new Vector3(0, ShurikenItem.ProjectileSpawnHeight.Value);   
            }
            else if (lastProjectile.name.StartsWith("ChebGonaz_JavelinProjectile"))
            {
                __instance.m_weapon.m_lastProjectile.transform.position +=
                    new Vector3(0, JavelinItem.ProjectileSpawnHeight.Value);   
            }
            else if (lastProjectile.name.StartsWith("ChebGonaz_ThrowingAxeProjectile"))
            {
                __instance.m_weapon.m_lastProjectile.transform.position +=
                    new Vector3(0, ThrowingAxeItem.ProjectileSpawnHeight.Value);   
            }
        }
    }
}