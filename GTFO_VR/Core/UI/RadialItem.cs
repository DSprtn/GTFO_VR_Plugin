using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GTFO_VR.Core.UI
{
    public class RadialItem : MonoBehaviour
    {

        public RadialItem(IntPtr value)
: base(value) { }


        Color defaultColor = new Color(0.83f, 1f, 0.964f, .2f);
        Color selectedColor = new Color(1f, 0.5f, 0f, .35f);

        TextMeshPro itemText;
        TextMeshPro itemInfo;
        SpriteRenderer iconImage;
        SpriteRenderer BGImage;

        Action OnExecuted;

        public void Setup(Action OnExecuted, Sprite BG)
        {
            this.OnExecuted = OnExecuted;
            itemText = SetupText(new Vector2(45, 45));
            itemInfo = SetupText(new Vector2(25, 25));
            itemInfo.transform.localPosition = new Vector3(0, -50f, 0);
            itemInfo.text = "30%";
            SetupImage(ref BGImage, "BG", 64f);
            SetupImage(ref iconImage, "Icon", 22f,0);
            iconImage.color = new Color(.0f, .0f, .0f, 1f);
            BGImage.sprite = BG;
            BGImage.color = defaultColor;
            Hide();
        }

        IEnumerator SetSize(RectTransform rect, Vector2 size)
        {
            yield return new WaitForSeconds(0.1f);
            rect.sizeDelta = size;
        }

        private void SetupImage(ref SpriteRenderer img, string name, float scale, float zOffset =.1f)
        {
            img = new GameObject(name).AddComponent<SpriteRenderer>();
            img.transform.SetParent(transform);
            img.transform.localPosition = new Vector3(0, 0, zOffset);
            img.transform.localRotation = Quaternion.identity;
            img.transform.localScale = Vector3.one * scale;
            img.receiveShadows = false;
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
            text.fontSizeMax = 244;
            text.alignment = TextAlignmentOptions.Midline;
            text.fontSizeMin = 6;
            text.color = Color.black;
            text.outlineWidth = 0.2f;
            text.outlineColor = new Color32(255, 255, 255, 255);

            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
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

        public void SetIcon(Sprite img)
        {
            iconImage.sprite = img;
        }

        public void Select()
        {
            BGImage.color = selectedColor;
        }

        public void Deselect()
        {
            BGImage.color = defaultColor;
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
            if(itemInfo.text.Length > 0)
            {
                itemInfo.enabled = true;
            }
        }

        internal void Hide()
        {
            itemText.enabled = false;
            iconImage.enabled = false;
            BGImage.enabled = false;
            itemInfo.enabled = false;
        }
    }
}
