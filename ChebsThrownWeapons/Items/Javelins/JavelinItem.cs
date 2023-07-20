using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;

namespace ChebsThrownWeapons.Items.Javelins
{
    public class JavelinItem : Item
    {
        public static ConfigEntry<float> ProjectileVelocity, ProjectileGravity, ProjectileSpawnHeight,
            AttackStartNoise, AttackHitNoise;

        public static void CreateSharedConfigs(BaseUnityPlugin plugin)
        {
            const string serverSynced = "JavelinItem (Server Synced)";
            ProjectileVelocity = plugin.Config.Bind(serverSynced, "ProjectileVelocity",
                70f, new ConfigDescription(
                    "The velocity of javelins being launched.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileGravity = plugin.Config.Bind(serverSynced, "ProjectileGravity",
                9f, new ConfigDescription(
                    "The gravity applied to javelins in flight.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileSpawnHeight = plugin.Config.Bind(serverSynced, "ProjectileSpawnHeight",
                1f, new ConfigDescription(
                    "The extra height applied to javelin's spawn height.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            ProjectileSpawnHeight = plugin.Config.Bind(serverSynced, "ProjectileSpawnHeight",
                1f, new ConfigDescription(
                    "The extra height applied to javelin's spawn height.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            AttackStartNoise = plugin.Config.Bind(serverSynced, "AttackStartNoise",
                10f, new ConfigDescription(
                    "The noise made by attacking with this weapon. 10 is default for most weapons.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            AttackHitNoise = plugin.Config.Bind(serverSynced, "AttackHitNoise",
                5f, new ConfigDescription(
                    "The noise made by this weapon on impact. 30 is default for most melee weapons, " +
                    "vanilla arrows have 0.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}