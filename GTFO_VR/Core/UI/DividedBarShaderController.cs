using System;
using UnityEngine;
using GTFO_VR.Util;
using Player;

namespace GTFO_VR.UI
{
    class DividedBarShaderController : MonoBehaviour
    {

        public DividedBarShaderController(IntPtr value)
: base(value) { }

        Material barGrid;

        public static Color normalColor = new Color(0.83f, 1f, 0.964f);
        public static Color selectedColor = new Color(1f, 0.5f, 0f);

        void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            barGrid = renderer.material;
           
            SetColor(normalColor);
            UpdateShaderVals(5, 2);
        }

        public int maxValue = 10;
        public int currentValue = 0;

        const string vertProperty = "_DivisionsVertical";
        const string horizProperty = "_DivisionsHorizontal";
        const string fillProperty = "_Fill";

        MeshRenderer renderer;

        public void ToggleRendering(bool toggle)
        {
            renderer.enabled = toggle;
        }

        public void UpdateShaderVals(int verticalDivisions, int horizontalDivisions)
        {
                barGrid.SetFloat(fillProperty, GetFill());
                barGrid.SetInt(vertProperty, verticalDivisions);
                barGrid.SetInt(horizProperty, horizontalDivisions);
        }


        public void UpdatePackOrConsumableDivisions()
        {
            if(maxValue == 0)
            {
                return;
            }
            UpdateShaderVals(maxValue, 1);
        }

        public void UpdateWeaponMagDivisions(float ammoInClip, float maxAmmo)
        {
            int numMags = Mathf.Max(1,Mathf.RoundToInt(maxAmmo / ammoInClip));

            UpdateShaderVals(numMags,1);
        }

        public void UpdateAmmoGridDivisions()
        {
            int horizDivisions = Utils.LargestDivisor(maxValue);

            int vertDivisions = maxValue / horizDivisions;

            if (maxValue > 10)
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
            if(currentValue == 0)
            {
                return 0;
            }
            return (float)currentValue / (float)maxValue;
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
            currentValue = ammoLeft;
            SetFill(GetFill());
        }

        public void UpdateMainWeaponDivisions(int maxAmmo, int maxClipAmmo)
        {
            UpdateShaderVals(maxAmmo / maxClipAmmo, 2);
        }
    }
}
