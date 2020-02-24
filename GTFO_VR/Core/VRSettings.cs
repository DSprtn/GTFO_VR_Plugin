using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Core
{
    public static class VRSettings
    {
        public static bool crouchOnIRLCrouch = true;

        public static TrackingType VR_TRACKING_TYPE = TrackingType.PositionAndRotation;

        public static HandType mainHand = HandType.Right;

        public static bool UseVRControllers = true;

        public static int lightRenderMode = 0;
    }
}
