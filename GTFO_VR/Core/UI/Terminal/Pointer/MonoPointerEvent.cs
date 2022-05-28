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
        public virtual Vector3 OnPointerMove(PointerEvent ev)
        {
            return ev.Position;
        }

        [HideFromIl2Cpp]
        public virtual void OnPointerDown(PointerEvent ev)
        {

        }

        [HideFromIl2Cpp]
        public virtual void OnPointerUp(PointerEvent ev)
        {

        }

        [HideFromIl2Cpp]
        public virtual float GetPointerSize( float defaultSize )
        {
            return defaultSize;
        }

    }
}
