namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public enum ElevatorState
    {
        None,
        SceneLoaded,
        FirstMovement,
        PendingCageRotating,
        CageRotating,
        PendingDoorOpening,
        PendingTopDeploying,
        TopDeploying,
        Preparing,
        FirstDescentPattern,
        Descending,
        SlowingDown,
        Landed,
        Deploying,
    }
}