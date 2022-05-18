using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core.UI.canvas.Pointer
{
    public class PointerHistory
    {
        private static readonly int POINTER_SMOOTHING_COUNT = 20;
        Queue<Vector3> m_pointerHistory = new Queue<Vector3>();

        public void addPointerHistory(Vector3 position)
        {
            m_pointerHistory.Enqueue(position);
            if (m_pointerHistory.Count >= POINTER_SMOOTHING_COUNT)
            {
                m_pointerHistory.Dequeue();
            }
        }

        public Vector3 getSmoothenedPointerPosition()
        {
            Vector3 smoothed = Vector3.zero;
            int count = 0;
            foreach (Vector3 position in m_pointerHistory)
            {
                smoothed += position;
                count++;
            }

            return smoothed / count;
        }

        public void clearPointerHistory()
        {
            m_pointerHistory.Clear();
        }
    }
}
