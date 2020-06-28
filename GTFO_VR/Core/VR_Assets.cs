using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core
{
    public class VR_Assets : MonoBehaviour
    {
        public static GameObject watchPrefab;

        public static Shader spriteAlwaysRender;

        public static Shader textAlwaysRender;

        public static Shader textCull;

        public static Shader spriteClip;


        void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            if (assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            watchPrefab = assetBundle.LoadAsset<GameObject>("assets/p_vrwatch.prefab");
            spriteAlwaysRender = assetBundle.LoadAsset<Shader>("assets/spritenoztest.shader");
            textCull = assetBundle.LoadAsset<Shader>("assets/textmesh pro/resources/shaders/tmp_clipsphere.shader");
            spriteClip = assetBundle.LoadAsset<Shader>("assets/spritenoztestandclip.shader");
            //fade = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_fade.shader");
            if (!spriteAlwaysRender)
            {
                Debug.LogError("Could not find sprite shader!");
            }
            textAlwaysRender = Shader.Find("TextMeshPro/Distance Field Overlay");
            if(!textAlwaysRender)
            {
                Debug.LogError("Could not find text overlay shader!");
            }
            
        }

    }
}
