using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using GTFO_VR.Util;
using Player;
using System;
using System.Text;
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

        static VR_UI_Overlay overlay;

        static string currentFrameInput = "";

        public static bool keyboardClosedThisFrame;

        public static GameObject watchPrefab;

        public static bool hackingToolRenderingOverriden;

        public static Resolution VR_Resolution;

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
            SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(OnKeyboardInput);
            SteamVR_Events.System(EVREventType.VREvent_KeyboardDone).Listen(OnKeyboardDone);
            SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(OnKeyboardDone);
            FocusStateEvents.OnFocusStateChange += FocusChanged;

            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            if (assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            watchPrefab = assetBundle.LoadAsset<GameObject>("assets/p_vrwatch.prefab");
            Setup();
            SteamVR_Settings.instance.poseUpdateMode = SteamVR_UpdateModes.OnLateUpdate;
        }



        public void OnKeyboardDone(VREvent_t arg0)
        {
            keyboardClosedThisFrame = true;
        }

        private void OnKeyboardInput(VREvent_t ev)
        {
            VREvent_Keyboard_t keyboard = ev.data.keyboard;
            byte[] inputBytes = new byte[] { keyboard.cNewInput0, keyboard.cNewInput1, keyboard.cNewInput2, keyboard.cNewInput3, keyboard.cNewInput4, keyboard.cNewInput5, keyboard.cNewInput6, keyboard.cNewInput7 };
            int len = 0;
            for (; inputBytes[len] != 0 && len < 7; len++) ;
            string input = System.Text.Encoding.UTF8.GetString(inputBytes, 0, len);
            input = HandleSpecialConversionAndShortcuts(input);
           
            currentFrameInput = input;
        }

        

        public static string GetKeyboardInput()
        {
            return currentFrameInput;
        }

        void LateUpdate()
        {
            currentFrameInput = "";
            keyboardClosedThisFrame = false;
        }


        public static bool GetPlayerPointingAtPositionOnScreen(out Vector2 uv)
        {
            if(overlay)
            {
                VR_UI_Overlay.IntersectionResults result = new VR_UI_Overlay.IntersectionResults();

                if(overlay.ComputeIntersection(Controllers.GetLocalPosition(), Controllers.GetLocalAimForward(), ref result))
                {
                    uv = result.UVs;
                    return true;
                }
            }
            uv = Vector2.zero;
            return false;
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

            if(UnityEngine.Input.GetKeyDown(KeyCode.F5))
            {
                HackingTool t = FindObjectOfType<HackingTool>(); 
                if(t)
                {
                    foreach(MeshRenderer m in t.GetComponentsInChildren<MeshRenderer>())
                    {
                        foreach(Material mat in m.sharedMaterials)
                        {
                            Debug.Log("Mat : " + mat.name + "  Shader" + mat.shader.name + "\n");
                        }
                    }
                }
            }
        }

        private void Setup()
        {
            WeaponArchetypeVRData.Setup();


            SteamVR.Initialize(false);
            gameObject.AddComponent<VRInput>();
            Invoke("SetupOverlay", .25f);
            gameObject.AddComponent<HMD>();
            gameObject.AddComponent<Controllers>();
            VR_Resolution = new Resolution
            {
                height = (int)SteamVR.instance.sceneHeight,
                width = (int)SteamVR.instance.sceneWidth
            };

            DontDestroyOnLoad(gameObject);
        }

        void SetupOverlay()
        {
            overlay = new GameObject("Overlay").AddComponent<VR_UI_Overlay>();
        }

        public static void ClearUIRenderTex()
        {

            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = UI_Core.UIPassHUD.Camera.targetTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
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

            if(state.Equals(eFocusState.ComputerTerminal))
            {
                SteamVR_Render.unfocusedRenderResolution = 1f;
                SteamVR.instance.overlay.ShowKeyboard(0, 0, "Terminal input", 256, "", true, 0);

                OrientKeyboard();
            }
            else
            {
                SteamVR.instance.overlay.HideKeyboard();
                SteamVR_Render.unfocusedRenderResolution = .5f;
            }
            ClearUIRenderTex();
        }


        private static void OrientKeyboard()
        {
            Quaternion Rot = Quaternion.Euler(Vector3.Project(HMD.hmd.transform.localRotation.eulerAngles, Vector3.up));
            Vector3 Pos = HMD.hmd.transform.localPosition + Rot * Vector3.forward * 1f;
            Pos.y = HMD.hmd.transform.localPosition.y + .5f;
            Rot = Quaternion.Euler(0f, Rot.eulerAngles.y, 0f);
            var t = new SteamVR_Utils.RigidTransform(Pos, Rot).ToHmdMatrix34();
            SteamVR.instance.overlay.SetKeyboardTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, ref t);
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
            ClusteredRendering.Current.OnResolutionChange(new Resolution());
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
            Invoke("DisableUnneccessaryCams",.1f);

        }

        void DisableUnneccessaryCams()
        {
            if(PlayerVR.VRCamera && PlayerVR.VRCamera.head)
            {
                foreach (Camera cam in PlayerVR.VRCamera.transform.root.GetComponentsInChildren<Camera>())
                {
                    cam.enabled = false;
                }
            }
        }

        private string HandleSpecialConversionAndShortcuts(string input)
        {
            switch (input)
            {
                case ("\n"):
                    {
                        return "\r";
                    }
                case ("-"):
                    {
                        return "_";
                    }
                case ("L"):
                    {
                        return "LIST ";
                    }
                case ("Q"):
                    {
                        return "QUERY ";
                    }
                case ("R"):
                    {
                        return "REACTOR";
                    }
                case ("H"):
                    {
                        return "HELP";
                    }
                case ("C"):
                    {
                        return "COMMANDS";
                    }
                case ("V"):
                    {
                        return "REACTOR_VERIFY ";
                    }
                case ("P"):
                    {
                        return "PING ";
                    }
                case ("A"):
                    {
                        return "AMMOPACK";
                    }
                case ("T"):
                    {
                        return "TOOL_REFILL";
                    }
                case ("M"):
                    {
                        return "MEDIPACK";
                    }
                case ("Z"):
                    {
                        return "ZONE_";
                    }
                case ("U"):
                    {
                        return "UPLINK_VERIFY ";
                    }
            }
            return input;
        }

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusChanged;
        }
    }
}
