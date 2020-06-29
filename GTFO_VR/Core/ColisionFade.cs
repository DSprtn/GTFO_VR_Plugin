using GTFO_VR.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Fades player's screen if he is within collision or trying to walk behind doors/walls in roomspace
    /// </summary>
    public class ColisionFade : MonoBehaviour
    {

        static bool headInCollision;
        static bool controllerHeadToHMDHeadBlocked;

        bool wasFadedLastFrame;

        public void HandleCameraInCollision()
        {
            if (Physics.OverlapBox(HMD.GetWorldPosition() + (HMD.GetWorldForward() * 0.05f), new Vector3(0.03f, 0.03f, 0.03f), HMD.hmd.transform.rotation, LayerManager.MASK_TENTACLE_BLOCKERS).Length > 0)
            {
                headInCollision = true;
            }
            else
            {
                headInCollision = false;
            }

            Vector3 centerPlayerHeadPos = PlayerVR.fpsCamera.GetCamerposForPlayerPos(PlayerVR.playerController.SmoothPosition);

            if (Physics.Linecast(centerPlayerHeadPos, HMD.GetWorldPosition(), LayerManager.MASK_TENTACLE_BLOCKERS))
            {
                controllerHeadToHMDHeadBlocked = true;
            }
            else
            {
                controllerHeadToHMDHeadBlocked = false;
            }

            if (controllerHeadToHMDHeadBlocked || headInCollision)
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
