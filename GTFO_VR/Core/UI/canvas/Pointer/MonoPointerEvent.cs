using GTFO_VR.Core.UI.Canvas.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core.UI.canvas.Pointer
{
    // Poor man's interface
    public class MonoPointerEvent : MonoBehaviour
    {

        public virtual void OnPointerEnter(PointerEvent ev)
        {

        }
        public virtual void OnPointerExit(PointerEvent ev)
        {

        }

        // Return desired pointer position

        public virtual Vector3 onPointerMove(PointerEvent ev)
        {
            return Vector3.zero;
        }
        public virtual void onPointerDown(PointerEvent ev)
        {

        }
        public virtual void onPointerUp(PointerEvent ev)
        {

        }

    }
}
