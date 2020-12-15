using Assets.SteamVR_Standalone.Standalone;
using Standalone;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Valve.VR
{

    [RequireComponent(typeof(Camera))]
    public class SteamVR_Camera : MonoBehaviour
    {


        public Transform head
        {
            get
            {
                return this._head;
            }
        }



        public Transform offset
        {
            get
            {
                return this._head;
            }
        }

        public Transform origin
        {
            get
            {
                return this._head.parent;
            }
        }

        public Camera camera { get; private set; }

        public Transform ears
        {
            get
            {
                return this._ears;
            }
        }


        public Ray GetRay()
        {
            return new Ray(this._head.position, this._head.forward);
        }

        public static float sceneResolutionScale = 1f;

        private void OnDisable()
        {
            SteamVR_Render.Remove(this);
        }

        private void OnEnable()
        {
            SteamVR instance = SteamVR.instance;
            if (instance == null)
            {
                if (this.head != null)
                {
                    this.head.GetComponent<SteamVR_GameView>().enabled = false;
                    this.head.GetComponent<SteamVR_TrackedObject>().enabled = false;
                }
                if (this.flip != null)
                {
                    this.flip.enabled = false;
                }
                base.enabled = false;
                return;
            }
            this.Expand();
            if (SteamVR_Camera.blitMaterial == null)
            {
                SteamVR_Camera.blitMaterial = new Material(VRShaders.GetShader(VRShaders.VRShader.blit));
            }
            Camera component = base.GetComponent<Camera>();
            component.fieldOfView = instance.fieldOfView;
            component.aspect = instance.aspect;
            component.eventMask = 0;
            component.orthographic = false;
            component.enabled = false;
            if (component.actualRenderingPath != RenderingPath.Forward && QualitySettings.antiAliasing > 1)
            {
                Debug.LogWarning("MSAA only supported in Forward rendering path. (disabling MSAA)");
                QualitySettings.antiAliasing = 0;
            }
            Camera component2 = this.head.GetComponent<Camera>();
            if (component2 != null)
            {
                component2.renderingPath = component.renderingPath;
            }
            if (this.ears == null)
            {
                SteamVR_Ears componentInChildren = base.transform.GetComponentInChildren<SteamVR_Ears>();
                if (componentInChildren != null)
                {
                    this._ears = componentInChildren.transform;
                }
            }
            if (this.ears != null)
            {
                this.ears.GetComponent<SteamVR_Ears>().vrcam = this;
            }
            SteamVR_Render.Add(this);
        }

        private void Awake()
        {
            this.camera = base.GetComponent<Camera>();

            this.ForceLast();
        }

        public void ForceLast()
        {
            if (SteamVR_Camera.values != null)
            {
                foreach (object obj in SteamVR_Camera.values)
                {
                    DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
                    (dictionaryEntry.Key as FieldInfo).SetValue(this, dictionaryEntry.Value);
                }
                SteamVR_Camera.values = null;
                return;
            }
            Component[] components = base.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                SteamVR_Camera steamVR_Camera = components[i] as SteamVR_Camera;
                if (steamVR_Camera != null && steamVR_Camera != this)
                {
                    if (steamVR_Camera.flip != null)
                    {
                        UnityEngine.Object.DestroyImmediate(steamVR_Camera.flip);
                    }
                    UnityEngine.Object.DestroyImmediate(steamVR_Camera);
                }
            }
            components = base.GetComponents<Component>();
            if (this != components[components.Length - 1] || this.flip == null)
            {
                if (this.flip == null)
                {
                    this.flip = base.gameObject.AddComponent<SteamVR_CameraFlip>();
                }
                SteamVR_Camera.values = new Hashtable();
                foreach (FieldInfo fieldInfo in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (fieldInfo.IsPublic || fieldInfo.IsDefined(typeof(SerializeField), true))
                    {
                        SteamVR_Camera.values[fieldInfo] = fieldInfo.GetValue(this);
                    }
                }
                GameObject gameObject = base.gameObject;
                UnityEngine.Object.DestroyImmediate(this);
                gameObject.AddComponent<SteamVR_Camera>().ForceLast();
            }
        }

        public string baseName
        {
            get
            {
                if (!base.name.EndsWith(" (eye)"))
                {
                    return base.name;
                }
                return base.name.Substring(0, base.name.Length - " (eye)".Length);
            }
        }

        public void Expand()
        {
            Transform transform = base.transform.parent;
            if (transform == null)
            {
                transform = new GameObject(base.name + " (origin)").transform;
                transform.localPosition = base.transform.localPosition;
                transform.localRotation = base.transform.localRotation;
                transform.localScale = base.transform.localScale;
            }
            if (this.head == null)
            {
                if (SteamVR_Camera.useHeadTracking)
                {
                    this._head = new GameObject(base.name + " (head)", new Type[]
                    {
                        typeof(SteamVR_GameView),
                        typeof(SteamVR_TrackedObject)
                    }).transform;
                }
                else
                {
                    this._head = new GameObject(base.name + " (head)", new Type[]
                    {
                        typeof(SteamVR_GameView)
                    }).transform;

                    
                    this.head.position = base.transform.position;
                    this.head.rotation = base.transform.rotation;
                    this.head.localScale = Vector3.one;
                    this.head.tag = base.tag;
                }
                this.head.parent = transform;
                Camera component = this.head.GetComponent<Camera>();
                component.clearFlags = CameraClearFlags.Nothing;
                component.cullingMask = 0;
                component.eventMask = 0;
                component.orthographic = true;
                component.orthographicSize = 1f;
                component.nearClipPlane = 0f;
                component.farClipPlane = 1f;
                component.useOcclusionCulling = false;
            }
            if (transform.parent != this.head)
            {
                transform.parent = this.head;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                while (base.transform.childCount > 0)
                {
                    base.transform.GetChild(0).parent = this.head;
                }
                AudioListener component3 = base.GetComponent<AudioListener>();
                if (component3 != null)
                {
                    UnityEngine.Object.DestroyImmediate(component3);
                    this._ears = new GameObject(base.name + " (ears)", new Type[]
                    {
                        typeof(SteamVR_Ears)
                    }).transform;
                    this.ears.parent = this._head;
                    this.ears.localPosition = Vector3.zero;
                    this.ears.localRotation = Quaternion.identity;
                    this.ears.localScale = Vector3.one;
                }
            }
            if (!base.name.EndsWith(" (eye)"))
            {
                base.name += " (eye)";
            }
        }

        public void Collapse()
        {
            base.transform.parent = null;
            while (this.head.childCount > 0)
            {
                this.head.GetChild(0).parent = base.transform;
            }
            if (this.ears != null)
            {
                while (this.ears.childCount > 0)
                {
                    this.ears.GetChild(0).parent = base.transform;
                }
                UnityEngine.Object.DestroyImmediate(this.ears.gameObject);
                this._ears = null;
                base.gameObject.AddComponent(typeof(AudioListener));
            }
            if (this.origin != null)
            {
                if (this.origin.name.EndsWith(" (origin)"))
                {
                    Transform origin = this.origin;
                    while (origin.childCount > 0)
                    {
                        origin.GetChild(0).parent = origin.parent;
                    }
                    UnityEngine.Object.DestroyImmediate(origin.gameObject);
                }
                else
                {
                    base.transform.parent = this.origin;
                }
            }
            UnityEngine.Object.DestroyImmediate(this.head.gameObject);
            this._head = null;
            if (base.name.EndsWith(" (eye)"))
            {
                base.name = base.name.Substring(0, base.name.Length - " (eye)".Length);
            }
        }

        public static RenderTexture GetSceneTexture(bool hdr)
        {
            SteamVR instance = SteamVR.instance;
            if (instance == null)
            {
                return null;
            }
            int num = (int)(instance.sceneWidth * SteamVR_Camera.sceneResolutionScale);
            int num2 = (int)(instance.sceneHeight * SteamVR_Camera.sceneResolutionScale);
            int num3 = (QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing;
            RenderTextureFormat renderTextureFormat = hdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
            if (SteamVR_Camera._sceneTexture != null && (SteamVR_Camera._sceneTexture.width != num || SteamVR_Camera._sceneTexture.height != num2 || SteamVR_Camera._sceneTexture.antiAliasing != num3 || SteamVR_Camera._sceneTexture.format != renderTextureFormat))
            {
                UnityEngine.Object.Destroy(SteamVR_Camera._sceneTexture);
                SteamVR_Camera._sceneTexture = null;
            }
            if (SteamVR_Camera._sceneTexture == null)
            {
                SteamVR_Camera._sceneTexture = new RenderTexture(num, num2, 0, renderTextureFormat);
                SteamVR_Camera._sceneTexture.antiAliasing = num3;
                SteamVR.Unity.SetColorSpace((hdr && QualitySettings.activeColorSpace == ColorSpace.Gamma) ? EColorSpace.Gamma : EColorSpace.Auto);
            }
            return SteamVR_Camera._sceneTexture;
        }


        private void OnPreRender()
        {
            if (this.flip)
            {
                this.flip.enabled = (SteamVR_Render.Top() == this && SteamVR.instance.textureType == ETextureType.DirectX);
            }
            Camera component = this.head.GetComponent<Camera>();
            if (component != null)
            {
                component.enabled = (SteamVR_Render.Top() == this);
            }
            if (this.wireframe)
            {
                GL.wireframe = true;
            }
        }

        private void OnPostRender()
        {
            if (this.wireframe)
            {
                GL.wireframe = false;
            }
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            this.DoRenderTextureThing(src, dest);
        }

        public void DoRenderTextureThing(RenderTexture src, RenderTexture dest)
        {
            if (SteamVR_Render.Top() == this)
            {
                int eventID;
                if (SteamVR_Render.eye == EVREye.Eye_Left)
                {
                    SteamVR_Utils.QueueEventOnRenderThread(201510023);
                    eventID = 201510021;
                }
                else
                {
                    eventID = 201510022;
                }
                SteamVR_Utils.QueueEventOnRenderThread(eventID);
            }
            Graphics.SetRenderTarget(dest);
            SteamVR_Camera.blitMaterial.mainTexture = src;
            GL.PushMatrix();
            GL.LoadOrtho();
            SteamVR_Camera.blitMaterial.SetPass(0);
            GL.Begin(7);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(-1f, 1f, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(1f, -1f, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(-1f, -1f, 0f);
            GL.End();
            GL.PopMatrix();
            Graphics.SetRenderTarget(null);
        }


        [SerializeField]
        private Transform _head;

        [SerializeField]
        private Transform _ears;

        public bool wireframe;

        private static Hashtable values;

        private const string eyeSuffix = " (eye)";


        private const string earsSuffix = " (ears)";


        private const string headSuffix = " (head)";


        private const string originSuffix = " (origin)";


        private static RenderTexture _sceneTexture;


        [SerializeField]
        private SteamVR_CameraFlip flip;


        public static Material blitMaterial;


        public static bool useHeadTracking = true;
    }
}
