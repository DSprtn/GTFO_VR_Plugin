using GTFO_VR.Events;
using System;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.VR_Input
{
    /// <summary>
    /// Responsible for managing VR keyboard input, shortcuts, appearing and disappearing.
    /// </summary>
    public class VRKeyboard : MonoBehaviour
    {
        public VRKeyboard(IntPtr value) : base(value)
        {
        }

        private static string CurrentFrameInput = "";

        public static bool KeyboardClosedThisFrame;

        private void Awake()
        {
            SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(OnKeyboardInput);
            SteamVR_Events.System(EVREventType.VREvent_KeyboardDone).Listen(OnKeyboardDone);
            SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(OnKeyboardDone);

            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
        }

        private void FocusStateChanged(eFocusState state)
        {
            if (state.Equals(eFocusState.ComputerTerminal))
            {
                SteamVR_Render.unfocusedRenderResolution = 1f;
                SteamVR.instance.overlay.ShowKeyboard(0, 0, "Terminal input", 256, "", true, 0);

                OrientKeyboard();
            }
            else
            {
                SteamVR.instance.overlay.HideKeyboard();
                SteamVR_Render.unfocusedRenderResolution = .5f;
            }
        }

        public void OnKeyboardDone(VREvent_t arg0)
        {
            KeyboardClosedThisFrame = true;
        }

        private void OnKeyboardInput(VREvent_t ev)
        {
            VREvent_Keyboard_t keyboard = ev.data.keyboard;
            byte[] inputBytes = new byte[] { keyboard.cNewInput0, keyboard.cNewInput1, keyboard.cNewInput2, keyboard.cNewInput3, keyboard.cNewInput4, keyboard.cNewInput5, keyboard.cNewInput6, keyboard.cNewInput7 };
            int len = 0;
            for (; inputBytes[len] != 0 && len < 7; len++) ;
            string input = Encoding.UTF8.GetString(inputBytes, 0, len);
            input = HandleSpecialConversionAndShortcuts(input);

            CurrentFrameInput = input;
        }

        public static string GetKeyboardInput()
        {
            if (CurrentFrameInput == null)
            {
                return "";
            }
            return CurrentFrameInput;
        }

        private static void OrientKeyboard()
        {
            Quaternion Rot = Quaternion.Euler(Vector3.Project(HMD.Hmd.transform.localRotation.eulerAngles, Vector3.up));
            Vector3 Pos = HMD.Hmd.transform.localPosition + Rot * Vector3.forward * 1f;
            Pos.y = HMD.Hmd.transform.localPosition.y + .5f;
            Rot = Quaternion.Euler(0f, Rot.eulerAngles.y, 0f);
            var t = new SteamVR_Utils.RigidTransform(Pos, Rot).ToHmdMatrix34();
            SteamVR.instance.overlay.SetKeyboardTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, ref t);
        }

        private void LateUpdate()
        {
            CurrentFrameInput = "";
            KeyboardClosedThisFrame = false;
        }

        // ToDo - Add external config for this?
        // ToDO - Make better shortcuts, some kind of in-game UI?
        private string HandleSpecialConversionAndShortcuts(string input)
        {
            switch (input)
            {
                case "\n":
                    {
                        return "\r";
                    }

                case "-":
                    {
                        return "_";
                    }
                case "~":
                    {
                        return "-";
                    }
                case "L":
                    {
                        return "LIST ";
                    }
                case "Q":
                    {
                        return "QUERY ";
                    }
                case "R":
                    {
                        return "REACTOR";
                    }
                case "H":
                    {
                        return "HELP";
                    }
                case "C":
                    {
                        return "COMMANDS";
                    }
                case "V":
                    {
                        return "REACTOR_VERIFY ";
                    }
                case "P":
                    {
                        return "PING ";
                    }
                case "A":
                    {
                        return "AMMOPACK_";
                    }
                case "T":
                    {
                        return "TOOL_REFILL_";
                    }
                case "M":
                    {
                        return "MEDIPACK_";
                    }
                case "Z":
                    {
                        return "ZONE_";
                    }
                case "U":
                    {
                        return "UPLINK_VERIFY ";
                    }
            }
            return input;
        }

        private void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
        }
    }
}