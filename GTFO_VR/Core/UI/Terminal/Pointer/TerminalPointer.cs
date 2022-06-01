using GTFO_VR.Core.UI.Terminal.KeyboardDefinition;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{
    /// <summary>
    /// Pointer responsible for interacting with the terminal keyboard and reader.
    /// Note that this does not use the EventSystem at all.
    /// </summary>
    class TerminalPointer : MonoBehaviour
    {
        public TerminalPointer(IntPtr value) : base(value) { }

        private static readonly float LINE_WIDTH = 0.003f;

        //private SteamVR_Action_Boolean m_click = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch"); // Use when testing in editor
        private SteamVR_Action_Boolean m_click = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/Shoot");

        private SteamVR_Input_Sources m_InputSource;    // Right hand left hand

        private RaycastHit m_prevHit;
        private RaycastHit m_currentHit;
        private RaycastHit m_ButtonPressHit;

        private AnimationCurve mFarCurve = new AnimationCurve();
        private LineRenderer m_LineRenderer = null;
        private Material m_pointerMaterial;
        private Material m_dotMaterial;

        private static readonly float m_DefaultLength = 0.3f; // Unity not reflecting change unless readonly??
        private static readonly float m_DefaultDotSize = 0.01f;
        private float m_CurrentDotSize = m_DefaultDotSize;
        private GameObject m_Dot;

        // Only valid if a collider has been hit
        private Vector3 m_PointerEndPosition = Vector3.zero;

        private enum SelectionState
        {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Selected = 3,
            Disabled = 4
        }

        public static GameObject Instantiate(SteamVR_Input_Sources inputSource )
        {
            GameObject pointerRoot = new GameObject();
            pointerRoot.name = "CanvasPointer";
            TerminalPointer pointer = pointerRoot.AddComponent<TerminalPointer>();

            pointer.m_InputSource = inputSource;

            return pointerRoot;
        }


        [HideFromIl2Cpp]
        public SteamVR_Input_Sources GetInputSource()
        {
            return m_InputSource;
        }

        private void Awake()
        {

            ///////////////////////
            // Line 
            ///////////////////////
            m_LineRenderer = gameObject.AddComponent<LineRenderer>();
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.receiveShadows = false;
            m_LineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // Line renderer appears to update with a significant delay.
            // If we keep it in local space it should only affect the length and be imperceptible.
            m_LineRenderer.useWorldSpace = false;
           
            m_pointerMaterial = new Material(Shader.Find("Sprites/Default")); // Required for gradient, so clips a bit
            m_pointerMaterial.renderQueue = (int)RenderQueue.Overlay + 2;

            m_LineRenderer.material = m_pointerMaterial;

            mFarCurve.AddKey(0, LINE_WIDTH);
            mFarCurve.AddKey(1, LINE_WIDTH);
            m_LineRenderer.widthCurve = mFarCurve;

            Color startColor = KeyboardStyle.GetPointerLineColor();
            Color endColor = KeyboardStyle.GetPointerLineColor();
            endColor.a = 0;

            m_LineRenderer.startColor = startColor;
            m_LineRenderer.endColor = endColor;

            ///////////////////
            // End dot
            ///////////////////
            m_Dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_Dot.transform.localScale = new Vector3(m_DefaultDotSize, m_DefaultDotSize, 0.001f);
            UnityEngine.Object.Destroy(m_Dot.GetComponent<SphereCollider>());
            m_Dot.transform.SetParent(gameObject.transform);

            m_dotMaterial = new Material(Shader.Find("UI/Default") );
            m_dotMaterial.renderQueue = (int)RenderQueue.Overlay +2;
            m_dotMaterial.color = KeyboardStyle.GetPointerDotColor();
            m_dotMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            m_Dot.GetComponent<MeshRenderer>().material = m_dotMaterial;
        }

        private void Start()
        {
            OrientBeam();
        }

        public void OrientBeam()
        {
            this.transform.localPosition = new Vector3(0, -0.025f, -0.06f);
            this.transform.localRotation = Quaternion.Euler(20, 0, 0);
        }

        private void Update()
        {
            DoRaycast();
            HandleMove();
            HandleInput();
            UpdateLine();
        }

        /// <summary>
        /// Get our MonoPointerEvent component from collider we hit, if it implements it.
        /// </summary>
        private static MonoPointerEvent GetButton(RaycastHit hit)
        {
            return hit.collider?.gameObject?.GetComponent<MonoPointerEvent>();
        }

        private static bool IsCollider(RaycastHit hit)
        {
            return hit.collider != null;
        }

        /// <summary>
        /// Deal with controller clicks, down and up
        /// </summary>
        private void HandleInput()
        {
            bool down = m_click.GetStateDown(m_InputSource);
            bool up = m_click.GetStateUp(m_InputSource);

            if (up || down)
            {
                MonoPointerEvent button = GetButton(m_currentHit);

                if (down)
                {
                    
                    if ( button != null)
                    {
                        button.OnPointerDown(new PointerEvent(m_currentHit.point));
                        m_ButtonPressHit = m_currentHit;
                    }
                }

                if (up)
                {
                    if ( m_ButtonPressHit.collider != m_currentHit.collider )
                    {
                        MonoPointerEvent downButton = GetButton(m_ButtonPressHit);
                        downButton?.OnPointerUp(new PointerEvent(m_currentHit.point));
                    }
                    else
                    {
                        button?.OnPointerUp( new PointerEvent(m_currentHit.point) );

                    }

                    m_ButtonPressHit = new RaycastHit();
                }
            }
        }

        private void DoRaycast()
        {
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(
                this.transform.position, 
                this.transform.forward, 
                out hit, 
                500, 
                TerminalKeyboardInterface.LAYER_MASK);

            m_prevHit = m_currentHit;
            m_currentHit = hit;

            m_PointerEndPosition = m_currentHit.point;
        }

        /// <summary>
        /// Handler pointer movement; pointer enter, exit, and move
        /// </summary>
        private void HandleMove()
        {
            MonoPointerEvent button = GetButton(m_currentHit);

            if (m_currentHit.collider != m_prevHit.collider)
            {
                MonoPointerEvent prevButton = GetButton(m_prevHit);
                prevButton?.OnPointerExit(new PointerEvent(m_prevHit.point));

                button?.OnPointerEnter(new PointerEvent(m_prevHit.point));
                m_CurrentDotSize = button ? button.GetPointerSize(m_DefaultDotSize) : m_DefaultDotSize;
            }

             // Target may decide to move pointer end somewhere else for smoothing.
             if ( button != null )
             {
                m_PointerEndPosition = button.OnPointerMove(new PointerEvent(m_currentHit.point));
              }

   
        }

        /// <summary>
        /// Position the pointer line and dot
        /// </summary>
        public void UpdateLine()
        {
            bool hit = IsCollider(m_currentHit);

            Vector3 endPosition;
            if (hit)
            {
                endPosition = m_PointerEndPosition;

                m_Dot.transform.position = endPosition; // Position and align dot
                m_Dot.transform.rotation = m_currentHit.collider.transform.rotation;
                m_Dot.transform.localScale = new Vector3(m_CurrentDotSize, m_CurrentDotSize, m_Dot.transform.localScale.z);
            }
            else
            {
                endPosition = transform.position + (transform.forward * m_DefaultLength);
                m_Dot.transform.position = Vector3.zero;
            }

            m_LineRenderer.SetPosition(0, transform.localPosition );
            m_LineRenderer.SetPosition(1, transform.InverseTransformPoint( endPosition ));
        }

        private void OnDisable()
        {
            // If we are hovering over a key, it will never receive an exit event
            MonoPointerEvent enteredButton = GetButton(m_currentHit);
            if (enteredButton != null)
            {
                enteredButton.OnPointerCancel(new PointerEvent( Vector3.zero ));
            }

            // We keep track of the button we down'd, even if the pointer exits it.
            MonoPointerEvent downButton = GetButton(m_ButtonPressHit);
            if (downButton != null)
            {
                downButton.OnPointerCancel(new PointerEvent(Vector3.zero));
            }

            m_currentHit = new RaycastHit();
            m_ButtonPressHit = new RaycastHit();
        }

        private void OnDestroy()
        {
            if (m_pointerMaterial != null)
                UnityEngine.Object.Destroy(m_pointerMaterial);
            if (m_dotMaterial != null)
                UnityEngine.Object.Destroy(m_dotMaterial);
        }
    }
}
