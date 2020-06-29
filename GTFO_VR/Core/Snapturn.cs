using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core
{
    public class Snapturn : MonoBehaviour
    {
        public Quaternion snapTurnRotation = Quaternion.identity;

        float snapTurnTime = 0f;

        float snapTurnDelay = 0.25f;

        public static Vector3 offsetFromPlayerToHMD = Vector3.zero;

        public static event Action OnSnapTurn;
        public static event Action OnAfterSnapTurn;

        void Awake()
        {
            Debug.Log("Snapturn init");
        }
        public void DoSnapTurn(float angle)
        {
            if (VR_Settings.useSmoothTurn)
            {
                snapTurnRotation *= Quaternion.Euler(new Vector3(0, angle * Time.deltaTime * 2f, 0f));
            }
            else
            {
                if (snapTurnTime + snapTurnDelay < Time.time)
                {
                    SnapTurnFade(1f);
                    snapTurnRotation *= Quaternion.Euler(new Vector3(0, angle, 0f));
                    snapTurnTime = Time.time;
                    
                    OnSnapTurn?.Invoke();
                    // Player origin updates in OnSnapTurn
                    OnAfterSnapTurn?.Invoke();
                }
            }
        }

        private static void SnapTurnFade(float mult)
        {
            SteamVR_Fade.Start(Color.black, 0f, true);
            SteamVR_Fade.Start(Color.clear, 0.2f * mult, true);
        }

        public void DoSnapTurnTowards(Vector3 rotation, float snapTurnFadeMult)
        {
            snapTurnRotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
            SnapTurnFade(snapTurnFadeMult);
        }

    }
}
