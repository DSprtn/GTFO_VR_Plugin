using GTFO_VR.Core.UI.Canvas;
using GTFO_VR.Core.UI.Canvas.KeyboardDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.KeyboardDefinition
{
    public interface KeyboardLayout
    {
        GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle style);
        void AddChild(KeyboardLayout layout);
        String GetName();

        void SetStyle(KeyboardStyle style);

    }
}
