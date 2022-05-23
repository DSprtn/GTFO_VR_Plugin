using GTFO_VR.Core;
using GTFO_VR.Core.UI.Canvas.Pointer;
using GTFO_VR.Core.UI.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using GTFO_VR.Core.UI.canvas.Pointer;

namespace GTFO_VR.Core.UI.Canvas.Pointer
{
    public struct ColorTransitionState
    {
        public Color destinationColor;
        public float transitionTime;
    }

    public class ColorTransition
    {
        public Color from;
        public ColorTransitionState to;

        bool transitionFinished;

        float elapsedTime = 0;

        public ColorTransition(Color from, ColorTransitionState to)
        {
            this.from = from;
            this.to = to;
        }

        public Color evaluate( float deltaTime )    // Pass 0 to just get current
        {
            elapsedTime += deltaTime;

            float transitionRatio = to.transitionTime <= 0 ? 1 : elapsedTime / to.transitionTime;
            if (transitionRatio > 1)
            {
                transitionRatio = 1;
                transitionFinished = true;
            }

            return (from * (1f - transitionRatio) + (to.destinationColor * transitionRatio));
        }

        public bool isFinished()
        {
            return transitionFinished;
        }
    }

    public struct ColorStates
    {
        public ColorTransitionState normal;
        public ColorTransitionState highlighted;
        public ColorTransitionState pressed;
    }

    class PhysicalButton : MonoPointerEvent
    {
        public BoxCollider m_collider;
        public RectTransform m_rectTrans;
        public RoundedCubeBackground m_background;
        public MeshRenderer m_renderer;

        private MaterialPropertyBlock m_propertyBlock;
        public ButtonClickedEvent onClick = new ButtonClickedEvent();

        private ColorTransitionState m_currentState;
        private ColorStates m_ColorStates;
        private ColorTransition m_Transition = null;    // Null when not in a transition.

        public bool isHighlighted = false;
        public bool isPressed = false;

        public bool m_repeatKey = false;
        public float m_repeatKeyTriggerTime = 0.5f;
        public float m_repeatKeyDelay = 0.01f;
        private float m_downDelta = 0;
        private float m_keyRepeatDelta = 0;


        private void Awake()
        {
            m_collider = this.gameObject.AddComponent<BoxCollider>();
            m_rectTrans = this.gameObject.AddComponent<RectTransform>();
            m_background = this.gameObject.AddComponent<RoundedCubeBackground>();
            m_renderer = this.gameObject.GetComponent<MeshRenderer>();
            m_propertyBlock = new MaterialPropertyBlock();
        }

        public void setColorStates(ColorStates states )
        {
            m_ColorStates = states;
            m_currentState = m_ColorStates.normal;
            m_propertyBlock.SetColor("_Color", m_currentState.destinationColor);
            m_renderer.SetPropertyBlock(m_propertyBlock);
        }

        private void Update()
        {
            if (m_Transition != null)
            {
                m_propertyBlock.SetColor("_Color", m_Transition.evaluate(Time.deltaTime));
                m_renderer.SetPropertyBlock(m_propertyBlock);

                if (m_Transition.isFinished())
                    m_Transition = null;
            }

            if (isPressed)
            {
                if (m_repeatKey && m_repeatKeyTriggerTime >= 0 &&  m_downDelta > m_repeatKeyTriggerTime)
                {
                    if (m_keyRepeatDelta > m_repeatKeyDelay)
                    {
                        onClick.Invoke();
                        m_keyRepeatDelta = 0;
                    }
                    else
                    {
                        m_keyRepeatDelta += Time.deltaTime;
                    }
                }

                m_downDelta += Time.deltaTime;
            }
        }

        public void setSize( float width, float height, float depth )
        {
            m_collider.size = new Vector3(width, height, depth);
            m_background.setSize(width, height);
        }

        private ColorTransition getTransitionFromCurrenTo(ColorTransitionState targetState )
        {
            Color startColor;
            if (m_Transition != null)
                startColor = m_Transition.evaluate(0);
            else
                startColor = m_currentState.destinationColor;

            return new ColorTransition(startColor, targetState);
        }

        public override void OnPointerEnter(PointerEvent ev)
        {
            isHighlighted = true;
            m_Transition = getTransitionFromCurrenTo(m_ColorStates.highlighted);
        }

        public override void OnPointerExit(PointerEvent ev)
        {
            isHighlighted = false;
            m_Transition = getTransitionFromCurrenTo(m_ColorStates.normal);
            m_currentState = isPressed ? m_ColorStates.pressed : m_ColorStates.normal;
        }

        public override Vector3 onPointerMove(PointerEvent ev)
        {

            return ev.position;
        }

        public override void onPointerDown(PointerEvent ev)
        {
            m_downDelta = 0;
            m_keyRepeatDelta = 0;
            isPressed = true;
            onClick.Invoke();
            m_Transition = getTransitionFromCurrenTo(m_ColorStates.pressed);
            m_currentState = m_ColorStates.pressed;
        }

        public override void onPointerUp(PointerEvent ev)
        {
            isPressed = false;
            m_Transition = getTransitionFromCurrenTo( isHighlighted ? m_ColorStates.highlighted : m_ColorStates.normal);
            m_currentState = isHighlighted ? m_ColorStates.highlighted : m_ColorStates.normal;
        }
    }
}
