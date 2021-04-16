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
            public Vector3 transformToVRGrip;
            public bool allowsDoubleHanded;
            public Quaternion rotationOffset;

            public VRWeaponData(Vector3 transformToGrip, bool doubleHandedAim)
            {
                transformToVRGrip = transformToGrip;
                allowsDoubleHanded = doubleHandedAim;
                rotationOffset = Quaternion.identity;
            }

            public VRWeaponData(Vector3 transformToGrip, Quaternion rotationOffset, bool doubleHandedAim)
            {
                transformToVRGrip = transformToGrip;
                allowsDoubleHanded = doubleHandedAim;
                this.rotationOffset = rotationOffset;
            }
        }

        public static Dictionary<string, VRWeaponData> weaponArchetypes;

        public static VRWeaponData GetVRWeaponData(ItemEquippable item)
        {
            return m_current;
        }

        public static void Setup()
        {
            ItemEquippableEvents.OnPlayerWieldItem += PlayerSwitchedWeapon;
            // WeaponTransform (z+ forward, y+ up, x+ right)
            weaponArchetypes = new Dictionary<string, VRWeaponData>
            {
                { "Default", new VRWeaponData(new Vector3(0f, 0f, 0f), false) },
                { "DefaultDoubleHanded", new VRWeaponData(new Vector3(0f, 0f, -0.05f), true) },
                { "Melee", new VRWeaponData(new Vector3(0f, -.25f, 0f), Quaternion.Euler(new Vector3(45f, 0, 0)), false) },

                { "Mine deployer", new VRWeaponData(new Vector3(0f, 0f, -.05f), false) },
                { "Bio Tracker", new VRWeaponData(new Vector3(0f, 0f, -.05f), false) },

                { "Pistol", new VRWeaponData(new Vector3(0f, 0f, 0f), false) },
                { "Revolver", new VRWeaponData(new Vector3(0f, -.01f, 0f), false) },
                { "HEL Revolver", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },
                { "Autorevolver", new VRWeaponData(new Vector3(.0f, 0f, 0f), false) },
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
            if (weaponArchetypes.TryGetValue(item.ArchetypeName, out VRWeaponData data))
            {
                m_current = data;
            }
            else
            {
                if (item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearStandard) || item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearSpecial))
                {
                    m_current = weaponArchetypes["DefaultDoubleHanded"];
                }
                else
                {
                    m_current = weaponArchetypes["Default"];
                }
            }
            CalculateGripOffset();
        }

        public static Vector3 CalculateGripOffset()
        {
            Transform itemEquip = ItemEquippableEvents.currentItem.transform;
            return itemEquip.position - itemEquip.TransformPoint(m_current.transformToVRGrip);
        }

    }
}
