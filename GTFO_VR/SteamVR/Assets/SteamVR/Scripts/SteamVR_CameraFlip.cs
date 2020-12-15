using Assets.SteamVR_Standalone.Standalone;
using System;
using UnityEngine;

namespace Valve.VR
{

    public class SteamVR_CameraFlip : MonoBehaviour
    {

        private void OnEnable()
        {
            if (SteamVR_CameraFlip.blitMaterial == null)
            {
                SteamVR_CameraFlip.blitMaterial = new Material(VRShaders.GetShader(VRShaders.VRShader.blitFlip));
            }
        }


        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, SteamVR_CameraFlip.blitMaterial);
        }


        private static Material blitMaterial;
    }
}
