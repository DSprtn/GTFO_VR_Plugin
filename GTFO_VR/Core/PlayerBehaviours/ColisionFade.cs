using GTFO_VR.Core.VR_Input;
using Player;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Fades player's screen if he is within collision or trying to walk behind doors/walls in roomspace
    /// </summary>
    public class CollisionFade : MonoBehaviour
    {
        public CollisionFade(IntPtr value) : base(value)
        {
        }

        private bool m_fpsCameraInCollision;
        private bool m_playerAgentToHMDPositionBlocked;
        private bool m_wasFadedLastFrame;

        private LocalPlayerAgent m_agent;

        public void Setup(LocalPlayerAgent agent)
        {
            m_agent = agent;
        }

        private void FixedUpdate()
        {
            HandleCameraInCollision();
        }

        /// <summary>
        /// If the player head is colliding with something or if an object like a door is between the camera and the true player agent position then
        /// we fade the screen. No cheating!
        /// </summary>
        public void HandleCameraInCollision()
        {
            m_fpsCameraInCollision = Physics.OverlapBox(HMD.GetWorldPosition() + HMD.GetWorldForward() * 0.05f, new Vector3(0.03f, 0.03f, 0.03f), HMD.Hmd.transform.rotation, LayerManager.MASK_TENTACLE_BLOCKERS).Length > 0;

            Vector3 centerPlayerHeadPos = VRPlayer.FpsCamera.GetCamerposForPlayerPos(m_agent.PlayerCharacterController.SmoothPosition);
            m_playerAgentToHMDPositionBlocked = Physics.Linecast(centerPlayerHeadPos, HMD.GetWorldPosition(), LayerManager.MASK_TENTACLE_BLOCKERS);

            if (m_playerAgentToHMDPositionBlocked || m_fpsCameraInCollision)
            {
                m_wasFadedLastFrame = true;
                SteamVR_Fade.Start(Color.black, 0.2f, true);
                return;
            }
            else if (m_wasFadedLastFrame)
            {
                SteamVR_Fade.Start(Color.clear, 0.2f, true);
                m_wasFadedLastFrame = false;
            }
        }
    }
}