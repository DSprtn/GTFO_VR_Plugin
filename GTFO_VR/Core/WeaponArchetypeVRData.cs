using GTFO_VR.Events;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Responsible for providing data for custom logic per wieldable weapon
    /// </summary>
    public static class WeaponArchetypeVRData
    {

        static VRWeaponData m_current;

        public struct VRWeaponData
        {
            public Vector3 positonOffset;
            public bool allowsDoubleHanded;
            public Quaternion rotationOffset;
            public float scaleMultiplier;

            public VRWeaponData(Vector3 transformToGrip, bool doubleHandedAim)
            {
                positonOffset = transformToGrip;
                allowsDoubleHanded = doubleHandedAim;
                rotationOffset = Quaternion.identity;
                scaleMultiplier = 1.1f;
            }

            public VRWeaponData(Vector3 posToGripOffset, Quaternion rotationOffset, bool doubleHandedAim)
            {
                positonOffset = posToGripOffset;
                allowsDoubleHanded = doubleHandedAim;
                this.rotationOffset = rotationOffset;
                scaleMultiplier = 1.1f;
            }

            public VRWeaponData(Vector3 posToGripOffset,bool doubleHandedAim, float scaleMult)
            {
                positonOffset = posToGripOffset;
                allowsDoubleHanded = doubleHandedAim;
                this.rotationOffset = Quaternion.identity;
                scaleMultiplier = scaleMult;
            }

            public VRWeaponData(Vector3 posToGripOffset, Quaternion rotationOffset, bool doubleHandedAim, float scaleMult)
            {
                positonOffset = posToGripOffset;
                allowsDoubleHanded = doubleHandedAim;
                this.rotationOffset = rotationOffset;
                scaleMultiplier = scaleMult;
            }
        }

        public static Dictionary<string, VRWeaponData> weaponArchetypes;
        public static Dictionary<string, VRWeaponData> publicWeaponNames; // Testing for localization issues with melee weapons---

        public static VRWeaponData GetVRWeaponData(ItemEquippable item)
        {
             
            return m_current;
        }

        public static void Setup()
        {
            ItemEquippableEvents.OnPlayerWieldItem += PlayerSwitchedWeapon;
            // WeaponTransform (z+ forward, y+ up, x+ right)

            publicWeaponNames = new Dictionary<string, VRWeaponData>
            {
                { "SANTONIAN HDH", new VRWeaponData(new Vector3(0f, -.25f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1.15f) },
                { "MASTABA FIXED BLADE", new VRWeaponData(new Vector3(0f, -.05f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1f) },
                { "MACO DRILLHEAD", new VRWeaponData(new Vector3(0f, -.3f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1.1f) },
                { "KOVAC PEACEKEEPER", new VRWeaponData(new Vector3(0f, -.05f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1.25f) }
            };


            weaponArchetypes = new Dictionary<string, VRWeaponData>
            {
                { "Default", new VRWeaponData(new Vector3(0f, 0f, 0f), false) },

                { "Sledgehammer", new VRWeaponData(new Vector3(0f, -.25f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1.15f) },
                { "Knife", new VRWeaponData(new Vector3(0f, -.05f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1f) },
                { "Spear", new VRWeaponData(new Vector3(0f, -.3f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1.1f) },
                { "Bat", new VRWeaponData(new Vector3(0f, -.05f, 0f), Quaternion.Euler(new Vector3(45f - VRConfig.configWeaponRotationOffset.Value, 0, 0)), false, 1.25f) },

                { "Mine deployer", new VRWeaponData(new Vector3(0f, 0f, -.05f), false) },
                { "Bio Tracker", new VRWeaponData(new Vector3(0f, 0f, -.05f), false, 1.3f) },

                { "Pistol", new VRWeaponData(new Vector3(0f, 0f, 0f), false, 1.1f) },
                { "Revolver", new VRWeaponData(new Vector3(0f, -.01f, 0f), false) },
                { "HEL Revolver", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },
                { "Autorevolver", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },
                { "Autopistol", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },
                { "Machinepistol", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },

                { "Shotgun", new VRWeaponData(new Vector3(0f, 0f, -.05f), true) },
                { "Combat Shotgun", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "Choke Mod Shotgun", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },


                { "Carbine", new VRWeaponData(new Vector3(.0f, 0f, 0f), true) },
                { "SMG", new VRWeaponData(new Vector3(0f, 0f, -.15f), true) },

                { "Sniper", new VRWeaponData(new Vector3(0f, 0f, -.05f), true) },
                { "DMR", new VRWeaponData(new Vector3(0f, 0f, -.05f), true) },
                { "Long gun", new VRWeaponData(new Vector3(0f, 0f, -.05f), true) },
                { "HEL Gun", new VRWeaponData(new Vector3(.0f, 0f, 0f), true) },

                { "Assault Rifle", new VRWeaponData(new Vector3(.0f, 0f, 0f), true) },
                { "Burst Rifle", new VRWeaponData(new Vector3(0f, 0f, 0f), true) },
                { "Bullpup Rifle", new VRWeaponData(new Vector3(.0f, 0f, -.08f), true) },
                { "Rifle", new VRWeaponData(new Vector3(.0f, 0f, 0f), true) },
                { "HEL Rifle", new VRWeaponData(new Vector3(.0f, 0f, 0f), true) },

                { "Machinegun", new VRWeaponData(new Vector3(0f, 0f, -.07f), true) },
                { "Burst Cannon", new VRWeaponData(new Vector3(.0f, 0f, 0f), true) }
            };

            m_current = weaponArchetypes["Default"];
        }

        private static void PlayerSwitchedWeapon(ItemEquippable item)
        {
            float muzzleDistance = 0;
            if(item.MuzzleAlign)
            {
                muzzleDistance = Vector3.Distance(item.transform.position, item.MuzzleAlign.position);
            }
            
            Log.Debug($"Item {item.ArchetypeName} - MuzzleDistance {muzzleDistance} - Allows DH? {muzzleDistance > 0.25f}");

            if (weaponArchetypes.TryGetValue(item.ArchetypeName, out VRWeaponData data) || publicWeaponNames.TryGetValue(item.PublicName.ToUpper(), out data))
            {
                m_current = data;

                item.transform.localScale = Vector3.one * m_current.scaleMultiplier;
            }
            else
            {
                if (item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearStandard) || item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearSpecial))
                {
                    VRWeaponData currentData = new VRWeaponData(Vector3.zero, Quaternion.identity, false);
                    currentData.allowsDoubleHanded = muzzleDistance > 0.25f;
                    weaponArchetypes.Add(item.ArchetypeName, currentData);
                    m_current = currentData;
                    Log.Debug($"Item {item.ArchetypeName} - MuzzleDistance {muzzleDistance} - Allows DH? {currentData.allowsDoubleHanded}");
                }
                else
                {
                    m_current = weaponArchetypes["Default"];
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
