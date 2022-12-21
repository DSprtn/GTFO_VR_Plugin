﻿using GTFO_VR.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Responsible for providing data for custom logic per wieldable weapon
    /// </summary>
    public static partial class WeaponArchetypeVRData
    {
        static VRWeaponData m_current;

        static Dictionary<string, VRWeaponData> weaponDataByPublicName;
        static Dictionary<string, ProtubeHapticsData> weaponHapticDataByPublicName;

        static readonly string HAPTICS_DATA_PATH = Path.Combine(BepInEx.Paths.PluginPath, "protubeHaptics/HapticsData.json");

        public static VRWeaponData GetVRWeaponData()
        {
            return m_current;
        }

        static Dictionary<string, ProtubeHapticsData> DeserializeHapticsData()
        {
            Dictionary<string, ProtubeHapticsData> m = null;
            try
            {
                m = JsonConvert.DeserializeObject<Dictionary<string, ProtubeHapticsData>>(File.ReadAllText(HAPTICS_DATA_PATH));
            } catch (JsonException)
            {
                Log.Error($"Failed to load haptics data! Haptics will not use finetuned values!");
            }
            return m;
        }

        public static ProtubeHapticsData GetVRWeaponHapticData(string publicWeaponName)
        {
            if (weaponHapticDataByPublicName.TryGetValue(publicWeaponName.ToUpper(), out ProtubeHapticsData data))
            {
                return data;
            }
            Log.Warning($"Did not find haptics data for {publicWeaponName}");
            return null;
        }

        internal static void LoadHapticsData()
        {
            weaponHapticDataByPublicName = DeserializeHapticsData();
            foreach(var data in weaponHapticDataByPublicName.Keys)
            {
                Log.Debug($"Loaded data for {data}");
            }
        }

        public static void Setup()
        {
            weaponDataByPublicName = new Dictionary<string, VRWeaponData>();
            LoadHapticsData();

            ItemEquippableEvents.OnPlayerWieldItem += PlayerSwitchedWeapon;

            // WeaponTransform offsets are --- x+ right, y+ up, z+ forward ---
            weaponDataByPublicName = new Dictionary<string, VRWeaponData>
            {
                { "Default", new VRWeaponData(new Vector3(0f, 0f, 0f), false) },
                // Melee
                { "SANTONIAN HDH", new VRWeaponData(new Vector3(0f, -.25f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false) },
                { "MASTABA FIXED BLADE", new VRWeaponData(new Vector3(0f, -.05f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false) },
                { "MACO DRILLHEAD", new VRWeaponData(new Vector3(0f, -.3f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false) },
                { "KOVAC PEACEKEEPER", new VRWeaponData(new Vector3(0f, -.05f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false) },

                // Tool
                { "STALWART FLOW G2", new VRWeaponData(new Vector3(0f, 0f, 0f), false) },
                { "D-TEK OPTRON IV", new VRWeaponData(new Vector3(0f, 0f, -.05f), false, 1.3f) },
                { "KRIEGER 04", new VRWeaponData(new Vector3(0f, 0f, -.05f), false) },

                // Primary
                { "SHELLING S49", new VRWeaponData(new Vector3(0f, 0f, 0f), false) },
                { "BATALDO 3RB", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },
                { "ACCRAT GOLOK DA", new VRWeaponData(new Vector3(.0f, 0f, -.08f), true) },
                { "VAN AUKEN LTC5", new VRWeaponData(new Vector3(0f, 0f, -.15f), true) },
                { "ACCRAT STB", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "VAN AUKEN CAB F4", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "TR22 HANAWAY", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "MALATACK LX", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "BATALDO J 300", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },

                // Special
                { "MALATACK HXC", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "BUCKLAND S870", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "BUCKLAND XDIST2", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "BUCKLAND AF6", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "MASTABA R66", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "TECHMAN ARBALIST V", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "TECHMAN VERUTA XII", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "SHELLING ARID 5", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "DREKKER DEL P1", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "KÖNING PR 11", new VRWeaponData(new Vector3(0f, 0f, 0f), true) }
            };

            m_current = weaponDataByPublicName["Default"];
        }

        private static void PlayerSwitchedWeapon(ItemEquippable item)
        {
            float muzzleDistance = 0;
            if(item.MuzzleAlign)
            {
                muzzleDistance = Vector3.Distance(item.transform.position, item.MuzzleAlign.position);
            }
            
            Log.Debug($"Item {item.ArchetypeName} - MuzzleDistance {muzzleDistance} - Allows DH? {muzzleDistance > 0.25f}");

            if (weaponDataByPublicName.TryGetValue(item.PublicName.ToUpper(), out VRWeaponData data))
            {
                m_current = data;
                item.transform.localScale = Vector3.one * m_current.scaleMultiplier;
            }
            else
            {
                Log.Debug($"Did not find item in publlic list - {item.PublicName}");
                if (item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearStandard) || item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearSpecial))
                {
                    VRWeaponData currentData = new VRWeaponData(Vector3.zero, Quaternion.identity, false);
                    currentData.allowsDoubleHanded = muzzleDistance > 0.25f;
                    weaponDataByPublicName.Add(item.PublicName.ToUpper(), currentData);
                    m_current = currentData;
                    Log.Debug($"Item {item.ArchetypeName} - MuzzleDistance {muzzleDistance} - Allows DH? {currentData.allowsDoubleHanded}");
                }
                else
                {
                    m_current = weaponDataByPublicName["Default"];
                }
            }
        }

        public static Vector3 CalculateGripOffset()
        {
            Transform itemEquip = ItemEquippableEvents.currentItem.transform;
            return itemEquip.position - itemEquip.TransformPoint(m_current.positonOffset);
        }

    }
}
