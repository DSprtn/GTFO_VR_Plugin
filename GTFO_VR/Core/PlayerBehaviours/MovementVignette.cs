using GTFO_VR.Events;
using Player;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using GTFO_VR.Util;
using Valve.VR.InteractionSystem;
using UnityEngine.Rendering.PostProcessing;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class MovementVignette : MonoBehaviour
    {
        public MovementVignette(IntPtr value)
: base(value) { }

        private FPSCamera m_fpsCamera;

        private PlayerLocomotion m_playerLocomotion;

        private Vignette m_vignettePost;

        private float m_targetIntensity = 0.25f;

        float m_currentIntensity = 0f;

        bool m_setup = false;

        private void Update()
        {
            if(m_vignettePost == null)
            {
                if(m_fpsCamera.m_postProcessing != null)
                {
                    m_vignettePost = m_fpsCamera.m_postProcessing.m_vignette;
                }
                return;
            }

            if (!m_setup)
            {
                m_vignettePost.color.Override(Color.black);
                m_vignettePost.mode.Override(VignetteMode.Classic);
                m_vignettePost.opacity.Override(1f);
                m_vignettePost.rounded.Override(true);
                m_vignettePost.roundness.Override(1f);
                m_setup = true;
            }

            bool shouldBeActive = VRConfig.configPostVignette.Value || VRConfig.configUseVignetteWhenMoving.Value;
            m_vignettePost.enabled.Override(shouldBeActive);

            m_targetIntensity = GetVignetteIntensityForVelocityPerGamestate();

            // Lerp to vignette intensity. Ramp up faster and ramp down slower.
            float lerpSpeed = m_vignettePost.intensity < m_targetIntensity ? 20f : 8f;
            m_currentIntensity = Mathf.Lerp(m_currentIntensity, m_targetIntensity, lerpSpeed * Time.deltaTime);

            m_vignettePost.intensity.Override(m_currentIntensity);
        }

        private float GetVignetteIntensityForVelocityPerGamestate()
        {
            float intensity = .25f;

            if (FocusStateEvents.currentState == eFocusState.InElevator || !VRConfig.configUseVignetteWhenMoving.Value)
            {
                return VRConfig.configMovementVignetteIntensity.Value / 2f; ;
            }

            if (FocusStateEvents.currentState == eFocusState.FPS)
            {
                intensity = Mathf.Clamp(m_playerLocomotion.HorizontalVelocity.magnitude, 0.25f, 2f);
                intensity = intensity.RemapClamped(0, 3f, 0, VRConfig.configMovementVignetteIntensity.Value);
            }

            return intensity;
        }

        public void Setup(PlayerLocomotion agentLocomotion, FPSCamera cam)
        {
            m_playerLocomotion = agentLocomotion;
            m_fpsCamera = cam;
        }
    }
}