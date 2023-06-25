using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsThrownWeapons.Locations
{
    public class ThrownWeaponsLocation : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_ThrownWeapons.prefab";
        public const string NameLocalization = "$chebgonaz_swordinthestone";

        private void Awake()
        {
            Logger.LogInfo($"Awakening at {transform.position}");

            if (!ChebsThrownWeapons.ShowMapMarker.Value) return;
            
            Minimap.instance.AddPin(transform.position,
                ChebsThrownWeapons.MapMarker.Value, 
                ChebsThrownWeapons.Localization.TryTranslate(NameLocalization), 
                true, false,
                Game.instance.GetPlayerProfile().GetPlayerID());
        }
    }
}