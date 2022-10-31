namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public interface ElevatorSequenceAgent
    {
        void Update();
        void ElevatorStateChanged(ElevatorState elevatorState);
    }
}