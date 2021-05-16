using GTFO_VR.Events;
using GTFO_VR.Util;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Responsible for managing the fancy weapon laserpointer.
    /// </summary>
    public class LaserPointer : MonoBehaviour
    {
        public LaserPointer(IntPtr value) : base(value)
        {
        }

        SteamVR_Action_Boolean toggleLaserPointer;

        private GameObject m_pointer;
        private GameObject m_dot;
        private readonly float m_thickness = 1f / 400f;

        private Vector3 m_dotScale = new Vector3(0.04f, 0.01f, 0.016f);

        private bool m_setup = false;

        private void Awake()
        {
            toggleLaserPointer = SteamVR_Input.GetBooleanAction("ToggleLaserPointer");

            CreatePointerObjects();
            ItemEquippableEvents.OnPlayerWieldItem += PlayerChangedItem;
            VRConfig.configUseLaserPointerOnWeapons.SettingChanged += LaserPointerToggled;
            VRConfig.configLaserPointerColor.SettingChanged += LaserPointerColorChanged;
        }

        void Update()
        {
            if (toggleLaserPointer.GetStateDown(SteamVR_Input_Sources.Any))
            {
                VRConfig.configUseLaserPointerOnWeapons.Value = !VRConfig.configUseLaserPointerOnWeapons.Value;
            }
        }

        private void LaserPointerColorChanged(object sender, EventArgs e)
        {
            if(!m_setup)
            {
                return;
            }
            m_pointer.GetComponent<MeshRenderer>().material.color = ExtensionMethods.FromString(VRConfig.configLaserPointerColor.Value);
            m_dot.GetComponent<MeshRenderer>().material.color = ExtensionMethods.FromString(VRConfig.configLaserPointerColor.Value);
        }

        private void LaserPointerToggled(object sender, EventArgs e)
        {
            PlayerChangedItem(ItemEquippableEvents.currentItem);
        }

        private void FixedUpdate()
        {
            if (transform.parent == null)
            {
                return;
            }
            float dist = 50f;

            Ray raycast = new Ray(transform.parent.position, transform.parent.forward);
            bool bHit = Physics.Raycast(raycast, out RaycastHit hit, 51f, LayerManager.MASK_BULLETWEAPON_RAY, QueryTriggerInteraction.Ignore);

            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
                m_dot.transform.rotation = Quaternion.LookRotation(m_pointer.transform.up);
                m_dot.transform.position = hit.point;
                m_dot.transform.localScale = Vector3.Lerp(m_dotScale, m_dotScale * 3f, dist / 51f);
            }
            else
            {
                m_dot.SetActive(false);
            }

            m_pointer.transform.localScale = new Vector3(m_thickness, m_thickness, dist);
            m_pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
        }

        public void PlayerChangedItem(ItemEquippable item)
        {
            if (!m_setup || item == null)
            {
                return;
            }
            if (item.HasFlashlight && item.AmmoType != Player.AmmoType.None && VRConfig.configUseLaserPointerOnWeapons.Value)
            {
                TogglePointer(true);
                SetHolderTransform(item.MuzzleAlign);
            }
            else
            {
                TogglePointer(false);
            }
        }

        private void TogglePointer(bool toggle)
        {
            m_pointer.SetActive(toggle);
            m_dot.SetActive(toggle);
        }

        private void SetHolderTransform(Transform t)
        {
            transform.SetParent(t);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            m_pointer.transform.localScale = new Vector3(m_thickness, m_thickness, 100f);
            m_pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            m_dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            m_pointer.transform.localRotation = Quaternion.identity;
            m_dot.transform.localRotation = Quaternion.identity;
            m_dot.transform.localScale = m_dotScale;
        }

        private void CreatePointerObjects()
        {
            m_pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_dot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            m_dot.transform.localScale = m_dotScale;
            m_dot.transform.SetParent(transform);
            m_dot.GetComponent<Collider>().enabled = false;
            m_pointer.GetComponent<Collider>().enabled = false;

            m_pointer.transform.parent = transform;
            m_pointer.transform.localScale = new Vector3(m_thickness, m_thickness, 100f);
            m_pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            m_dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            m_pointer.transform.localRotation = Quaternion.identity;
            m_dot.transform.localRotation = Quaternion.identity;
            Material material = new Material(Shader.Find("Unlit/Color"));
            material.SetColor("_Color", ExtensionMethods.FromString(VRConfig.configLaserPointerColor.Value));
            m_pointer.GetComponent<MeshRenderer>().material = material;
            m_dot.GetComponent<MeshRenderer>().material = material;

            
            m_setup = true;

            TogglePointer(false);
        }

        private void OnDestroy()
        {
            VRConfig.configUseLaserPointerOnWeapons.SettingChanged -= LaserPointerToggled;
            VRConfig.configLaserPointerColor.SettingChanged -= LaserPointerColorChanged;
            ItemEquippableEvents.OnPlayerWieldItem -= PlayerChangedItem;
        }
    }
}