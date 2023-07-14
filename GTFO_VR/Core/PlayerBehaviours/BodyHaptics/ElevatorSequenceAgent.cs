namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public interface ElevatorSequenceAgent
    {
        bool AgentActive();
        void Update();
        void ElevatorStateChanged(ElevatorState elevatorState);
        void SetIsInElevator(bool inElevator);
    }
}