using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using Player;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GTFO_VR.Core.UI
{
    public class WeaponAmmoHologram : MonoBehaviour
    {
        public WeaponAmmoHologram(IntPtr value) : base(value) { }

        List<TextMeshPro> m_holoTextDisplays = new List<TextMeshPro>();
        List<SpriteRenderer> m_holoBGs = new List<SpriteRenderer>();
        Light m_light;
        GameObject m_holoHolder;

        Vector3 offset = new Vector3(-.1f, 0, 0);

        Color m_defaultHoloColor = new Color(0, .5f, .5f, 1);

        public void Setup()
        {
            m_holoHolder = new GameObject("WeaponHoloText");

            AddSubText(m_holoHolder.transform, .0f);
            AddSubText(m_holoHolder.transform, .15f);
            AddSubText(m_holoHolder.transform, .25f);

            m_light = m_holoHolder.AddComponent<Light>();
            m_light.range = 0.15f;
            m_light.shadows = LightShadows.None;
            m_light.intensity = 1.1f;
            m_light.color = m_defaultHoloColor;

            m_holoHolder.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            ItemEquippableEvents.OnPlayerWieldItem += ChangedWeapon;
            InventoryAmmoEvents.OnInventoryAmmoUpdate += AmmoUpdate;
            VRConfig.configWeaponAmmoHoloText.SettingChanged += HoloToggled;
            PlayerLocomotionEvents.OnStateChange += PLOCStateChange;
            FocusStateEvents.OnFocusStateChange += StateChange;
            Toggle(false);
        }

        private void PLOCStateChange(PlayerLocomotion.PLOC_State state)
        {
            ChangedWeapon(ItemEquippableEvents.currentItem);
        }

        private void StateChange(eFocusState newState)
        {
            if(newState != eFocusState.FPS)
            {
                Toggle(false);
            } else
            {
                ChangedWeapon(ItemEquippableEvents.currentItem);
            }
        }

        private void HoloToggled(object sender, EventArgs e)
        {
            Toggle(VRConfig.configWeaponAmmoHoloText.Value);
        }

        private void AmmoUpdate(InventorySlotAmmo item, int clipLeft)
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon() &&
                ItemEquippableEvents.currentItem.ItemDataBlock.inventorySlot.Equals(item.Slot))
            {
                SetText(clipLeft.ToString());
            }
        }

        void Toggle(bool toggle)
        {
            if (!VRConfig.configWeaponAmmoHoloText.Value)
            {
                toggle = false;
            }
            foreach(TextMeshPro t in m_holoTextDisplays)
            {
                t.enabled = toggle;
                t.ForceMeshUpdate(false);
            }
            foreach(SpriteRenderer r in m_holoBGs)
            {
                r.enabled = toggle;
            }
            m_light.enabled = toggle;
        }

        void SetText(string text)
        {
            foreach(TextMeshPro t in m_holoTextDisplays)
            {
                t.text = text;
                t.ForceMeshUpdate(false);
            }
        }

        private void ChangedWeapon(ItemEquippable e)
        {
            if(ItemEquippableEvents.IsCurrentItemShootableWeapon() && PlayerLocomotionEvents.InControllablePLOCState())
            {
                Toggle(true);
            } else
            {
                Toggle(false);
            }
        }

        void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= ChangedWeapon;
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= AmmoUpdate;
            VRConfig.configWeaponAmmoHoloText.SettingChanged -= HoloToggled;
            FocusStateEvents.OnFocusStateChange -= StateChange;
            PlayerLocomotionEvents.OnStateChange -= PLOCStateChange;
        }

        internal void UpdateTransform()
        {
            if(ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                ItemEquippable current = ItemEquippableEvents.currentItem;
                m_holoHolder.transform.rotation = Quaternion.LookRotation(current.MuzzleAlign.forward, current.MuzzleAlign.up);
                Vector3 posOffset = VRConfig.configUseLeftHand.Value ? -offset : offset;
                float muzzleDistance = (current.transform.position - current.MuzzleAlign.position).magnitude;
                m_holoHolder.transform.position = current.transform.TransformPoint(posOffset + new Vector3(0,0, muzzleDistance * .5f));
            }
        }

        void AddSubText(Transform parent, float zOffset)
        {
            GameObject textSubDisplay = new GameObject("HoloText");
            textSubDisplay.transform.SetParent(parent);
            textSubDisplay.transform.localPosition = new Vector3(0, 0, zOffset);
            TextMeshPro holoText = textSubDisplay.AddComponent<TextMeshPro>();
            holoText.fontSize = 22f;
            holoText.faceColor = m_defaultHoloColor;
            holoText.fontMaterial.EnableKeyword(ShaderUtilities.Keyword_Glow);
            holoText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, m_defaultHoloColor);
            holoText.fontMaterial.SetFloat("_GlowPower", 0.065f);
            holoText.fontMaterial.SetFloat("_GlowOuter", 0.162f);
            holoText.fontMaterial.SetFloat("_GlowInner", 0.215f);
            holoText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -.5f);
            holoText.alignment = TextAlignmentOptions.CenterGeoAligned;
            holoText.fontStyle = FontStyles.Bold;

            holoText.UpdateMeshPadding();
            holoText.ForceMeshUpdate(false);


            GameObject spriteHolder = new GameObject("HoloBG");
            spriteHolder.transform.SetParent(textSubDisplay.transform);
            spriteHolder.transform.localPosition = Vector3.zero;
            spriteHolder.transform.localScale = Vector3.one * 3f;

            SpriteRenderer holoBG = spriteHolder.AddComponent<SpriteRenderer>();
            holoBG.sprite = VRAssets.holoBG;
            holoBG.color = m_defaultHoloColor * new Vector4(1, 1, 1, 0.04f);
            m_holoBGs.Add(holoBG);


            m_holoTextDisplays.Add(holoText);
        }
    }
}