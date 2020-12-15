//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Valve.VR
{
    public enum SteamVR_Input_Sources
    {
        
        Any,


        LeftHand,


        RightHand,


        LeftFoot,


        RightFoot,


        LeftShoulder,


        RightShoulder,


        Waist,


        Chest,


        Head,


        Gamepad,


        Camera,


        Keyboard,


        Treadmill,
    }
}

namespace Valve.VR.InputSources
{
    using Sources = SteamVR_Input_Sources;
}