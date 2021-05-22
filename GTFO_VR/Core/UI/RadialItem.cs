using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GTFO_VR.Core.UI
{
    public class RadialItem : MonoBehaviour
    {
        public RadialItem(IntPtr value)
: base(value) { }

        public bool Active = true;
        public float scale = 1f;

        private Color defaultColor = new Color(0.83f, 1f, 0.964f, .2f);
        private Color inactiveColor = new Color(0.988f, 0.192f, 0.192f, .2f);
        private Color selectedColor = new Color(1f, 0.5f, 0f, .35f);

        private TextMeshPro itemText;
        private TextMeshPro itemInfo;
        private SpriteRenderer iconImage;
        private SpriteRenderer BGImage;

        private Action OnExecuted;

        private float m_iconBaseScale = 21f;

        private bool m_itemInfoTextEnabled = true;

        public void Setup(Action OnExecuted, Sprite BG)
        {
            this.OnExecuted = OnExecuted;
            itemText = SetupText(new Vector2(45, 45) * scale);
            itemInfo = SetupText(new Vector2(15, 12.5f) * scale);
            itemInfo.enableAutoSizing = false;
            itemInfo.fontSize = 110f;
            itemInfo.enableWordWrapping = false;
            itemInfo.transform.localPosition = new Vector3(0, -53f, 0) * scale;
            itemInfo.lineSpacing = -45f;
            SetupImage(ref BGImage, "BG", 64f * scale);
            SetupImage(ref iconImage, "Icon", 21f * scale, 0);
            iconImage.color = new Color(.0f, .0f, .0f, 1f);
            BGImage.sprite = BG;
            BGImage.color = defaultColor;
            Hide();
        }

        private IEnumerator SetSize(RectTransform rect, Vector2 size)
        {
            yield return new WaitForSeconds(0.1f);
            rect.sizeDelta = size;
        }

        private void SetupImage(ref SpriteRenderer img, string name, float scale, float zOffset = .1f)
        {
            img = new GameObject(name).AddComponent<SpriteRenderer>();
            img.transform.SetParent(transform);
            img.transform.localPosition = new Vector3(0, 0, zOffset);
            img.transform.localRotation = Quaternion.identity;
            img.transform.localScale = Vector3.one * scale;
            img.receiveShadows = false;
            img.material.shader = VRAssets.SpriteAlwaysRender;
        }

        private TextMeshPro SetupText(Vector2 size)
        {
            TextMeshPro text = new GameObject("Text").AddComponent<TextMeshPro>();
            text.transform.SetParent(transform);
            text.transform.localPosition = Vector3.zero;
            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = Vector3.one;

            text.fontStyle = FontStyles.Bold;
            text.autoSizeTextContainer = true;
            MelonCoroutines.Start(SetSize(text.rectTransform, size));
            text.enableAutoSizing = true;
            text.fontSize = 144f;
            text.fontSizeMax = 244f;
            text.alignment = TextAlignmentOptions.Midline;
            text.fontSizeMin = 55f;
            text.color = Color.white;
            text.outlineWidth = 0.2f;
            text.outlineColor = new Color32(0, 0, 0, 255);

            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
            text.fontSharedMaterial.shader = VRAssets.GetTextNoCull();
            text.ForceMeshUpdate(false);
            return text;
        }

        public void SetInfoText(string text)
        {
            this.itemInfo.text = text;
            this.itemInfo.ForceMeshUpdate(false);
        }

        public void SetText(string text)
        {
            this.itemText.text = text;
            this.itemText.ForceMeshUpdate(false);
        }

        public void SetIcon(Sprite img, float scaleMult = 1f)
        {
            iconImage.sprite = img;
            iconImage.transform.localScale = Vector2.one * m_iconBaseScale * scale * scaleMult;
        }

        public void Select()
        {
            BGImage.color = selectedColor;
        }

        public void Deselect()
        {
            if (Active)
            {
                BGImage.color = defaultColor;
            }
            else
            {
                BGImage.color = inactiveColor;
            }
        }

        public void Execute()
        {
            Deselect();
            OnExecuted?.Invoke();
        }

        internal void Show()
        {
            if (iconImage.sprite != null)
            {
                iconImage.enabled = true;
                itemText.enabled = false;
            }
            else
            {
                iconImage.enabled = false;
                itemText.enabled = true;
            }
            BGImage.enabled = true;
            if(m_itemInfoTextEnabled)
            {
                itemInfo.enabled = true;
            }

            itemInfo.ForceMeshUpdate(false);
            itemText.ForceMeshUpdate(false);
        }

        internal void Hide()
        {
            itemText.enabled = false;
            iconImage.enabled = false;
            BGImage.enabled = false;
            itemInfo.enabled = false;
            itemInfo.ForceMeshUpdate(false);
            itemText.ForceMeshUpdate(false);
        }


        public void ToggleInfoText(bool toggle)
        {
            m_itemInfoTextEnabled = toggle;
        }
    }
}