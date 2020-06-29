using GTFO_VR.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Util
{
    public static class WeaponArchetypeVRData
    {

        static VRWeaponData current;

        public struct VRWeaponData
        {
            public Vector3 transformToVRGrip;
            public bool allowsDoubleHanded;
            
            public VRWeaponData(Vector3 transformToGrip, bool doubleHandedAim)
            {
                transformToVRGrip = transformToGrip;
                this.allowsDoubleHanded = doubleHandedAim;
            }
        }

        public static Dictionary<string, VRWeaponData> weaponArchetypes; 

        public static VRWeaponData GetVRWeaponData(ItemEquippable item)
        {
            return current;
        }

        public static void Setup()
        {
            ItemEquippableEvents.OnPlayerWieldItem += PlayerSwitchedWeapon;
            // WeaponTransform (z forward, y up, x right)
            weaponArchetypes = new Dictionary<string, VRWeaponData>();
            weaponArchetypes.Add("Default", new VRWeaponData(new Vector3(0f, 0f, 0f), false));
            weaponArchetypes.Add("DefaultDoubleHanded", new VRWeaponData(new Vector3(0f, 0f, -0.05f), true));
            weaponArchetypes.Add("Melee", new VRWeaponData(new Vector3(0f, -.10f, 0f), false));

            weaponArchetypes.Add("Mine deployer", new VRWeaponData(new Vector3(0f, 0f, -.05f), false));
            weaponArchetypes.Add("Bioscanner", new VRWeaponData(new Vector3(0f, 0f, -.05f), false));

            weaponArchetypes.Add("Pistol", new VRWeaponData(new Vector3(0f, 0f, 0f), false));
            weaponArchetypes.Add("Revolver", new VRWeaponData(new Vector3(0f, -.01f, 0f), false));
            weaponArchetypes.Add("SMG", new VRWeaponData(new Vector3(0f, 0f, -.15f), true));
            weaponArchetypes.Add("DMR", new VRWeaponData(new Vector3(0f, 0f, -.05f), true));
            weaponArchetypes.Add("Assault Rifle", new VRWeaponData(new Vector3(.0f, 0f, 0f), true));

            weaponArchetypes.Add("Machinepistol", new VRWeaponData(new Vector3(.0f, 0f, 0f), false));

            weaponArchetypes.Add("Sniper", new VRWeaponData(new Vector3(0f, 0f, -.05f), true));
            weaponArchetypes.Add("Shotgun", new VRWeaponData(new Vector3(0f, 0f, -.05f), true));
            weaponArchetypes.Add("Machinegun", new VRWeaponData(new Vector3(0f, 0f, -.07f), true));
            weaponArchetypes.Add("Combat Shotgun", new VRWeaponData(new Vector3(0f, 0f, 0f), true));
            weaponArchetypes.Add("Burst Rifle", new VRWeaponData(new Vector3(0f, 0f, 0f), true));

            weaponArchetypes.Add("Carbine", new VRWeaponData(new Vector3(.0f, 0f, 0f), true));

            weaponArchetypes.Add("Bullpup Rifle", new VRWeaponData(new Vector3(.0f, 0f, -.08f), true));
            weaponArchetypes.Add("Rifle", new VRWeaponData(new Vector3(.0f, 0f, 0f), true));

            weaponArchetypes.Add("HEL Gun", new VRWeaponData(new Vector3(.0f, 0f, 0f), true));
            weaponArchetypes.Add("HEL Rifle", new VRWeaponData(new Vector3(.0f, 0f, 0f), true));
            weaponArchetypes.Add("Burst Cannon", new VRWeaponData(new Vector3(.0f, 0f, 0f), true));

            current = weaponArchetypes["Default"];
        }

        private static void PlayerSwitchedWeapon(ItemEquippable item)
        {
            VRWeaponData data;
            if (weaponArchetypes.TryGetValue(item.ArchetypeName, out data))
            {
                current = data;
            }
            else 
            { 
                if(item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearStandard) || item.ItemDataBlock.inventorySlot.Equals(Player.InventorySlot.GearSpecial)) {
                    current = weaponArchetypes["DefaultDoubleHanded"];
                } else
                {
                    current = weaponArchetypes["Default"];
                }
            }
            CalculateGripOffset();
        }

        public static Vector3 CalculateGripOffset()
        {
           Transform itemEquip = ItemEquippableEvents.currentItem.transform;
           return itemEquip.position - itemEquip.TransformPoint(current.transformToVRGrip);
        }
    }
}
