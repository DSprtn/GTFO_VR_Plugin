using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{
    public class PointerEvent
    {
        public Vector3 position = Vector3.zero;
        public int pointerId = -1;

        public PointerEvent( Vector3 position )
        {
            this.position = position;
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
