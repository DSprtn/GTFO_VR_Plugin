using System;

namespace GTFO_VR.Events
{
    public static class ElevatorEvents
    {
        public static event Action OnElevatorRideStarted;
        public static event Action OnElevatorRideStopped;

        public static void ElevatorRideStarted()
        {
            OnElevatorRideStarted?.Invoke();
        }

        public static void ElevatorRideStopped()
        {
            OnElevatorRideStopped?.Invoke();
        }
    }
}