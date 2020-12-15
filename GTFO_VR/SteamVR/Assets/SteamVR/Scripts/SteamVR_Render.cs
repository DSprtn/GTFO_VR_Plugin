using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{

    public class SteamVR_Render : MonoBehaviour
    {



        public static EVREye eye { get; private set; }

        public static float unfocusedRenderResolution = .5f;



        public static SteamVR_Render instance
        {
            get
            {
                return SteamVR_Behaviour.instance.steamvr_render;
            }
        }


        private void OnApplicationQuit()
        {
            SteamVR_Render.isQuitting = true;
            SteamVR.SafeDispose();
        }


        public static void Add(SteamVR_Camera vrcam)
        {
            if (!SteamVR_Render.isQuitting)
            {
                SteamVR_Render.instance.AddInternal(vrcam);
            }
        }


        public static void Remove(SteamVR_Camera vrcam)
        {
            if (!SteamVR_Render.isQuitting && SteamVR_Render.instance != null)
            {
                SteamVR_Render.instance.RemoveInternal(vrcam);
            }
        }


        public static SteamVR_Camera Top()
        {
            if (!SteamVR_Render.isQuitting)
            {
                return SteamVR_Render.instance.TopInternal();
            }
            return null;
        }


        private void AddInternal(SteamVR_Camera vrcam)
        {
            Camera component = vrcam.GetComponent<Camera>();
            int num = this.cameras.Length;
            SteamVR_Camera[] array = new SteamVR_Camera[num + 1];
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                Camera component2 = this.cameras[i].GetComponent<Camera>();
                if (i == num2 && component2.depth > component.depth)
                {
                    array[num2++] = vrcam;
                }
                array[num2++] = this.cameras[i];
            }
            if (num2 == num)
            {
                array[num2] = vrcam;
            }
            this.cameras = array;
            base.enabled = true;
        }


        private void RemoveInternal(SteamVR_Camera vrcam)
        {
            int num = this.cameras.Length;
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                if (this.cameras[i] == vrcam)
                {
                    num2++;
                }
            }
            if (num2 == 0)
            {
                return;
            }
            SteamVR_Camera[] array = new SteamVR_Camera[num - num2];
            int num3 = 0;
            for (int j = 0; j < num; j++)
            {
                SteamVR_Camera steamVR_Camera = this.cameras[j];
                if (steamVR_Camera != vrcam)
                {
                    array[num3++] = steamVR_Camera;
                }
            }
            this.cameras = array;
        }


        private SteamVR_Camera TopInternal()
        {
            if (this.cameras.Length != 0)
            {
                return this.cameras[this.cameras.Length - 1];
            }
            return null;
        }




        public static bool pauseRendering
        {
            get
            {
                return SteamVR_Render._pauseRendering;
            }
            set
            {
                SteamVR_Render._pauseRendering = value;
                CVRCompositor compositor = OpenVR.Compositor;
                if (compositor != null)
                {
                    compositor.SuspendRendering(value);
                }
            }
        }


        private IEnumerator RenderLoop()
        {
            while (Application.isPlaying)
            {
                yield return this.waitForEndOfFrame;
                if (!SteamVR_Render.pauseRendering)
                {
                    CVRCompositor compositor = OpenVR.Compositor;
                    if (compositor != null)
                    {
                        if (!compositor.CanRenderScene() || cameras.Length < 1)
                        {
                            continue;
                        }
                        compositor.SetTrackingSpace(SteamVR.settings.trackingSpace);
                        SteamVR_Utils.QueueEventOnRenderThread(201510020);
                        SteamVR.Unity.EventWriteString("[UnityMain] GetNativeTexturePtr - Begin");
                        SteamVR_Camera.GetSceneTexture(this.cameras[0].GetComponent<Camera>()).GetNativeTexturePtr();
                        SteamVR.Unity.EventWriteString("[UnityMain] GetNativeTexturePtr - End");
                        compositor.GetLastPoses(this.poses, this.gamePoses);
                        SteamVR_Events.NewPoses.Send(this.poses);
                        SteamVR_Events.NewPosesApplied.Send();
                    }
                    SteamVR_Overlay instance = SteamVR_Overlay.instance;
                    if (instance != null)
                    {
                        instance.UpdateOverlay();
                    }
                    if (this.CheckExternalCamera())
                    {
                        this.RenderExternalCamera();
                    }
                    SteamVR instance2 = SteamVR.instance;
                    this.RenderEye(instance2, EVREye.Eye_Left);
                    this.RenderEye(instance2, EVREye.Eye_Right);
                    foreach (SteamVR_Camera steamVR_Camera in this.cameras)
                    {
                        steamVR_Camera.transform.localPosition = Vector3.zero;
                        steamVR_Camera.transform.localRotation = Quaternion.identity;
                    }
                    if (this.cameraMask != null)
                    {
                        this.cameraMask.Clear();
                    }
                }
            }
            yield break;
        }


        private bool CheckExternalCamera()
        {
            bool? flag = this.doesPathExist;
            bool flag2 = false;
            if (flag.GetValueOrDefault() == flag2 & flag != null)
            {
                return false;
            }
            if (this.doesPathExist == null)
            {
                this.doesPathExist = new bool?(File.Exists(this.externalCameraConfigPath));
            }
            if (this.externalCamera == null)
            {
                flag = this.doesPathExist;
                flag2 = true;
                if (flag.GetValueOrDefault() == flag2 & flag != null)
                {
                    GameObject gameObject = Resources.Load<GameObject>("SteamVR_ExternalCamera");
                    if (gameObject == null)
                    {
                        this.doesPathExist = new bool?(false);
                        return false;
                    }
                    if (SteamVR_Settings.instance.legacyMixedRealityCamera)
                    {
                        if (!SteamVR_ExternalCamera_LegacyManager.hasCamera)
                        {
                            return false;
                        }
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                        gameObject2.gameObject.name = "External Camera";
                        this.externalCamera = gameObject2.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
                        this.externalCamera.configPath = this.externalCameraConfigPath;
                        this.externalCamera.ReadConfig();
                        this.externalCamera.SetupDeviceIndex(SteamVR_ExternalCamera_LegacyManager.cameraIndex);
                    }
                    else
                    {
                        SteamVR_Action_Pose mixedRealityCameraPose = SteamVR_Settings.instance.mixedRealityCameraPose;
                        SteamVR_Input_Sources mixedRealityCameraInputSource = SteamVR_Settings.instance.mixedRealityCameraInputSource;
                        if (mixedRealityCameraPose != null && SteamVR_Settings.instance.mixedRealityActionSetAutoEnable && mixedRealityCameraPose.actionSet != null && !mixedRealityCameraPose.actionSet.IsActive(mixedRealityCameraInputSource))
                        {
                            mixedRealityCameraPose.actionSet.Activate(mixedRealityCameraInputSource, 0, false);
                        }
                        if (mixedRealityCameraPose == null)
                        {
                            this.doesPathExist = new bool?(false);
                            return false;
                        }
                        if (mixedRealityCameraPose != null && mixedRealityCameraPose[mixedRealityCameraInputSource].active && mixedRealityCameraPose[mixedRealityCameraInputSource].deviceIsConnected)
                        {
                            GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                            gameObject3.gameObject.name = "External Camera";
                            this.externalCamera = gameObject3.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
                            this.externalCamera.configPath = this.externalCameraConfigPath;
                            this.externalCamera.ReadConfig();
                            this.externalCamera.SetupPose(mixedRealityCameraPose, mixedRealityCameraInputSource);
                        }
                    }
                }
            }
            return this.externalCamera != null;
        }


        private void RenderExternalCamera()
        {
            if (this.externalCamera == null)
            {
                return;
            }
            if (!this.externalCamera.gameObject.activeInHierarchy)
            {
                return;
            }
            int num = (int)Mathf.Max(this.externalCamera.config.frameSkip, 0f);
            if (Time.frameCount % (num + 1) != 0)
            {
                return;
            }
            this.externalCamera.AttachToCamera(this.TopInternal());
            this.externalCamera.RenderNear();
            this.externalCamera.RenderFar();
        }


        private void OnInputFocus(bool hasFocus)
        {
            if (!SteamVR.active)
            {
                return;
            }
            if (hasFocus)
            {
                if (SteamVR.settings.pauseGameWhenDashboardVisible)
                {
                    Time.timeScale = this.timeScale;
                }
                SteamVR_Camera.sceneResolutionScale = this.sceneResolutionScale;
                return;
            }
            if (SteamVR.settings.pauseGameWhenDashboardVisible)
            {
                this.timeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            this.sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
            SteamVR_Camera.sceneResolutionScale = unfocusedRenderResolution;
        }


        private string GetScreenshotFilename(uint screenshotHandle, EVRScreenshotPropertyFilenames screenshotPropertyFilename)
        {
            EVRScreenshotError evrscreenshotError = EVRScreenshotError.None;
            uint screenshotPropertyFilename2 = OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, null, 0u, ref evrscreenshotError);
            if (evrscreenshotError != EVRScreenshotError.None && evrscreenshotError != EVRScreenshotError.BufferTooSmall)
            {
                return null;
            }
            if (screenshotPropertyFilename2 <= 1u)
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder((int)screenshotPropertyFilename2);
            OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, stringBuilder, screenshotPropertyFilename2, ref evrscreenshotError);
            if (evrscreenshotError != EVRScreenshotError.None)
            {
                return null;
            }
            return stringBuilder.ToString();
        }


        private void OnRequestScreenshot(VREvent_t vrEvent)
        {
            uint handle = vrEvent.data.screenshot.handle;
            EVRScreenshotType type = (EVRScreenshotType)vrEvent.data.screenshot.type;
            if (type == EVRScreenshotType.StereoPanorama)
            {
                string screenshotFilename = this.GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.Preview);
                string screenshotFilename2 = this.GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.VR);
                if (screenshotFilename == null || screenshotFilename2 == null)
                {
                    return;
                }
                SteamVR_Utils.TakeStereoScreenshot(handle, new GameObject("screenshotPosition")
                {
                    transform =
                    {
                        position = SteamVR_Render.Top().transform.position,
                        rotation = SteamVR_Render.Top().transform.rotation,
                        localScale = SteamVR_Render.Top().transform.lossyScale
                    }
                }, 32, 0.064f, ref screenshotFilename, ref screenshotFilename2);
                OpenVR.Screenshots.SubmitScreenshot(handle, type, screenshotFilename, screenshotFilename2);
            }
        }


        private void OnEnable()
        {
            base.StartCoroutine(this.RenderLoop());
            SteamVR_Events.InputFocus.Listen(new UnityAction<bool>(this.OnInputFocus));
            SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Listen(new UnityAction<VREvent_t>(this.OnRequestScreenshot));
            if (SteamVR_Settings.instance.legacyMixedRealityCamera)
            {
                SteamVR_ExternalCamera_LegacyManager.SubscribeToNewPoses();
            }
            Application.onBeforeRender += this.OnBeforeRender;
            Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.OnCameraPreCull));
            if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
            {
                OpenVR.Screenshots.HookScreenshot(this.screenshotTypes);
                return;
            }
            SteamVR_Events.Initialized.AddListener(new UnityAction<bool>(this.OnSteamVRInitialized));
        }


        private void OnSteamVRInitialized(bool success)
        {
            if (success)
            {
                OpenVR.Screenshots.HookScreenshot(this.screenshotTypes);
            }
        }


        private void OnDisable()
        {
            base.StopAllCoroutines();
            SteamVR_Events.InputFocus.Remove(new UnityAction<bool>(this.OnInputFocus));
            SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Remove(new UnityAction<VREvent_t>(this.OnRequestScreenshot));
            Application.onBeforeRender -= this.OnBeforeRender;
            Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.OnCameraPreCull));
            if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            {
                SteamVR_Events.Initialized.RemoveListener(new UnityAction<bool>(this.OnSteamVRInitialized));
            }
        }


        public void UpdatePoses()
        {
            CVRCompositor compositor = OpenVR.Compositor;
            if (compositor != null)
            {
                compositor.GetLastPoses(this.poses, this.gamePoses);
                SteamVR_Events.NewPoses.Send(this.poses);
                SteamVR_Events.NewPosesApplied.Send();
            }
        }


        private void OnBeforeRender()
        {
            if (!SteamVR.active)
            {
                return;
            }
            if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
            {
                this.UpdatePoses();
            }
        }


        private void Update()
        {
            if (!SteamVR.active)
            {
                return;
            }
            this.UpdatePoses();
            CVRSystem system = OpenVR.System;
            if (system != null)
            {
                VREvent_t vrevent_t = default(VREvent_t);
                uint uncbVREvent = (uint)Marshal.SizeOf(typeof(VREvent_t));
                int num = 0;
                while (num < 64 && system.PollNextEvent(ref vrevent_t, uncbVREvent))
                {
                    EVREventType eventType = (EVREventType)vrevent_t.eventType;
                    if (eventType <= EVREventType.VREvent_InputFocusReleased)
                    {
                        if (eventType != EVREventType.VREvent_InputFocusCaptured)
                        {
                            if (eventType != EVREventType.VREvent_InputFocusReleased)
                            {
                                goto IL_CA;
                            }
                            if (vrevent_t.data.process.pid == 0u)
                            {
                                SteamVR_Events.InputFocus.Send(true);
                            }
                        }
                        else if (vrevent_t.data.process.oldPid == 0u)
                        {
                            SteamVR_Events.InputFocus.Send(false);
                        }
                    }
                    else if (eventType != EVREventType.VREvent_HideRenderModels)
                    {
                        if (eventType != EVREventType.VREvent_ShowRenderModels)
                        {
                            goto IL_CA;
                        }
                        SteamVR_Events.HideRenderModels.Send(false);
                    }
                    else
                    {
                        SteamVR_Events.HideRenderModels.Send(true);
                    }
                    IL_C4:
                    num++;
                    continue;
                    IL_CA:
                    SteamVR_Events.System((EVREventType)vrevent_t.eventType).Send(vrevent_t);
                    goto IL_C4;
                }
            }
            Application.targetFrameRate = -1;
            Application.runInBackground = true;
            QualitySettings.maxQueuedFrames = -1;
            QualitySettings.vSyncCount = 0;
            if (SteamVR.settings.lockPhysicsUpdateRateToRenderFrequency && Time.timeScale > 0f)
            {
                SteamVR instance = SteamVR.instance;
                if (instance != null)
                {
                    Time.fixedDeltaTime = Time.timeScale / instance.hmd_DisplayFrequency;
                }
            }
        }


        private void OnCameraPreCull(Camera cam)
        {
            if (!SteamVR.active)
            {
                return;
            }
            if (cam.cameraType != CameraType.VR)
            {
                return;
            }
            if (!cam.stereoEnabled)
            {
                return;
            }
            if (Time.frameCount != SteamVR_Render.lastFrameCount)
            {
                SteamVR_Render.lastFrameCount = Time.frameCount;
                if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
                {
                    this.UpdatePoses();
                }
            }
        }


        private void RenderEye(SteamVR vr, EVREye eye)
        {
            SteamVR_Render.eye = eye;
            if (this.cameraMask != null)
            {
                this.cameraMask.Set(vr, eye);
            }
            foreach (SteamVR_Camera steamVR_Camera in this.cameras)
            {
                steamVR_Camera.transform.localPosition = vr.eyes[(int)eye].pos;
                steamVR_Camera.transform.localRotation = vr.eyes[(int)eye].rot;
                this.cameraMask.transform.position = steamVR_Camera.transform.position;
                Camera camera = steamVR_Camera.camera;
                camera.targetTexture = SteamVR_Camera.GetSceneTexture(false);
                int cullingMask = camera.cullingMask;
                if (eye == EVREye.Eye_Left)
                {
                    camera.cullingMask &= ~this.rightMask;
                    camera.cullingMask |= this.leftMask;
                }
                else
                {
                    camera.cullingMask &= ~this.leftMask;
                    camera.cullingMask |= this.rightMask;
                }
                camera.Render();
                camera.cullingMask = cullingMask;
            }
        }


        private void FixedUpdate()
        {
            SteamVR_Utils.QueueEventOnRenderThread(201510024);
        }


        private void Awake()
        {
            this.cameraMask = new GameObject("cameraMask")
            {
                transform =
                {
                    parent = base.transform
                }
            }.AddComponent<SteamVR_CameraMask>();
            if (this.externalCamera == null && File.Exists(this.externalCameraConfigPath))
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("SteamVR_ExternalCamera"));
                gameObject.gameObject.name = "External Camera";
                this.externalCamera = gameObject.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
                this.externalCamera.configPath = this.externalCameraConfigPath;
                this.externalCamera.ReadConfig();
            }
        }


        public SteamVR_ExternalCamera externalCamera;


        public string externalCameraConfigPath = "externalcamera.cfg";


        private static bool isQuitting;


        public SteamVR_Camera[] cameras = new SteamVR_Camera[0];


        public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[64];


        public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];


        private static bool _pauseRendering;


        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();


        private bool? doesPathExist;


        private float sceneResolutionScale = 1f;


        private float timeScale = 1f;


        private EVRScreenshotType[] screenshotTypes = new EVRScreenshotType[]
        {
            EVRScreenshotType.StereoPanorama
        };


        private static int lastFrameCount = -1;


        public LayerMask leftMask;


        public LayerMask rightMask;


        private SteamVR_CameraMask cameraMask;
    }
}
