using System;

namespace Valve.VR
{
    public class SteamVR_Input_ActionSet_default : SteamVR_ActionSet
    {
        public virtual SteamVR_Action_Pose Pose
        {
            get
            {
                return SteamVR_Actions.default_Pose;
            }
        }

        public virtual SteamVR_Action_Skeleton SkeletonLeftHand
        {
            get
            {
                return SteamVR_Actions.default_SkeletonLeftHand;
            }
        }

        public virtual SteamVR_Action_Skeleton SkeletonRightHand
        {
            get
            {
                return SteamVR_Actions.default_SkeletonRightHand;
            }
        }

        public virtual SteamVR_Action_Single Squeeze
        {
            get
            {
                return SteamVR_Actions.default_Squeeze;
            }
        }

        public virtual SteamVR_Action_Boolean HeadsetOnHead
        {
            get
            {
                return SteamVR_Actions.default_HeadsetOnHead;
            }
        }

        public virtual SteamVR_Action_Boolean SnapTurnLeft
        {
            get
            {
                return SteamVR_Actions.default_SnapTurnLeft;
            }
        }

        public virtual SteamVR_Action_Boolean SnapTurnRight
        {
            get
            {
                return SteamVR_Actions.default_SnapTurnRight;
            }
        }

        public virtual SteamVR_Action_Vector2 Movement
        {
            get
            {
                return SteamVR_Actions.default_Movement;
            }
        }

        public virtual SteamVR_Action_Boolean ToggleWatchMode
        {
            get
            {
                return SteamVR_Actions.default_ToggleWatchMode;
            }
        }

        public virtual SteamVR_Action_Boolean Shoot
        {
            get
            {
                return SteamVR_Actions.default_Shoot;
            }
        }

        public virtual SteamVR_Action_Boolean Interact
        {
            get
            {
                return SteamVR_Actions.default_Interact;
            }
        }

        public virtual SteamVR_Action_Boolean Jump
        {
            get
            {
                return SteamVR_Actions.default_Jump;
            }
        }

        public virtual SteamVR_Action_Boolean Crouch
        {
            get
            {
                return SteamVR_Actions.default_Crouch;
            }
        }

        public virtual SteamVR_Action_Boolean WeaponSwitchLeft
        {
            get
            {
                return SteamVR_Actions.default_WeaponSwitchLeft;
            }
        }

        public virtual SteamVR_Action_Boolean WeaponSwitchRight
        {
            get
            {
                return SteamVR_Actions.default_WeaponSwitchRight;
            }
        }

        public virtual SteamVR_Action_Boolean ToggleFlashlight
        {
            get
            {
                return SteamVR_Actions.default_ToggleFlashlight;
            }
        }

        public virtual SteamVR_Action_Boolean Sprint
        {
            get
            {
                return SteamVR_Actions.default_Sprint;
            }
        }

        public virtual SteamVR_Action_Boolean Reload
        {
            get
            {
                return SteamVR_Actions.default_Reload;
            }
        }

        public virtual SteamVR_Action_Boolean OpenMap
        {
            get
            {
                return SteamVR_Actions.default_OpenMap;
            }
        }

        public virtual SteamVR_Action_Boolean OpenMenu
        {
            get
            {
                return SteamVR_Actions.default_OpenMenu;
            }
        }

        public virtual SteamVR_Action_Boolean Ping
        {
            get
            {
                return SteamVR_Actions.default_Ping;
            }
        }

        public virtual SteamVR_Action_Vibration Haptic
        {
            get
            {
                return SteamVR_Actions.default_Haptic;
            }
        }
    }
}
