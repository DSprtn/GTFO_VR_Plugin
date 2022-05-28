using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using static UnityEngine.UI.Button;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{
    public struct ColorTransitionState
    {
        public Color destinationColor;
        public float transitionTime;
    }

    public class ColorTransition
    {
        public Color From;
        public ColorTransitionState To;

        bool m_transitionFinished;
        float m_elapsedTime = 0;

        public ColorTransition(Color from, ColorTransitionState to)
        {
            this.From = from;
            this.To = to;
        }

        public Color evaluate( float deltaTime )    // Pass 0 to just get current
        {
            m_elapsedTime += deltaTime;

            float transitionRatio = To.transitionTime <= 0 ? 1 : m_elapsedTime / To.transitionTime;
            if (transitionRatio > 1)
            {
                transitionRatio = 1;
                m_transitionFinished = true;
            }

            return (From * (1f - transitionRatio) + (To.destinationColor * transitionRatio));
        }

        public bool isFinished()
        {
            return m_transitionFinished;
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
        public PhysicalButton(IntPtr value) : base(value) { }

        private BoxCollider m_collider;
        private RectTransform m_rectTrans;
        private RoundedCubeBackground m_background;
        private MeshRenderer m_renderer;

        private MaterialPropertyBlock m_propertyBlock;
        public ButtonClickedEvent OnClick = new ButtonClickedEvent();

        private ColorTransitionState m_currentState;
        private ColorStates m_ColorStates;
        private ColorTransition m_Transition = null;    // Null when not in a transition.

        public bool IsHighlighted = false;
        public bool IsPressed = false;

        public bool RepeatKey = false;
        public float RepeatKeyTriggerTime = 0.5f;
        public float RepeatKeyDelay = 0.01f;

        private float m_downDelta = 0;
        private float m_keyRepeatDelta = 0;

        private void OnRectTransformDimensionsChange()
        {
            m_collider.size = new Vector3(m_rectTrans.sizeDelta.x, m_rectTrans.sizeDelta.y, 0.01f);
        }

        private void Awake()
        {
            m_collider = this.gameObject.AddComponent<BoxCollider>();
            m_rectTrans = this.gameObject.AddComponent<RectTransform>();
            m_background = this.gameObject.AddComponent<RoundedCubeBackground>();
            m_background.AutoSize = true;
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

            if (IsPressed)
            {
                if (RepeatKey && RepeatKeyTriggerTime >= 0 &&  m_downDelta > RepeatKeyTriggerTime)
                {
                    if (m_keyRepeatDelta > RepeatKeyDelay)
                    {
                        OnClick.Invoke();
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

        public void setBackgroundEnabled(bool enable)
        {
            m_background.enabled = enable;
        }

        public RoundedCubeBackground getBackground()
        {
            return m_background;
        }

        [HideFromIl2Cpp]
        private ColorTransition getTransitionFromCurrenTo(ColorTransitionState targetState )
        {
            Color startColor;
            if (m_Transition != null)
                startColor = m_Transition.evaluate(0);
            else
                startColor = m_currentState.destinationColor;

            return new ColorTransition(startColor, targetState);
        }

        [HideFromIl2Cpp]
        public override void OnPointerEnter(PointerEvent ev)
        {
            IsHighlighted = true;
            m_Transition = getTransitionFromCurrenTo(m_ColorStates.highlighted);
        }

        [HideFromIl2Cpp]
        public override void OnPointerExit(PointerEvent ev)
        {
            IsHighlighted = false;
            if (IsPressed)
            {
                m_currentState = m_ColorStates.pressed;
            }
            else
            {
                m_currentState = m_ColorStates.normal;
                m_Transition = getTransitionFromCurrenTo(m_ColorStates.normal);
            }
        }

        [HideFromIl2Cpp]
        public override Vector3 onPointerMove(PointerEvent ev)
        {
            return ev.Position;
        }

        [HideFromIl2Cpp]
        public override void onPointerDown(PointerEvent ev)
        {
            m_downDelta = 0;
            m_keyRepeatDelta = 0;
            IsPressed = true;
            OnClick.Invoke();
            m_Transition = getTransitionFromCurrenTo(m_ColorStates.pressed);
            m_currentState = m_ColorStates.pressed;
        }

        [HideFromIl2Cpp]
        public override void onPointerUp(PointerEvent ev)
        {
            IsPressed = false;
            m_Transition = getTransitionFromCurrenTo( IsHighlighted ? m_ColorStates.highlighted : m_ColorStates.normal);
            m_currentState = IsHighlighted ? m_ColorStates.highlighted : m_ColorStates.normal;
        }
    }
}
