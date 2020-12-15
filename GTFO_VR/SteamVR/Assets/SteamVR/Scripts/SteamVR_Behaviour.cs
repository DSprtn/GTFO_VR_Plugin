// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour
// Assembly: SteamVR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DF474E11-42EA-4738-BF41-6A2D38F0B79C
// Assembly location: S:\SteamLibrary\steamapps\common\GTFO\GTFO_Data\BrokenAssembly20012020\Managed\SteamVR.dll

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace Valve.VR
{
    public class SteamVR_Behaviour : MonoBehaviour
    {
        public static bool forcingInitialization = false;
        internal static bool isPlaying = false;
        private static bool initializing = false;
        protected static int lastFrameCount = -1;
        public bool initializeSteamVROnAwake = true;
        public bool doNotDestroy = true;
        private const string openVRDeviceName = "OpenVR";
        private static SteamVR_Behaviour _instance;
        [HideInInspector]
        public SteamVR_Render steamvr_render;
        private Coroutine initializeCoroutine;
        private bool loadedOpenVRDeviceSuccess;

        public static SteamVR_Behaviour instance
        {
            get
            {
                if ((UnityEngine.Object)SteamVR_Behaviour._instance == (UnityEngine.Object)null)
                    SteamVR_Behaviour.Initialize(false);
                return SteamVR_Behaviour._instance;
            }
        }

        public static void Initialize(bool forceUnityVRToOpenVR = false)
        {
            if (!((UnityEngine.Object)SteamVR_Behaviour._instance == (UnityEngine.Object)null) || SteamVR_Behaviour.initializing)
                return;
            SteamVR_Behaviour.initializing = true;
            GameObject gameObject1 = (GameObject)null;
            if (forceUnityVRToOpenVR)
                SteamVR_Behaviour.forcingInitialization = true;
            SteamVR_Render objectOfType1 = UnityEngine.Object.FindObjectOfType<SteamVR_Render>();
            if ((UnityEngine.Object)objectOfType1 != (UnityEngine.Object)null)
                gameObject1 = objectOfType1.gameObject;
            SteamVR_Behaviour objectOfType2 = UnityEngine.Object.FindObjectOfType<SteamVR_Behaviour>();
            if ((UnityEngine.Object)objectOfType2 != (UnityEngine.Object)null)
                gameObject1 = objectOfType2.gameObject;
            if ((UnityEngine.Object)gameObject1 == (UnityEngine.Object)null)
            {
                GameObject gameObject2 = new GameObject("[SteamVR]");
                SteamVR_Behaviour._instance = gameObject2.AddComponent<SteamVR_Behaviour>();
                SteamVR_Behaviour._instance.steamvr_render = gameObject2.AddComponent<SteamVR_Render>();
            }
            else
            {
                SteamVR_Behaviour steamVrBehaviour = gameObject1.GetComponent<SteamVR_Behaviour>();
                if ((UnityEngine.Object)steamVrBehaviour == (UnityEngine.Object)null)
                    steamVrBehaviour = gameObject1.AddComponent<SteamVR_Behaviour>();
                if ((UnityEngine.Object)objectOfType1 != (UnityEngine.Object)null)
                {
                    steamVrBehaviour.steamvr_render = objectOfType1;
                }
                else
                {
                    steamVrBehaviour.steamvr_render = gameObject1.GetComponent<SteamVR_Render>();
                    if ((UnityEngine.Object)steamVrBehaviour.steamvr_render == (UnityEngine.Object)null)
                        steamVrBehaviour.steamvr_render = gameObject1.AddComponent<SteamVR_Render>();
                }
                SteamVR_Behaviour._instance = steamVrBehaviour;
            }
            if ((UnityEngine.Object)SteamVR_Behaviour._instance != (UnityEngine.Object)null && SteamVR_Behaviour._instance.doNotDestroy)
                UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)SteamVR_Behaviour._instance.transform.root.gameObject);
            SteamVR_Behaviour.initializing = false;
        }

        protected void Awake()
        {
            SteamVR_Behaviour.isPlaying = true;
            if (!this.initializeSteamVROnAwake || SteamVR_Behaviour.forcingInitialization)
                return;
            this.InitializeSteamVR(false);
        }

        public void InitializeSteamVR(bool forceUnityVRToOpenVR = false)
        {
            if (forceUnityVRToOpenVR)
            {
                SteamVR_Behaviour.forcingInitialization = true;
                if (this.initializeCoroutine != null)
                    this.StopCoroutine(this.initializeCoroutine);
                if (XRSettings.loadedDeviceName == "OpenVR")
                    this.EnableOpenVR();
                else
                    this.initializeCoroutine = this.StartCoroutine(this.DoInitializeSteamVR(forceUnityVRToOpenVR));
            }
            else
                SteamVR.Initialize(false);
        }

        private IEnumerator DoInitializeSteamVR(bool forceUnityVRToOpenVR = false)
        {
            SteamVR_Behaviour steamVrBehaviour = this;
            XRDevice.deviceLoaded += new System.Action<string>(steamVrBehaviour.XRDevice_deviceLoaded);
            XRSettings.LoadDeviceByName("OpenVR");
            while (!steamVrBehaviour.loadedOpenVRDeviceSuccess)
                yield return (object)null;
            XRDevice.deviceLoaded -= new System.Action<string>(steamVrBehaviour.XRDevice_deviceLoaded);
            steamVrBehaviour.EnableOpenVR();
        }

        private void XRDevice_deviceLoaded(string deviceName)
        {
            if (deviceName == "OpenVR")
            {
                this.loadedOpenVRDeviceSuccess = true;
            }
            else
            {
                Debug.LogError((object)("<b>[SteamVR]</b> Tried to async load: OpenVR. Loaded: " + deviceName), (UnityEngine.Object)this);
                this.loadedOpenVRDeviceSuccess = true;
            }
        }

        private void EnableOpenVR()
        {
            XRSettings.enabled = true;
            SteamVR.Initialize(false);
            this.initializeCoroutine = (Coroutine)null;
            SteamVR_Behaviour.forcingInitialization = false;
        }

        protected void OnEnable()
        {
            Application.onBeforeRender += new UnityAction(this.OnBeforeRender);
            SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(new UnityAction<VREvent_t>(this.OnQuit));
        }

        protected void OnDisable()
        {
            Application.onBeforeRender -= new UnityAction(this.OnBeforeRender);
            SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(new UnityAction<VREvent_t>(this.OnQuit));
        }

        protected void OnBeforeRender()
        {
            this.PreCull();
        }

        protected void PreCull()
        {
            if (Time.frameCount == SteamVR_Behaviour.lastFrameCount)
                return;
            SteamVR_Behaviour.lastFrameCount = Time.frameCount;
            SteamVR_Input.OnPreCull();
        }

        protected void FixedUpdate()
        {
            SteamVR_Input.FixedUpdate();
        }

        protected void LateUpdate()
        {
            SteamVR_Input.LateUpdate();
        }

        protected void Update()
        {
            SteamVR_Input.Update();
        }

        protected void OnQuit(VREvent_t vrEvent)
        {
            Application.Quit();
        }
    }
}
