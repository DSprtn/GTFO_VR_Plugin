using GTFO_VR.Core.UI.canvas.Pointer;
using GTFO_VR.Core.UI.Canvas;
using GTFO_VR.Core.UI.Canvas.KeyboardDefinition;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

namespace GTFO_VR.Core.UI.Canvas.Pointer
{
    class CanvasPointer : MonoBehaviour
    {
        //private SteamVR_Action_Boolean m_click = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch");
        private SteamVR_Action_Boolean m_click = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/Shoot");

        private static readonly float LINE_WIDTH = 0.003f;

        public readonly float m_DefaultLength = 0.3f; // Unity not reflecting change unless readonly??
        public GameObject m_Dot;

        private AnimationCurve mFarCurve = new AnimationCurve();

        private SteamVR_Input_Sources m_InputSource;    // Right hand left hand

        private RaycastHit m_prevHit;
        private RaycastHit m_currentHit;
        private RaycastHit m_ButtonPressHit;

        private LineRenderer m_LineRenderer = null;

        private Material m_pointerMaterial;
        private Material m_dotMaterial;

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

        public static GameObject create(SteamVR_Input_Sources inputSource )
        {
            GameObject pointerRoot = new GameObject();
            pointerRoot.name = "CanvasPointer";
            CanvasPointer pointer = pointerRoot.AddComponent<CanvasPointer>();

            pointer.m_InputSource = inputSource;

            return pointerRoot;
        }


        [HideFromIl2Cpp]
        public SteamVR_Input_Sources getInputSource()
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

            m_pointerMaterial = new Material(Shader.Find("Sprites/Default"));
            m_pointerMaterial.renderQueue = (int)RenderQueue.Overlay + 2;

            m_LineRenderer.material = m_pointerMaterial;

            mFarCurve.AddKey(0, LINE_WIDTH);
            mFarCurve.AddKey(1, LINE_WIDTH);
            m_LineRenderer.widthCurve = mFarCurve;

            Color startColor = KeyboardStyle.getPointerLineColor();
            Color endColor = KeyboardStyle.getPointerLineColor();
            endColor.a = 0;

            m_LineRenderer.SetColors(startColor, endColor);


            ///////////////////
            // End dot
            ///////////////////
            m_Dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_Dot.transform.localScale = new Vector3(0.005f, 0.005f, 0.001f);
            UnityEngine.Object.Destroy(m_Dot.GetComponent<SphereCollider>());
            m_Dot.transform.SetParent(gameObject.transform);

            m_dotMaterial = new Material(Shader.Find("Unlit/Color") );
            m_dotMaterial.renderQueue = (int)RenderQueue.Overlay +2;
            m_dotMaterial.color = KeyboardStyle.getPointerLineColor();
            m_dotMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            m_Dot.GetComponent<MeshRenderer>().material = m_dotMaterial;
        }

        private void Start()
        {
            orientBeam();
        }

        public void orientBeam()
        {
            this.transform.localPosition = new Vector3(0, -0.025f, -0.06f);
            this.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }

        private void Update()
        {
            doRaycast();
            handleMove();
            handleInput();
            updateLine();
        }

        private static MonoPointerEvent getButton(RaycastHit hit)
        {
            return hit.collider?.gameObject?.GetComponent<MonoPointerEvent>();
        }

        private static bool isCollider(RaycastHit hit)
        {
            return hit.collider != null;
        }

        private void handleInput()
        {
            bool down = m_click.GetStateDown(m_InputSource);
            bool up = m_click.GetStateUp(m_InputSource);

            if (up || down)
            {
                MonoPointerEvent button = getButton(m_currentHit);

                if (down)
                {
                    
                    if ( button != null)
                    {
                        button.onPointerDown(new PointerEvent(m_currentHit.point));
                        m_ButtonPressHit = m_currentHit;
                    }
                }

                if (up)
                {
                    if ( m_ButtonPressHit.collider != m_currentHit.collider )
                    {
                        MonoPointerEvent downButton = getButton(m_ButtonPressHit);
                        downButton?.onPointerUp(new PointerEvent(m_currentHit.point));
                    }
                    else
                    {
                        button?.onPointerUp( new PointerEvent(m_currentHit.point) );

                    }

                    m_ButtonPressHit = new RaycastHit();
                }
            }
        }

        private void doRaycast()
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

        private void handleMove()
        {
            MonoPointerEvent button = getButton(m_currentHit);

            if (m_currentHit.collider != m_prevHit.collider)
            {
                MonoPointerEvent prevButton = getButton(m_prevHit);
                prevButton?.OnPointerExit(new PointerEvent(m_prevHit.point));

                button?.OnPointerEnter(new PointerEvent(m_prevHit.point));
            }

             // Target may decide to move pointer end somewhere else for smoothing.
             if ( button != null )
            {
                m_PointerEndPosition = button.onPointerMove(new PointerEvent(m_currentHit.point));
            }

   
        }

        public void updateLine()
        {
            bool hit = isCollider(m_currentHit);

            Vector3 endPosition;
            if (hit)
            {
                endPosition = m_PointerEndPosition;

                m_Dot.transform.position = endPosition; // Position and align dot
                m_Dot.transform.rotation = m_currentHit.collider.transform.rotation;
            }
            else
            {
                endPosition = transform.position + (transform.forward * m_DefaultLength);
                m_Dot.transform.position = Vector3.zero;
            }

            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, endPosition);
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
