using GTFO_VR.Core;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;
using Vector3 = UnityEngine.Vector3;

namespace GTFO_VR.Core.PlayerBehaviours.Melee
{

    /// <summary>
    /// Keeps track of a pointer position for a few frames in order to smoothen its position
    /// </summary>
    public class MeleeTracker
    {
        class MeleeHistory
        {
            public Vector3 position;
            public Vector3 localPosition; // Within player
            public Vector3 velocityVector;
            public float deltaTime;
            public float velocity = 0;

            public MeleeHistory(Vector3 position, Vector3 localPosition, float delta) : this(position, localPosition, delta, 0) { }

            public MeleeHistory(Vector3 position, Vector3 localPosition, float delta, float velocity)
            {
                this.position = position;
                this.localPosition = localPosition;
                deltaTime = delta;
            }

            public MeleeHistory(Vector3 position, Vector3 localPosition, float delta, MeleeHistory previous)
            {
                this.position = position;
                this.localPosition = localPosition;
                deltaTime = delta;
                calculateVelocity(previous);
            }

            public void calculateVelocity(MeleeHistory prev)
            {
                if (prev != null)
                {
                    // Calculate velocity of local position ( ignoring player movement ), and divide by delta time
                    velocityVector = (localPosition - prev.localPosition) / deltaTime;
                    velocity = velocityVector.magnitude;
                }
            }
        }

        //private int m_velocitySmoothingCount = 3;
        private float m_targetHistoryDuration = 0.050f; // idk 
        private float m_cumulativeDuration = 0;
        private float m_cumulativeVelocity = 0;
        private Queue<MeleeHistory> m_PositionHistory = new Queue<MeleeHistory>();
        MeleeHistory m_newestHistory = null;

        public void AddPosition(Vector3 position, Vector3 localPosition, float delta)
        {
            // Create a new melee history, feeding it the previous history
            m_newestHistory = new MeleeHistory(position, localPosition, delta, m_newestHistory);
            m_PositionHistory.Enqueue(m_newestHistory);

            // add delta and velocity to sum
            m_cumulativeDuration += m_newestHistory.deltaTime;
            m_cumulativeVelocity += m_newestHistory.velocity;

            // Remove oldest entry until quque duration is within target range, but always leave one
            while (m_cumulativeDuration > m_targetHistoryDuration && m_PositionHistory.Count > 2)
            {
                var removed = m_PositionHistory.Dequeue();
                // Remember to remove it from duration and velocity
                m_cumulativeDuration -= removed.deltaTime;
                m_cumulativeVelocity -= removed.velocity;
            }
        }

        public float GetSmoothVelocity()
        {
            if (m_PositionHistory.Count <= 0)   // Just in case
                return 0;

            // Return average velocity
            return m_cumulativeVelocity / m_PositionHistory.Count;
        }

        public Vector3 getVelocityVector()
        {
            return m_newestHistory != null ? m_newestHistory.velocityVector : Vector3.zero;
        }

        public void ClearPointerHistory()
        {
            m_PositionHistory.Clear();
        }
    }
}
