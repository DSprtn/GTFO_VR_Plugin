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

        private static Dictionary<InputAction, SteamVR_Action_Boolean> boolActions;

        public static bool crouchOnIRLCrouch = true;

        public static float IRLCrouchBorder = 1f;

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

        public static bool GetActionDown(InputAction action)
        {
            if (!VRInput.Initialized)
            {
                return false;
            }
            
            SteamVR_Action_Boolean boolActionMapping = VRInput.instance.GetBoolActionMapping(action);
            if (VRInput.crouchOnIRLCrouch && action.Equals(InputAction.Crouch))
            {
                return (boolActionMapping != null && boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any)) || VRInitiator.hmd.transform.localPosition.y < VRInput.IRLCrouchBorder;
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
            if (VRInitiator.crouchOnIRLCrouch && action.Equals(InputAction.Crouch))
            {
                return (boolActionMapping != null && boolActionMapping.GetState(SteamVR_Input_Sources.Any)) || VRInitiator.hmd.transform.localPosition.y < VRInput.IRLCrouchBorder;
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
            if (VRInput.crouchOnIRLCrouch && action.Equals(InputAction.Crouch))
            {
                return boolActionMapping != null && !boolActionMapping.GetStateDown(SteamVR_Input_Sources.Any) || !boolActionMapping.GetState(SteamVR_Input_Sources.Any) && (boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any) || (VRInitiator.hmd.transform.localPosition.y > VRInput.IRLCrouchBorder));
            }
            return boolActionMapping != null && boolActionMapping.GetStateUp(SteamVR_Input_Sources.Any);
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
            return VRInput.Initialized && VRInput.instance.snapLeftAction.GetState(SteamVR_Input_Sources.Any);
        }

        public static bool GetSnapTurningRight()
        {
            return VRInput.Initialized && VRInput.instance.snapRightAction.GetState(SteamVR_Input_Sources.Any);
        }

        private void InitializeActionMapping()
        {
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


            VRInput.boolActions = new Dictionary<InputAction, SteamVR_Action_Boolean>();
            VRInput.boolActions.Add(InputAction.Jump, jumpAction);
            VRInput.boolActions.Add(InputAction.Use, interactAction);
            VRInput.boolActions.Add(InputAction.Aim, aimAction);
            VRInput.boolActions.Add(InputAction.Fire, shootAction);
            VRInput.boolActions.Add(InputAction.Run, sprintAction);
            VRInput.boolActions.Add(InputAction.Crouch, crouchAction);
            VRInput.boolActions.Add(InputAction.Reload, reloadAction);
            VRInput.boolActions.Add(InputAction.Melee, aimAction);
            VRInput.boolActions.Add(InputAction.TerminalExit, reloadAction);
            VRInput.boolActions.Add(InputAction.ScrollItems, weaponSwitchLeftAction);
            VRInput.boolActions.Add(InputAction.Flashlight, flashlightAction);
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
                if (InputAction.MoveHorizontal.Equals(action))
                {
                    return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).x;
                }
                if (InputAction.MoveVertical.Equals(action))
                {
                    return movementAxisAction.GetAxis(SteamVR_Input_Sources.Any).y;
                }
                return 0f;
            }
        }

    }

}
