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

        private GameObject m_thermalDot;
        private GameObject m_thermalPointer;

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
            Color c = ExtensionMethods.FromString(VRConfig.configLaserPointerColor.Value);
            c *= .28f;
            m_pointer.GetComponent<MeshRenderer>().material.color = c;
            m_dot.GetComponent<MeshRenderer>().material.color = c;
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
                // Offset the hit location so dot doesn't get buried in geometry. Colliders are not always very accurate.
                Vector3 offsetHit = (transform.parent.position - hit.point).normalized * 0.1f;  // Offset vector
                dist = hit.distance - offsetHit.magnitude;

                if (!m_dot.active && VRConfig.configUseLaserPointerOnWeapons.Value)
                    m_dot.SetActive(true);

                m_dot.transform.rotation = Quaternion.LookRotation(m_pointer.transform.up);
                m_dot.transform.position = hit.point + offsetHit;
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

            if (item.HasFlashlight && item.AmmoType != Player.AmmoType.None)
            {
                SetHolderTransform(item.MuzzleAlign);
                TogglePointer(VRConfig.configUseLaserPointerOnWeapons.Value);
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
            m_pointer.transform.localRotation = Quaternion.identity;

            m_dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            m_dot.transform.localRotation = Quaternion.identity;
            m_dot.transform.localScale = m_dotScale;
        }

        private GameObject CreateDot(Transform parent, bool applyTransforms)
        {
            GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dot.GetComponent<Collider>().enabled = false;
            dot.transform.SetParent(parent);
            if (applyTransforms)
            {
                dot.transform.localScale = m_dotScale;
                dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);

            }
            else
            {
                // Parent will be transformed instead
                dot.transform.localScale = new Vector3(1,1,1);
                dot.transform.localPosition = new Vector3(0.0f, 0.0f, 0);
            }

            dot.transform.localRotation = Quaternion.identity;

            return dot;
        }

        private GameObject CreatePointer( Transform parent, bool applyTransforms)
        {
            GameObject pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.GetComponent<Collider>().enabled = false;
            pointer.transform.parent = parent;
            if (applyTransforms)
            {
                pointer.transform.localScale = new Vector3(m_thickness, m_thickness, 100f);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            }
            else
            {
                // Parent will be transformed instead
                pointer.transform.localScale = new Vector3(1, 1, 1);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 0);
            }

            pointer.transform.localRotation = Quaternion.identity;

            return pointer;
        }

        private void CreatePointerObjects()
        {
            ///////////////////////
            //// Visible light
            ///////////////////////

            m_pointer = CreatePointer(this.transform, true);
            m_dot = CreateDot(this.transform, true);

            Material pointerMaterial = new Material(Shader.Find("Unlit/Color")); // Renders after deferred pass
            Color c = ExtensionMethods.FromString(VRConfig.configLaserPointerColor.Value);
            Color origColor = c;
            c *= .28f;
            pointerMaterial.SetColor("_Color", c);
            pointerMaterial.renderQueue = 3001; // Renders after thermals

            m_pointer.GetComponent<MeshRenderer>().material = pointerMaterial;
            m_dot.GetComponent<MeshRenderer>().material = pointerMaterial;

            /////////////////////////
            /// Thermal
            /// /////////////////////

            m_thermalPointer = CreatePointer(m_pointer.transform, false);
            m_thermalDot = CreateDot(m_dot.transform, false);

            Material thermalMaterial = new Material(VRAssets.ThermalGlowShader); // Normal facing camera, writes _ShadingType to emission texture

            thermalMaterial.SetColor("_EmissionColor", origColor * 0.25f);  // Alpha replaced by shader. Too bright and it does funny things to glass.
            thermalMaterial.SetColor("_Bump", new Color(1, 1, 1, 1f));      // Makes thermal color brighter?
            thermalMaterial.SetColor("_Color", c);          // Not unlit, so emission is what makes it constant.
            thermalMaterial.SetFloat("_ShadingType", 2);    // Skin, brightest of the lot
            thermalMaterial.renderQueue = 2500;             // Deferred shaders won't render above this

            m_thermalPointer.GetComponent<MeshRenderer>().material = thermalMaterial;
            m_thermalDot.GetComponent<MeshRenderer>().material = thermalMaterial;

            // Makes things even brighter?
            m_thermalPointer.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            m_thermalDot.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

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