using GTFO_VR.Core.UI.Canvas.Pointer;
using GTFO_VR.Events;
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

        private static GameObject TerminalCanvasPointer;

        public static SteamVR_Behaviour_Pose mainControllerPose;

        public static bool aimingTwoHanded;

        private float m_doubleHandStartDistance = .1f;

        private float m_doubleHandLeaveDistance = .55f;

        private bool m_wasInDoubleHandPosLastFrame = false;

        public static event Action HandednessSwitched;


        private void Awake()
        {
            SetupControllers();
            SetMainController();
            ItemEquippableEvents.OnPlayerWieldItem += CheckShouldDoubleHand;
            VRConfig.configUseLeftHand.SettingChanged += HandednessSwitch;
            VRConfig.configWeaponRotationOffset.SettingChanged += TiltChanged;
        }


        private void Update()
        {
            if (!VRConfig.configAlwaysDoubleHanded.Value && !FocusStateEvents.currentState.Equals(eFocusState.InElevator))
            {
                HandleDoubleHandedChecks();
            }
        }

        private void SetMainController()
        {
            if (!VRConfig.configUseLeftHand.Value)
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
            return !VRConfig.configUseTwoHanded.Value || (aimingTwoHanded || !GetVRWeaponData(ItemEquippableEvents.currentItem).allowsDoubleHanded);
        }

        private GameObject SetupController(SteamVR_Input_Sources source)
        {
            GameObject controller = new GameObject("Controller");
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = controller.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = source;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;
            steamVR_Behaviour_Pose.rotationOffset = Quaternion.Euler(VRConfig.configWeaponRotationOffset.Value, 0, 0);
            return controller;
        }

        public static void removeCanvasPointer()
        {
            if (TerminalCanvasPointer)
            {
                GameObject.Destroy(TerminalCanvasPointer);
                TerminalCanvasPointer = null;
            }    
        }

        public static void setupCanvasPointer( SteamVR_Input_Sources source = SteamVR_Input_Sources.Any )
        {
            if (!mainController)
                return;

            if (TerminalCanvasPointer)
            {
                GameObject.Destroy(TerminalCanvasPointer);
                TerminalCanvasPointer = null;
            }

            TerminalCanvasPointer = CanvasPointer.create(source);
            TerminalCanvasPointer.transform.SetParent(mainController.transform);
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

        public static GameObject GetInteractionHandGO(InteractionHand hand)
        {
            if(hand == InteractionHand.MainHand)
            {
                return mainController;
            }
            return offhandController;
        }

        public static SteamVR_Input_Sources GetDeviceFromInteractionHandType(InteractionHand type)
        {
            if (type.Equals(InteractionHand.MainHand))
            {
                return GetDeviceFromHandType(mainControllerType);
            }
            return GetDeviceFromHandType(offHandControllerType);
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
            if (!VRConfig.configUseTwoHanded.Value)
            {
                return;
            }
            VRWeaponData itemData = GetVRWeaponData(item);
            if (itemData.allowsDoubleHanded)
            {
                Log.Debug("Item allows double hand!");
                if (VRConfig.configAlwaysDoubleHanded.Value)
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
                    Vector3 correctedGripPosition = ItemEquippableEvents.GetCorrectedGripPosition();

                    if (GTFO_VR_Plugin.DEBUG_ENABLED)
                    {
                        if (VRConfig.configDebugShowTwoHHitboxes.Value)
                        {
                            DebugDraw3D.DrawSphere(correctedGripPosition, m_doubleHandStartDistance, ColorExt.Blue(0.2f));
                        }
                    }

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
                    Vector3 correctedGripPosition = ItemEquippableEvents.GetCorrectedGripPosition();
                    if (GTFO_VR_Plugin.DEBUG_ENABLED)
                    {
                        if (VRConfig.configDebugShowTwoHHitboxes.Value)
                        {
                            DebugDraw3D.DrawSphere(correctedGripPosition, m_doubleHandLeaveDistance, ColorExt.Red(0.1f));
                        }
                    }
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

            if ((VRConfig.configUseTwoHanded.Value || VRConfig.configAlwaysDoubleHanded.Value) && aimingTwoHanded)
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


        private void TiltChanged(object sender, EventArgs e)
        {
            TiltChanged(LeftController);
            TiltChanged(RightController);
        }

        private static void TiltChanged(GameObject controller)
        {
            if(controller)
            {
                SteamVR_Behaviour_Pose pose = controller.GetComponent<SteamVR_Behaviour_Pose>();
                if (pose)
                    pose.rotationOffset = Quaternion.Euler(VRConfig.configWeaponRotationOffset.Value, 0, 0);
            }
        }

        private void HandednessSwitch(object sender, EventArgs e)
        {
            SetMainController();
            HandednessSwitched?.Invoke();
        }

        private void OnDestroy()
        {
            VRConfig.configUseLeftHand.SettingChanged -= HandednessSwitch;
            VRConfig.configWeaponRotationOffset.SettingChanged -= TiltChanged;
            ItemEquippableEvents.OnPlayerWieldItem -= CheckShouldDoubleHand;
        }
    }
}