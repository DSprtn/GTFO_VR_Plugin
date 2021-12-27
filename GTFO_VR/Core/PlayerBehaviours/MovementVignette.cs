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

        private Vignette vignettePost;

        private float targetIntensity = 0.25f;

        private void Update()
        {
            if(vignettePost == null)
            {
                if(m_fpsCamera.m_postProcessing != null)
                {
                    vignettePost = m_fpsCamera.m_postProcessing.m_vignette;
                }
                return;
            }
            vignettePost.active = VRConfig.configUseVignetteWhenMoving.Value;

            if (vignettePost == null || !VRConfig.configUseVignetteWhenMoving.Value)
            {
                return;
            }

            targetIntensity = GetVignetteIntensityForVelocityPerGamestate();

            // Lerp to vignette intensity. Ramp up faster and ramp down slower.
            if(vignettePost.intensity <= targetIntensity)
            {
                vignettePost.intensity.Override(Mathf.Lerp(vignettePost.intensity.value, targetIntensity, 20f * Time.deltaTime));
            } else
            {
                vignettePost.intensity.Override(Mathf.Lerp(vignettePost.intensity.value, targetIntensity, 8f * Time.deltaTime));
            }
        }

        private float GetVignetteIntensityForVelocityPerGamestate()
        {
            float intensity = .25f;
            if (FocusStateEvents.currentState == eFocusState.InElevator)
            {
                return VRConfig.configMovementVignetteIntensity.Value;
            }

            if (FocusStateEvents.currentState == eFocusState.FPS)
            {
                intensity = Mathf.Clamp(m_playerLocomotion.HorizontalVelocity.magnitude, 0.25f, 3f);
                intensity = intensity.RemapClamped(0, 2.5f, 0, VRConfig.configMovementVignetteIntensity.Value);
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