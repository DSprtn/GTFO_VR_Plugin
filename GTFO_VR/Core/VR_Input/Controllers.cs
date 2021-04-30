﻿using GTFO_VR.Events;
using System;
using UnityEngine;
using Valve.VR;
using static GTFO_VR.Core.WeaponArchetypeVRData;

namespace GTFO_VR.Core.VR_Input

{
    /// <summary>
    /// Handles all VR controller related actions. Includes double handing weapons, interactions, transforms etc.
    /// </summary>
    public class Controllers : MonoBehaviour
    {
        public Controllers(IntPtr value)
: base(value) { }

        public static HandType mainControllerType = HandType.Right;
        public static HandType offHandControllerType = HandType.Left;

        public static GameObject mainController;

        public static GameObject offhandController;

        private static GameObject LeftController;

        private static GameObject RightController;

        public static SteamVR_Behaviour_Pose mainControllerPose;

        public static bool aimingTwoHanded;

        private float m_doubleHandStartDistance = .08f;

        private float m_doubleHandLeaveDistance = .55f;

        private bool m_wasInDoubleHandPosLastFrame = false;


        private void Awake()
        {
            SetupControllers();
            SetMainController();
            ItemEquippableEvents.OnPlayerWieldItem += CheckShouldDoubleHand;
        }

        private void Update()
        {
            if (!VRSettings.alwaysDoubleHanded && !FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                HandleDoubleHandedChecks();
            }
        }

        private void SetMainController()
        {
            if (VRSettings.mainHand.Equals(HandType.Right))
            {
                mainController = RightController;
                offhandController = LeftController;
                mainControllerType = HandType.Right;
                offHandControllerType = HandType.Left;
            }
            else
            {
                mainController = LeftController;
                offhandController = RightController;
                mainControllerType = HandType.Left;
                offHandControllerType = HandType.Right;
            }
            mainControllerPose = mainController.GetComponent<SteamVR_Behaviour_Pose>();
        }

        private void SetupControllers()
        {
            LeftController = SetupController(SteamVR_Input_Sources.LeftHand);
            RightController = SetupController(SteamVR_Input_Sources.RightHand);
            LeftController.name = "LeftController";
            RightController.name = "RightController";

            DontDestroyOnLoad(RightController);
            DontDestroyOnLoad(LeftController);
        }

        public static void SetOrigin(Transform origin)
        {
            LeftController.transform.SetParent(origin);
            RightController.transform.SetParent(origin);
        }

        public static void OnOriginDestroyed()
        {
            if (LeftController)
            {
                LeftController.transform.SetParent(null);
                DontDestroyOnLoad(LeftController);
            }
            if (RightController)
            {
                RightController.transform.SetParent(null);
                DontDestroyOnLoad(RightController);
            }
        }

        public static bool IsFiringFromADS()
        {
            return !VRSettings.twoHandedAimingEnabled || (aimingTwoHanded || !GetVRWeaponData(ItemEquippableEvents.currentItem).allowsDoubleHanded);
        }

        private GameObject SetupController(SteamVR_Input_Sources source)
        {
            GameObject controller = new GameObject("Controller");
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = controller.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = source;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;
            steamVR_Behaviour_Pose.rotationOffset = Quaternion.Euler(VRSettings.globalWeaponRotationOffset, 0, 0);
            return controller;
        }

