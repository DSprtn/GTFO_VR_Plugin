using GTFO_VR.Core.VR_Input;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Responsible for the snapturn functionality.
    /// </summary>
    public class Snapturn : MonoBehaviour
    {

        public Snapturn(IntPtr value)
: base(value) { }


        public Quaternion snapTurnRotation = Quaternion.identity;

        float snapTurnTime = 0f;

        float snapTurnDelay = 0.25f;

        public static Vector3 offsetFromPlayerToHMD = Vector3.zero;

        public static event Action OnSnapTurn;
        public static event Action OnAfterSnapTurn;

        void Awake()
        {
            GTFO_VR_Plugin.log.LogInfo("Snapturn init");
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
            Vector3 deltaRot = Quaternion.LookRotation(rotation).eulerAngles;
            deltaRot.x = 0;
            deltaRot.z = 0;
            deltaRot.y -= HMD.GetFPSCameraRelativeVRCameraEuler().y;

            snapTurnRotation *= Quaternion.Euler(deltaRot);
            SnapTurnFade(snapTurnFadeMult);
        }

    }
}
