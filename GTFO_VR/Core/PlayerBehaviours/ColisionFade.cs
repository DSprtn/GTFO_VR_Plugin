using GTFO_VR.Core.VR_Input;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Fades player's screen if he is within collision or trying to walk behind doors/walls in roomspace
    /// </summary>
    public static class CollisionFade
    {

        static bool fpsCameraInCollision;
        static bool playerAgentToHMDPositionBlocked;

        static bool wasFadedLastFrame;

        /// <summary>
        /// Check for collision between the player's ture position and VR camera position, and in a box at the player's VR camera position
        /// </summary>
        public static void HandleCameraInCollision()
        {
            fpsCameraInCollision = Physics.OverlapBox(HMD.GetWorldPosition() + HMD.GetWorldForward() * 0.05f, new Vector3(0.03f, 0.03f, 0.03f), HMD.hmd.transform.rotation, LayerManager.MASK_TENTACLE_BLOCKERS).Length > 0;

            Vector3 centerPlayerHeadPos = PlayerVR.fpsCamera.GetCamerposForPlayerPos(PlayerVR.playerController.SmoothPosition);

            if (Physics.Linecast(centerPlayerHeadPos, HMD.GetWorldPosition(), LayerManager.MASK_TENTACLE_BLOCKERS))
            {
                playerAgentToHMDPositionBlocked = true;
            }
            else
            {
                playerAgentToHMDPositionBlocked = false;
            }

            if (playerAgentToHMDPositionBlocked || fpsCameraInCollision)
            {
                wasFadedLastFrame = true;
                SteamVR_Fade.Start(Color.black, 0.2f, true);
                return;
            }
            else if (wasFadedLastFrame)
            {
                SteamVR_Fade.Start(Color.clear, 0.2f, true);
                wasFadedLastFrame = false;
            }
        }
    }
}
