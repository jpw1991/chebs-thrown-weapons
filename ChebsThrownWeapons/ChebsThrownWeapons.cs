using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using ChebsThrownWeapons.Items;
using ChebsThrownWeapons.Items.Axes;
using ChebsThrownWeapons.Items.Javelins;
using ChebsThrownWeapons.Items.Shurikens;
using ChebsValheimLibrary;
using HarmonyLib;
using Jotunn;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Paths = BepInEx.Paths;

namespace ChebsThrownWeapons
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ChebsThrownWeapons : BaseUnityPlugin
    {
        public const string PluginGuid = "com.chebgonaz.chebsthrownweapons";
        public const string PluginName = "ChebsThrownWeapons";
        public const string PluginVersion = "1.0.1";
        
        private const string ConfigFileName = PluginGuid + ".cfg";
        private static readonly string ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);

        public readonly System.Version ChebsValheimLibraryVersion = new("2.0.0");

        private readonly Harmony harmony = new(PluginGuid);

        // if set to true, the particle effects that for some reason hurt radeon are dynamically disabled
        public static ConfigEntry<bool> RadeonFriendly;

        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        public static IronJavelinItem IronJavelin = new();
        public static BronzeJavelinItem BronzeJavelin = new();
        public static WoodJavelinItem WoodJavelin = new();
        public static FireJavelinItem FireJavelin = new();
        public static BlackMetalJavelinItem BlackMetalJavelin = new();
        
        public static BronzeShurikenItem BronzeShuriken = new();
        public static IronShurikenItem IronShuriken = new();
        public static BlackMetalShurikenItem BlackMetalShuriken = new();

        public static BronzeThrowingAxeItem BronzeThrowingAxe = new();
        public static IronThrowingAxeItem IronThrowingAxe = new();
        public static BlackMetalThrowingAxeItem BlackMetalThrowingAxe = new();

        private void Awake()
        {
            if (!Base.VersionCheck(ChebsValheimLibraryVersion, out string message))
            {
                Jotunn.Logger.LogWarning(message);
            }

            CreateConfigValues();
            LoadAssetBundle();
            harmony.PatchAll();

            SetupWatcher();
        }
        
        private void CreateConfigValues()
        {
            Config.SaveOnConfigSet = true;

            RadeonFriendly = Config.Bind($"{GetType().Name} (Client)", "RadeonFriendly",
                false, new ConfigDescription("ONLY set this to true if you have graphical issues with " +
                                             "the mod. It will disable all particle effects for the mod's prefabs " +
                                             "which seem to give users with Radeon cards trouble for unknown " +
                                             "reasons. If you have problems with lag it might also help to switch" +
                                             "this setting on."));
            JavelinItem.CreateSharedConfigs(this);
            IronJavelin.CreateConfigs(this);
            BronzeJavelin.CreateConfigs(this);
            WoodJavelin.CreateConfigs(this);
            FireJavelin.CreateConfigs(this);
            BlackMetalJavelin.CreateConfigs(this);
            
            ShurikenItem.CreateSharedConfigs(this);
            BronzeShuriken.CreateConfigs(this);
            IronShuriken.CreateConfigs(this);
            BlackMetalShuriken.CreateConfigs(this);
            
            ThrowingAxeItem.CreateSharedConfigs(this);
            BronzeThrowingAxe.CreateConfigs(this);
            IronThrowingAxe.CreateConfigs(this);
            BlackMetalThrowingAxe.CreateConfigs(this);
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.Error += (sender, e) => Jotunn.Logger.LogError($"Error watching for config changes: {e}");
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Logger.LogInfo("Read updated config values");
                Config.Reload();
                IronJavelin.UpdateRecipe();
                BronzeJavelin.UpdateRecipe();
                WoodJavelin.UpdateRecipe();
                FireJavelin.UpdateRecipe();
                BlackMetalJavelin.UpdateRecipe();
                BronzeShuriken.UpdateRecipe();
                IronShuriken.UpdateRecipe();
                BlackMetalShuriken.UpdateRecipe();
                BronzeThrowingAxe.UpdateRecipe();
                IronThrowingAxe.UpdateRecipe();
                BlackMetalThrowingAxe.UpdateRecipe();
            }
            catch (Exception exc)
            {
                Logger.LogError($"There was an issue loading your {ConfigFileName}: {exc}");
                Logger.LogError("Please check your config entries for spelling and format!");
            }
        }

        private void LoadAssetBundle()
        {
            // order is important (I think): items, creatures, structures
            var assetBundlePath = Path.Combine(Path.GetDirectoryName(Info.Location), "chebsthrownweapons");
            var chebgonazAssetBundle = AssetUtils.LoadAssetBundle(assetBundlePath);
            try
            {
                {
                    var ironJavelinProjectilePrefab =
                        Base.LoadPrefabFromBundle(IronJavelin.ProjectilePrefabName, chebgonazAssetBundle,
                            RadeonFriendly.Value);
                    ironJavelinProjectilePrefab.GetComponent<Projectile>().m_gravity = JavelinItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(ironJavelinProjectilePrefab);
                
                    var ironJavelinPrefab = Base.LoadPrefabFromBundle(IronJavelin.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = ironJavelinPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = ironJavelinProjectilePrefab;
                    shared.m_attack.m_projectileVel = JavelinItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = IronJavelinItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = IronJavelinItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = IronJavelinItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = IronJavelinItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(IronJavelin.GetCustomItemFromPrefab(ironJavelinPrefab));   
                }
                {
                    var bronzeJavelinProjectilePrefab =
                        Base.LoadPrefabFromBundle(BronzeJavelin.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    bronzeJavelinProjectilePrefab.GetComponent<Projectile>().m_gravity = JavelinItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(bronzeJavelinProjectilePrefab);

                    var bronzeJavelinPrefab = Base.LoadPrefabFromBundle(BronzeJavelin.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = bronzeJavelinPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = bronzeJavelinProjectilePrefab;
                    shared.m_attack.m_projectileVel = JavelinItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = BronzeJavelinItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = BronzeJavelinItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = BronzeJavelinItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = BronzeJavelinItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(BronzeJavelin.GetCustomItemFromPrefab(bronzeJavelinPrefab));                    
                }
                {
                    var woodJavelinProjectilePrefab =
                        Base.LoadPrefabFromBundle(WoodJavelin.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    woodJavelinProjectilePrefab.GetComponent<Projectile>().m_gravity = JavelinItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(woodJavelinProjectilePrefab);

                    var woodJavelinPrefab = Base.LoadPrefabFromBundle(WoodJavelin.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = woodJavelinPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = woodJavelinProjectilePrefab;
                    shared.m_attack.m_projectileVel = JavelinItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = WoodJavelinItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = WoodJavelinItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = WoodJavelinItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = WoodJavelinItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(WoodJavelin.GetCustomItemFromPrefab(woodJavelinPrefab));                    
                }
                {
                    var fireJavelinProjectilePrefab =
                        Base.LoadPrefabFromBundle(FireJavelin.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    fireJavelinProjectilePrefab.GetComponent<Projectile>().m_gravity = JavelinItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(fireJavelinProjectilePrefab);

                    var fireJavelinPrefab = Base.LoadPrefabFromBundle(FireJavelin.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = fireJavelinPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = fireJavelinProjectilePrefab;
                    shared.m_attack.m_projectileVel = JavelinItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = FireJavelinItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = FireJavelinItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = FireJavelinItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = FireJavelinItem.SlashingDamagePerLevel.Value;
                    shared.m_damages.m_fire = FireJavelinItem.BaseFireDamage.Value;
                    shared.m_damagesPerLevel.m_fire = FireJavelinItem.FireDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(FireJavelin.GetCustomItemFromPrefab(fireJavelinPrefab));
                }
                {
                    var blackMetalJavelinProjectilePrefab = Base.LoadPrefabFromBundle(BlackMetalJavelin.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    blackMetalJavelinProjectilePrefab.GetComponent<Projectile>().m_gravity = JavelinItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(blackMetalJavelinProjectilePrefab);

                    var blackMetalJavelinPrefab = Base.LoadPrefabFromBundle(BlackMetalJavelin.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = blackMetalJavelinPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = blackMetalJavelinProjectilePrefab;
                    shared.m_attack.m_projectileVel = JavelinItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = BlackMetalJavelinItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = BlackMetalJavelinItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = BlackMetalJavelinItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = BlackMetalJavelinItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(BlackMetalJavelin.GetCustomItemFromPrefab(blackMetalJavelinPrefab));
                }
                {
                    var bronzeShurikenProjectilePrefab = Base.LoadPrefabFromBundle(BronzeShuriken.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    bronzeShurikenProjectilePrefab.GetComponent<Projectile>().m_gravity = ShurikenItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(bronzeShurikenProjectilePrefab);

                    var bronzeShurikenPrefab = Base.LoadPrefabFromBundle(BronzeShuriken.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = bronzeShurikenPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = bronzeShurikenProjectilePrefab;
                    shared.m_attack.m_projectileVel = ShurikenItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = BronzeShurikenItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = BronzeShurikenItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = BronzeShurikenItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = BronzeShurikenItem.SlashingDamagePerLevel.Value;
                    shared.m_damages.m_poison = BronzeShurikenItem.BasePoisonDamage.Value;
                    shared.m_damagesPerLevel.m_poison = BronzeShurikenItem.PoisonDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(BronzeShuriken.GetCustomItemFromPrefab(bronzeShurikenPrefab));
                }
                {
                    var ironShurikenProjectilePrefab = Base.LoadPrefabFromBundle(IronShuriken.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    ironShurikenProjectilePrefab.GetComponent<Projectile>().m_gravity = ShurikenItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(ironShurikenProjectilePrefab);

                    var ironShurikenPrefab = Base.LoadPrefabFromBundle(IronShuriken.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = ironShurikenPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = ironShurikenProjectilePrefab;
                    shared.m_attack.m_projectileVel = ShurikenItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = IronShurikenItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = IronShurikenItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = IronShurikenItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = IronShurikenItem.SlashingDamagePerLevel.Value;
                    shared.m_damages.m_poison = IronShurikenItem.BasePoisonDamage.Value;
                    shared.m_damagesPerLevel.m_poison = IronShurikenItem.PoisonDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(IronShuriken.GetCustomItemFromPrefab(ironShurikenPrefab));
                }
                {
                    var blackMetalShurikenProjectilePrefab = Base.LoadPrefabFromBundle(BlackMetalShuriken.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    blackMetalShurikenProjectilePrefab.GetComponent<Projectile>().m_gravity = ShurikenItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(blackMetalShurikenProjectilePrefab);

                    var blackMetalShurikenPrefab = Base.LoadPrefabFromBundle(BlackMetalShuriken.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = blackMetalShurikenPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = blackMetalShurikenProjectilePrefab;
                    shared.m_attack.m_projectileVel = ShurikenItem.ProjectileVelocity.Value;
                    shared.m_damages.m_pierce = BlackMetalShurikenItem.BasePierceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = BlackMetalShurikenItem.PierceDamagePerLevel.Value;
                    shared.m_damages.m_slash = BlackMetalShurikenItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = BlackMetalShurikenItem.SlashingDamagePerLevel.Value;
                    shared.m_damages.m_poison = BlackMetalShurikenItem.BasePoisonDamage.Value;
                    shared.m_damagesPerLevel.m_poison = BlackMetalShurikenItem.PoisonDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(BlackMetalShuriken.GetCustomItemFromPrefab(blackMetalShurikenPrefab));
                }
                {
                    var bronzeThrowingAxeProjectilePrefab =
                        Base.LoadPrefabFromBundle(BronzeThrowingAxe.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    bronzeThrowingAxeProjectilePrefab.GetComponent<Projectile>().m_gravity = ThrowingAxeItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(bronzeThrowingAxeProjectilePrefab);

                    var bronzeThrowingAxePrefab = Base.LoadPrefabFromBundle(BronzeThrowingAxe.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = bronzeThrowingAxePrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = bronzeThrowingAxeProjectilePrefab;
                    shared.m_attack.m_projectileVel = ThrowingAxeItem.ProjectileVelocity.Value;
                    shared.m_damages.m_blunt = BronzeThrowingAxeItem.BaseBluntDamage.Value;
                    shared.m_damagesPerLevel.m_blunt = BronzeThrowingAxeItem.BluntDamagePerLevel.Value;
                    shared.m_damages.m_slash = BronzeThrowingAxeItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = BronzeThrowingAxeItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(BronzeThrowingAxe.GetCustomItemFromPrefab(bronzeThrowingAxePrefab));
                }
                {
                    var ironThrowingAxeProjectilePrefab =
                        Base.LoadPrefabFromBundle(IronThrowingAxe.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    ironThrowingAxeProjectilePrefab.GetComponent<Projectile>().m_gravity = ThrowingAxeItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(ironThrowingAxeProjectilePrefab);

                    var ironThrowingAxePrefab = Base.LoadPrefabFromBundle(IronThrowingAxe.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = ironThrowingAxePrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = ironThrowingAxeProjectilePrefab;
                    shared.m_attack.m_projectileVel = ThrowingAxeItem.ProjectileVelocity.Value;
                    shared.m_damages.m_blunt = IronThrowingAxeItem.BaseBluntDamage.Value;
                    shared.m_damagesPerLevel.m_blunt = IronThrowingAxeItem.BaseBluntDamage.Value;
                    shared.m_damages.m_slash = IronThrowingAxeItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = IronThrowingAxeItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(IronThrowingAxe.GetCustomItemFromPrefab(ironThrowingAxePrefab));
                }
                {
                    var blackMetalThrowingAxeProjectilePrefab =
                        Base.LoadPrefabFromBundle(BlackMetalThrowingAxe.ProjectilePrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    blackMetalThrowingAxeProjectilePrefab.GetComponent<Projectile>().m_gravity = ThrowingAxeItem.ProjectileGravity.Value;
                    PrefabManager.Instance.AddPrefab(blackMetalThrowingAxeProjectilePrefab);

                    var blackMetalThrowingAxePrefab = Base.LoadPrefabFromBundle(BlackMetalThrowingAxe.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    var shared = blackMetalThrowingAxePrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
                    shared.m_attack.m_attackProjectile = blackMetalThrowingAxeProjectilePrefab;
                    shared.m_attack.m_projectileVel = ThrowingAxeItem.ProjectileVelocity.Value;
                    shared.m_damages.m_blunt = BlackMetalThrowingAxeItem.BaseBluntDamage.Value;
                    shared.m_damagesPerLevel.m_blunt = BlackMetalThrowingAxeItem.BaseBluntDamage.Value;
                    shared.m_damages.m_slash = BlackMetalThrowingAxeItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = BlackMetalThrowingAxeItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(BlackMetalThrowingAxe.GetCustomItemFromPrefab(blackMetalThrowingAxePrefab));
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Exception caught while loading assets: {ex}");
            }
            finally
            {
                chebgonazAssetBundle.Unload(false);
            }
        }
    }
}