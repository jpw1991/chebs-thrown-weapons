using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;

namespace ChebsThrownWeapons.Items.Axes
{
    public class ThrowingAxeItem : Item
    {
        public static ConfigEntry<float> ProjectileVelocity, ProjectileGravity, ProjectileSpawnHeight;

        public static void CreateSharedConfigs(BaseUnityPlugin plugin)
        {
            ProjectileVelocity = plugin.Config.Bind($"ThrowingAxeItem (Server Synced)", "ProjectileVelocity",
                50f, new ConfigDescription(
                    "The velocity of throwing axes being launched.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileGravity = plugin.Config.Bind($"ThrowingAxeItem (Server Synced)", "ProjectileGravity",
                10f, new ConfigDescription(
                    "The gravity applied to throwing axes in flight.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileSpawnHeight = plugin.Config.Bind($"ThrowingAxeItem (Server Synced)", "ProjectileSpawnHeight",
                .25f, new ConfigDescription(
                    "The extra height applied to throwing axe's spawn height.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}