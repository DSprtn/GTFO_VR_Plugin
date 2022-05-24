using GTFO_VR.Core.UI.Canvas.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.Core.UI.canvas.Pointer
{
    // Poor man's interface
    public class MonoPointerEvent : MonoBehaviour
    {

        [HideFromIl2Cpp]
        public virtual void OnPointerEnter(PointerEvent ev)
        {

        }

        [HideFromIl2Cpp]
        public virtual void OnPointerExit(PointerEvent ev)
        {

        }

        [HideFromIl2Cpp]
        public virtual Vector3 onPointerMove(PointerEvent ev)
        {
            return ev.position;
        }

        [HideFromIl2Cpp]
        public virtual void onPointerDown(PointerEvent ev)
        {

        }

        [HideFromIl2Cpp]
        public virtual void onPointerUp(PointerEvent ev)
        {

        }

        [HideFromIl2Cpp]
        public virtual float getPointerSize( float defaultSize )
        {
            return defaultSize;
        }

    }
}
