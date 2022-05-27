using System;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.KeyboardDefinition
{
    public interface KeyboardLayout
    {
        GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle style);
        void AddChild(KeyboardLayout layout);
        String GetName();

        void SetStyle(KeyboardStyle style);

    }
}
