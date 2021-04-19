using GTFO_VR.Core.VR_Input;
using UnityEngine;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Responsible for serving as a middle-man to the config file.
    /// </summary>
    public static class VRSettings
    {
        public static bool crouchOnIRLCrouch = true;

        public static TrackingType VR_TRACKING_TYPE = TrackingType.PositionAndRotation;

        public static HandType mainHand = HandType.Right;

        public static bool useVRControllers = true;

        public static int lightRenderMode = 0;

        public static bool twoHandedAimingEnabled = true;

        public static bool disableCompass = true;

        public static bool alwaysDoubleHanded = false;

        public static float snapTurnAmount = 90f;

        public static bool useSmoothTurn = false;

        public static float watchScale = 1f;

        public static bool toggleVRBySteamVRRunning = true;

        public static bool VREnabled = true;

        public static bool useNumbersForAmmoDisplay = false;

        public static bool Render2DUI = false;

        public static Color watchColor = Color.white;

        public static Color laserPointerColor = ColorExt.OrangeBright();

        public static float IRLCrouchBorder = 1.15f;

        public static bool alternateLightRenderingPerEye = false;

        public static bool recenterOnSmoothTurn = true;

        public static bool useLaserPointer = true;

        public static bool useHapticForShooting = true;

        public static float shootingHapticsStrength = .75f;
    }
}
