using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(ElevatorRide), nameof(ElevatorRide.Update))]
    internal class InjectElevatorRideUpdateEvents
    {
        private static ElevatorRideState m_previousElevatorRideState;

        private static void Postfix(ElevatorRide __instance)
        {
            ElevatorRideState currentState = __instance.m_currentState;
            if (m_previousElevatorRideState != currentState)
            {
                m_previousElevatorRideState = currentState;
                ElevatorEvents.ElevatorStateChanged(currentState);
            }
        }
    }

    [HarmonyPatch(typeof(ElevatorCable), nameof(ElevatorCable.UpdateElevatorPos))]
    internal class InjectUpdateElevatorPosEvents
    {
        private static Vector3 m_previousPos;

        private static void Postfix(Vector3 pos)
        {
            if (pos != m_previousPos)
            {
                ElevatorEvents.ElevatorPositionChanged(pos);
                m_previousPos = pos;
            }
        }
    }
}