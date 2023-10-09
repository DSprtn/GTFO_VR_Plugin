using GTFO_VR.Core.UI.Terminal.Pointer;
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

        public static HandType MainControllerType = HandType.Right;
        public static HandType offHandControllerType = HandType.Left;

        public static GameObject MainController;

        public static GameObject OffhandController;

        private static GameObject m_leftController;

        private static GameObject m_rightController;

        private static GameObject m_terminalCanvasPointer;

        public static SteamVR_Behaviour_Pose MainControllerPose;

        public static bool AimingTwoHanded;

        private float m_doubleHandStartDistance = .1f;

        private float m_doubleHandLeaveDistance = .55f;

        private bool m_wasInDoubleHandPosLastFrame = false;

        public static event Action HandednessSwitched;


        private void Awake()
        {
            SetupControllers();
            SetupTerminalCanvasPointer();
            SetMainController();
            ItemEquippableEvents.OnPlayerWieldItem += CheckShouldDoubleHand;
            VRConfig.configUseLeftHand.SettingChanged += HandednessSwitch;
            VRConfig.configWeaponRotationOffset.SettingChanged += TiltChanged;
            VRConfig.configPosePredictionTranslation.SettingChanged += PosePredictionChanged;
            VRConfig.configPosePredictionRotation.SettingChanged += PosePredictionChanged;
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
                MainController = m_rightController;
                OffhandController = m_leftController;
                MainControllerType = HandType.Right;
                offHandControllerType = HandType.Left;
            }
            else
            {
                MainController = m_leftController;
                OffhandController = m_rightController;
                MainControllerType = HandType.Left;
                offHandControllerType = HandType.Right;
            }

            m_terminalCanvasPointer?.transform.SetParent(MainController.transform, false);
            MainControllerPose = MainController.GetComponent<SteamVR_Behaviour_Pose>();
        }

        private void SetupControllers()
        {
            m_leftController = SetupController(SteamVR_Input_Sources.LeftHand);
            m_rightController = SetupController(SteamVR_Input_Sources.RightHand);
            m_leftController.name = "LeftController";
            m_rightController.name = "RightController";

            DontDestroyOnLoad(m_rightController);
            DontDestroyOnLoad(m_leftController);
        }

        private void SetupTerminalCanvasPointer()
        {
            m_terminalCanvasPointer = TerminalPointer.Instantiate(SteamVR_Input_Sources.Any);
            m_terminalCanvasPointer.SetActive(false);
        }

        public static void ToggleTerminalCanvasPointer(bool enable)
        {
            m_terminalCanvasPointer?.SetActive(enable);
        }

        public static void SetOrigin(Transform origin)
        {
            m_leftController.transform.SetParent(origin);
            m_rightController.transform.SetParent(origin);
        }

        public static void OnOriginDestroyed()
        {
            if (m_leftController)
            {
                m_leftController.transform.SetParent(null);
                DontDestroyOnLoad(m_leftController);
            }
            if (m_rightController)
            {
                m_rightController.transform.SetParent(null);
                DontDestroyOnLoad(m_rightController);
            }
        }

        public static bool IsFiringFromADS()
        {
            return !VRConfig.configUseTwoHanded.Value || (AimingTwoHanded || !GetVRWeaponData().allowsDoubleHanded) || !VRConfig.configUseControllers.Value;
        }

        private GameObject SetupController(SteamVR_Input_Sources source)
        {
            GameObject controller = new GameObject("Controller");
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = controller.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = source;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;
            steamVR_Behaviour_Pose.rotationOffset = Quaternion.Euler(VRConfig.configWeaponRotationOffset.Value, 0, 0);
            steamVR_Behaviour_Pose.setPosePrediction(VRConfig.configPosePredictionTranslation.Value, VRConfig.configPosePredictionRotation.Value);
            return controller;
        }

        private void HandleDoubleHandedChecks()
        {
            if(!VRConfig.configUseControllers.Value)
            {
                return;
            }
            bool isInDoubleHandPos = false;
            if (FocusStateEvents.currentState == eFocusState.FPS)
            {
                VRWeaponData itemData = GetVRWeaponData();

                if (itemData.allowsDoubleHanded)
                {
                    bool wasAimingTwoHanded = AimingTwoHanded;
                    isInDoubleHandPos = AreControllersWithinDoubleHandStartDistance();

                    if (!AimingTwoHanded && !m_wasInDoubleHandPosLastFrame && isInDoubleHandPos)
                    {
                        SteamVR_InputHandler.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, GetDeviceFromHandType(offHandControllerType));
                    }

                    if (AimingTwoHanded)
                    {
                        AimingTwoHanded = !AreControllersOutsideOfDoubleHandExitDistance();
                        if (wasAimingTwoHanded && !AimingTwoHanded)
                        {
                            SteamVR_InputHandler.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, GetDeviceFromHandType(offHandControllerType));
                        }
                    }
                    else
                    {
                        AimingTwoHanded = AreControllersWithinDoubleHandStartDistance();
                    }
                }
                else
                {
                    AimingTwoHanded = false;
                }
                m_wasInDoubleHandPosLastFrame = isInDoubleHandPos;
            }
        }

        public static GameObject GetInteractionHandGO(InteractionHand hand)
        {
            if(hand == InteractionHand.MainHand)
            {
                return MainController;
            }
            return OffhandController;
        }

        public static SteamVR_Input_Sources GetDeviceFromInteractionHandType(InteractionHand type)
        {
            if (type.Equals(InteractionHand.MainHand))
            {
                return GetDeviceFromHandType(MainControllerType);
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
            if (!VRConfig.configUseTwoHanded.Value || !VRConfig.configUseControllers.Value)
            {
                return;
            }
            VRWeaponData itemData = GetVRWeaponData();
            if (itemData.allowsDoubleHanded)
            {
                Log.Debug("Item allows double hand!");
                if (VRConfig.configAlwaysDoubleHanded.Value)
                {
                    Log.Debug("Always double hand is on!");
                    AimingTwoHanded = true;
                }
            }
            else
            {
                AimingTwoHanded = false;
            }
        }

        private bool AreControllersWithinDoubleHandStartDistance()
        {
            if(!VRConfig.configUseControllers.Value)
            {
                return false;
            }
            if(ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                ItemEquippable currentHeldItem = ItemEquippableEvents.currentItem;
                if (currentHeldItem.LeftHandGripTrans)
                {
                    Vector3 correctedGripPosition = ItemEquippableEvents.GetCorrectedGripPosition();
#if DEBUG_GTFO_VR
                    if (VRConfig.configDebugShowTwoHHitboxes.Value)
                    {
                        DebugDraw3D.DrawSphere(correctedGripPosition, m_doubleHandStartDistance, ColorExt.Blue(0.2f));
                    }
#endif
                    return Vector3.Distance(OffhandController.transform.position, ItemEquippableEvents.GetCorrectedGripPosition()) < m_doubleHandStartDistance;
                }
            }

            return Vector3.Distance(MainController.transform.position, OffhandController.transform.position) < m_doubleHandStartDistance;
        }

        private bool AreControllersOutsideOfDoubleHandExitDistance()
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return true;
            }
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
#if DEBUG_GTFO_VR
                    if (VRConfig.configDebugShowTwoHHitboxes.Value)
                    {
                        DebugDraw3D.DrawSphere(correctedGripPosition, m_doubleHandLeaveDistance, ColorExt.Red(0.1f));
                    }
