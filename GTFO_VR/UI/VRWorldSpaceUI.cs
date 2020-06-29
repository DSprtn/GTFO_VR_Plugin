using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.UI
{
    public class VRWorldSpaceUI : MonoBehaviour
    {
        public static PUI_InteractionPrompt statusBar;
        public static PUI_InteractionPrompt interactionBar;
        public static PUI_Compass compass;
        public static PUI_WardenIntel intel;

        GameObject statusBarHolder;
        GameObject interactionBarHolder;
        GameObject compassHolder;
        GameObject intelHolder;

        public static InteractionGuiLayer interactGUI;
        public static PlayerGuiLayer playerGUI;

        // Compass will not be visible after this distance from the center of its rect
        float compassCullDistance = 1.2f;

        void Awake()
        {
            SteamVR_Events.NewPosesApplied.AddListener(() => OnNewPoses());
            Snapturn.OnAfterSnapTurn += PlayerUsedSnapturn;
        }

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

        public static void SetPlayerGUIRef(PlayerGuiLayer playerGUIRef, PUI_Compass compassRef, PUI_WardenIntel intelRef)
        {
            intel = intelRef;
            compass = compassRef;
            playerGUI = playerGUIRef;
        }

        void Start()
        {

            Debug.Log("Creating status and interaction prompt VR UI");
            statusBarHolder = new GameObject("VR_StatusUI");
            interactionBarHolder = new GameObject("VR_InteractionUI");
            compassHolder = new GameObject("CompassHolder");
            intelHolder = new GameObject("IntelHolder");
            
            Invoke(nameof(VRWorldSpaceUI.Setup), 1f);

        }

        void Setup()
        {
            SetupElement(statusBar.transform, statusBarHolder.transform, 0.0018f);
            SetupElement(interactionBar.transform, interactionBarHolder.transform, 0.0018f);
            SetupElement(compass.transform, compassHolder.transform, 0.0036f, false);
            SetupElement(intel.transform, intelHolder.transform, 0.0018f);

            SetTextShader(compass.transform, VR_Assets.textSphereClip);
            SetSpriteRendererShader(compass.transform, VR_Assets.spriteSphereClip);

            intelHolder.SetActive(true);

            interactionBar.transform.FindChildRecursive("Timer BG").gameObject.SetActive(false);
            CenterRect(intel.transform);
            CenterRect(compass.transform);
        }
        

        static void CenterRect(Transform t)
        {
            RectTransformComp rect = t.GetComponent<RectTransformComp>();
            rect.SetAnchor(GuiAnchor.MidCenter);
            rect.transform.localPosition = Vector3.zero;
        }

        static void SetupElement(Transform ui, Transform holder, float scale, bool replaceShaders = true)
        {
            ui.SetParent(holder);
            SetTransformHierarchyLayer(ui);
            if(replaceShaders)
            {
                SetSpriteRendererShader(ui);
                SetTextShader(ui);
            }

            ui.transform.localScale = Vector3.one * scale;
            ui.transform.localPosition = Vector3.zero;
            ui.transform.localRotation = Quaternion.identity;
        }

        static void SetTextShader(Transform ui)
        {
            foreach (TextMeshPro p in ui.GetComponentsInChildren<TextMeshPro>(true))
            {
                p.GetComponent<MeshRenderer>().material.shader = VR_Assets.textAlwaysRender;
            }
        }

        static void SetSpriteRendererShader(Transform ui)
        {
            foreach (SpriteRenderer s in ui.GetComponentsInChildren<SpriteRenderer>(true))
            {
                s.material.shader = VR_Assets.spriteAlwaysRender;
            }
        }

        static void SetSpriteRendererShader(Transform ui, Shader shader)
        {
            foreach (SpriteRenderer s in ui.GetComponentsInChildren<SpriteRenderer>(true))
            {
                s.material.shader = shader;
            }
        }

        static void SetTextShader(Transform ui, Shader shader)
        {
            foreach (TextMeshPro p in ui.GetComponentsInChildren<TextMeshPro>(true))
            {
                p.GetComponent<MeshRenderer>().material.shader = shader;
            }
        }

        static void SetTransformHierarchyLayer(Transform transform)
        {
            foreach (RectTransform t in transform.GetComponentsInChildren<RectTransform>(true))
            {
                t.gameObject.layer = LayerManager.LAYER_THIRD_PERSON_ITEM;
            }

            foreach(Transform t in transform.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = LayerManager.LAYER_THIRD_PERSON_ITEM;
            }
        }

        void PlayerUsedSnapturn()
        {
            if(statusBarHolder)
            {
                statusBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
            if(intelHolder)
            {
                intelHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        void UpdateWorldSpaceUI()
        {
            UpdateInteraction();
            UpdateStatus();
            UpdateCompass();
            UpdateIntel();
        }

        void UpdateIntel()
        {
            intelHolder.transform.position = GetIntelPosition();
            
            if(FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                Vector3 flatForward = (intelHolder.transform.position - PlayerVR.fpsCamera.transform.position).normalized;
                flatForward.y = 0;
                intelHolder.transform.rotation = Quaternion.LookRotation(flatForward.normalized);
            } else
            {
                intelHolder.transform.rotation = LerpUIRot(intelHolder.transform);
            }
            
        }

        void UpdateStatus()
        {
            statusBarHolder.SetActive(interactGUI.IsVisible() && interactGUI.MessageVisible);
            if (statusBarHolder.activeSelf)
            {
                statusBarHolder.transform.position = GetStatusBarPosition();
                statusBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        void UpdateInteraction()
        {
            interactionBarHolder.SetActive(interactGUI.IsVisible() && interactGUI.InteractPromptVisible);
            if (interactionBarHolder.activeSelf)
            {
                interactionBarHolder.transform.position = GetInteractionPromptPosition();
                interactionBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection()); 
            }
        }

        void UpdateCompass()
        {
            compassHolder.SetActive(playerGUI.IsVisible());
            if(compassHolder.activeSelf)
            {
                compassHolder.transform.position = GetCompassPosition();
                compassHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
            UpdateCompassCull();
        }

        void UpdateCompassCull()
        {
            Vector3 compassPos = compassHolder.transform.position;
            Shader.SetGlobalColor("_ClippingSphere", new Color(compassPos.x, compassPos.y, compassPos.z, compassCullDistance));
        }


        Vector3 GetIntelPosition()
        {
            if(FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                Vector3 flatForward = PlayerVR.fpsCamera.m_camera.transform.forward;
                flatForward.y = 0f;
                Vector3 pos = PlayerVR.fpsCamera.m_camera.transform.position;
                return pos + flatForward.normalized * 1.2f;
            }
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.75f;
        }
        Vector3 GetCompassPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.45f + new Vector3(0, 2.15f, 0);
        }
        
        Vector3 GetInteractionPromptPosition()
        {
            if (ShouldUsePointerPosition())
            {
                return PlayerVR.fpsCamera.CameraRayPos + new Vector3(0, 0.05f, 0) - PlayerVR.fpsCamera.transform.forward * .1f;
            }
            return PlayerOrigin.GetUnadjustedPosition() + HMD.GetFlatForwardDirection() * .7f + new Vector3(0, .75f, 0);
        }

        Vector3 GetStatusBarPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.75f + new Vector3(0, 1.5f, 0);
        }

        Quaternion LerpUIRot(Transform t)
        {
            return Quaternion.Lerp(t.rotation, Quaternion.LookRotation(HMD.GetFlatForwardDirection()), Time.deltaTime * 5f);
        }

        static bool ShouldUsePointerPosition()
        {
            return PlayerVR.fpsCamera.CameraRayObject && Vector3.Distance(HMD.GetVRInteractionFromPosition(), PlayerVR.fpsCamera.CameraRayPos) < 1f;
        }

        public static void PrepareNavMarker(NavMarker n)
        {
            n.transform.SetParent(null);

            n.m_initScale *= 0.009f;
            SetTransformHierarchyLayer(n.transform);
            SetTextShader(n.transform);
            SetSpriteRendererShader(n.transform);

            if (n.m_trackingObj)
            {
                n.transform.position = n.m_trackingObj.transform.position;
                n.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
        }

        public static void UpdateAllNavMarkers(List<NavMarker> markers)
        {
            
            float tempScale = 1f;
            bool inElevator = FocusStateManager.CurrentState.Equals(eFocusState.InElevator);

            foreach (NavMarker n in markers)
            {
                if(inElevator && n)
                {
                    n.transform.localScale = Vector3.zero;
                    return;
                }

                if (n != null && n.m_trackingObj != null)
                {
                    Quaternion rotToCamera = Quaternion.LookRotation((n.m_trackingObj.transform.position - HMD.GetWorldPosition()).normalized);
                    n.transform.position = n.m_trackingObj.transform.position;
                    n.transform.rotation = rotToCamera;

                    float dotToCamera = Vector3.Dot((n.m_trackingObj.transform.position - HMD.GetWorldPosition()).normalized, HMD.GetWorldForward());


                    if (dotToCamera < 0)
                    {
                        n.SetState(NavMarkerState.Inactive);
                    }
                    else
                    {
                        float distanceToCamera = Vector3.Distance(n.m_trackingObj.transform.position, HMD.GetWorldPosition());

                        if (dotToCamera > 0.94f)
                        {
                            if (n.m_currentState != NavMarkerState.InFocus)
                            {

                                n.SetState(NavMarkerState.InFocus);
                            }
                        }
                        else if (n.m_currentState != NavMarkerState.Visible)
                        {

                            n.SetState(NavMarkerState.Visible);
                        }
                        n.SetDistance(distanceToCamera);

                        tempScale = 1 + Mathf.Clamp(distanceToCamera / 25f, 0, 5);
                       
                        n.transform.localScale = n.m_initScale * tempScale;
                       
                    }
                }
            }
        }

        void OnDestroy()
        {
            SteamVR_Events.NewPosesApplied.RemoveListener(() => OnNewPoses());
            Snapturn.OnAfterSnapTurn -= PlayerUsedSnapturn;
        }
    }
}


