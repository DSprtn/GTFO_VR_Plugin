using GTFO_VR.UI.CANVAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GTFO_VR.Core.UI.canvas.KeyboardDefinition
{
    class RectColliderSizer : UIBehaviour
    {
        protected override void OnRectTransformDimensionsChange()
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            RectTransform trans = GetComponent<RectTransform>();

            // multiplier is Temporary until I can figure out a better way of adding borders
            collider.size = new Vector3(trans.sizeDelta.x * TerminalKeyboardInterface.HITBOX_SCALE, trans.sizeDelta.y * TerminalKeyboardInterface.HITBOX_SCALE, collider.size.z);
        }


    }
}
