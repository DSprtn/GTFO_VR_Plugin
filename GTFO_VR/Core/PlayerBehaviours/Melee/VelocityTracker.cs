using GTFO_VR.Core;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;
using Vector3 = UnityEngine.Vector3;

namespace GTFO_VR.Core.PlayerBehaviours.Melee
{

    /// <summary>
    /// Keeps track of a position for a few frames in order to smoothen position and calculate velocity/vector
    /// </summary>
    public class VelocityTracker
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
                CalculateVelocity(previous);
            }

            public void CalculateVelocity(MeleeHistory prev)
            {
                if (prev != null)
                {
                    // Calculate velocity of local position ( ignoring player movement ), and divide by delta time
                    velocityVector = (localPosition - prev.localPosition) / deltaTime;
                    velocity = velocityVector.magnitude;
                }
            }
        }

        private float m_targetHistoryDuration = 0.050f; // smoothed velocity is average across x seconds
        private float m_cumulativeDuration = 0;
        private float m_cumulativeVelocity = 0;
        private Queue<MeleeHistory> m_PositionHistory = new Queue<MeleeHistory>();
        MeleeHistory m_newestHistory = null;
        MeleeHistory m_prevHistory = null;

        public void AddPosition(Vector3 position, Vector3 localPosition, float delta)
        {
            m_prevHistory = m_newestHistory;

            // Create a new melee history, feeding it the previous history
            m_newestHistory = new MeleeHistory(position, localPosition, delta, m_newestHistory);
            m_PositionHistory.Enqueue(m_newestHistory);

            // add delta and velocity to sum
            m_cumulativeDuration += m_newestHistory.deltaTime;
            m_cumulativeVelocity += m_newestHistory.velocity;

            // Remove oldest entry until quque duration is within target range, but always leave 2
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

        // Returns only the latest vector
        public Vector3 GetVelocityVector()
        {
            return m_newestHistory != null ? m_newestHistory.velocityVector : Vector3.zero;
        }

        public Vector3 GetLatestPosition()
        {
            return m_newestHistory != null ? m_newestHistory.position : Vector3.zero;
        }

        public Vector3 getPreviousPosition()
        {
            return m_prevHistory != null ? m_prevHistory.position : Vector3.zero;
        }

        public void ClearTrackerHistory()
        {
            m_newestHistory = null;
            m_prevHistory = null;
            m_cumulativeDuration = 0;
            m_cumulativeVelocity = 0;
            m_PositionHistory.Clear();
        }
    }
}
