using GTFO_VR.Events;
using HarmonyLib;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(ElevatorRide), nameof(ElevatorRide.StartElevatorRide))]
    internal class InjectElevatorStartRideEvents
    {
        private static void Postfix()
        {
            ElevatorEvents.ElevatorRideStarted();
        }
    }

    [HarmonyPatch(typeof(ElevatorRide), nameof(ElevatorRide.StopElevatorRide))]
    internal class InjectElevatorStopRideEvents
    {
        private static void Postfix()
        {
            ElevatorEvents.ElevatorRideStopped();
        }
    }
}