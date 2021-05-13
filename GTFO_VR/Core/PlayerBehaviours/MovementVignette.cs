using GTFO_VR.Events;
using Player;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using GTFO_VR.Util;
using Valve.VR.InteractionSystem;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class MovementVignette : MonoBehaviour
    {
        public MovementVignette(IntPtr value)
: base(value) { }

        public static PostProcessingBehaviour postProcessing;

        private PlayerLocomotion m_playerLocomotion;

        private float targetIntensity = 0.25f;

        private void Update()
        {
            VignetteModel vignettePost = postProcessing.m_Vignette.model;
            if (vignettePost == null || !VRConfig.configUseVignetteWhenMoving.Value)
            {
                return;
            }

            targetIntensity = GetVignetteIntensityForVelocityPerGamestate();

            VignetteModel.Settings newSettings = vignettePost.settings;


            // Lerp to vignette intensity. Ramp up faster and ramp down slower.
            if(newSettings.intensity <= targetIntensity)
            {
                newSettings.intensity = Mathf.Lerp(vignettePost.settings.intensity, targetIntensity, 20f * Time.deltaTime);
            } else
            {
                newSettings.intensity = Mathf.Lerp(vignettePost.settings.intensity, targetIntensity, 8f * Time.deltaTime);
            }
            
            vignettePost.settings = newSettings;
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

        public void Setup(PlayerLocomotion agentLocomotion, PostProcessingBehaviour postprocess)
        {
            m_playerLocomotion = agentLocomotion;
            postProcessing = postprocess;
        }
    }
}