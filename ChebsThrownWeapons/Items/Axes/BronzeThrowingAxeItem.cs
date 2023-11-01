using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsThrownWeapons.Items.Axes
{
    public class BronzeThrowingAxeItem : ThrowingAxeItem
    {
        public override string ItemName => "ChebGonaz_ThrowingAxeBronze";
        public override string PrefabName => "ChebGonaz_ThrowingAxeBronze.prefab";
        public override string NameLocalization => "$chebgonaz_throwingaxebronze";
        public override string DescriptionLocalization => "$chebgonaz_throwingaxebronze_desc";
        public string ProjectilePrefabName => "ChebGonaz_ThrowingAxeBronzeProjectile.prefab";
        protected override string DefaultRecipe => "Bronze:20,Wood:20";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;
        public static ConfigEntry<int> CraftingStationLevel, MaxQuality;
        public static ConfigEntry<string> CraftingCost;

        public static ConfigEntry<float> BaseBluntDamage,
            BluntDamagePerLevel,
            BaseSlashingDamage,
            SlashingDamagePerLevel,
            Durability, DurabilityPerLevel;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            base.CreateConfigs(plugin);

            CraftingStationRequired = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingStation",
                CraftingTable.Forge, new ConfigDescription("Crafting station where it's available",
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

            BaseBluntDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BaseBluntDamage",
                25f, new ConfigDescription(
                    "The blunt damage dealt by the throwing axe.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BluntDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BluntDamagePerLevel",
                5f, new ConfigDescription(
                    "The bonus blunt damage dealt by the throwing axe every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BaseSlashingDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BaseSlashingDamage",
                10f, new ConfigDescription(
                    "The piercing damage dealt by the throwing axe.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SlashingDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SlashingDamagePerLevel",
                5f, new ConfigDescription(
                    "The bonus slashing damage dealt by the throwing axe every time you upgrade.", null,
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
            shared.m_damages.m_blunt = BaseBluntDamage.Value;
            shared.m_damagesPerLevel.m_blunt = BluntDamagePerLevel.Value;
            shared.m_damages.m_slash = BaseSlashingDamage.Value;
            shared.m_damagesPerLevel.m_slash = SlashingDamagePerLevel.Value;
            shared.m_movementModifier = MovementModifier.Value;
            shared.m_maxDurability = Durability.Value;
            shared.m_durabilityPerLevel = DurabilityPerLevel.Value;
            shared.m_maxQuality = MaxQuality.Value;
            var attack = shared.m_attack;
            attack.m_attackHitNoise = AttackHitNoise.Value;
            attack.m_attackStartNoise = AttackStartNoise.Value;

            return item;
        }

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab)
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