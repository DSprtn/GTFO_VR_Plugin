using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SteamVR_Standalone.Standalone
{
    public static class VRShaders
    {
        public enum VRShader
        {
            blit,
            blitFlip,
            overlay,
            occlusion,
            fade
        }

        static Shader blit;
        static Shader blitFlip;
        static Shader overlay;
        static Shader occlusion;
        static Shader fade;

        public static Shader GetShader(VRShader shader)
        {
            if(blit == null)
            {
                TryLoadShaders();
            }

            switch(shader)
            {
                case (VRShader.blit):
                    return blit;
                case (VRShader.blitFlip):
                    return blitFlip;
                case (VRShader.overlay):
                    return overlay;
                case (VRShader.occlusion):
                    return occlusion;
                case (VRShader.fade):
                    return fade;
            }
            Debug.LogWarning("No valid shader found");
            return null;
        }

        public static void TryLoadShaders()
        {
            Debug.Log("Loading shaders from asset bundle...");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrshaders");
            if(assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            Debug.Log(assetBundle.name);
            occlusion = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_hiddenarea.shader");
            blit = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_blit.shader");
            blitFlip = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_blitFlip.shader");
            overlay = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_overlay.shader");
            fade = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_fade.shader");
            string[] allAssetNames = assetBundle.GetAllAssetNames();
            for (int i = 0; i < allAssetNames.Length; i++)
            {
                Debug.Log(allAssetNames[i]);
            }
        }
    }
}
