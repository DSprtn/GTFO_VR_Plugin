using GTFO_VR.Core;
using GTFO_VR.Input;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR
{
    /// <summary>
    /// Handles binding and getting VR input into GTFO's input systems
    /// </summary>
    public class VRInput : MonoBehaviour
    {
        private static VRInput instance;

        public static bool Initialized;

        public static SteamVR_Action_Vibration hapticAction;

        private SteamVR_Action_Boolean snapLeftAction;

        private SteamVR_Action_Boolean snapRightAction;

        private SteamVR_Action_Boolean shootAction;

        private SteamVR_Action_Boolean aimAction;

        private SteamVR_Action_Vector2 movementAxisAction;

        private SteamVR_Action_Boolean sprintAction;

        private SteamVR_Action_Boolean weaponSwitchLeftAction;

        private SteamVR_Action_Boolean weaponSwitchRightAction;

        private SteamVR_Action_Boolean reloadAction;

        private SteamVR_Action_Boolean flashlightAction;

        private SteamVR_Action_Boolean jumpAction;

        private SteamVR_Action_Boolean interactAction;

        private SteamVR_Action_Boolean crouchAction;

        private SteamVR_Action_Boolean openMenuAction;

        private SteamVR_Action_Boolean openMapAction;

        private static Dictionary<InputAction, SteamVR_Action_Boolean> boolActions;

        
        public static float IRLCrouchBorder = 1.2f;

        public void Start()
        {
            if (VRInput.instance != null)
            {
                Debug.LogError("Trying to create duplicate VR_Input class! -- Don't!");
                return;
            }
            VRInput.instance = this;
           
            InitializeActionMapping();
            VRInput.Initialized = true;
            Debug.Log("Input initialized");
        }


        public static void TriggerHapticPulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources controller)
        {
            hapticAction.Execute(0.0f, duration, frequency, amplitude, controller);
        }

        public static bool GetActionDown(InputAction action)
        {
            if (!VRInput.Initialized)
            {
                return false;
            }
            
            SteamVR_Action_Boolean boolActionMapping = VRInput.instance.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return (boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any)) || HMD.hmd.transform.localPosition.y < VRInput.IRLCrouchBorder;
            }

            if(action.Equals(InputAction.TerminalExit) && VRGlobal.keyboardClosedThisFrame)
            {
                return true;
            }
            return boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any);
        }

        public static bool GetAction(InputAction action)
        {
            if (!VRInput.Initialized)
            {
                return false;
            }
           
            SteamVR_Action_Boolean boolActionMapping = VRInput.instance.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return (boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any)) || HMD.hmd.transform.localPosition.y < VRInput.IRLCrouchBorder;
            }
            return boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any);
        }

        public static bool GetActionUp(InputAction action)
        {
            if (!VRInput.Initialized)
            {
                return false;
            }
            
            SteamVR_Action_Boolean boolActionMapping = VRInput.instance.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
              return boolActionMapping != null && !boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any) || !boolActionMapping.GetState(SteamVR_Input_Sources.Any) && (boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any) || (HMD.hmd.transform.localPosition.y > VRInput.IRLCrouchBorder));
            }
            return boolActionMapping != null && boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any);
        }

        static bool IsIRLCrouchValid(InputAction action)
        {
            return action.Equals(InputAction.Crouch) && PlayerVR.LoadedAndInGame && VRSettings.crouchOnIRLCrouch;
        }

        public static float GetAxis(InputAction action)
        {
            if (!VRInput.Initialized)
            {
                return 0f;
            }
            return VRInput.instance.GetAxisValue(action);
        }

        public static bool GetSnapTurningLeft()
        {
            return VRInput.Initialized && VRInput.instance.snapLeftAction.GetStateDown(SteamVR_Input_Sources.Any);
        }

        public static bool GetSnapTurningRight()
        {
            return VRInput.Initialized && VRInput.instance.snapRightAction.GetStateDown(SteamVR_Input_Sources.Any);
        }

        private void InitializeActionMapping()
        {
            hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic", false);
            weaponSwitchLeftAction = SteamVR_Input.GetBooleanAction("WeaponSwitchLeft", false);
            weaponSwitchRightAction = SteamVR_Input.GetBooleanAction("WeaponSwitchRight", false);
            reloadAction = SteamVR_Input.GetBooleanAction("Reload", false);
            flashlightAction = SteamVR_Input.GetBooleanAction("ToggleFlashlight", false);
            snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft", false);
            snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight", false);
            shootAction = SteamVR_Input.GetBooleanAction("Shoot", false);
            aimAction = SteamVR_Input.GetBooleanAction("Aim", false);
            movementAxisAction = SteamVR_Input.GetVector2Action("Movement", false);
            interactAction = SteamVR_Input.GetBooleanAction("interact", false);
            crouchAction = SteamVR_Input.GetBooleanAction("Crouch", false);
            sprintAction = SteamVR_Input.GetBooleanAction("Sprint", false);
            jumpAction = SteamVR_Input.GetBooleanAction("Jump", false);
            openMapAction = SteamVR_Input.GetBooleanAction("OpenMap", false);
            openMenuAction = SteamVR_Input.GetBooleanAction("OpenMenu", false);


            VRInput.boolActions = new Dictionary<InputAction, SteamVR_Action_Boolean>
            {
                { InputAction.Jump, jumpAction },
                { InputAction.Use, interactAction },
                { InputAction.Aim, aimAction },
                { InputAction.Fire, shootAction },
                { InputAction.Run, sprintAction },
                { InputAction.Crouch, crouchAction },
                { InputAction.Reload, reloadAction },
                { InputAction.Melee, aimAction },
                { InputAction.TerminalUp, weaponSwitchLeftAction },
                { InputAction.TerminalDown, weaponSwitchRightAction },
                { InputAction.TerminalExit, reloadAction },
                { InputAction.MenuClick, shootAction },
                { InputAction.MenuClickAlternate, interactAction },
                { InputAction.MenuExit, reloadAction },
                { InputAction.MenuToggle, openMenuAction },
                { InputAction.ToggleMap, openMapAction },
                { InputAction.Flashlight, flashlightAction }
            };
        }

        private SteamVR_Action_Boolean GetBoolActionMapping(InputAction action)
        {
            if (VRInput.boolActions.ContainsKey(action))
            {
                return VRInput.boolActions[action];
            }
            return null;
        }

        private float GetAxisValue(InputAction action)
        {
            if (InputAction.ScrollItems.Equals(action))
            {
                if (VRInput.instance.weaponSwitchLeftAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return -1f;
                }
                if (weaponSwitchRightAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return 1f;
                }
                return 0f;
            }
            else
            {
                if (InputAction.MoveHorizontal.Equals(action) || InputAction.GamepadLookHorizontal.Equals(action))
                {
                    if(FocusStateManager.CurrentState.Equals(eFocusState.MainMenu))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x / 20f;
                    }

                    if(FocusStateManager.CurrentState.Equals(eFocusState.Map))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x / 6f;
                    }
                    return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x;
                    
                }
                if (InputAction.MoveVertical.Equals(action) || InputAction.GamepadLookVertical.Equals(action))
                {
                    if (FocusStateManager.CurrentState.Equals(eFocusState.MainMenu))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y / 20f;
                    }

                    if (FocusStateManager.CurrentState.Equals(eFocusState.Map))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y / 6f;
                    }
                    return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y;
                }
                return 0f;
            }
        }

    }

}
