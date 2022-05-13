using System;
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


        private SteamVR_Action_Boolean m_click = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch");

        private static readonly float LINE_WIDTH = 0.003f;

        public float m_DefaultLength = 0.5f;
        public GameObject m_Dot;

        private AnimationCurve mFarCurve = new AnimationCurve();

        private SteamVR_Input_Sources m_InputSource;    // Right hand left hand

        private RaycastHit m_prevHit;
        private RaycastHit m_currentHit;
        private RaycastHit m_ButtonPressHit;

        private LineRenderer m_LineRenderer = null;

        static MethodInfo s_Selectable_DoStateTransition = typeof(Selectable).GetMethod("DoStateTransition",
    BindingFlags.NonPublic | BindingFlags.Instance);

        //static Type s_Selectable_StateEnum = typeof(Selectable).GetNestedTypes().FirstOrDefault(x => x.IsEnum && x.Name.Equals("SelectionState" ));

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

        // 0 normal, 1 highlighted, rest we don't care about.
        private void setSelectableState( Selectable selectable, SelectionState state)
        {
           // Debug.Log("Setting state to: " + state);

            s_Selectable_DoStateTransition.Invoke(selectable, new object[]{(int)state, false});
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
            //m_LineRenderer.widthMultiplier = 0.003f;

            Material lineMaterial = m_LineRenderer.material;
            lineMaterial.renderQueue = (int)RenderQueue.Overlay + 1;
            lineMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?

            mFarCurve.AddKey(0, LINE_WIDTH);
            mFarCurve.AddKey(1, 0);
            m_LineRenderer.widthCurve = mFarCurve;

            ///////////////////
            // End dot
            ///////////////////
            m_Dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_Dot.transform.localScale = new Vector3(0.01f, 0.01f, 0.001f);
            UnityEngine.Object.Destroy(m_Dot.GetComponent<SphereCollider>());
            m_Dot.transform.SetParent(gameObject.transform);

            Material dotMat = m_Dot.GetComponent<MeshRenderer>().sharedMaterial;
            dotMat.renderQueue = (int)RenderQueue.Overlay +1;
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

        private void handleInput()
        {
            if (m_currentHit.collider == null)
                return;

            bool down = m_click.GetStateDown(m_InputSource);
            bool up = m_click.GetStateUp(m_InputSource);

            Selectable selectable = m_prevHit.collider.gameObject.GetComponent<Selectable>();
            Button button = m_prevHit.collider.gameObject.GetComponent<Button>();

            if (down)
            {
                //Debug.Log("Down");
                m_ButtonPressHit = m_currentHit;
                button.onClick.Invoke();
                setSelectableState(selectable, SelectionState.Pressed);
            }

            if (up)
            {
                //Debug.Log("Up");
                // Restore state to highlighted if we are still hovering over the same button, 
                // otherwise return state of the button we clicked on to normal.
                if ( m_currentHit.collider == m_ButtonPressHit.collider)
                    setSelectableState(selectable, SelectionState.Highlighted);
                else if ( m_ButtonPressHit.collider != null )
                {
                    Selectable oldSelectable = m_ButtonPressHit.collider.gameObject.GetComponent<Selectable>();
                    setSelectableState(oldSelectable, SelectionState.Normal);
                }
                   
            }
        }

        private void doRaycast()
        {
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(this.transform.position, this.transform.forward, out hit, 500);

            m_prevHit = m_currentHit;
            m_currentHit = hit;

            
        }

        private void handleHighlight()
        {
            if (m_currentHit.collider != m_prevHit.collider)
            {
                if (m_prevHit.collider != null)
                {
                    Selectable prevButton = m_prevHit.collider.gameObject.GetComponent<Selectable>();
                    setSelectableState(prevButton, SelectionState.Normal);
                }

                Selectable button = m_currentHit.collider.gameObject.GetComponent<Selectable>();
                setSelectableState(button, SelectionState.Highlighted);
            }
        }

        public void updateLine()
        {
            bool hit = m_currentHit.collider != null;

            Vector3 endPosition;
            if (hit)
                endPosition = m_currentHit.point;
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
          
        }
    }
}
