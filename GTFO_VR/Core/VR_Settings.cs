using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core
{
    public static class VR_Settings
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

        public static bool enabled = true;

        public static bool useNumbersForAmmoDisplay = false;

        public static bool Render2DUI = false;

        public static Color watchColor = Color.white;

    }
}
