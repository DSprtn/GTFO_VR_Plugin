using GTFO_VR.Events;
using GTFO_VR.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core
{
    public class VR_Keyboard : MonoBehaviour
    {
        static string currentFrameInput = "";

        public static bool keyboardClosedThisFrame;


        void Awake()
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
            keyboardClosedThisFrame = true;
        }

        private void OnKeyboardInput(VREvent_t ev)
        {
            VREvent_Keyboard_t keyboard = ev.data.keyboard;
            byte[] inputBytes = new byte[] { keyboard.cNewInput0, keyboard.cNewInput1, keyboard.cNewInput2, keyboard.cNewInput3, keyboard.cNewInput4, keyboard.cNewInput5, keyboard.cNewInput6, keyboard.cNewInput7 };
            int len = 0;
            for (; inputBytes[len] != 0 && len < 7; len++) ;
            string input = System.Text.Encoding.UTF8.GetString(inputBytes, 0, len);
            input = HandleSpecialConversionAndShortcuts(input);

            currentFrameInput = input;
        }

        public static string GetKeyboardInput()
        {
            return currentFrameInput;
        }

        private static void OrientKeyboard()
        {
            Quaternion Rot = Quaternion.Euler(Vector3.Project(HMD.hmd.transform.localRotation.eulerAngles, Vector3.up));
            Vector3 Pos = HMD.hmd.transform.localPosition + Rot * Vector3.forward * 1f;
            Pos.y = HMD.hmd.transform.localPosition.y + .5f;
            Rot = Quaternion.Euler(0f, Rot.eulerAngles.y, 0f);
            var t = new SteamVR_Utils.RigidTransform(Pos, Rot).ToHmdMatrix34();
            SteamVR.instance.overlay.SetKeyboardTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, ref t);
        }

        void LateUpdate()
        {
            currentFrameInput = "";
            keyboardClosedThisFrame = false;
        }

        private string HandleSpecialConversionAndShortcuts(string input)
        {
            switch (input)
            {
                case ("\n"):
                    {
                        return "\r";
                    }
                case ("-"):
                    {
                        return "_";
                    }
                case ("L"):
                    {
                        return "LIST ";
                    }
                case ("Q"):
                    {
                        return "QUERY ";
                    }
                case ("R"):
                    {
                        return "REACTOR";
                    }
                case ("H"):
                    {
                        return "HELP";
                    }
                case ("C"):
                    {
                        return "COMMANDS";
                    }
                case ("V"):
                    {
                        return "REACTOR_VERIFY ";
                    }
                case ("P"):
                    {
                        return "PING ";
                    }
                case ("A"):
                    {
                        return "AMMOPACK";
                    }
                case ("T"):
                    {
                        return "TOOL_REFILL";
                    }
                case ("M"):
                    {
                        return "MEDIPACK";
                    }
                case ("Z"):
                    {
                        return "ZONE_";
                    }
                case ("U"):
                    {
                        return "UPLINK_VERIFY ";
                    }
            }
            return input;
        }
        

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
        }

    }
}
