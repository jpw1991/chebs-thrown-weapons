using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsThrownWeapons.Items.Javelins
{
    public class FireJavelinItem : JavelinItem
    {
        public override string ItemName => "ChebGonaz_JavelinFire";
        public override string PrefabName => "ChebGonaz_JavelinFire.prefab";
        public override string NameLocalization => "$chebgonaz_javelinfire";
        public override string DescriptionLocalization => "$chebgonaz_javelinfire_desc";
        public string ProjectilePrefabName => "ChebGonaz_JavelinProjectileFire.prefab";
        protected override string DefaultRecipe => "Wood:20,Resin:20";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;
        public static ConfigEntry<int> CraftingStationLevel, MaxQuality;
        public static ConfigEntry<string> CraftingCost;

        public static ConfigEntry<float> BasePierceDamage,
            PierceDamagePerLevel,
            BaseSlashingDamage,
            SlashingDamagePerLevel,
            BaseFireDamage,
            FireDamagePerLevel,
            Durability, DurabilityPerLevel;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            base.CreateConfigs(plugin);

            CraftingStationRequired = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingStation",
                CraftingTable.Workbench, new ConfigDescription("Crafting station where it's available",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            CraftingStationLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "CraftingStationLevel",
                1,
                new ConfigDescription("Crafting station level required to craft",
                    new AcceptableValueRange<int>(1, 5),
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            CraftingCost = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingCosts",
                DefaultRecipe, new ConfigDescription(
                    "Materials needed to craft it. None or Blank will use Default settings.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BasePierceDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BasePierceDamage",
                20f, new ConfigDescription(
                    "The piercing damage dealt by the javelin.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            PierceDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "PierceDamagePerLevel",
                2.5f, new ConfigDescription(
                    "The bonus piercing damage dealt by the javelin every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BaseSlashingDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BaseSlashingDamage",
                2.5f, new ConfigDescription(
                    "The piercing damage dealt by the javelin.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SlashingDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SlashingDamagePerLevel",
                2.5f, new ConfigDescription(
                    "The bonus slashing damage dealt by the javelin every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BaseFireDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BaseFireDamage",
                15f, new ConfigDescription(
                    "The fire damage dealt by the javelin.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            FireDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "FireDamagePerLevel",
                5f, new ConfigDescription(
                    "The bonus fire damage dealt by the javelin every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            Durability = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "Durability",
                50f, new ConfigDescription(
                    "The base durability of the weapon.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            DurabilityPerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DurabilityPerLevel",
                10f, new ConfigDescription(
                    "The bonus durability every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            MaxQuality = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "MaxQuality",
                4, new ConfigDescription(
                    "How much the item can be upgraded. 4 is max.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        public new ItemDrop UpdateRecipe()
        {
            UpdateRecipe(CraftingStationRequired, CraftingCost, CraftingStationLevel);
            return UpdateItemValues();
        }

        public ItemDrop UpdateItemValues()
        {
            var prefab = ZNetScene.instance?.GetPrefab(ItemName) ?? PrefabManager.Instance.GetPrefab(ItemName);
            if (prefab == null)
            {
                Logger.LogError($"Failed to update item values: prefab with name {ItemName} is null");
                return null;
            }

            var projectileName = ProjectilePrefabName.Substring(0, ProjectilePrefabName.Length - 7);
            var projectilePrefab = ZNetScene.instance?.GetPrefab(projectileName)
                                   ?? PrefabManager.Instance.GetPrefab(projectileName);
            if (projectilePrefab == null)
            {
                Logger.LogError($"Failed to update item values: prefab with name {ItemName} is null");
            }
            else
            {
                projectilePrefab.GetComponent<Projectile>().m_gravity = ProjectileGravity.Value;
            }

            var item = prefab.GetComponent<ItemDrop>();
            var shared = item.m_itemData.m_shared;
            shared.m_attack.m_projectileVel = ProjectileVelocity.Value;
            shared.m_damages.m_pierce = BasePierceDamage.Value;
            shared.m_damagesPerLevel.m_pierce = PierceDamagePerLevel.Value;
            shared.m_damages.m_slash = BaseSlashingDamage.Value;
            shared.m_damagesPerLevel.m_slash = SlashingDamagePerLevel.Value;
            shared.m_damages.m_fire = BaseFireDamage.Value;
            shared.m_damagesPerLevel.m_fire = FireDamagePerLevel.Value;
            shared.m_movementModifier = MovementModifier.Value;
            shared.m_maxDurability = Durability.Value;
            shared.m_durabilityPerLevel = DurabilityPerLevel.Value;
            shared.m_maxQuality = MaxQuality.Value;
            var attack = shared.m_attack;
            attack.m_attackHitNoise = AttackHitNoise.Value;
            attack.m_attackStartNoise = AttackStartNoise.Value;
            
            return item;
        }

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab, bool fixReferences = true)
        {
            var config = new ItemConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization
            };

            if (string.IsNullOrEmpty(CraftingCost.Value))
            {
                CraftingCost.Value = DefaultRecipe;
            }

            SetRecipeReqs(
                config,
                CraftingCost,
                CraftingStationRequired,
                CraftingStationLevel
            );

            var customItem = new CustomItem(prefab, false, config);
            if (customItem.ItemPrefab == null)
            {
                Logger.LogError($"GetCustomItemFromPrefab: {PrefabName}'s ItemPrefab is null!");
                return null;
            }

            return customItem;
        }
    }
}