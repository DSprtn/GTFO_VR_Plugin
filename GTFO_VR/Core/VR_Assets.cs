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

        public static Shader textSphereClip;

        public static Shader spriteSphereClip;


        void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            if (assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            watchPrefab = assetBundle.LoadAsset<GameObject>("assets/p_vrwatch.prefab");
            spriteAlwaysRender = assetBundle.LoadAsset<Shader>("assets/spritenoztest.shader");
            textSphereClip = assetBundle.LoadAsset<Shader>("assets/textmesh pro/resources/shaders/tmp_clipsphere.shader");
            spriteSphereClip = assetBundle.LoadAsset<Shader>("assets/spritenoztestandclip.shader");
            textAlwaysRender = assetBundle.LoadAsset<Shader>("assets/textmesh pro/resources/shaders/tmp_noztest.shader");
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
