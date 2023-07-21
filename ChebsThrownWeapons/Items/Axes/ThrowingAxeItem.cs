using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;

namespace ChebsThrownWeapons.Items.Axes
{
    public class ThrowingAxeItem : Item
    {
        public static ConfigEntry<float> ProjectileVelocity, ProjectileGravity, ProjectileSpawnHeight,
            AttackStartNoise, AttackHitNoise,
            MovementModifier;

        public static void CreateSharedConfigs(BaseUnityPlugin plugin)
        {
            const string serverSynced = "ThrowingAxeItem (Server Synced)";
            
            ProjectileVelocity = plugin.Config.Bind(serverSynced, "ProjectileVelocity",
                50f, new ConfigDescription(
                    "The velocity of throwing axes being launched.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileGravity = plugin.Config.Bind(serverSynced, "ProjectileGravity",
                10f, new ConfigDescription(
                    "The gravity applied to throwing axes in flight.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileSpawnHeight = plugin.Config.Bind(serverSynced, "ProjectileSpawnHeight",
                .25f, new ConfigDescription(
                    "The extra height applied to throwing axe's spawn height.", null,
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
            
            MovementModifier = plugin.Config.Bind(serverSynced, "MovementModifier",
                -0.025f, new ConfigDescription(
                    "The weapon's movement modifier when equipped. -0.025 is 2.5% slower.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}