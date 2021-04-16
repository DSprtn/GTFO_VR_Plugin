using System;
using UnityEngine;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Responsible for loading VR specific assets, including but not limited to the vr watch and vr shaders.
    /// </summary>
    public class VRAssets : MonoBehaviour
    {
        public VRAssets(IntPtr value) : base(value)
        {
        }

        static AssetBundle assetBundle;

        public static GameObject WatchPrefab;

        public static Shader SpriteAlwaysRender;

        public static Shader TextAlwaysRender;

        public static Shader TextSphereClip;

        public static Shader SpriteSphereClip;


        // GameObjects loaded from bundles can get wiped on load, so we sometimes need to reload them
        public static GameObject GetWatchPrefab()
        {
            if(WatchPrefab)
            {
                return WatchPrefab;
            }
            if(assetBundle != null)
            {
                WatchPrefab = assetBundle.LoadAsset("assets/p_vrwatch.prefab").Cast<GameObject>();
            }
            return WatchPrefab;
        }

        private void Awake()
        {
            assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            if (assetBundle == null)
            {
                Log.Error("No assetbundle present!");
            }
            WatchPrefab = assetBundle.LoadAsset("assets/p_vrwatch.prefab").Cast<GameObject>();
            SpriteAlwaysRender = assetBundle.LoadAsset("assets/spritenoztest.shader").Cast<Shader>();
            TextSphereClip = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_clipsphere.shader").Cast<Shader>();
            SpriteSphereClip = assetBundle.LoadAsset("assets/spritenoztestandclip.shader").Cast<Shader>();
            TextAlwaysRender = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_noztest.shader").Cast<Shader>();

            if (!SpriteAlwaysRender)
            {
                Log.Error("Could not find sprite shader!");
            }

            if (!TextAlwaysRender)
            {
                Log.Error("Could not find text noclip shader!");
            }
        }
    }
}