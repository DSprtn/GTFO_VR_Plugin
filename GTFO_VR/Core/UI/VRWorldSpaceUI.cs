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
        private enum WorldUIElementType
        {
            Default, Interact, Status, Intel, Compass
        }
        
        private class WorldUIElement
        {
            public RectTransformComp Element;
            public GameObject Holder;
            public float Scale = 1;
            public float MaxScale = 1;
            public float HolderScale = 1;
            public bool ReplaceShaders = false;
            public bool SmoothRotate = false;
            public WorldUIElementType Type;
            public Func<Vector3> GetPosition;

            public WorldUIElement(
                RectTransformComp element,
                GameObject holder,
                float scale,
                float maxScale,
                float holderScale,
                Func<Vector3> getPosition,
                WorldUIElementType type = WorldUIElementType.Default,
                bool replaceShaders = true,
                bool smoothRotate = false)
            {
                Element = element;
                Holder = holder;
                Scale = scale;
                MaxScale = maxScale;
                HolderScale = holderScale;
                Type = type;
                ReplaceShaders = replaceShaders;
                SmoothRotate = smoothRotate;
                GetPosition = getPosition;
            }
        }


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

        private WorldUIElement m_statusBarElement;
        private WorldUIElement m_interactionBarElement;
        private WorldUIElement m_compassElement;
        private WorldUIElement m_intelElement;
        private WorldUIElement m_timerElement;
        private WorldUIElement m_subtitlesElement;
        private WorldUIElement m_commsElement;

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
            m_statusBarElement =        new WorldUIElement(statusBar,       m_statusBarHolder,      0.0018f,    0.002f,     1.6f,   GetStatusBarPosition, type: WorldUIElementType.Status);
            m_interactionBarElement =   new WorldUIElement(interactionBar,  m_interactionBarHolder, 0.0018f,    0.002f,     1.1f,   GetInteractionPromptPosition, type: WorldUIElementType.Interact);
            m_compassElement =          new WorldUIElement(compass,         m_compassHolder,        0.0036f,    0.0037f,    1.35f,  GetCompassPosition, type: WorldUIElementType.Compass, replaceShaders: false);
            m_intelElement =            new WorldUIElement(intel,           m_intelHolder,          0.0018f,    0.002f,     1f,     GetIntelPosition, type: WorldUIElementType.Intel, smoothRotate: true);
            m_subtitlesElement =        new WorldUIElement(subtitles,       m_subtitlesHolder,      0.0018f,    0.002f,     1f,     GetSubtitlesPosition);
            m_commsElement =            new WorldUIElement(comms,           m_commsHolder,          0.0018f,    0.002f,     0.5f,   GetCommsPosition, smoothRotate: true);
            m_timerElement =            new WorldUIElement(timer,           m_timerHolder,          0.0036f,    0.0037f,    1f,     GetTimerPosition);

            SetupElement(m_statusBarElement);
            SetupElement(m_interactionBarElement);
            SetupElement(m_compassElement);
            SetupElement(m_intelElement);
            SetupElement(m_timerElement);
            SetupElement(m_subtitlesElement);
            SetupElement(m_commsElement);

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
            UpdateUIElement(m_statusBarElement);
            UpdateUIElement(m_interactionBarElement);
            UpdateUIElement(m_compassElement);
            UpdateCompassCull();
            UpdateUIElement(m_intelElement);
            UpdateUIElement(m_timerElement);
            UpdateUIElement(m_subtitlesElement);
            UpdateUIElement(m_commsElement);
        }

        private void UpdateUIElement( WorldUIElement ui )
        {
            if (ui.Holder == null)
            {
                Log.Error("World UI holder was null!");
                return;
            }

            switch (ui.Type)
            {
                case WorldUIElementType.Compass:
                    ui.Holder.SetActive(playerGUI.IsVisible());
                    break;
                case WorldUIElementType.Interact:
                    ui.Holder.SetActive(interactGUI.IsVisible() && interactGUI.InteractPromptVisible);
                    break;
                case WorldUIElementType.Status:
                    ui.Holder.SetActive(interactGUI.IsVisible() && interactGUI.MessageVisible);
                    break;
                default:
                    break;
            }

            if (!ui.Holder.active)
                return;

            ui.Holder.transform.position = ui.GetPosition();

            if ( ui.Type == WorldUIElementType.Intel && FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                Vector3 flatForward = (m_intelHolder.transform.position - VRPlayer.FpsCamera.transform.position).normalized;
                flatForward.y = 0;
                ui.Holder.transform.rotation = Quaternion.LookRotation(flatForward.normalized);
            }
            else if (ui.SmoothRotate)
            {
                ui.Holder.transform.rotation = LerpUIRot(ui.Holder.transform);
            }
            else
            {
                ui.Holder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }

            if (ui.Element != null && ui.Element.transform.localScale.x > ui.MaxScale)
            {
                ui.Element.transform.localScale = Vector3.one * ui.Scale;
            }
        }

        private void UpdateCompassCull()
        {
            Vector3 compassPos = m_compassHolder.transform.position;
            Shader.SetGlobalColor("_ClippingSphere", new Color(compassPos.x, compassPos.y, compassPos.z, m_compassCullDistance));
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

        private static void SetupElement( WorldUIElement ui )
        {
            ui.Holder.transform.localScale *= ui.HolderScale;
            ui.Element.transform.SetParent(ui.Holder.transform);
            SetTransformHierarchyLayer(ui.Element.transform);
            if (ui.ReplaceShaders)
            {
                SetSpriteRendererShader(ui.Element.transform);
            }

            ui.Element.transform.localScale = Vector3.one * ui.Scale;
            ui.Element.transform.localPosition = Vector3.zero;
            ui.Element.transform.localRotation = Quaternion.identity;
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