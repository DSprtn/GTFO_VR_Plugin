using System.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal.Pointer
{

    /// <summary>
    /// Keeps track of a pointer position for a few frames in order to smoothen its position
    /// </summary>
    public class PointerHistory
    {
        private static readonly int POINTER_SMOOTHING_COUNT = 5;
        private Queue<Vector3> m_pointerHistory = new Queue<Vector3>();

        public void AddPointerHistory(Vector3 position)
        {
            m_pointerHistory.Enqueue(position);
            if (m_pointerHistory.Count >= POINTER_SMOOTHING_COUNT)
            {
                m_pointerHistory.Dequeue();
            }
        }

        public Vector3 GetSmoothenedPointerPosition()
        {
            Vector3 smoothed = Vector3.zero;
            int count = 0;
            foreach (Vector3 position in m_pointerHistory)
            {
                smoothed += position;
                count++;
            }

            if (count <= 0)   // Just incase
                return smoothed;

            return smoothed / count;
        }

        public void ClearPointerHistory()
        {
            m_pointerHistory.Clear();
        }
    }
}