        private void HandleDoubleHandedChecks()
        {
            bool isInDoubleHandPos = false;
            if (FocusStateEvents.currentState == eFocusState.FPS)
            {
                VRWeaponData itemData = GetVRWeaponData(ItemEquippableEvents.currentItem);

                if (itemData.allowsDoubleHanded)
                {
                    bool wasAimingTwoHanded = aimingTwoHanded;
                    isInDoubleHandPos = AreControllersWithinDoubleHandStartDistance();

                    if (!aimingTwoHanded && !m_wasInDoubleHandPosLastFrame && isInDoubleHandPos)
                    {
                        SteamVR_InputHandler.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, GetDeviceFromHandType(offHandControllerType));
                    }

                    if (aimingTwoHanded)
                    {
                        aimingTwoHanded = !AreControllersOutsideOfDoubleHandExitDistance();
                        if (wasAimingTwoHanded && !aimingTwoHanded)
                        {
                            SteamVR_InputHandler.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, GetDeviceFromHandType(offHandControllerType));
                        }
                    }
                    else
                    {
                        aimingTwoHanded = AreControllersWithinDoubleHandStartDistance();
                    }
                }
                else
                {
                    aimingTwoHanded = false;
                }
                m_wasInDoubleHandPosLastFrame = isInDoubleHandPos;
            }
        }

        public static SteamVR_Input_Sources GetDeviceFromHandType(HandType type)
        {
            if (type.Equals(HandType.Left))
            {
                return SteamVR_Input_Sources.LeftHand;
            }
            return SteamVR_Input_Sources.RightHand;
        }

        private void CheckShouldDoubleHand(ItemEquippable item)
        {
            if (!VRSettings.twoHandedAimingEnabled)
            {
                return;
            }
            VRWeaponData itemData = GetVRWeaponData(item);
            if (itemData.allowsDoubleHanded)
            {
                Log.Debug("Item allows double hand!");
                if (VRSettings.alwaysDoubleHanded)
                {
                    Log.Debug("Always double hand is on!");
                    aimingTwoHanded = true;
                }
            }
            else
            {
                aimingTwoHanded = false;
            }
        }

        private bool AreControllersWithinDoubleHandStartDistance()
        {
            if(ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                ItemEquippable currentHeldItem = ItemEquippableEvents.currentItem;
                if (currentHeldItem.LeftHandGripTrans)
                {
                    return Vector3.Distance(offhandController.transform.position, ItemEquippableEvents.GetCorrectedGripPosition()) < m_doubleHandStartDistance;
                }
            }

            return Vector3.Distance(mainController.transform.position, offhandController.transform.position) < m_doubleHandStartDistance;
        }

        private bool AreControllersOutsideOfDoubleHandExitDistance()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                ItemEquippable currentHeldItem = ItemEquippableEvents.currentItem;
                if(currentHeldItem.IsReloading)
                {
                    return false;
                }
                if (currentHeldItem.LeftHandGripTrans)
                {
                    return Vector3.Distance(offhandController.transform.position, ItemEquippableEvents.GetCorrectedGripPosition()) > m_doubleHandLeaveDistance;
                }
            }
            return (Vector3.Distance(mainController.transform.position, offhandController.transform.position) > m_doubleHandLeaveDistance);
        }

        public static Vector3 GetAimForward()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.forward;
            }
            if (!mainController)
            {
                return HMD.Hmd.transform.forward;
            }
            return mainController.transform.rotation * Vector3.forward;
        }

        public static Vector3 GetLocalAimForward()
        {
            return mainController ? mainController.transform.localRotation * Vector3.forward : Vector3.forward;
        }

        public static Vector3 GetLocalPosition()
        {
            return mainController ? mainController.transform.localPosition : Vector3.zero;
        }

        public static Vector3 GetTwoHandedAimForward()
        {
            return (offhandController.transform.position - mainController.transform.position).normalized;
        }

        public static Quaternion GetTwoHandedRotation()
        {
            return Quaternion.LookRotation(GetTwoHandedAimForward(), mainController.transform.up);
        }

        public static Vector3 GetAimFromPos()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.position;
            }
            if (!mainController)
            {
                return HMD.GetWorldPosition();
            }
            return mainController.transform.position;
        }

        public static Quaternion GetRotationFromFiringPoint()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.rotation;
            }
            if (!mainController)
            {
                return Quaternion.identity;
            }
            return mainController.transform.rotation;
        }

        public static Quaternion GetControllerAimRotation()
        {
            if (!mainController)
            {
                return Quaternion.identity;
            }

            if ((VRSettings.twoHandedAimingEnabled || VRSettings.alwaysDoubleHanded) && aimingTwoHanded)
            {
                return GetTwoHandedRotation();
            }
            return mainController.transform.rotation;
        }

        public static Vector3 GetControllerPosition()
        {
            if (!mainController)
            {
                return Vector3.zero;
            }
            return mainController.transform.position;
        }

        private void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= CheckShouldDoubleHand;
        }
    }
}