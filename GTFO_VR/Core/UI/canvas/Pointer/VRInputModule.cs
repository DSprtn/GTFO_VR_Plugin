using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace Assets.scripts.Pointer
{
    

    class VRInputModule : BaseInputModule
    {
        public Camera m_Camera;

        public SteamVR_Action_Boolean m_ClickR = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch");
        public SteamVR_Action_Boolean m_ClickL = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch");

        public SteamVR_Action_Boolean m_ReleaseR = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch");
        public SteamVR_Action_Boolean m_ReleaseL = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/GrabPinch");

        private GameObject m_CurrentObject = null;
        private PointerEventData m_Data = null;

        public static HashSet<Canvas> m_TargetCanvases = new HashSet<Canvas>();
        public static HashSet<CanvasPointer> m_CanvasPointers = new HashSet<CanvasPointer>();
        
        private static VRInputModule Current = null;

        private CanvasPointer mCurrentPointer = null;

        
        public static VRInputModule getOrCreate()
        {
            if (Current != null)
                return Current;

            // Beware of different scenes having spearate event systems
            GameObject EventSystemGameObject = EventSystem.current.gameObject;   

            Current = EventSystemGameObject.AddComponent<VRInputModule>();

            return Current;
        }

        public float getDistance( CanvasPointer pointer, float defaultLength )
        {
            if (mCurrentPointer == pointer)
            {
                float distance = m_Data.pointerCurrentRaycast.distance;
                if (distance <= 0)
                    return defaultLength;
                else
                    return distance;
            }
            else
            {
                return -1;  // Don't display anything
            }
        }

        protected override void Awake()
        {
            m_Data = new PointerEventData(eventSystem);
        }

        public void setPointer(CanvasPointer pointer)
        {
            mCurrentPointer = pointer;
            m_Camera = mCurrentPointer.m_Camera;

            Debug.Log("Set camera to: " + m_Camera.name);

            foreach(Canvas canvas in m_TargetCanvases)
            {
                canvas.worldCamera = m_Camera;
            }
        }

        private CanvasPointer getPointerForInputSource( SteamVR_Input_Sources inputSource)
        {
            foreach(CanvasPointer pointer in m_CanvasPointers)
            {
                if (pointer.m_InputSource == inputSource)
                    return pointer;
            }

            Debug.LogError("Could not find pointer for inpout source: " + inputSource);

            return null;
        }

        public override void Process()
        {
            m_Data.Reset();
            m_Data.position = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2);

            eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
            m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;

            m_RaycastResultCache.Clear();

            HandlePointerExitAndEnter(m_Data, m_CurrentObject);

            /*
            if (m_ClickR.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                if ( mCurrentPointer.m_InputSource == SteamVR_Input_Sources.RightHand )
                {
                    // pointer already in focus. do things.
                    ProcessPress(m_Data);
                }
                else
                {
                    // Switch to the right pointer, don't do anything until next down
                    setPointer(getPointerForInputSource(SteamVR_Input_Sources.RightHand));
                }
            }

            if (m_ClickL.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                if (mCurrentPointer.m_InputSource == SteamVR_Input_Sources.LeftHand)
                {
                    // pointer already in focus. do things.
                    ProcessPress(m_Data);
                }
                else
                {
                    // Switch to the right pointer, don't do anything until next down
                    setPointer(getPointerForInputSource(SteamVR_Input_Sources.LeftHand));
                }
            }*/

            handleDown(SteamVR_Input_Sources.LeftHand, m_ClickL, true);
            handleDown(SteamVR_Input_Sources.RightHand, m_ClickR, true);

            handleDown(SteamVR_Input_Sources.LeftHand, m_ReleaseL, false);
            handleDown(SteamVR_Input_Sources.RightHand, m_ReleaseR,false);
        }

        private void handleDown( SteamVR_Input_Sources inputSource, SteamVR_Action_Boolean action, bool isDown )
        {
            bool validAction = isDown ? action.GetStateDown(inputSource) : action.GetStateUp(inputSource);

            if (validAction)
            {
                if (mCurrentPointer.m_InputSource == inputSource)
                {
                    if (isDown)
                    {
                        ProcessPress(m_Data);
                    }
                    else
                    {
                        ProcessRelease(m_Data);
                    }
                   
                }
                else
                {
                    // Switch to the right pointer, don't do anything until next down
                    if (isDown)
                        setPointer(getPointerForInputSource(inputSource));
                }
            }
        }

        public PointerEventData GetData()
        {
            return m_Data;
        }

        private void ProcessPress( PointerEventData data )
        {
            if (!m_CurrentObject)
                return;

            GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(m_CurrentObject, data, ExecuteEvents.pointerDownHandler);

            if (newPointerPress == null)
                newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);

            data.pressPosition = data.position;
            data.pointerPress = newPointerPress;
            data.rawPointerPress = m_CurrentObject;

            Debug.Log("Press");
        }

        private void ProcessRelease(PointerEventData data )
        {

            //if (!m_CurrentObject)
            //    return;

            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);

            if (data.pointerPress == pointerUpHandler)
            {
                ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
            }

            // Will this mess with game stuff?
            eventSystem.SetSelectedGameObject(null);

            data.pressPosition = Vector2.zero;
            data.pointerPress = null;
            data.rawPointerPress = null;




            Debug.Log("Release");
        }

        protected override void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }





    }
}
