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

        public static Color NormalColor = new Color(0.83f, 1f, 0.964f) * .8f;
        public static Color SelectedColor = new Color(1f, 0.5f, 0f);

        public int MaxValue = 10;
        public int CurrentValue = 0;

        private MeshRenderer m_renderer;
        private Material m_barGrid;

        private const string SHADERPROP_VERTICAL_DIVISIONS = "_DivisionsVertical";
        private const string SHADERPROP_HORIZONTAL_DIVISIONS = "_DivisionsHorizontal";
        private const string SHADERPROP_FILL = "_Fill";

        private void Awake()
        {
            m_renderer = GetComponent<MeshRenderer>();
            m_barGrid = m_renderer.material;

            SetColor(NormalColor);
            UpdateShaderVals(5, 2);
        }

        public void ToggleRendering(bool toggle)
        {
            m_renderer.enabled = toggle;
        }

        public void UpdateShaderVals(int verticalDivisions, int horizontalDivisions)
        {
            m_barGrid.SetFloat(SHADERPROP_FILL, GetFill());
            m_barGrid.SetInt(SHADERPROP_VERTICAL_DIVISIONS, verticalDivisions);
            m_barGrid.SetInt(SHADERPROP_HORIZONTAL_DIVISIONS, horizontalDivisions);
        }

        public void UpdatePackOrConsumableDivisions()
        {
            if (MaxValue == 0)
            {
                return;
            }
            UpdateShaderVals(MaxValue, 1);
        }

        public void UpdateWeaponMagDivisions(float ammoInClip, float maxAmmo)
        {
            int numMags = Mathf.Max(1, Mathf.RoundToInt(maxAmmo / ammoInClip));

            UpdateShaderVals(numMags, 1);
        }

        public void UpdateAmmoGridDivisions()
        {
            int horizDivisions = LargestVisuallyAppealingDivisor(MaxValue);

            int vertDivisions = MaxValue / horizDivisions;

            if (MaxValue > 10)
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

        public void UpdateFill(int current)
        {
            if (current > MaxValue)
                    current = MaxValue;
            CurrentValue = current;
            SetFill(GetFill());
        }

        private float GetFill()
        {
            if (CurrentValue == 0)
            {
                return 0;
            }
            return (float)CurrentValue / (float)MaxValue;
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
            CurrentValue = ammoLeft;
            SetFill(GetFill());
        }

        public void UpdateMainWeaponDivisions(int maxAmmo, int maxClipAmmo)
        {
            UpdateShaderVals(maxAmmo / maxClipAmmo, 2);
        }

        public static int LargestVisuallyAppealingDivisor(int n)
        {
            if (n % 2 == 0)
            {
                return n / 2;
            }
            int sqrtn = (int)Math.Sqrt(n);
            for (int i = 3; i <= sqrtn; i += 2)
            {
                if (n % i == 0)
                {
                    return n / i;
                }
            }
            return 1;
        }

    }
}