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
            if(item == null)
            {
                return weaponArchetypes["Default"];
            } else
            {
                return GetVRWeaponData(item.ArchetypeName);
            }
        }

        public static VRWeaponData GetVRWeaponData(string archetype)
        {
            
            VRWeaponData data;
            if(weaponArchetypes.TryGetValue(archetype, out data))
            {
                return data;
            }
            return weaponArchetypes["Default"];
        }

        public static void Setup()
        {
            // WeaponTransform (z forward, y up, x right)
            weaponArchetypes = new Dictionary<string, VRWeaponData>();
            weaponArchetypes.Add("Default", new VRWeaponData(new Vector3(0f, 0f, 0f), false));
            weaponArchetypes.Add("Melee", new VRWeaponData(new Vector3(0f, -.33f, 0f), false));

            weaponArchetypes.Add("Mine deployer", new VRWeaponData(new Vector3(0f, 0f, -.05f), false));
            weaponArchetypes.Add("Bioscanner", new VRWeaponData(new Vector3(0f, 0f, -.05f), false));

            weaponArchetypes.Add("Pistol", new VRWeaponData(new Vector3(0f, 0f, 0f), false));
            weaponArchetypes.Add("Revolver", new VRWeaponData(new Vector3(-.02f, -.01f, 0f), false));
            weaponArchetypes.Add("SMG", new VRWeaponData(new Vector3(0f, 0f, -.15f), true));
            weaponArchetypes.Add("DMR", new VRWeaponData(new Vector3(0f, 0f, -.05f), true));
            weaponArchetypes.Add("Assault Rifle", new VRWeaponData(new Vector3(-.05f, 0f, 0f), true));
            weaponArchetypes.Add("Machinepistol", new VRWeaponData(new Vector3(-.05f, 0f, 0f), false));

            weaponArchetypes.Add("Sniper", new VRWeaponData(new Vector3(0f, 0f, -.05f), true));
            weaponArchetypes.Add("Shotgun", new VRWeaponData(new Vector3(0f, 0f, -.05f), true));
            weaponArchetypes.Add("Machinegun", new VRWeaponData(new Vector3(0f, 0f, -.07f), true));
            weaponArchetypes.Add("Combat Shotgun", new VRWeaponData(new Vector3(-.05f, 0f, 0f), true));
            weaponArchetypes.Add("Burst Rifle", new VRWeaponData(new Vector3(-.05f, 0f, 0f), true));

        }
    }
}
