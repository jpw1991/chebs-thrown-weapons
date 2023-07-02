using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;

namespace ChebsThrownWeapons.Items.Javelins
{
    public class JavelinItem : Item
    {
        public static ConfigEntry<float> ProjectileVelocity, ProjectileGravity, ProjectileSpawnHeight;

        public static void CreateSharedConfigs(BaseUnityPlugin plugin)
        {
            ProjectileVelocity = plugin.Config.Bind($"JavelinItem (Server Synced)", "ProjectileVelocity",
                50f, new ConfigDescription(
                    "The velocity of javelins being launched.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileGravity = plugin.Config.Bind($"JavelinItem (Server Synced)", "ProjectileGravity",
                10f, new ConfigDescription(
                    "The gravity applied to javelins in flight.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileSpawnHeight = plugin.Config.Bind($"JavelinItem (Server Synced)", "ProjectileSpawnHeight",
                1f, new ConfigDescription(
                    "The extra height applied to javelin's spawn height.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}