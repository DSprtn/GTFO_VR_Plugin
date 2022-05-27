using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{
    // Poor man's interface
    public class MonoPointerEvent : MonoBehaviour
    {

        public MonoPointerEvent(IntPtr value) : base(value) { }

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
