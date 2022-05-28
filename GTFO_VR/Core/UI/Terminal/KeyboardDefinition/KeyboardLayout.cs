using System;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.KeyboardDefinition
{
    /// <summary>
    /// Interface for anything that can be inflated into a LinearLayout
    /// Layout is defined by LinearLayout and KeyDefinition objects.
    /// </summary>
    public interface KeyboardLayout
    {
        /// <summary>
        /// Generates actual GameObjects with Vertical/HorizontalLayoutGroups and PhyscialButtons
        /// </summary>
        GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle style);

        /// <summary>
        /// Children layout definitions that this object should inflate and add to itself
        /// </summary>
        void AddChild(KeyboardLayout layout);
        String GetName();

        void SetStyle(KeyboardStyle style);

    }
}
