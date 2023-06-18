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
        static AssetBundle secondaryAssetBundle;

        public static GameObject WatchPrefab;

        public static Shader SpriteAlwaysRender;

        public static Shader TextAlwaysRender;

        public static Shader TextSphereClip;

        public static Shader SpriteSphereClip;

        public static Shader ThermalGlowShader;

        public static Sprite RadialBG;
        public static Sprite Objective;
        public static Sprite Chat;
        public static Sprite ChatType;
        public static Sprite MeleeFallback;
        public static Sprite PrimaryFallback;
        public static Sprite SecondaryFallback;
        public static Sprite ToolFallback;
        public static Sprite ThrowableFallback;
        public static Sprite PackFallback;
        public static Sprite HackingToolFallback;
        public static Sprite holoBG;



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

        public static Shader GetTextNoCull()
        {
            if (assetBundle == null)
            {
                assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            }

            if(TextAlwaysRender == null)
            {
                TextAlwaysRender = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_noztest.shader").Cast<Shader>();
            }
            return TextAlwaysRender;
        }

        public static Shader GetThermalGlowShader()
        {
            if (ThermalGlowShader == null)
            {
                if (secondaryAssetBundle == null)
                {
                    secondaryAssetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/gtfovr_thermal_glow");
                }

                ThermalGlowShader = secondaryAssetBundle.LoadAsset("assets/GTFOVR_Thermal_Glow.shader").Cast<Shader>();
            }

            return ThermalGlowShader;
        }

        public Sprite CreateSpriteFromTexture2D(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f,0.5f), 100f);
        }

        private void Awake()
        {
            if(assetBundle == null)
            {
                assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            }
            if (assetBundle == null)
            {
                Log.Error("No assetbundle present!");
            }

            if (secondaryAssetBundle == null)
            {
                secondaryAssetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrthermal");
            }
            if (secondaryAssetBundle == null)
            {
                Log.Error("No secondaryAssetBundle present!");
            }

            RadialBG = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/bg.png").Cast<Texture2D>());
            Objective = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/objective.png").Cast<Texture2D>());
            Chat = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/chat.png").Cast<Texture2D>());
            ChatType = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/chattype.png").Cast<Texture2D>());
            MeleeFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/melee.png").Cast<Texture2D>());
            HackingToolFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/hackingtool.png").Cast<Texture2D>());
            PackFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/packfallback.png").Cast<Texture2D>());
            ToolFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/tool.png").Cast<Texture2D>());
            SecondaryFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/secondary.png").Cast<Texture2D>());
            PrimaryFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/primary.png").Cast<Texture2D>());
            ThrowableFallback = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/radialicons/throwable.png").Cast<Texture2D>());

            holoBG = CreateSpriteFromTexture2D(assetBundle.LoadAsset("assets/blob.png").Cast<Texture2D>());

            WatchPrefab = assetBundle.LoadAsset("assets/p_vrwatch.prefab").Cast<GameObject>();
            SpriteAlwaysRender = assetBundle.LoadAsset("assets/spritenoztest.shader").Cast<Shader>();
            TextSphereClip = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_clipsphere.shader").Cast<Shader>();
            SpriteSphereClip = assetBundle.LoadAsset("assets/spritenoztestandclip.shader").Cast<Shader>();

            ThermalGlowShader = secondaryAssetBundle.LoadAsset("assets/GTFOVR_Thermal_Glow.shader").Cast<Shader>();

            if (!ThermalGlowShader)
            {
                Log.Error("Could not find thermal glow shader!");
            }

            if (TextAlwaysRender == null)
            {
                TextAlwaysRender = assetBundle.LoadAsset("assets/textmesh pro/resources/shaders/tmp_noztest.shader").Cast<Shader>();
            }

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