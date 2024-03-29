﻿using System;
using UnityEngine;

namespace GTFO_VR.Events
{
    public static class ElevatorEvents
    {
        public static event Action<Vector3> OnElevatorPositionChanged;
        public static event Action<ElevatorRideState> OnElevatorStateChanged;
        public static event Action OnPreReleaseSequenceStarted;
        public static event Action OnPreReleaseSequenceSkipped;

        public static void ElevatorPositionChanged(Vector3 position)
        {
            OnElevatorPositionChanged?.Invoke(position);
        }

        public static void ElevatorStateChanged(ElevatorRideState state)
        {
            OnElevatorStateChanged?.Invoke(state);
        }

        public static void PreReleaseSequenceStarted()
        {
            OnPreReleaseSequenceStarted?.Invoke();
        }

        public static void PreReleaseSequenceSkipped()
        {
            OnPreReleaseSequenceSkipped?.Invoke();
        }
    }
}