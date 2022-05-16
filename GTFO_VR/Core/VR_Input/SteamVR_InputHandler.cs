﻿using GTFO_VR.Events;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.VR_Input
{
    /// <summary>
    /// Handles binding and getting VR input into GTFO's input systems
    /// </summary>
    public static class SteamVR_InputHandler
    {
        public static bool Initialized;

        public static SteamVR_Action_Vibration hapticAction;

        private static SteamVR_Action_Boolean m_shootAction;

        private static SteamVR_Action_Vector2 m_movementAxisAction;

        private static SteamVR_Action_Boolean m_sprintAction;

        private static SteamVR_Action_Boolean m_weaponSwitchLeftAction;

        private static SteamVR_Action_Boolean m_weaponSwitchRightAction;

        private static SteamVR_Action_Boolean m_reloadAction;

        private static SteamVR_Action_Boolean m_flashlightAction;

        private static SteamVR_Action_Boolean m_jumpAction;

        private static SteamVR_Action_Boolean m_interactAction;

        private static SteamVR_Action_Boolean m_crouchAction;

        private static SteamVR_Action_Boolean m_openMenuAction;

        private static SteamVR_Action_Boolean m_openMapAction;

        private static SteamVR_Action_Boolean m_pingAction;

        private static SteamVR_Action_Boolean m_pushToTalkAction;

        private static SteamVR_Action_Boolean m_openObjectivesAction;

        private static SteamVR_Action_Boolean m_aimOrShoveAction;

        private static Dictionary<InputAction, SteamVR_Action_Boolean> boolActions;

        private static DummyAction m_terminalDeleteAction= new DummyAction(InputAction.TerminalDel, true);
        private static DummyAction m_terminalExitAction = new DummyAction(InputAction.TerminalExit, true);

        private static DummyAction m_terminalLeftAction = new DummyAction(InputAction.TerminalLeft, true);
        private static DummyAction m_terminalUpAction = new DummyAction(InputAction.TerminalUp, true);
        private static DummyAction m_terminalRightAction = new DummyAction(InputAction.TerminalRight, true);
        private static DummyAction m_terminalDownAction = new DummyAction(InputAction.TerminalDown, true);

        private static Dictionary<InputAction, DummyAction> dummyBoolActions;

        public static void Setup()
        {
            if(Initialized)
            {
                return;
            }
            InitializeActionMapping();
            Initialized = true;
            Log.Info("Input initialized");
        }

        public static bool TryGetActionNameFromInputAction(InputAction a, ref string output)
        {
            if(!Initialized)
            {
                Setup();
            }
            if(boolActions.ContainsKey(a))
            {
                output = boolActions[a].GetShortName();
                Log.Debug($"Found action name for {a} - {output}");
                return true;
            }
            Log.Debug($"Action {a} was not in boolActions...");
            return false;
        }

        public static void TriggerHapticPulse(ushort microSecondsDuration, SteamVR_Input_Sources controller)
        {
            float seconds = (float)microSecondsDuration / 1000000f;
            hapticAction.Execute(0, seconds, 1f / seconds, 1, controller);
        }

        public static void TriggerHapticPulse(float seconds, SteamVR_Input_Sources controller)
        {
            hapticAction.Execute(0, seconds, 1f / seconds, 1, controller);
        }

        public static void TriggerHapticPulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources controller)
        {
            if (!Initialized)
            {
                return;
            }
            hapticAction.Execute(0.0f, duration, frequency, amplitude, controller);
        }

        public static void triggerDummyAction(InputAction action )
        {
            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null )
            {
                dummyAction.requestPress();
            }
        }

        public static bool GetActionDown(InputAction action)
        {
            if (!Initialized)
            {
                return false;
            }

            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null && dummyAction.getDown())
                return true;

            SteamVR_Action_Boolean boolActionMapping = GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return (boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any)) || HMD.Hmd.transform.localPosition.y + VRConfig.configFloorOffset.Value / 100f < VRConfig.configCrouchHeight.Value / 100f;
            }

            if (action.Equals(InputAction.TerminalExit) && VRKeyboard.KeyboardClosedThisFrame)
            {
                return true;
            }
            if(action.Equals(InputAction.TextChatClose) && VRKeyboard.KeyboardClosedThisFrame)
            {
                return true;
            }
            return boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any);
        }

        public static bool GetAction(InputAction action)
        {
            if (!Initialized)
            {
                return false;
            }

            //DummyAction dummyAction = GetDummyBoolActionMapping(action);
            //if (dummyAction != null && dummyAction.getDown())
            //    return true;

            SteamVR_Action_Boolean boolActionMapping = SteamVR_InputHandler.GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return (boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any)) || HMD.Hmd.transform.localPosition.y + VRConfig.configFloorOffset.Value / 100f < VRConfig.configCrouchHeight.Value / 100f;
            }
            return boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any);
        }

        public static bool GetActionUp(InputAction action)
        {
            if (!Initialized)
            {
                return false;
            }

            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null && dummyAction.getUp())
                return true;

            SteamVR_Action_Boolean boolActionMapping = GetBoolActionMapping(action);
            if (IsIRLCrouchValid(action))
            {
                return boolActionMapping != null && !boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any) || !boolActionMapping.GetState(SteamVR_Input_Sources.Any) && (boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any) || (HMD.Hmd.transform.localPosition.y + VRConfig.configFloorOffset.Value > VRConfig.configCrouchHeight.Value / 100f));
            }
            return boolActionMapping != null && boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any);
        }

        private static bool IsIRLCrouchValid(InputAction action)
        {
            return action.Equals(InputAction.Crouch) && VRConfig.configIRLCrouch.Value && FocusStateEvents.currentState == eFocusState.FPS;
        }

        public static float GetAxis(InputAction action)
        {
            if (!Initialized)
            {
                return 0f;
            }
            return SteamVR_InputHandler.GetAxisValue(action);
        }


        private static void InitializeActionMapping()
        {
            hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic", false);
            m_weaponSwitchLeftAction = SteamVR_Input.GetBooleanAction("WeaponSwitchLeft", false);
            m_weaponSwitchRightAction = SteamVR_Input.GetBooleanAction("WeaponSwitchRight", false);
            m_reloadAction = SteamVR_Input.GetBooleanAction("Reload", false);
            m_flashlightAction = SteamVR_Input.GetBooleanAction("ToggleFlashlight", false);
            m_shootAction = SteamVR_Input.GetBooleanAction("Shoot", false);
            m_movementAxisAction = SteamVR_Input.GetVector2Action("Movement", false);
            m_interactAction = SteamVR_Input.GetBooleanAction("interact", false);
            m_crouchAction = SteamVR_Input.GetBooleanAction("Crouch", false);
            m_sprintAction = SteamVR_Input.GetBooleanAction("Sprint", false);
            m_jumpAction = SteamVR_Input.GetBooleanAction("Jump", false);
            m_openMapAction = SteamVR_Input.GetBooleanAction("OpenMap", false);
            m_openMenuAction = SteamVR_Input.GetBooleanAction("OpenMenu", false);
            m_pingAction = SteamVR_Input.GetBooleanAction("Ping", false);
            m_openObjectivesAction = SteamVR_Input.GetBooleanAction("OpenObjectives", false);
            m_aimOrShoveAction = SteamVR_Input.GetBooleanAction("AimOrShove", false);
            m_pushToTalkAction = SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/PushToTalk");


            boolActions = new Dictionary<InputAction, SteamVR_Action_Boolean>
            {

                { InputAction.Jump, m_jumpAction },
                { InputAction.Use, m_interactAction },
                { InputAction.Aim, m_aimOrShoveAction},
                { InputAction.ToggleObjectives, m_openObjectivesAction},
                { InputAction.Fire, m_shootAction },
                { InputAction.Run, m_sprintAction },
                { InputAction.Crouch, m_crouchAction },
                { InputAction.Reload, m_reloadAction },
                { InputAction.VoiceChatPushToTalk, m_pushToTalkAction},
                { InputAction.NavMarkerPing, m_pingAction },
                { InputAction.TerminalUp, m_weaponSwitchLeftAction },
                { InputAction.TerminalDown, m_weaponSwitchRightAction },
                { InputAction.TerminalExit, m_reloadAction },
                { InputAction.MenuClick, m_shootAction },
                //{ InputAction.MenuClickAlternate, interactAction },
                { InputAction.MenuExit, m_reloadAction },
                { InputAction.MenuToggle, m_openMenuAction },
                { InputAction.ToggleMap, m_openMapAction },
                { InputAction.Flashlight, m_flashlightAction },
            };

            dummyBoolActions = new Dictionary<InputAction, DummyAction>
            {
                { InputAction.TerminalDel, m_terminalDeleteAction },
                { InputAction.TerminalExit, m_terminalExitAction },

                { InputAction.TerminalLeft, m_terminalLeftAction },
                { InputAction.TerminalUp, m_terminalUpAction },
                { InputAction.TerminalRight, m_terminalRightAction },
                { InputAction.TerminalDown, m_terminalDownAction }
            };

        }

        private static SteamVR_Action_Boolean GetBoolActionMapping(InputAction action)
        {
            if (boolActions.ContainsKey(action))
            {
                return boolActions[action];
            }

            return null;
        }

        private static DummyAction GetDummyBoolActionMapping(InputAction action)
        {
            if (dummyBoolActions.ContainsKey(action))
            {
                return dummyBoolActions[action];
            }
            return null;
        }

        private static float GetAxisValue(InputAction action)
        {
            if (InputAction.ScrollItems.Equals(action))
            {
                if (m_weaponSwitchLeftAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return -1f;
                }
                if (m_weaponSwitchRightAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return 1f;
                }
                return 0f;
            }
            else if (InputAction.MenuScroll.Equals(action))
            {
                if (m_weaponSwitchLeftAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return 1f;
                }
                if (m_weaponSwitchRightAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    return -1f;
                }
            }
            else if (InputAction.MapZoom.Equals(action))
            {
                if (m_weaponSwitchLeftAction.GetState(SteamVR_Input_Sources.Any))
                {
                    return -1f * Time.deltaTime;
                }
                if (m_weaponSwitchRightAction.GetState(SteamVR_Input_Sources.Any))
                {
                    return 1f * Time.deltaTime;
                }
            }
            else
            {
                if (InputAction.MoveHorizontal.Equals(action) || InputAction.GamepadLookHorizontal.Equals(action))
                {
                    if (FocusStateManager.CurrentState.Equals(eFocusState.MainMenu) || FocusStateManager.CurrentState.Equals(eFocusState.GlobalPopupMessage))
                    {
                        return m_movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x / 10f;
                    }

                    if (FocusStateManager.CurrentState.Equals(eFocusState.Map))
                    {
                        return m_movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x / 2f;
                    }
                    return m_movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x;
                }
                if (InputAction.MoveVertical.Equals(action) || InputAction.GamepadLookVertical.Equals(action))
                {
                    if (FocusStateManager.CurrentState.Equals(eFocusState.MainMenu) || FocusStateManager.CurrentState.Equals(eFocusState.GlobalPopupMessage))
                    {
                        return m_movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y / 10f;
                    }

                    if (FocusStateManager.CurrentState.Equals(eFocusState.Map))
                    {
                        return m_movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y / 2f;
                    }
                    return m_movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y;
                }
            }
            return 0f;
        }
    }
}