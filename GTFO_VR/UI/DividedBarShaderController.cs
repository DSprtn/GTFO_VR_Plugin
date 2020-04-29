using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GTFO_VR.Util;
using Player;

namespace GTFO_VR.UI
{
    class DividedBarShaderController : MonoBehaviourExtended
    {
        Material barGrid;

        public static Color normalColor = new Color(0.83f / 3, 1f / 3, 0.964f / 3);
        public static Color selectedColor = new Color(1f / 2.5f, 0.5f / 2.5f, 0f);

        public InventorySlot inventorySlot;

        void Awake()
        {
            barGrid = GetComponent<MeshRenderer>().material;
            SetColor(normalColor);
            UpdateShaderVals(5, 2);
        }

        public int maxAmmo = 10;
        public int currentAmmo = 0;

        const string vertProperty = "_DivisionsVertical";
        const string horizProperty = "_DivisionsHorizontal";
        const string fillProperty = "_Fill";

        public void UpdateShaderVals(int verticalDivisions, int horizontalDivisions)
        {
                barGrid.SetFloat(fillProperty, GetFill());
                barGrid.SetInt(vertProperty, verticalDivisions);
                barGrid.SetInt(horizProperty, horizontalDivisions);
        }


        public void UpdatePackOrConsumableDivisions()
        {
            if(maxAmmo == 0)
            {
                return;
            }
            UpdateShaderVals(maxAmmo, 1);
        }

        public void UpdateAmmoGridDivisions()
        {
            //Debug.Log("MaxAmmo: " + maxAmmo + " Current ammo: " + currentAmmo + " Inventory slot: " + inventorySlot.ToString());
            int horizDivisions = Utils.LargestDivisor(maxAmmo);

            int vertDivisions = maxAmmo / horizDivisions;

            if (maxAmmo > 10)
            {
                if (horizDivisions % 2 == 0)
                {
                    horizDivisions /= 2;
                    vertDivisions *= 2;
                }
                else if (horizDivisions % 5 == 0)
                {
                    horizDivisions /= 5;
                    vertDivisions *= 5;
                }


            }
            UpdateShaderVals(vertDivisions, horizDivisions);
        }

        float GetFill()
        {
            if(currentAmmo == 0)
            {
                return 0;
            }
            return (float)currentAmmo / (float)maxAmmo;
        }

        public void SetFill(float fill)
        {
            barGrid.SetFloat("_Fill", fill);
        }

        public void SetColor(Color color)
        {
            barGrid.SetColor("_Color", color);
        }

        public void SetSelected()
        {
            SetColor(selectedColor);
        }
        public void SetUnselected()
        {
            SetColor(normalColor);
        }

        public void UpdateCurrentAmmo(int ammoLeft)
        {
            currentAmmo = ammoLeft;
            SetFill(GetFill());
        }

        public void UpdateMainWeaponDivisions(int maxAmmo, int maxClipAmmo)
        {
            UpdateShaderVals(maxAmmo / maxClipAmmo, 2);
        }
    }
}
