using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.UI;
using Player;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Serves as the in-game VR player, sets up all relevant player systems and handles general VR player-related methods
    /// </summary>

    public class VRPlayer : MonoBehaviour
    {
        public VRPlayer(IntPtr value) : base(value)
        {
        }

        private PlayerOrigin m_origin;
        private Snapturn m_snapTurn;
        private Watch m_watch;
        private LaserPointer m_pointer;
        private CollisionFade m_fade;
        private Haptics m_haptics;

        public static PlayerAgent PlayerAgent;
        public static FPSCamera FpsCamera;

        public void Setup(FPSCamera camera, PlayerAgent agent)
        {
            FpsCamera = camera;
            PlayerAgent = agent;

            m_origin = new GameObject("Origin").AddComponent<PlayerOrigin>();
            m_origin.Setup(PlayerAgent);
            m_snapTurn = gameObject.AddComponent<Snapturn>();
            m_snapTurn.Setup(m_origin);

            gameObject.AddComponent<VRWorldSpaceUI>();

            if (VRSettings.useLaserPointer)
            {
                GameObject laserPointer = new GameObject("LaserPointer");
                m_pointer = laserPointer.AddComponent<LaserPointer>();
            }

            m_fade = FpsCamera.gameObject.AddComponent<CollisionFade>();
            m_fade.Setup(PlayerAgent);

            FpsCamera.gameObject.AddComponent<SteamVR_Camera>();
            FpsCamera.gameObject.AddComponent<VRRendering>();
            FpsCamera.gameObject.AddComponent<SteamVR_Fade>();

            m_watch = Instantiate(VRAssets.GetWatchPrefab(), Vector3.zero, Quaternion.identity, null).AddComponent<Watch>();
            Vector3 watchScale = new Vector3(1.25f, 1.25f, 1.25f);
            watchScale *= VRSettings.watchScale;
            m_watch.transform.localScale = watchScale;

            m_haptics = gameObject.AddComponent<Haptics>();
            m_haptics.Setup();

            PlayerLocomotionEvents.OnPlayerEnterLadder += PlayerEnteredLadder;
            SteamVR_Events.NewPosesApplied.Listen(new Action(OnNewPoses));

            ClusteredRendering.Current.OnResolutionChange(new Resolution());
        }

        private void Update()
        {
            HandleSnapturnInput();
        }

        private void OnNewPoses()
        {
            if (!FpsCamera || !m_origin)
            {
                return;
            }

            m_origin.UpdateOrigin();
            UpdateVRCameraTransform(FpsCamera);
            UpdateHeldItemTransform();
        }

        private void HandleSnapturnInput()
        {
            if (SteamVR_InputHandler.GetSnapTurningLeft())
            {
                m_snapTurn.DoSnapTurn(-VRSettings.snapTurnAmount);
            }

            if (SteamVR_InputHandler.GetSnapTurningRight())
            {
                m_snapTurn.DoSnapTurn(VRSettings.snapTurnAmount);
            }
        }

        public static void UpdateVRCameraTransform(FPSCamera fpsCamera)
        {
            if (VRSettings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation))
            {
                if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
                {
                    fpsCamera.transform.position = HMD.GetWorldPosition();
                }
            }
            fpsCamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRelativeToFPSCameraParent());
            fpsCamera.UpdateCameraRay();
        }

        public static void UpdateHeldItemTransform()
        {
            if (!VRSettings.useVRControllers)
                return;

            ItemEquippable heldItem = PlayerAgent.FPItemHolder.WieldedItem;
            if (heldItem != null)
            {
                heldItem.transform.position = Controllers.GetControllerPosition() + WeaponArchetypeVRData.CalculateGripOffset();
                Vector3 recoilRot = heldItem.GetRecoilRotOffset();

                if (!Controllers.IsFiringFromADS())
                {
                    recoilRot.x *= 2f;
                }
                heldItem.transform.rotation = Controllers.GetControllerAimRotation();
                heldItem.transform.localRotation *= Quaternion.Euler(recoilRot) * WeaponArchetypeVRData.GetVRWeaponData(heldItem).rotationOffset;
                heldItem.transform.position += Controllers.GetControllerAimRotation() * heldItem.GetRecoilPosOffset();
            }
        }

        private void PlayerEnteredLadder(LG_Ladder ladder)
        {
            m_snapTurn.DoSnapTurnTowards(Quaternion.LookRotation(ladder.transform.forward).eulerAngles, 2f);
        }

        private void OnDestroy()
        {
            PlayerLocomotionEvents.OnPlayerEnterLadder -= PlayerEnteredLadder;
            SteamVR_Events.NewPosesApplied.Remove(OnNewPoses);

            if (m_origin)
            {
                Destroy(m_origin.gameObject);
            }
            if (m_pointer)
            {
                Destroy(m_pointer.gameObject);
            }
            if (m_watch)
            {
                Destroy(m_watch.gameObject);
            }
        }
    }
}