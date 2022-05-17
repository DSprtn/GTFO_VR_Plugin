﻿using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.UI;
using Player;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
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
        private BhapticsIntegration m_bhapticsIntegration;
        private MovementVignette m_movementVignette;
        private WeaponRadialMenu m_weaponRadial;
        private WeaponAmmoHologram m_weaponAmmoHolo;

        public static LocalPlayerAgent PlayerAgent;
        public static FPSCamera FpsCamera;

        public void Setup(FPSCamera camera, LocalPlayerAgent agent)
        {
            FpsCamera = camera;
            PlayerAgent = agent;

            m_origin = new GameObject("Origin").AddComponent<PlayerOrigin>();
            m_origin.Setup(PlayerAgent);
            m_snapTurn = gameObject.AddComponent<Snapturn>();
            m_snapTurn.Setup(m_origin);

            gameObject.AddComponent<VRWorldSpaceUI>();

            m_movementVignette = gameObject.AddComponent<MovementVignette>();
            m_movementVignette.Setup(agent.Locomotion, camera);

            m_weaponRadial = gameObject.AddComponent<WeaponRadialMenu>();
            m_weaponRadial.Setup(m_origin.transform);

            m_weaponAmmoHolo = gameObject.AddComponent<WeaponAmmoHologram>();
            m_weaponAmmoHolo.Setup();

            GameObject laserPointer = new GameObject("LaserPointer");
            m_pointer = laserPointer.AddComponent<LaserPointer>();


            m_fade = FpsCamera.gameObject.AddComponent<CollisionFade>();
            m_fade.Setup(PlayerAgent);

            FpsCamera.gameObject.AddComponent<SteamVR_Camera>();
            FpsCamera.gameObject.AddComponent<VRRendering>();
            FpsCamera.gameObject.AddComponent<SteamVR_Fade>();

            FpsCamera.UpdateUI = false;

            m_watch = Instantiate(VRAssets.GetWatchPrefab(), Vector3.zero, Quaternion.identity, null).AddComponent<Watch>();
            m_watch.Setup(m_origin.transform);

            m_haptics = gameObject.AddComponent<Haptics>();
            m_haptics.Setup();

            m_bhapticsIntegration = gameObject.AddComponent<BhapticsIntegration>();
            m_bhapticsIntegration.Setup(agent);

            PlayerLocomotionEvents.OnPlayerEnterLadder += PlayerEnteredLadder;
            SteamVR_Events.NewPosesApplied.Listen(new Action(OnNewPoses));

            RefreshClusteredRenderingResolution();
        }

        private void LightResChanged(object sender, EventArgs e)
        {
            RefreshClusteredRenderingResolution();
        }

        private static void RefreshClusteredRenderingResolution()
        {
            ClusteredRendering.Current.OnResolutionChange(new Resolution());
        }

        public void OnNewPoses()
        {
            if (!FpsCamera || !m_origin)
            {
                return;
            }
            m_origin.UpdateOrigin();
            UpdateVRCameraTransform(FpsCamera);
            UpdateHeldItemTransform();
            m_weaponAmmoHolo.UpdateTransform();
            //UpdateHandIK();
        }

        // ToDO - Tweak hand IK so it actually works
        private void UpdateHandIK()
        {
            if(PlayerAgent.FPItemHolder == null || PlayerAgent.FPItemHolder.WieldedItem == null)
            {
                return;
            }
            if(!VRConfig.configUseLeftHand.Value)
            {
                PlayerAgent.FPItemHolder.FPSArms.SetRightArmTargetPosRot(PlayerAgent.FPItemHolder.WieldedItem.RightHandGripTrans);
                PlayerAgent.FPItemHolder.FPSArms.SetLeftArmTargetPosRot(Controllers.offhandController.transform);
            } else
            {
                PlayerAgent.FPItemHolder.FPSArms.SetLeftArmTargetPosRot(PlayerAgent.FPItemHolder.WieldedItem.RightHandGripTrans);
                PlayerAgent.FPItemHolder.FPSArms.SetRightArmTargetPosRot(Controllers.offhandController.transform);
            }

        }

        public static void UpdateVRCameraTransform(FPSCamera fpsCamera)
        {
            if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                fpsCamera.transform.position = HMD.GetWorldPosition();
            }
            
            fpsCamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRelativeToFPSCameraParent());
            fpsCamera.UpdateCameraRay();
        }

        public static void UpdateHeldItemTransform()
        {
            if (!VRConfig.configUseControllers.Value || !PlayerAgent)
            {
                return;
            }

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

        public static LevelGeneration.LG_ComputerTerminal getInteractingTerminal()
        {
            // There are a number of interaction references, including "m_currentInteractionObj", but they're all null.
            // camera ray probably depends on where you're looking, so proximity it is.
            Il2CppSystem.Collections.Generic.List<IInteractable> interactables = PlayerAgent?.Interaction?.m_proximityInteracts;

            if (interactables == null)
            {
                return null;
            }

            Interact_ComputerTerminal terminalInteract = null;
            foreach( IInteractable interactable in interactables)
            {

                terminalInteract = interactable.TryCast<Interact_ComputerTerminal>();
                if (terminalInteract != null)
                    break;
            }

            if (terminalInteract == null)
                return null;

            return terminalInteract.m_terminal;
        }

        public static void hideWielded(bool hide)
        {
            if (!VRConfig.configUseControllers.Value || !PlayerAgent)
            {
                return;
            }

            ItemEquippable heldItem = PlayerAgent.FPItemHolder.WieldedItem;
            if (heldItem != null)
            {
                if (hide)
                {
                    heldItem.gameObject.SetActive(false);
                }
                else
                {
                    heldItem.gameObject.SetActive(true);
                }
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