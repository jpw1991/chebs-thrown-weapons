using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using ChebsThrownWeapons.Items;
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
        public const string PluginVersion = "0.0.1";
        
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
                    shared.m_damages.m_pierce = IronJavelinItem.BasePieceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = IronJavelinItem.PieceDamagePerLevel.Value;
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
                    shared.m_damages.m_pierce = BronzeJavelinItem.BasePieceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = BronzeJavelinItem.PieceDamagePerLevel.Value;
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
                    shared.m_damages.m_pierce = WoodJavelinItem.BasePieceDamage.Value;
                    shared.m_damagesPerLevel.m_pierce = WoodJavelinItem.PieceDamagePerLevel.Value;
                    shared.m_damages.m_slash = WoodJavelinItem.BaseSlashingDamage.Value;
                    shared.m_damagesPerLevel.m_slash = WoodJavelinItem.SlashingDamagePerLevel.Value;
                    ItemManager.Instance.AddItem(WoodJavelin.GetCustomItemFromPrefab(woodJavelinPrefab));                    
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