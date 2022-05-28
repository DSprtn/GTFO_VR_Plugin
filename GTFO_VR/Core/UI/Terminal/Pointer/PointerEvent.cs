using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{
    public struct PointerEvent
    {
        public Vector3 Position;
        public int PointerId;

        public PointerEvent( Vector3 position )
        {
            this.Position = position;
            PointerId = -1;
        }

        /*
        public interface IPointerEvent
        {
            void OnPointerEnter(PointerEvent ev);
            void OnPointerExit(PointerEvent ev);
            void onPointerMove(PointerEvent ev);
            void onPointerDown(PointerEvent ev);
            void onPointerUp(PointerEvent ev);
        }
        */
    }
}
