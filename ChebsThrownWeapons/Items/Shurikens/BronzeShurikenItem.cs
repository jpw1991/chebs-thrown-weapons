using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsThrownWeapons.Items.Shurikens
{
    public class BronzeShurikenItem : ShurikenItem
    {
        public override string ItemName => "ChebGonaz_ShurikenBronze";
        public override string PrefabName => "ChebGonaz_ShurikenBronze.prefab";
        public override string NameLocalization => "$chebgonaz_shurikenbronze";
        public override string DescriptionLocalization => "$chebgonaz_shurikenbronze_desc";
        public string ProjectilePrefabName => "ChebGonaz_ShurikenProjectileBronze.prefab";
        protected override string DefaultRecipe => "Bronze:20";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;
        public static ConfigEntry<int> CraftingStationLevel;
        public static ConfigEntry<string> CraftingCost;

        public static ConfigEntry<float> BasePierceDamage,
            PierceDamagePerLevel,
            BaseSlashingDamage,
            SlashingDamagePerLevel,
            BasePoisonDamage,
            PoisonDamagePerLevel;

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

            BasePierceDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BasePierceDamage",
                5f, new ConfigDescription(
                    "The piercing damage dealt by the shuriken.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            PierceDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "PierceDamagePerLevel",
                5f, new ConfigDescription(
                    "The bonus piercing damage dealt by the shuriken every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BaseSlashingDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BaseSlashingDamage",
                15f, new ConfigDescription(
                    "The piercing damage dealt by the shuriken.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SlashingDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SlashingDamagePerLevel",
                5f, new ConfigDescription(
                    "The bonus slashing damage dealt by the shuriken every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BasePoisonDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BasePoisonDamage",
                5f, new ConfigDescription(
                    "The poison damage dealt by the shuriken.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            PoisonDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "PoisonDamagePerLevel",
                5f, new ConfigDescription(
                    "The bonus poison damage dealt by the shuriken every time you upgrade.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        public override void UpdateRecipe()
        {
            UpdateRecipe(CraftingStationRequired, CraftingCost, CraftingStationLevel);

            PrefabManager.Instance.GetPrefab(ProjectilePrefabName.Substring(0, ProjectilePrefabName.Length - 7))
                .GetComponent<Projectile>().m_gravity = ProjectileGravity.Value;

            var shared = ItemManager.Instance.GetItem(ItemName).ItemDrop.m_itemData.m_shared;
            shared.m_attack.m_projectileVel = ProjectileVelocity.Value;
            shared.m_damages.m_pierce = BasePierceDamage.Value;
            shared.m_damagesPerLevel.m_pierce = PierceDamagePerLevel.Value;
            shared.m_damages.m_slash = BaseSlashingDamage.Value;
            shared.m_damagesPerLevel.m_slash = SlashingDamagePerLevel.Value;
            shared.m_damages.m_poison = BasePoisonDamage.Value;
            shared.m_damagesPerLevel.m_poison = PoisonDamagePerLevel.Value;
            var attack = shared.m_attack;
            attack.m_attackHitNoise = AttackHitNoise.Value;
            attack.m_attackStartNoise = AttackStartNoise.Value;
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