#endif
                    return Vector3.Distance(OffhandController.transform.position, ItemEquippableEvents.GetCorrectedGripPosition()) > m_doubleHandLeaveDistance;
                }
            }
            return (Vector3.Distance(MainController.transform.position, OffhandController.transform.position) > m_doubleHandLeaveDistance);
        }

        public static Vector3 GetAimForward()
        {
            if(!VRConfig.configUseControllers.Value)
            {
                return HMD.GetWorldForward();
            }

            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.forward;
            }

            if (!MainController)
            {
                return HMD.Hmd.transform.forward;
            }
            return MainController.transform.rotation * Vector3.forward;
        }

        public static Vector3 GetLocalAimForward()
        {
            return MainController ? MainController.transform.localRotation * Vector3.forward : HMD.GetWorldForward();
        }

        public static Vector3 GetLocalPosition()
        {
            return MainController ? MainController.transform.localPosition : Vector3.zero;
        }

        public static Vector3 GetTwoHandedAimForward()
        {
            return (OffhandController.transform.position - MainController.transform.position).normalized;
        }

        public static Quaternion GetTwoHandedRotation()
        {
            return Quaternion.LookRotation(GetTwoHandedAimForward(), MainController.transform.up);
        }

        public static Vector3 GetAimFromPos()
        {
            if(!VRConfig.configUseControllers.Value)
            {
                return HMD.GetWorldPosition();
            }
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.position;
            }
            if (!MainController)
            {
                return HMD.GetWorldPosition();
            }
            return MainController.transform.position;
        }

        public static Quaternion GetRotationFromFiringPoint()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.rotation;
            }
            if (!MainController)
            {
                return Quaternion.identity;
            }
            return MainController.transform.rotation;
        }

        public static Quaternion GetControllerAimRotation()
        {
            if (!MainController)
            {
                return Quaternion.identity;
            }

            if ((VRConfig.configUseTwoHanded.Value || VRConfig.configAlwaysDoubleHanded.Value) && AimingTwoHanded)
            {
                return GetTwoHandedRotation();
            }
            return MainController.transform.rotation;
        }

        public static Vector3 GetControllerPosition()
        {
            if (!MainController)
            {
                return Vector3.zero;
            }
            return MainController.transform.position;
        }


        private void TiltChanged(object sender, EventArgs e)
        {
            TiltChanged(m_leftController);
            TiltChanged(m_rightController);
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

        private void PosePredictionChanged(object sender, EventArgs e)
        {
            PosePredictionChanged(m_leftController);
            PosePredictionChanged(m_rightController);
        }

        private static void PosePredictionChanged(GameObject controller)
        {
            if (controller)
            {
                SteamVR_Behaviour_Pose pose = controller.GetComponent<SteamVR_Behaviour_Pose>();
                if (pose)
                {
                    pose.setPosePrediction(VRConfig.configPosePredictionTranslation.Value, VRConfig.configPosePredictionRotation.Value);
                } 
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
            VRConfig.configPosePredictionTranslation.SettingChanged -= PosePredictionChanged;
            VRConfig.configPosePredictionRotation.SettingChanged -= PosePredictionChanged;
        }
    }
}