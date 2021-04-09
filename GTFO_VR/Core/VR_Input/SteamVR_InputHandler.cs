using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR_BepInEx.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.VR_Input
{
    /// <summary>
    /// Handles binding and getting VR input into GTFO's input systems
    /// </summary>
    public class SteamVR_InputHandler : MonoBehaviour
    {

        public SteamVR_InputHandler(IntPtr value)
: base(value) { }

        private static SteamVR_InputHandler instance;

        public static bool Initialized;

        public static SteamVR_Action_Vibration hapticAction;

        private SteamVR_Action_Boolean snapLeftAction;

        private SteamVR_Action_Boolean snapRightAction;

        private SteamVR_Action_Boolean shootAction;

        private SteamVR_Action_Boolean toggleWatchAction;

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

        private SteamVR_Action_Boolean pingAction;

        //private SteamVR_Action_Boolean pushToTalkAction;

        private static Dictionary<InputAction, SteamVR_Action_Boolean> boolActions;



        public void Start()
        {
            if (SteamVR_InputHandler.instance != null)
            {
                GTFO_VR_Plugin.log.LogError("Trying to create duplicate VR_Input class! -- Don't!");
                return;
            }
            SteamVR_InputHandler.instance = this;
           
            InitializeActionMapping();
            SteamVR_InputHandler.Initialized = true;
            GTFO_VR_Plugin.log.LogInfo("Input initialized");
        }


        public static void TriggerHapticPulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources controller)
        {
            hapticAction.Execute(0.0f, duration, frequency, amplitude, controller);
        }

        public static bool GetActionDown(InputAction action)
        {
            if (!SteamVR_InputHandler.Initialized)
            {
                return false;
            }
            
            SteamVR_Action_Boolean boolActionMapping = SteamVR_InputHandler.instance.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return (boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any)) || HMD.hmd.transform.localPosition.y < VR_Settings.IRLCrouchBorder;
            }

            if(action.Equals(InputAction.TerminalExit) && VR_Keyboard.keyboardClosedThisFrame)
            {
                return true;
            }
            return boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any);
        }

        public static bool GetAction(InputAction action)
        {
            if (!SteamVR_InputHandler.Initialized)
            {
                return false;
            }
           
            SteamVR_Action_Boolean boolActionMapping = SteamVR_InputHandler.instance.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return (boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any)) || HMD.hmd.transform.localPosition.y < VR_Settings.IRLCrouchBorder;
            }
            return boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any);
        }

        public static bool GetActionUp(InputAction action)
        {
            if (!Initialized)
            {
                return false;
            }
            
            SteamVR_Action_Boolean boolActionMapping = instance.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
              return boolActionMapping != null && !boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any) || !boolActionMapping.GetState(SteamVR_Input_Sources.Any) && (boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any) || (HMD.hmd.transform.localPosition.y > VR_Settings.IRLCrouchBorder));
            }
            return boolActionMapping != null && boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any);
        }

        static bool IsIRLCrouchValid(InputAction action)
        {
            return action.Equals(InputAction.Crouch) && PlayerVR.LoadedAndInIngameView && VR_Settings.crouchOnIRLCrouch;
        }

        public static float GetAxis(InputAction action)
        {
            if (!SteamVR_InputHandler.Initialized)
            {
                return 0f;
            }
            return SteamVR_InputHandler.instance.GetAxisValue(action);
        }

        public static bool GetSnapTurningLeft()
        {
            if(SteamVR_InputHandler.Initialized)
            {
                if(VR_Settings.useSmoothTurn)
                {
                    return SteamVR_InputHandler.instance.snapLeftAction.GetState(SteamVR_Input_Sources.Any);
                } else
                {
                    return SteamVR_InputHandler.instance.snapLeftAction.GetStateDown(SteamVR_Input_Sources.Any);
                }
            }
            return false;
        }

        public static bool GetSnapTurningRight()
        {
            if (SteamVR_InputHandler.Initialized)
            {
                if (VR_Settings.useSmoothTurn)
                {
                    return SteamVR_InputHandler.instance.snapRightAction.GetState(SteamVR_Input_Sources.Any);
                }
                else
                {
                    return SteamVR_InputHandler.instance.snapRightAction.GetStateDown(SteamVR_Input_Sources.Any);
                }
            }
            return false;
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
            toggleWatchAction = SteamVR_Input.GetBooleanAction("ToggleWatchMode", false);
            movementAxisAction = SteamVR_Input.GetVector2Action("Movement", false);
            interactAction = SteamVR_Input.GetBooleanAction("interact", false);
            crouchAction = SteamVR_Input.GetBooleanAction("Crouch", false);
            sprintAction = SteamVR_Input.GetBooleanAction("Sprint", false);
            jumpAction = SteamVR_Input.GetBooleanAction("Jump", false);
            openMapAction = SteamVR_Input.GetBooleanAction("OpenMap", false);
            openMenuAction = SteamVR_Input.GetBooleanAction("OpenMenu", false);
            pingAction = SteamVR_Input.GetBooleanAction("Ping", false);
            //pushToTalkAction = SteamVR_Input.GetBooleanAction("PushToTalk", false);


            SteamVR_InputHandler.boolActions = new Dictionary<InputAction, SteamVR_Action_Boolean>
            {
                { InputAction.Jump, jumpAction },
                { InputAction.Use, interactAction },
                { InputAction.Aim, toggleWatchAction },
                { InputAction.Fire, shootAction },
                { InputAction.Run, sprintAction },
                { InputAction.Crouch, crouchAction },
                { InputAction.Reload, reloadAction },
                //{ InputAction.VoiceChatPushToTalk, pushToTalkAction},
                { InputAction.NavMarkerPing, pingAction },
                { InputAction.TerminalUp, weaponSwitchLeftAction },
                { InputAction.TerminalDown, weaponSwitchRightAction },
                { InputAction.TerminalExit, reloadAction },
                { InputAction.MenuClick, shootAction },
                //{ InputAction.MenuClickAlternate, interactAction },
                { InputAction.MenuExit, reloadAction },
                { InputAction.MenuToggle, openMenuAction },
                { InputAction.ToggleMap, openMapAction },
                { InputAction.Flashlight, flashlightAction }
            };
        }

        private SteamVR_Action_Boolean GetBoolActionMapping(InputAction action)
        {
            if (SteamVR_InputHandler.boolActions.ContainsKey(action))
            {
                return SteamVR_InputHandler.boolActions[action];
            }
            return null;
        }

        private float GetAxisValue(InputAction action)
        {
            if (InputAction.ScrollItems.Equals(action))
            {
                if (SteamVR_InputHandler.instance.weaponSwitchLeftAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return -1f;
                }
                if (weaponSwitchRightAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return 1f;
                }
                return 0f;
            } else if (InputAction.MenuScroll.Equals(action))
            {
                if (SteamVR_InputHandler.instance.weaponSwitchLeftAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return 1f;
                }
                if (weaponSwitchRightAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return -1f;
                }
            }
            else if(InputAction.MapZoom.Equals(action))
            {
                if (SteamVR_InputHandler.instance.weaponSwitchLeftAction.GetState(SteamVR_Input_Sources.Any))
                {
                    return -1f * Time.deltaTime;
                }
                if (weaponSwitchRightAction.GetState(SteamVR_Input_Sources.Any))
                {
                    return 1f * Time.deltaTime;
                }
            }
            else
            {
                if (InputAction.MoveHorizontal.Equals(action) || InputAction.GamepadLookHorizontal.Equals(action))
                {
                    if(FocusStateManager.CurrentState.Equals(eFocusState.MainMenu))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x / 10f;
                    }

                    if(FocusStateManager.CurrentState.Equals(eFocusState.Map))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x / 2f;
                    }
                    return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x;
                    
                }
                if (InputAction.MoveVertical.Equals(action) || InputAction.GamepadLookVertical.Equals(action))
                {
                    if (FocusStateManager.CurrentState.Equals(eFocusState.MainMenu))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y / 10f;
                    }

                    if (FocusStateManager.CurrentState.Equals(eFocusState.Map))
                    {
                        return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y / 2f;
                    }
                    return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y;
                }
            }
            return 0f;
        }

    }

}
