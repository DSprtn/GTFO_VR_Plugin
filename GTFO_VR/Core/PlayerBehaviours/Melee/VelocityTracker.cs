using GTFO_VR.Core;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;
using Quaternion = UnityEngine.Quaternion;
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
            public Quaternion rotation;

            public float deltaTime;
            public float positionalVelocity = 0;
            public float angularVelocity = 0;

            public MeleeHistory(Vector3 position, Quaternion rotation, float delta)
            {
                this.position = position;
                this.rotation = rotation;
                deltaTime = delta;
            }

            public MeleeHistory(Vector3 position, Quaternion rotation, float delta, MeleeHistory previous)
            {
                this.position = position;
                this.rotation = rotation;
                deltaTime = delta;
                CalculateVelocity(previous);
            }

            public void CalculateVelocity(MeleeHistory prev)
            {
                if (prev != null)
                {
                    // Calculate velocity of position and rotation and divide by delta time
                    positionalVelocity = ((position - prev.position) / deltaTime).magnitude;
                    angularVelocity = Quaternion.Angle(prev.rotation, this.rotation) / deltaTime; // Degrees
                }
            }
        }

        private float m_targetHistoryDuration = 0.050f; // smoothed velocity is average across x seconds
        private float m_cumulativeDuration = 0;
        private float m_cumulativeVelocity = 0;
        private float m_cumulativeAngularVelocity = 0;
        private Queue<MeleeHistory> m_PositionHistory = new Queue<MeleeHistory>();
        MeleeHistory m_newestHistory = null;
        MeleeHistory m_prevHistory = null;

        public void AddPosition(Vector3 position, float delta)
        {
            AddPosition(position, Quaternion.identity, delta);
        }

        public void AddPosition(Vector3 position, Quaternion rotation, float delta)
        {
            m_prevHistory = m_newestHistory;

            // Create a new melee history, feeding it the previous history
            m_newestHistory = new MeleeHistory(position, rotation, delta, m_newestHistory);
            m_PositionHistory.Enqueue(m_newestHistory);

            // add delta and velocity to sum
            m_cumulativeDuration += m_newestHistory.deltaTime;
            m_cumulativeVelocity += m_newestHistory.positionalVelocity;
            m_cumulativeAngularVelocity += m_newestHistory.angularVelocity;

            // Remove oldest entry until quque duration is within target range, but always leave 1
            while (m_cumulativeDuration > m_targetHistoryDuration && m_PositionHistory.Count > 1)
            {
                var removed = m_PositionHistory.Dequeue();
                // Remember to remove it from duration and velocity
                m_cumulativeDuration -= removed.deltaTime;
                m_cumulativeVelocity -= removed.positionalVelocity;
                m_cumulativeAngularVelocity -= removed.angularVelocity;
            }
        }

        public float GetSmoothVelocity()
        {
            if (m_PositionHistory.Count <= 0)   // Just in case
                return 0;

            // Return average velocity
            return m_cumulativeVelocity / m_PositionHistory.Count;
        }

        public float GetSmoothAngularVelocity()
        {
            if (m_PositionHistory.Count <= 0)   // Just in case
                return 0;

            // Return average velocity
            return m_cumulativeAngularVelocity / m_PositionHistory.Count;
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
