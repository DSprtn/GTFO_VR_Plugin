using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Util;
using GTFO_VR_BepInEx.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;
using static GTFO_VR.Util.WeaponArchetypeVRData;

namespace GTFO_VR.Input
{
    public class Controllers : MonoBehaviour
    {
        public Controllers(IntPtr value)
: base(value) { }


        public static GameObject mainController;

        public static GameObject offhandController;

        static GameObject leftController;

        static GameObject rightController;

        public static bool aimingTwoHanded;

        float doubleHandStartDistance = .14f;

        float doubleHandLeaveDistance = .60f;

        bool wasInDoubleHandPosLastFrame = false;

        public static HandType mainControllerType = HandType.Right;
        public static HandType offHandControllerType = HandType.Left;

        void Awake()
        {
            SetupControllers();
            SetMainController();
            ItemEquippableEvents.OnPlayerWieldItem += CheckShouldDoubleHand;
        }

        void Update()
        {
            if(!VR_Settings.alwaysDoubleHanded && !FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                HandleDoubleHandedChecks();
            }
        }

        private void HandleDoubleHandedChecks()
        {
            bool isInDoubleHandPos = false;
            if (PlayerVR.LoadedAndInIngameView)
            {

                VRWeaponData itemData = WeaponArchetypeVRData.GetVRWeaponData(ItemEquippableEvents.currentItem);

                if (itemData.allowsDoubleHanded)
                {
                    bool wasAimingTwoHanded = aimingTwoHanded;
                    isInDoubleHandPos = AreControllersInDoubleHandedPosition();

                    if (!aimingTwoHanded && !wasInDoubleHandPosLastFrame && isInDoubleHandPos)
                    {
                        VRInput.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, GetDeviceFromType(offHandControllerType));
                    }

                    if (aimingTwoHanded)
                    {
                        aimingTwoHanded = !AreControllersOutsideOfDoubleHandedAimingRange();
                        if (wasAimingTwoHanded && !aimingTwoHanded)
                        {
                            VRInput.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, GetDeviceFromType(offHandControllerType));
                        }
                    }
                    else
                    {
                        aimingTwoHanded = AreControllersInDoubleHandedPosition();
                    }
                }
                else
                {
                    aimingTwoHanded = false;
                }
                wasInDoubleHandPosLastFrame = isInDoubleHandPos;
            }
        }



        public static SteamVR_Input_Sources GetDeviceFromType(HandType type)
        {
            if (type.Equals(HandType.Left))
            {
                return SteamVR_Input_Sources.LeftHand;
            }
            return SteamVR_Input_Sources.RightHand;
        }


        private void CheckShouldDoubleHand(ItemEquippable item)
        {
            VRWeaponData itemData = WeaponArchetypeVRData.GetVRWeaponData(item);
            if (itemData.allowsDoubleHanded)
            {
                GTFO_VR_Plugin.log.LogDebug("Item allows double hand!");
                if (VR_Settings.alwaysDoubleHanded)
                {
                    GTFO_VR_Plugin.log.LogDebug("Always double hand is on!");
                    aimingTwoHanded = true;
                }
            } else
            {
                aimingTwoHanded = false;
            }
        }


        bool AreControllersInDoubleHandedPosition()
        {
            if (Vector3.Distance(mainController.transform.position, offhandController.transform.position) < doubleHandStartDistance)
            {
                return true;
            }
            return false;
        }

        bool AreControllersOutsideOfDoubleHandedAimingRange()
        {
            if (Vector3.Distance(mainController.transform.position, offhandController.transform.position) > doubleHandLeaveDistance)
            {
                return true;
            }
            return false;
        }

        private void SetMainController()
        {
            if (VR_Settings.mainHand.Equals(HandType.Right))
            {
                mainController = rightController;
                offhandController = leftController;
                mainControllerType = HandType.Right;
                offHandControllerType = HandType.Left;
            }
            else
            {
                mainController = leftController;
                offhandController = rightController;
                mainControllerType = HandType.Left;
                offHandControllerType = HandType.Right;
            }
        }

        private void SetupControllers()
        {
            leftController = SetupController(SteamVR_Input_Sources.LeftHand);
            rightController = SetupController(SteamVR_Input_Sources.RightHand);
            leftController.name = "LeftController";
            rightController.name = "RightController";

            DontDestroyOnLoad(rightController);
            DontDestroyOnLoad(leftController);
        }

        public static void SetOrigin(Transform origin)
        {
            leftController.transform.SetParent(origin);
            rightController.transform.SetParent(origin);
        }

        GameObject SetupController(SteamVR_Input_Sources source)
        {
            GameObject controller = new GameObject("Controller");
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = controller.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = source;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;
            return controller;
        }

        // Used in bioscanner patch!
        public static Vector3 GetMainHandTransformRight()
        {
            return mainController.transform.right;
        }

        public static Vector3 GetAimForward()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.forward;
            }
            if (!mainController)
            {
                return HMD.hmd.transform.forward;
            }
            return (mainController.transform.rotation * Vector3.forward);
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
            float currentItemYOffset = 0f;
            Vector3 offhandPos = offhandController.transform.position;
            offhandPos.y += currentItemYOffset;
            return (offhandPos - mainController.transform.position).normalized;
        }

        public static Vector3 GetTwoHandedTransformUp()
        {
            return (mainController.transform.up + offhandController.transform.up) / 2;
        }

        public static Quaternion GetTwoHandedRotation()
        {
            return Quaternion.LookRotation(GetTwoHandedAimForward());
        }

        public static Vector3 GetTwoHandedPos()
        {
            return (mainController.transform.position + offhandController.transform.position) / 2;
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

        public static Quaternion GetAimFromRot()
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

            if ((VR_Settings.twoHandedAimingEnabled || VR_Settings.alwaysDoubleHanded) && Controllers.aimingTwoHanded)
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


        void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= CheckShouldDoubleHand;
        }
    }
}
