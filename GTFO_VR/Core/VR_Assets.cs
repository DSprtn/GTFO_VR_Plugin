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

        void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrwatch");
            if (assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            watchPrefab = assetBundle.LoadAsset<GameObject>("assets/p_vrwatch.prefab");
        }

    }
}
