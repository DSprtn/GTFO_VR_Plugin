using Il2CppInterop.Runtime.Attributes;
using System;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{
    /// <summary>
    /// We don't get to implement our own interfaces, 
    /// so our terminal keyboard interactables ( buttons, reader ) all inherit from this instead
    /// </summary>
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

        /// <summary>
        /// Move position called after OnPointer Enter.
        /// Implementation may return a different position to be used to display the pointer location.
        /// </summary>
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

        /// <summary>
        /// The event was cancelled. Target should revert to its default state.
        /// </summary>
        [HideFromIl2Cpp]
        public virtual void OnPointerCancel(PointerEvent ev)
        {

        }

        /// <summary>
        /// The event was cancelled. Target should revert to its default state.
        /// </summary>
        [HideFromIl2Cpp]
        public virtual void OnFocusLost(PointerEvent ev)
        {

        }

        /// <summary>
        /// Implementation may provide a different pointer size
        /// </summary>
        [HideFromIl2Cpp]
        public virtual float GetPointerSize( float defaultSize )
        {
            return defaultSize;
        }

    }
}
