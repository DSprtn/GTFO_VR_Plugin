using GTFO_VR.Core.VR_Input;
using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Core.UI
{
    public class RadialMenu : MonoBehaviour
    {
        public RadialMenu(IntPtr value)
: base(value) { }

        private UnityEngine.Canvas m_canvas;

        private InteractionHand targetHand;
        private GameObject originOverride;

        private List<RadialItem> radialItems;
        private RadialItem closest;

        private float m_maxDistance = 0.125f;
        private float m_itemDistance = 135f;
        private float m_outsideofMenuDistance = 0.3f;

        private float m_scale = 1.25f;
        private float m_lastOpenTime;
        private RadialItem m_lastHovered;

        public event Action<float> OnMenuClosedWithoutItem;

        public void Setup(InteractionHand hand, GameObject originOverride = null)
        {
            this.originOverride = originOverride;
            targetHand = hand;
            m_canvas = gameObject.AddComponent<UnityEngine.Canvas>();
            m_canvas.renderMode = RenderMode.WorldSpace;
            m_canvas.enabled = false;
            RectTransform canvasTransform = m_canvas.GetComponent<RectTransform>();
            MelonCoroutines.Start(SetSize(canvasTransform, new Vector2(80, 80) * m_scale));
            canvasTransform.localScale = Vector3.one * .001f;
            radialItems = new List<RadialItem>();
        }

        private IEnumerator SetSize(RectTransform rect, Vector2 size)
        {
            yield return new WaitForSeconds(0.1f);
            rect.sizeDelta = size;
        }

        private void Update()
        {
            if (m_canvas == null)
            {
                return;
            }
            if (m_canvas.enabled)
            {
                SelectClosestRadialItem();
                CheckForQuickSelectOutsideMenu();
            }
        }

        private void CheckForQuickSelectOutsideMenu()
        {
            if (m_lastHovered && m_lastHovered.Active)
            {
                GameObject hand = GetHand();
                float distance = Vector3.Distance(transform.position, hand.transform.position);
                if (distance > m_outsideofMenuDistance)
                {
                    closest = m_lastHovered;
                    Hide();
                }
            }
        }

        public void AddRadialItem(string text, Action OnExecuted, out RadialItem newItem)
        {
            RadialItem item = new GameObject("RadialItem").AddComponent<RadialItem>();
            item.transform.SetParent(transform);
            item.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            item.transform.localScale = Vector3.one;
            item.scale = m_scale;
            item.Setup(OnExecuted, VRAssets.RadialBG);
            item.SetText(text);
            newItem = item;
            radialItems.Add(item);
        }

        private GameObject GetHand()
        {
            GameObject hand = Controllers.GetInteractionHandGO(targetHand);
            if (originOverride != null)
            {
                hand = originOverride;
            }
            return hand;
        }

        private void SelectClosestRadialItem()
        {
            RadialItem lastClosest = closest;
            float closestDistance = 9999f;

            GameObject hand = GetHand();

            if (closest != null && Vector3.Distance(closest.transform.position, hand.transform.position) > m_maxDistance * m_scale)
            {
                closest = null;
            }

            foreach (RadialItem item in radialItems)
            {
                if (item != closest)
                {
                    item.Deselect();
                }
            }

            foreach (RadialItem item in radialItems)
            {
                if (!item.Active)
                {
                    continue;
                }
                float distance = Vector3.Distance(item.transform.position, hand.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = item;
                }
            }

            if (closest != null && closest != lastClosest)
            {
                if (closestDistance < m_maxDistance * m_scale)
                {
                    CellSound.Post(AK.EVENTS.GAME_MENU_SELECT_GEAR, closest.transform.position);
                    SteamVR_InputHandler.TriggerHapticPulse(0.02f, 40f, 0.25f, Controllers.GetDeviceFromInteractionHandType(targetHand));
                    closest.Select();
                    m_lastHovered = closest;
                }
                else
                {
                    closest = null;
                }
            }
        }

        public void Show()
        {
            OrientMenu();
            OrientAllItems();
            m_canvas.enabled = true;
            foreach (RadialItem item in radialItems)
            {
                item.Show();
            }
            m_lastOpenTime = Time.time;
        }

        private void OrientMenu()
        {
            if (originOverride != null)
            {
                transform.position = originOverride.transform.position;
            }
            else
            {
                transform.position = Controllers.GetInteractionHandGO(targetHand).transform.position;
            }

            transform.rotation = Quaternion.LookRotation(transform.position -HMD.GetWorldPosition());
        }

        private void OrientAllItems()
        {
            int itemsAmount = radialItems.Count;
            if (itemsAmount == 0)
            {
                return;
            }
            float currAngle = 0f;
            float angleStep = 360 / itemsAmount;
            foreach (RadialItem item in radialItems)
            {
                Vector3 offset = new Vector3(0, m_itemDistance, 0f) * m_scale;
                offset = Quaternion.Euler(0, 0, -currAngle) * offset;
                item.transform.localPosition = offset;
                currAngle += angleStep;
            }
        }

        public void ToggleAllInfoText(bool toggle)
        {
            foreach(RadialItem item in radialItems)
            {
                item.ToggleInfoText(toggle);
            }
        }

        public void Hide()
        {
            if (!m_canvas.enabled)
            {
                return;
            }
            m_canvas.enabled = false;
            if (closest != null)
            {
                closest.Execute();
                closest = null;
            }
            else
            {
                Log.Debug($"Radial menu closed after {Time.time - m_lastOpenTime} seconds...");
                OnMenuClosedWithoutItem?.Invoke(Time.time - m_lastOpenTime);
            }

            foreach (RadialItem item in radialItems)
            {
                item.Hide();
            }
        }
    }
}