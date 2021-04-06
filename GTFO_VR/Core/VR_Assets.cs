

using System;
using UnityEngine;

namespace GTFO_VR.Core
{
    public class VR_Assets : MonoBehaviour
    {

        public VR_Assets(IntPtr value)
        : base(value) { }


        public static GameObject watchPrefab;

        public static Shader spriteAlwaysRender;

        public static Shader textAlwaysRender;

        public static Shader textSphereClip;

        public static Shader spriteSphereClip;


        void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            if (assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            watchPrefab = assetBundle.LoadAsset("assets/p_vrwatch.prefab").Cast<GameObject>();
            spriteAlwaysRender = assetBundle.LoadAsset("assets/spritenoztest.shader").Cast<Shader>();
            textSphereClip = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_clipsphere.shader").Cast<Shader>();
            spriteSphereClip = assetBundle.LoadAsset("assets/spritenoztestandclip.shader").Cast<Shader>();
            textAlwaysRender = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_noztest.shader").Cast<Shader>();
            //fade = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_fade.shader");
            if (!spriteAlwaysRender)
            {
                Debug.LogError("Could not find sprite shader!");
            }

            if(!textAlwaysRender)
            {
                Debug.LogError("Could not find text noclip shader!");
            }
            
        }

    }
}
