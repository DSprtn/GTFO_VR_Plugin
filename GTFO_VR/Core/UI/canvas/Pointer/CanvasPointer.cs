using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace Assets.scripts.Pointer
{
    class CanvasPointer : MonoBehaviour
    {
        public float m_DefaultLength = 100.0f;
        public GameObject m_Dot;
        public Camera m_Camera;

        public VRInputModule m_InputModule;

        public SteamVR_Input_Sources m_InputSource;    // Right hand left hand

        //public RightHand

        private LineRenderer m_LineRenderer = null;
        public static GameObject create(SteamVR_Input_Sources inputSource )
        {
            GameObject pointerRoot = new GameObject();
            pointerRoot.name = "CanvasPointer";
            CanvasPointer pointer = pointerRoot.AddComponent<CanvasPointer>();
            pointer.m_InputSource = inputSource;

            return pointerRoot;
        }

        private void Awake()
        {
            //////////////////////////
            // Camera 
            //////////////////////////
            m_Camera = gameObject.AddComponent<Camera>();
            m_Camera.enabled = false;
            m_Camera.clearFlags = CameraClearFlags.Nothing;
            m_Camera.fieldOfView = 1;
            m_Camera.nearClipPlane = 0.01f;

            ///////////////////////
            // Line 
            ///////////////////////
            m_LineRenderer = gameObject.AddComponent<LineRenderer>();
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.receiveShadows = false;
            m_LineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            m_LineRenderer.widthMultiplier = 0.003f;

            ///////////////////
            // End dot
            ///////////////////
            m_Dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_Dot.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            UnityEngine.Object.Destroy(m_Dot.GetComponent<SphereCollider>());
            m_Dot.transform.SetParent(gameObject.transform);

            ////////////////
            // Input module
            ////////////////

            m_InputModule = VRInputModule.getOrCreate();
        }

        private void Start()
        {
            VRInputModule.m_CanvasPointers.Add(this);
            orientBeam();

            if (m_InputSource == SteamVR_Input_Sources.RightHand)
            {
                m_InputModule.setPointer(this);
            }
        }

        public void orientBeam()
        {
            this.transform.localPosition = new Vector3(0, 0, 0);
            this.transform.rotation = new Quaternion();

            this.transform.Translate(this.transform.forward * -0.05f);
            this.transform.Rotate(this.transform.right, 45);
        }

        private void Update()
        {
            updateLine();
        }

        public void updateLine()
        {
            float targetLength = m_InputModule.getDistance(this, m_DefaultLength);

            //Debug.Log("Distance: " + targetLength);

            // Treat as disabled, so something with this later.
            if (targetLength < 0)
                targetLength = 0;

            Vector3 endPosition = transform.position + (transform.forward * targetLength);

            m_Dot.transform.position = endPosition;

            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, endPosition);
        }

        private void UpdateLine()
        {

        }

        private void OnDestroy()
        {
            VRInputModule.m_CanvasPointers.Remove(this);
        }
    }
}
