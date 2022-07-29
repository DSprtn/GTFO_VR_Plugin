using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using System;
using TMPro;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.UI
{
    /// <summary>
    /// Responsible for patching all UI elements to 3D and positioning them correctly.
    /// </summary>
    public class VRWorldSpaceUI : MonoBehaviour
    {
        public VRWorldSpaceUI(IntPtr value)
: base(value) { }

        public static InteractionGuiLayer interactGUI;
        public static PlayerGuiLayer playerGUI;

        public static PUI_InteractionPrompt statusBar;
        public static PUI_InteractionPrompt interactionBar;
        public static PUI_Compass compass;
        public static PUI_WardenIntel intel;
        public static PUI_ObjectiveTimer timer;
        public static PUI_Subtitles subtitles;
        public static PUI_CommunicationMenu comms;

        private GameObject m_statusBarHolder;
        private GameObject m_interactionBarHolder;
        private GameObject m_compassHolder;
        private GameObject m_intelHolder;
        private GameObject m_timerHolder;
        private GameObject m_subtitlesHolder;
        private GameObject m_commsHolder;

        // Compass will not be visible after this distance from the center of its rect
        private float m_compassCullDistance = 1.2f;

        private void OnNewPoses()
        {
            UpdateWorldSpaceUI();
        }

        public static void SetInteractionPromptRef(PUI_InteractionPrompt status, PUI_InteractionPrompt interact, InteractionGuiLayer interaction)
        {
            statusBar = status;
            interactionBar = interact;
            interactGUI = interaction;
        }

        public static void SetPlayerGUIRef(PlayerGuiLayer playerGUIRef, PUI_Compass compassRef, PUI_WardenIntel intelRef, PUI_ObjectiveTimer timerRef, PUI_Subtitles subtitlesRef)
        {
            intel = intelRef;
            compass = compassRef;
            playerGUI = playerGUIRef;
            timer = timerRef;
            subtitles = subtitlesRef;
        }

        public static void SetCommsGUIRef(PUI_CommunicationMenu commsRef)
        {
            comms = commsRef;
        }

        private void Start()
        {
            Log.Info("Creating status and interaction prompt VR UI");
            m_statusBarHolder = new GameObject("VR_StatusUI");
            m_interactionBarHolder = new GameObject("VR_InteractionUI");
            m_compassHolder = new GameObject("CompassHolder");
            m_intelHolder = new GameObject("IntelHolder");
            m_timerHolder = new GameObject("TimerHolder");
            m_subtitlesHolder = new GameObject("SubtitlesHolder");
            m_commsHolder = new GameObject("CommsHolder");
            Invoke(nameof(VRWorldSpaceUI.Setup), 1f);
        }

        private void Setup()
        {
            SetupElement(statusBar.transform, m_statusBarHolder.transform, 0.0018f, holderScale:1.6f);
            SetupElement(interactionBar.transform, m_interactionBarHolder.transform, 0.0018f, holderScale:1.1f);
            SetupElement(compass.transform, m_compassHolder.transform, 0.0036f, false, holderScale:1.35f);
            SetupElement(intel.transform, m_intelHolder.transform, 0.0018f, holderScale:1f);
            SetupElement(subtitles.transform, m_subtitlesHolder.transform, 0.0018f, holderScale: 1f);
            SetupElement(comms.transform, m_commsHolder.transform, 0.0018f, holderScale: 0.5f);

            //TODO: timer is destroyed when expedition is changed, so it will vanish and not be repopulated here.
            SetupElement(timer.transform, m_timerHolder.transform, 0.0036f, false, holderScale: 1f);

            SetSpriteRendererShader(compass.transform, VRAssets.SpriteSphereClip);
            setSharedMaterialShader();

            m_intelHolder.SetActive(true);

            interactionBar.transform.FindDeepChild("Timer BG").gameObject.SetActive(false);
            CenterRect(intel.transform);
            CenterRect(compass.transform);

            SteamVR_Events.NewPosesApplied.Listen(OnNewPoses);
            PlayerOrigin.OnOriginShift += SnapUIToPlayerView;
        }

        private void SnapUIToPlayerView()
        {
            if (m_statusBarHolder)
            {
                m_statusBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
            if (m_intelHolder)
            {
                m_intelHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        private void UpdateWorldSpaceUI()
        {
            UpdateInteraction();
            UpdateStatus();
            UpdateCompass();
            UpdateIntel();
            UpdateTimer();
            UpdateSubtitles();
            UpdateComms();
        }

        private void UpdateIntel()
        {
            m_intelHolder.transform.position = GetIntelPosition();

            if (FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                Vector3 flatForward = (m_intelHolder.transform.position - VRPlayer.FpsCamera.transform.position).normalized;
                flatForward.y = 0;
                m_intelHolder.transform.rotation = Quaternion.LookRotation(flatForward.normalized);
            }
            else
            {
                m_intelHolder.transform.rotation = LerpUIRot(m_intelHolder.transform);
            }

            if(intel != null && intel.transform.localScale.x > 0.002) {
                intel.transform.localScale = Vector3.one * 0.0018f;
            }
            
        }

        private void UpdateComms()
        {
            m_commsHolder.transform.position = GetCommsPosition();

            if (FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                Vector3 flatForward = (m_commsHolder.transform.position - VRPlayer.FpsCamera.transform.position).normalized;
                flatForward.y = 0;
                m_commsHolder.transform.rotation = Quaternion.LookRotation(flatForward.normalized);
            }
            else
            {
                m_commsHolder.transform.rotation = LerpUIRot(m_commsHolder.transform);
            }

            if (comms != null && comms.transform.localScale.x > 0.002)
            {
                comms.transform.localScale = Vector3.one * 0.0018f;
            }

        }

        private void UpdateStatus()
        {
            if (m_statusBarHolder == null)
            {
                Log.Error("m_statusBarHolder bar was null!");
                return;
            }
            m_statusBarHolder.SetActive(interactGUI.IsVisible() && interactGUI.MessageVisible);
            if (m_statusBarHolder.activeSelf)
            {
                m_statusBarHolder.transform.position = GetStatusBarPosition();
                m_statusBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        private void UpdateInteraction()
        {
            if(m_interactionBarHolder == null)
            {
                Log.Error("m_interactionBarHolder was null!");
                return;
            }
            m_interactionBarHolder.SetActive(interactGUI.IsVisible() && interactGUI.InteractPromptVisible);
            if (m_interactionBarHolder.activeSelf)
            {
                m_interactionBarHolder.transform.position = GetInteractionPromptPosition();
                m_interactionBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        private void UpdateCompass()
        {
            if (m_compassHolder == null)
            {
                Log.Error("m_compassHolder bar was null!");
                return;
            }
            if(compass != null && compass.transform.localScale.x > 0.0037)
            {
                compass.transform.localScale = Vector3.one * 0.0036f;
            }
            m_compassHolder.SetActive(playerGUI.IsVisible());
            if (m_compassHolder.activeSelf)
            {
                m_compassHolder.transform.position = GetCompassPosition();
                m_compassHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
            UpdateCompassCull();
        }

        private void UpdateCompassCull()
        {
            Vector3 compassPos = m_compassHolder.transform.position;
            Shader.SetGlobalColor("_ClippingSphere", new Color(compassPos.x, compassPos.y, compassPos.z, m_compassCullDistance));
        }

        private void UpdateTimer()
        {
            if (m_timerHolder == null)
            {
                Log.Error("m_timerHolder was null!");
                return;
            }
            
            if (timer != null && timer.transform.localScale.x > 0.0037)
            {
                timer.transform.localScale = Vector3.one * 0.0036f;
            }

            // do not use playerGUI.TimerOnMainScreenVisible, it is not true when it should or is be visible. 
            m_timerHolder.SetActive(playerGUI.IsVisible()); 
            if (m_timerHolder.activeSelf)
            {
                m_timerHolder.transform.position = GetTimerPosition();
                m_timerHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        private void UpdateSubtitles()
        {
            if (m_subtitlesHolder == null)
            {
                Log.Error("m_subtitlesHolder was null!");
                return;
            }

            if (subtitles != null && subtitles.transform.localScale.x > 0.02f)
            {
                subtitles.transform.localScale = Vector3.one * 0.0018f;
            }

            m_subtitlesHolder.SetActive(playerGUI.IsVisible());
            if (m_subtitlesHolder.activeSelf)
            {
                m_subtitlesHolder.transform.position = GetSubtitlesPosition();
                m_subtitlesHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        public static void PrepareNavMarker(NavMarker n)
        {
            n.transform.SetParent(null);
            n.m_initScale *= 0.009f;
            SetTransformHierarchyLayer(n.transform);

            if (n.m_trackingObj)
            {
                n.transform.position = n.m_trackingObj.transform.position;
                n.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        private Vector3 GetIntelPosition()
        {
            if (FocusStateEvents.currentState.Equals(eFocusState.InElevator) && VRPlayer.FpsCamera)
            {
                Vector3 flatForward = VRPlayer.FpsCamera.m_camera.transform.forward;
                flatForward.y = 0f;
                Vector3 pos = VRPlayer.FpsCamera.m_camera.transform.position;
                return pos + flatForward.normalized * 1.2f;
            }
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.75f;
        }

        private Vector3 GetCompassPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.45f + new Vector3(0, 2.15f, 0);
        }

        private Vector3 GetTimerPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.45f + new Vector3(0, 1.50f, 0);
        }

        private Vector3 GetCommsPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.45f + new Vector3(0f, 0.5f, 0);
        }

        private Vector3 GetSubtitlesPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.45f + new Vector3(0, -0.75f, 0);
        }

        private Vector3 GetInteractionPromptPosition()
        {
            if (ShouldUsePointerPosition())
            {
                return VRPlayer.FpsCamera.CameraRayPos + new Vector3(0, 0.05f, 0) - VRPlayer.FpsCamera.transform.forward * .1f;
            }

            return VRPlayer.PlayerAgent.PlayerCharacterController.SmoothPosition + HMD.GetFlatForwardDirection() * .7f + new Vector3(0, 1f, 0);
        }

        private Vector3 GetStatusBarPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.75f + new Vector3(0, 1.5f, 0);
        }

        private Quaternion LerpUIRot(Transform t)
        {
            return Quaternion.Lerp(t.rotation, Quaternion.LookRotation(HMD.GetFlatForwardDirection()), Time.deltaTime * 5f);
        }

        private static bool ShouldUsePointerPosition()
        {
            return VRPlayer.FpsCamera.CameraRayObject && Vector3.Distance(HMD.GetVRInteractionFromPosition(), VRPlayer.FpsCamera.CameraRayPos) < 1.2f;
        }

        private static void CenterRect(Transform t)
        {
            RectTransformComp rect = t.GetComponent<RectTransformComp>();
            rect.SetAnchor(GuiAnchor.MidCenter);
            rect.transform.localPosition = Vector3.zero;
        }

        private static void SetupElement(Transform ui, Transform holder, float scale, bool replaceShaders = true, float holderScale = 1f)
        {
            holder.transform.localScale *= holderScale;
            ui.SetParent(holder);
            SetTransformHierarchyLayer(ui);
            if (replaceShaders)
            {
                SetSpriteRendererShader(ui);
            }

            ui.transform.localScale = Vector3.one * scale;
            ui.transform.localPosition = Vector3.zero;
            ui.transform.localRotation = Quaternion.identity;
        }

        private static void setSharedMaterialShader()
        {
            if ( comms != null )
            {
                // A single item is always populated during PUI_CommunicationMenu.Setup()
                PUI_CommunicationButton btn = comms.m_buttons[0];

                // This material is shared by most of the UI elements
                btn.m_lineText.fontSharedMaterial.shader = VRAssets.GetTextNoCull();
                btn.m_numberText.fontSharedMaterial.shader = VRAssets.GetTextNoCull();
                btn.m_lineText.fontSharedMaterial.renderQueue = 4001;
                btn.m_numberText.fontSharedMaterial.renderQueue = 4001;
            }

            if (compass != null)
            {
                // Compass has its own font material, but is shared between all its components.
                compass.m_fontMaterial.shader = VRAssets.TextSphereClip;
                compass.m_fontMaterial.renderQueue = 4001;
            }
        }

        private static void SetSpriteRendererShader(Transform ui)
        {
            foreach (SpriteRenderer s in ui.GetComponentsInChildren<SpriteRenderer>(true))
            {
                s.material.shader = VRAssets.SpriteAlwaysRender;
            }
        }

        private static void SetSpriteRendererShader(Transform ui, Shader shader)
        {
            foreach (SpriteRenderer s in ui.GetComponentsInChildren<SpriteRenderer>(true))
            {
                s.material.shader = shader;
            }
        }

        private static void SetTransformHierarchyLayer(Transform transform)
        {
            foreach (RectTransform t in transform.GetComponentsInChildren<RectTransform>(true))
            {
                t.gameObject.layer = LayerManager.LAYER_FIRST_PERSON_ITEM;
            }

            foreach (Transform t in transform.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = LayerManager.LAYER_FIRST_PERSON_ITEM;
            }
        }

        private void OnDestroy()
        {
            if (m_statusBarHolder != null)
            {
                m_statusBarHolder.transform.DetachChildren();
                Destroy(m_statusBarHolder);
            }
            if (m_interactionBarHolder != null)
            {
                m_interactionBarHolder.transform.DetachChildren();
                Destroy(m_interactionBarHolder);
            }
            if (m_compassHolder != null)
            {
                m_compassHolder.transform.DetachChildren();
                Destroy(m_compassHolder);
            }
            if (m_intelHolder != null)
            {
                m_intelHolder.transform.DetachChildren();
                Destroy(m_intelHolder);
            }
            if (m_timerHolder != null)
            {
                m_timerHolder.transform.DetachChildren();
                Destroy(m_timerHolder);
            }
            if (m_subtitlesHolder != null)
            {
                m_subtitlesHolder.transform.DetachChildren();
                Destroy(m_subtitlesHolder);
            }
            if (m_commsHolder != null)
            {
                m_commsHolder.transform.DetachChildren();
                Destroy(m_commsHolder);
            }
            SteamVR_Events.NewPosesApplied.Remove(OnNewPoses);
            PlayerOrigin.OnOriginShift -= SnapUIToPlayerView;
        }
    }
}