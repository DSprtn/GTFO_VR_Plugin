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
        public Snapturn(IntPtr value): base(value) { }

        PlayerOrigin m_origin;

        public static event Action OnSnapTurn;

        private float m_lastSnapTurnTime = 0f;

        private float m_snapTurnDelay = 0.25f;

        private void Awake()
        {
            Log.Info("Snapturn init");
        }

        public void Setup(PlayerOrigin origin)
        {
            m_origin = origin;
        }

        public void DoSnapTurn(float angle)
        {
            if (VRSettings.useSmoothTurn)
            {
                m_origin.RotatePlayer(Quaternion.Euler(new Vector3(0, angle * Time.deltaTime * 2f, 0f)));
            }
            else
            {
                if (m_lastSnapTurnTime + m_snapTurnDelay < Time.time)
                {
                    SnapTurnFade(.2f);
                    m_origin.RotatePlayer(Quaternion.Euler(new Vector3(0, angle, 0f)));
                    m_lastSnapTurnTime = Time.time;
                    OnSnapTurn?.Invoke();
                }
            }
        }

        private static void SnapTurnFade(float duration)
        {
            SteamVR_Fade.Start(Color.black, 0f, true);
            SteamVR_Fade.Start(Color.clear, duration, true);
        }

        public void DoSnapTurnTowards(Vector3 rotation, float fadeDuration)
        {
            Vector3 deltaRot = Quaternion.LookRotation(rotation).eulerAngles;
            deltaRot.x = 0;
            deltaRot.z = 0;
            deltaRot.y -= HMD.GetVRCameraEulerRelativeToFPSCameraParent().y;
            m_origin.RotatePlayer(Quaternion.Euler(deltaRot));
            SnapTurnFade(fadeDuration);
            OnSnapTurn.Invoke();
        }
    }
}