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

        float compassCullDistance = 1.05f;

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
            SetupElement(interactionBar.transform, interactionBarHolder.transform, 0.0012f);
            SetupElement(compass.transform, compassHolder.transform, 0.0036f, false);
            SetupElement(intel.transform, intelHolder.transform, 0.0012f);

            SetTextShader(compass.transform, VR_Assets.textCull);
            SetSpriteRendererShader(compass.transform, VR_Assets.spriteClip);

            intelHolder.SetActive(true);

            CenterRect(intel.transform);
            CenterRect(compass.transform);
            
        }

        private static void CenterRect(Transform t)
        {
            RectTransformComp rect = t.GetComponent<RectTransformComp>();
            rect.SetAnchor(GuiAnchor.MidCenter);
            rect.transform.localPosition = Vector3.zero;
        }

        void SetupElement(Transform ui, Transform holder, float scale, bool replaceShaders = true)
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

        private void SetTextShader(Transform ui)
        {
            foreach (TextMeshPro p in ui.GetComponentsInChildren<TextMeshPro>())
            {
                Debug.Log("changing mat on " + p);
                if (p.canvas)
                {
                    p.canvas.renderMode = RenderMode.WorldSpace;
                    p.canvas.worldCamera = UI_Core.UIPassHUD.Camera;

                }
                p.GetComponent<MeshRenderer>().material.shader = VR_Assets.textAlwaysRender;
                p.GetComponent<MeshRenderer>().material.renderQueue = -1;
            }
        }

        private void SetSpriteRendererShader(Transform ui)
        {
            foreach (SpriteRenderer s in ui.GetComponentsInChildren<SpriteRenderer>())
            {
                Debug.Log("changing mat on " + s);
                s.material.shader = VR_Assets.spriteAlwaysRender;
                s.material.renderQueue = -1;
            }
        }

        private void SetSpriteRendererShader(Transform ui, Shader shader)
        {
            foreach (SpriteRenderer s in ui.GetComponentsInChildren<SpriteRenderer>())
            {
                Debug.Log("changing mat on " + s);
                s.material.shader = shader;
                s.material.renderQueue = -1;
            }
        }

        private void SetTextShader(Transform ui, Shader shader)
        {
            foreach (TextMeshPro p in ui.GetComponentsInChildren<TextMeshPro>())
            {
                Debug.Log("changing mat on " + p);
                if (p.canvas)
                {
                    p.canvas.renderMode = RenderMode.WorldSpace;
                    p.canvas.worldCamera = UI_Core.UIPassHUD.Camera;

                }
                p.GetComponent<MeshRenderer>().material.shader = shader;
                p.GetComponent<MeshRenderer>().material.renderQueue = -1;
            }
        }

        

        void SetTransformHierarchyLayer(Transform transform)
        {
            foreach (RectTransform t in transform.GetComponentsInChildren<RectTransform>())
            {
                t.gameObject.layer = LayerManager.LAYER_FIRST_PERSON_ITEM;

            }
        }

        void Update()
        {
            DebugKeys();
        }

        

        private void DebugKeys()
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.F5))
            {
                DebugHelper.LogTransformHierarchy(statusBar.transform);
                DebugHelper.LogTransformHierarchy(interactionBar.transform);
                DebugHelper.LogTransformHierarchy(intel.transform);
                DebugHelper.LogTransformHierarchy(compass.transform);
            }
            if(UnityEngine.Input.GetKeyDown(KeyCode.F9))
            {
                compassCullDistance += .1f;
                Debug.Log(compassCullDistance);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
            {
                compassCullDistance -= .1f;
                Debug.Log(compassCullDistance);
            }
        }

        private void PlayerUsedSnapturn()
        {
            if(statusBarHolder)
            {
                statusBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
            if(intelHolder)
            {
                intelHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
            }
            if (interactionBarHolder) {
                interactionBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
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
                Vector3 flatForward = PlayerVR.fpsCamera.m_camera.transform.forward;
                flatForward.y = 0;
                intelHolder.transform.rotation = Quaternion.LookRotation(flatForward);
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
                statusBarHolder.transform.rotation = LerpUIRot(statusBarHolder.transform);
            }
        }

        Quaternion LerpUIRot(Transform t)
        {
            return Quaternion.Lerp(t.rotation, Quaternion.LookRotation(HMD.GetFlatForwardDirection()), Time.deltaTime * 5f);
        }

        void UpdateInteraction()
        {
            interactionBarHolder.SetActive(interactGUI.IsVisible() && interactGUI.InteractPromptVisible);
            if (interactionBarHolder.activeSelf)
            {
                interactionBarHolder.transform.position = GetInteractionPromptPosition();
                if(ShouldUsePointerPosition())
                {
                    interactionBarHolder.transform.rotation = Quaternion.LookRotation(HMD.GetFlatForwardDirection());
                } else
                {
                    interactionBarHolder.transform.rotation = LerpUIRot(interactionBarHolder.transform);
                }
                
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

        private void UpdateCompassCull()
        {
            Vector3 compassPos = compassHolder.transform.position;
            Shader.SetGlobalColor("_ClippingSphere", new Color(compassPos.x, compassPos.y, compassPos.z, compassCullDistance));
        }


        Vector3 GetIntelPosition()
        {
            if(FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                Vector3 flatForward = PlayerVR.fpsCamera.m_camera.transform.forward;
                flatForward.y = 0;
                Vector3 pos = PlayerVR.fpsCamera.HolderPosition;
                pos.y = PlayerVR.fpsCamera.HolderPosition.y;
                pos -= new Vector3(0, 0.25f, 0);
                return pos + flatForward;
            }
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.75f;
        }
        Vector3 GetCompassPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.45f + new Vector3(0, 2.2f, 0);
        }
        
        Vector3 GetInteractionPromptPosition()
        {
            if (ShouldUsePointerPosition())
            {
                return PlayerVR.fpsCamera.CameraRayPos + new Vector3(0, 0.05f, 0) - PlayerVR.fpsCamera.transform.forward * .1f;
            }
            return PlayerOrigin.GetUnadjustedPosition() + HMD.GetFlatForwardDirection() * .7f + new Vector3(0, .75f, 0);
        }

        private static bool ShouldUsePointerPosition()
        {
            return PlayerVR.fpsCamera.CameraRayObject && Vector3.Distance(HMD.GetVRInteractionFromPosition(), PlayerVR.fpsCamera.CameraRayPos) < 1f;
        }

        Vector3 GetStatusBarPosition()
        {
            return HMD.GetWorldPosition() + HMD.GetFlatForwardDirection() * 1.75f + new Vector3(0, 1.8f, 0);
        }

        void OnDestroy()
        {
            SteamVR_Events.NewPosesApplied.RemoveListener(() => OnNewPoses());
            Snapturn.OnAfterSnapTurn -= PlayerUsedSnapturn;
        }
    }
}


