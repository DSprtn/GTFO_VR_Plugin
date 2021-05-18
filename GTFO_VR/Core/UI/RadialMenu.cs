using GTFO_VR.Core.VR_Input;
using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.UI
{
    public class RadialMenu : MonoBehaviour
    {

        public RadialMenu(IntPtr value)
: base(value) { }

        private Canvas m_canvas;

        private InteractionHand targetHand;
        private GameObject originOverride;

        private List<RadialItem> radialItems;
        private RadialItem closest;

        private float maxDistance = 0.11f;
        private float itemDistance = 120f;


        public void Setup(InteractionHand hand, GameObject originOverride = null)
        {
            this.originOverride = originOverride;
            targetHand = hand;
            m_canvas = gameObject.AddComponent<Canvas>();
            m_canvas.renderMode = RenderMode.WorldSpace;
            RectTransform canvasTransform = m_canvas.GetComponent<RectTransform>();
            canvasTransform.sizeDelta = new Vector2(80f, 80f);
            MelonCoroutines.Start(SetSize(canvasTransform, new Vector2(80, 80)));
            canvasTransform.localScale = Vector3.one * .001f;
            radialItems = new List<RadialItem>();
        }

        IEnumerator SetSize(RectTransform rect, Vector2 size)
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
            }
        }

        public void AddRadialItem(string text, Action OnExecuted, out RadialItem newItem)
        {
            RadialItem item = new GameObject("RadialItem").AddComponent<RadialItem>();
            item.transform.SetParent(transform);
            item.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            item.transform.localScale = Vector3.one;
            item.Setup(OnExecuted, VRAssets.RadialBG);
            item.SetText(text);
            newItem = item;
            radialItems.Add(item);
        }

        GameObject GetOrigin()
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
            float closestDistance = 9999f;

            GameObject hand = GetOrigin();

            if (closest == null || Vector3.Distance(closest.transform.position, hand.transform.position) > maxDistance)
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

            if (closest == null)
            {
                foreach (RadialItem item in radialItems)
                {
                    if(!item.Active)
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

                if (closest && closestDistance < maxDistance)
                {
                    CellSound.Post(AK.EVENTS.GAME_MENU_SELECT_GEAR, closest.transform.position);
                    SteamVR_InputHandler.TriggerHapticPulse(0.02f, 40f, 0.25f, Controllers.GetDeviceFromInteractionHandType(targetHand));
                    closest.Select();
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
                if(item.Active)
                {
                    item.Show();
                } else
                {
                    item.Hide();
                }
            }
        }

        private void OrientMenu()
        {
            if(originOverride != null)
            {
                transform.position = originOverride.transform.position;
            } else
            {
                transform.position = Controllers.GetInteractionHandGO(targetHand).transform.position;
            }

            transform.rotation = Quaternion.LookRotation(HMD.Hmd.transform.forward, Vector3.up);
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
                Vector3 offset = new Vector3(0, itemDistance, 0f);
                offset = Quaternion.Euler(0, 0, -currAngle) * offset;
                item.transform.localPosition = offset;
                currAngle += angleStep;
            }
        }

        public void Hide()
        {
            m_canvas.enabled = false;
            if (closest != null)
            {
                closest.Execute();
                closest = null;
            }

            foreach (RadialItem item in radialItems)
            {
                item.Hide();
            }
        }
    }
}