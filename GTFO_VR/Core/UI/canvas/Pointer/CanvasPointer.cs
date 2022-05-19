using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core.UI.canvas;
using GTFO_VR.Core.UI.canvas.Pointer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Valve.VR;

namespace GTFO_VR.UI.CANVAS.POINTER
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

        static MethodInfo s_Selectable_DoStateTransition;

        private Material m_pointerMaterial;
        private Material m_dotMaterial;

        private PointerHistory m_PointerHistory = new PointerHistory();

        static CanvasPointer()
        {
            // GetMethod() doesn't find it, GetRuntimeMethod() requires parameters.
            Type clazz = typeof(Selectable);
            IEnumerable<MethodInfo> methods = clazz.GetRuntimeMethods();
            foreach(MethodInfo method in methods)
            {
                if (method.Name.Equals("DoStateTransition"))
                {
                    Debug.Log("Found the method");
                    s_Selectable_DoStateTransition = method;
                    break;
                }
            }
        }


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
        private void setSelectableState( Selectable selectable, SelectionState state)
        {
            if (s_Selectable_DoStateTransition == null)
                return;

            s_Selectable_DoStateTransition.Invoke(selectable, new object[]{(int)state, true});
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

            m_pointerMaterial = new Material(Shader.Find("Unlit/Color"));
            m_pointerMaterial.renderQueue = (int)RenderQueue.Overlay + 2;
            m_pointerMaterial.color = KeyboardStyle.getPointerLineColor();
            m_LineRenderer.material = m_pointerMaterial;

            mFarCurve.AddKey(0, LINE_WIDTH);
            mFarCurve.AddKey(1, 0);
            m_LineRenderer.widthCurve = mFarCurve;

            ///////////////////
            // End dot
            ///////////////////
            m_Dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_Dot.transform.localScale = new Vector3(0.005f, 0.005f, 0.001f);
            UnityEngine.Object.Destroy(m_Dot.GetComponent<SphereCollider>());
            m_Dot.transform.SetParent(gameObject.transform);

            m_dotMaterial = new Material(Shader.Find("Unlit/Color") );
            m_dotMaterial.renderQueue = (int)RenderQueue.Overlay +2;
            m_dotMaterial.color = KeyboardStyle.getPointerColor();
            m_Dot.GetComponent<MeshRenderer>().material = m_dotMaterial;
        }

        private void Start()
        {
            orientBeam();
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
            doRaycast();
            updateLine();
            handleHighlight();
            handleInput();
        }

        private static bool isButton(RaycastHit hit)
        {
            if (hit.collider == null)
                return false;

            Button button = hit.collider.gameObject.GetComponent<Button>();
            return button != null;
        }

        private static bool isCollider(RaycastHit hit)
        {
            return hit.collider != null;
        }

        private static bool isTextCanvas(RaycastHit hit)
        {
            if (hit.collider == null)
                return false;

            return hit.collider?.gameObject.GetComponent<TerminalReader>() != null;
        }

        private void handleInput()
        {
            bool down = m_click.GetStateDown(m_InputSource);
            bool up = m_click.GetStateUp(m_InputSource);

            if (up || down)
            {
                if (down)
                {
                    if ( isButton( m_currentHit ))
                    {
                        Selectable selectable = m_currentHit.collider.gameObject.GetComponent<Selectable>();
                        Button button = m_currentHit.collider.gameObject.GetComponent<Button>();

                        m_ButtonPressHit = m_currentHit;
                        button.onClick.Invoke();
                        setSelectableState(selectable, SelectionState.Pressed);
                    }

                    if (isTextCanvas(m_currentHit))
                    {
                        TerminalReader reader = m_currentHit.collider.gameObject.GetComponent<TerminalReader>();
                        reader.submitSelection(true);
                    }
                }

                if (up)
                {
                    // Restore state to highlighted if we are still hovering over the same button, 
                    // otherwise return state of the button we clicked on to normal.
                    if (isButton(m_currentHit) && m_currentHit.collider == m_ButtonPressHit.collider )
                    {
                        Selectable selectable = m_currentHit.collider.gameObject.GetComponent<Selectable>();
                        setSelectableState(selectable, SelectionState.Highlighted);
                    }
                    else if (m_ButtonPressHit.collider != null)
                    {
                        Selectable oldSelectable = m_ButtonPressHit.collider.gameObject.GetComponent<Selectable>();
                        setSelectableState(oldSelectable, SelectionState.Normal);
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

            // Clear history ( and thus smoothing ) when transition from/to hitting something
            if (hitSomething != isCollider(m_prevHit) )
            {
                m_PointerHistory.clearPointerHistory();
            }

            m_PointerHistory.addPointerHistory(hit.point);   
        }

        private void handleHighlight()
        {
            if (m_currentHit.collider != m_prevHit.collider)
            {
                if ( isButton(m_prevHit))
                {
                    Selectable prevButton = m_prevHit.collider.gameObject.GetComponent<Selectable>();
                    setSelectableState(prevButton, SelectionState.Normal);
                }

                if (isButton(m_currentHit))
                {
                    Selectable button = m_currentHit.collider.gameObject.GetComponent<Selectable>();
                    setSelectableState(button, SelectionState.Highlighted);
                }
            }

            if ( isTextCanvas(m_currentHit) )
            {
                TerminalReader reader = m_currentHit.collider.gameObject.GetComponent<TerminalReader>();
                // Text on terminal is tiny. Use smoothened position to make things easier to select.
                reader.hoverPointer(m_PointerHistory.getSmoothenedPointerPosition());
            }
        }

        public void updateLine()
        {
            bool hit = isCollider(m_currentHit);

            Vector3 endPosition;
            if (hit)
            {
                // Text on terminal is tiny and uses smoothened position to make things easier to select.
                if (isTextCanvas(m_currentHit))
                {
                    endPosition = m_PointerHistory.getSmoothenedPointerPosition();
                }
                else
                {
                    endPosition = m_currentHit.point;
                }
            }
                
            else
                endPosition = transform.position + (transform.forward * m_DefaultLength);

            if (hit)
            {
                m_Dot.transform.position = endPosition;

                //Align with surface
                m_Dot.transform.rotation = m_currentHit.collider.transform.rotation;
            }
            else
                m_Dot.transform.position = Vector3.zero;

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
