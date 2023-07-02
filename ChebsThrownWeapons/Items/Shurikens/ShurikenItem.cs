using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;

namespace ChebsThrownWeapons.Items.Shurikens
{
    public class ShurikenItem : Item
    {
        public static ConfigEntry<float> ProjectileVelocity, ProjectileGravity, ProjectileSpawnHeight;

        public static void CreateSharedConfigs(BaseUnityPlugin plugin)
        {
            ProjectileVelocity = plugin.Config.Bind($"ShurikenItem (Server Synced)", "ProjectileVelocity",
                50f, new ConfigDescription(
                    "The velocity of shurikens being launched.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileGravity = plugin.Config.Bind($"ShurikenItem (Server Synced)", "ProjectileGravity",
                10f, new ConfigDescription(
                    "The gravity applied to shurikens in flight.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileSpawnHeight = plugin.Config.Bind($"ShurikenItem (Server Synced)", "ProjectileSpawnHeight",
                0.5f, new ConfigDescription(
                    "The extra height applied to shuriken's spawn height.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}