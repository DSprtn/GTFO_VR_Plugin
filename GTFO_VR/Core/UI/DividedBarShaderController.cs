using GTFO_VR.Util;
using System;
using UnityEngine;

namespace GTFO_VR.UI
{
    /// <summary>
    /// Responsible for managing the fancy watch UI shaders.
    /// </summary>
    internal class DividedBarShaderController : MonoBehaviour
    {
        public DividedBarShaderController(IntPtr value)
: base(value) { }

        public static Color NormalColor = new Color(0.83f, 1f, 0.964f);
        public static Color SelectedColor = new Color(1f, 0.5f, 0f);

        private Material m_barGrid;

       

        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            m_barGrid = renderer.material;

            SetColor(NormalColor);
            UpdateShaderVals(5, 2);
        }

        public int maxValue = 10;
        public int currentValue = 0;

        private const string vertProperty = "_DivisionsVertical";
        private const string horizProperty = "_DivisionsHorizontal";
        private const string fillProperty = "_Fill";

        private MeshRenderer renderer;

        public void ToggleRendering(bool toggle)
        {
            renderer.enabled = toggle;
        }

        public void UpdateShaderVals(int verticalDivisions, int horizontalDivisions)
        {
            m_barGrid.SetFloat(fillProperty, GetFill());
            m_barGrid.SetInt(vertProperty, verticalDivisions);
            m_barGrid.SetInt(horizProperty, horizontalDivisions);
        }

        public void UpdatePackOrConsumableDivisions()
        {
            if (maxValue == 0)
            {
                return;
            }
            UpdateShaderVals(maxValue, 1);
        }

        public void UpdateWeaponMagDivisions(float ammoInClip, float maxAmmo)
        {
            int numMags = Mathf.Max(1, Mathf.RoundToInt(maxAmmo / ammoInClip));

            UpdateShaderVals(numMags, 1);
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

        private float GetFill()
        {
            if (currentValue == 0)
            {
                return 0;
            }
            return (float)currentValue / (float)maxValue;
        }

        public void SetFill(float fill)
        {
            m_barGrid.SetFloat("_Fill", fill);
        }

        public void SetColor(Color color)
        {
            m_barGrid.SetColor("_Color", color);
        }

        public void SetSelected()
        {
            SetColor(SelectedColor);
        }

        public void SetUnselected()
        {
            SetColor(NormalColor);
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