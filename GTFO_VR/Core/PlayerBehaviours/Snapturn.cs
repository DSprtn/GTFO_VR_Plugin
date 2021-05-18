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

        SteamVR_Action_Vector2 smoothTurn;

        private void Awake()
        {
            Log.Info("Snapturn init");

            smoothTurn = SteamVR_Input.GetVector2Action("SnapTurn");
        }

        public void Setup(PlayerOrigin origin)
        {
            m_origin = origin;
        }

        void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            float smoothTurnX = smoothTurn.GetAxis(SteamVR_Input_Sources.Any).x;
            if (VRConfig.configSmoothSnapTurn.Value)
            {
                if (Mathf.Abs(smoothTurnX) > 0.01f)
                {
                    DoSmoothTurn(VRConfig.configSmoothTurnSpeed.Value * smoothTurnX * Time.deltaTime);
                }
            }
            else
            {
                if (Mathf.Abs(smoothTurnX) > 0.9f)
                {
                    float angle = VRConfig.configSnapTurnAmount.Value;
                    angle *= smoothTurnX > 0 ? 1 : -1;
                    DoSnapTurn(angle);
                }
            }
        }

        public void DoSnapTurn(float angle)
        {
            if (m_lastSnapTurnTime + m_snapTurnDelay < Time.time)
            {
                SnapTurnFade(.2f);
                m_origin.RotatePlayer(Quaternion.Euler(new Vector3(0, angle, 0f)));
                m_lastSnapTurnTime = Time.time;
                OnSnapTurn?.Invoke();
            }
        }

        internal void DoSmoothTurn(float angle)
        {
            m_origin.RotatePlayer(Quaternion.Euler(new Vector3(0, angle, 0f)));
            OnSnapTurn?.Invoke();
        }

        private static void SnapTurnFade(float duration)
        {
            SteamVR_Fade.Start(Color.black, 0f, true);
            SteamVR_Fade.Start(Color.clear, duration, true);
        }

        public void DoSnapTurnTowards(Vector3 direction, float fadeDuration)
        {
            Vector3 deltaRot = Quaternion.LookRotation(direction).eulerAngles;
            deltaRot.x = 0;
            deltaRot.z = 0;
            deltaRot.y -= HMD.GetVRCameraEulerRelativeToFPSCameraParent().y;
            m_origin.RotatePlayer(Quaternion.Euler(deltaRot));
            SnapTurnFade(fadeDuration);
            OnSnapTurn.Invoke();
        }
    }
}