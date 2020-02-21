using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using Player;
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

namespace GTFO_VR
{
    public class VRGlobal : MonoBehaviour
    {

        public static VRGlobal instance;

        public static bool VR_ENABLED;

        public static bool Overlay_Active = true;

        static GameObject ingamePlayer;

        VR_UI_Overlay overlay;


        void Awake()
        {
            if(!instance)
            {
                instance = this;
            } else
            {
                Debug.LogError("Trying to create duplicate VRGlobal class");
                return;
            }
            // Prevent SteamVR from adding a tracking script automatically. We handle this manually in HMD
            SteamVR_Camera.useHeadTracking = false;

            FocusStateEvents.OnFocusStateChange += FocusChanged;

            Setup();
        }

        

        void Update()
        {
            DoDebugOnKeyDown();
        }

        private void DoDebugOnKeyDown()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                DebugHelper.LogScene();
            }
        }

        private void Setup()
        {
            SteamVR.Initialize(false);
            //menuPlayer = new GameObject("VrMenuPlayer").AddComponent<VRMenuPlayer>();
            gameObject.AddComponent<VRInput>();
            Invoke("SetupOverlay", .25f);
            gameObject.AddComponent<HMD>();
            gameObject.AddComponent<Controllers>();
           
            DontDestroyOnLoad(gameObject);
        }

        void SetupOverlay()
        {
            overlay = new GameObject("Overlay").AddComponent<VR_UI_Overlay>();
        }

        public void FocusChanged(eFocusState state)
        {
            if (state.Equals(eFocusState.FPS) || state.Equals(eFocusState.InElevator))
            {
                HandleIngameFocus();
            }

            if(state.Equals(eFocusState.MainMenu) || state.Equals(eFocusState.Map)) {
                HandleOutOfGameFocus();
            }
        }

        private void HandleOutOfGameFocus()
        {
            if(!overlay)
            {
                return;
            }

            ToggleOverlay(true);
            TogglePlayerCam(false);
        }

        private void HandleIngameFocus()
        {
            if(!overlay)
            {
                return;
            }
            if(ingamePlayer == null)
            {
                Debug.Log("Creating VR Player...");
                ingamePlayer = new GameObject();
                ingamePlayer.AddComponent<PlayerVR>();
            }
           
            ToggleOverlay(false);
            TogglePlayerCam(true);
        }

        void ToggleOverlay(bool toggle)
        {
            if(!toggle)
            {
                overlay.DestroyOverlay();
            } else
            {
                overlay.SetupOverlay();
            }
            
            overlay.gameObject.SetActive(toggle);
            overlay.OrientateOverlay();
        }

        void TogglePlayerCam(bool toggle)
        {
            PlayerVR.LoadedAndInGame = toggle;
            SteamVR_Render.pauseRendering = !toggle;
            return;
            if (PlayerVR.VRCamera)
            {
                foreach (SteamVR_Camera cam in PlayerVR.VRCamera.transform.root.gameObject.GetComponentsInChildren<SteamVR_Camera>())
                {
                    Debug.LogWarning("Toggling VR cam...");
                    cam.enabled = toggle;
                }
            }


        }

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusChanged;
        }
    }
}
