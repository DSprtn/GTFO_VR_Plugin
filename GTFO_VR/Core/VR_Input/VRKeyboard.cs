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
        static bool m_chatMode = false;
        public VRKeyboard(IntPtr value) : base(value)
        {
        }

        private static string CurrentFrameInput = "";
        private static string CurrentTotalInput = "";

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
                m_chatMode = false;

                OrientKeyboard();
            } else if (state.Equals(eFocusState.FPS_TypingInChat))
            {
                SteamVR_Render.unfocusedRenderResolution = 1f;
                SteamVR.instance.overlay.ShowKeyboard(0, 0, "Chat input", 256, "", false, 0);
                OrientKeyboard();
                m_chatMode = true;
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
            Log.Debug("KB done/closed");
            if (m_chatMode)
            {
                if (PlayerChatManager.Current != null && PlayerChatManager.InChatMode)
                {
                    if(CurrentTotalInput.Length > 0)
                    {
                        PlayerChatManager.Current.m_currentValue = CurrentTotalInput;
                        PlayerChatManager.Current.PostMessage();
                    } else
                    {
                        PlayerChatManager.Current.ExitChatMode();
                    }
                }
            }
            CurrentTotalInput = "";
        }

        private void OnKeyboardInput(VREvent_t ev)
        {
            string input = "";
            if(!m_chatMode)
            {
                VREvent_Keyboard_t keyboard = ev.data.keyboard;
                byte[] inputBytes = new byte[] { keyboard.cNewInput0, keyboard.cNewInput1, keyboard.cNewInput2, keyboard.cNewInput3, keyboard.cNewInput4, keyboard.cNewInput5, keyboard.cNewInput6, keyboard.cNewInput7 };
                int len = 0;
                for (; inputBytes[len] != 0 && len < 7; len++) ;
                input = Encoding.UTF8.GetString(inputBytes, 0, len);
                input = HandleSpecialConversionAndShortcuts(input);
                CurrentFrameInput = input;
            } else
            {
                StringBuilder textBuilder = new StringBuilder(256);
                SteamVR.instance.overlay.GetKeyboardText(textBuilder, 256);
                input = textBuilder.ToString();
                CurrentTotalInput = input;
                if (KeyboardClosedThisFrame || input.Contains("\r") || input.Contains("\n"))
                {
                    input = input.Replace("\r", "").Replace("\n", "");
                    if (PlayerChatManager.Current != null && PlayerChatManager.InChatMode)
                    {
                        if(input.Length < 1)
                        {
                            PlayerChatManager.Current.ExitChatMode();
                        }
                        else
                        {
                            PlayerChatManager.Current.m_currentValue = input;
                            PlayerChatManager.Current.PostMessage();
                            Log.Debug(input);
                        }
                    }
                }
            }

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