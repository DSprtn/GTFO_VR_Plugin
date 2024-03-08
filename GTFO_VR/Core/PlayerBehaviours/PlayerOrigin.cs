using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using Player;
using System;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Responsible for managing the player's position and rotation within the playspace and within GTFO's systems.
    /// </summary>
    public class PlayerOrigin : MonoBehaviour
    {
        public PlayerOrigin(IntPtr value)
       : base(value) { }

        public static event Action OnOriginShift;

        private LocalPlayerAgent m_agent;

        private Vector3 m_offsetFromPlayerToHMD = Vector3.zero;

        private Vector3 m_roomscaleOffset = Vector3.zero;

        Quaternion PlayerRotationOffset = Quaternion.identity;

        private bool m_shouldRecenter = true;

        private void Awake()
        {
            Log.Info("Origin created");
        }

        public void Setup(LocalPlayerAgent agent)
        {
            m_agent = agent;
            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
            Snapturn.OnSnapTurn += HandleOriginShift;
            SetupOrigin();
            SetInitialRotationOffset();
        }

        private void SetupOrigin()
        {
            Controllers.SetOrigin(transform);
            HMD.SetOrigin(transform);
            DontDestroyOnLoad(gameObject);
            Log.Info("Origin created and set");
        }

        private void FixedUpdate()
        {
            updateRoomscale();
        }

        private void Update()
        {
            UpdateOrigin();

            if (m_shouldRecenter)
            {
                // When the player respawns from a checkpoint, the player is recreated.
                // The player should be centerd as part of this process, but performing it
                // too early ( e.g. in Start() ) will not work, resulting in the player 
                // being stuck in a wall if they're very far away from their center.
                m_shouldRecenter = false;
                CenterPlayerToOrigin();
            }
        }

        private void LateUpdate()
        {
            UpdateOrigin();
        }

        private void FocusStateChanged(eFocusState newState)
        {
            if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && newState.Equals(eFocusState.FPS))
            {
                CenterPlayerToOrigin();
            }

            if (newState.Equals(eFocusState.InElevator))
            {
                SetInitialRotationOffset();
            }
        }

        private void HandleOriginShift()
        {
            m_roomscaleOffset = Vector3.zero;
            UpdateOrigin();
            CenterPlayerToOrigin();
            OnOriginShift?.Invoke();
        }

        public void RotatePlayer(Quaternion rotation)
        {
            PlayerRotationOffset *= rotation;
        }

        private void SetInitialRotationOffset()
        {
            m_offsetFromPlayerToHMD = Vector3.zero;
            m_roomscaleOffset = Vector3.zero;
            PlayerRotationOffset = Quaternion.Euler(new Vector3(0, -HMD.Hmd.transform.localRotation.eulerAngles.y, 0f));
            UpdateOrigin();
        }

        public void UpdateOrigin()
        {
            if (m_agent.PlayerCharacterController == null)
            {
                return;
            }
            Vector3 newPosition = m_agent.PlayerCharacterController.SmoothPosition + new Vector3(0, VRConfig.configFloorOffset.Value / 100f, 0);

            transform.position = newPosition - m_offsetFromPlayerToHMD;
            transform.rotation = PlayerRotationOffset;
            transform.position -= CalculateCrouchOffset();

            // Player is moved, and origin is shifted in the opposite direction
            // so the camera ends up in the same position. 
            transform.position += m_roomscaleOffset;
        }

        private Vector3 getTorsoPosition()
        {
            Vector3 hmdPos = HMD.Hmd.transform.position;
            // Player's center will be a bit behind the HMD position, especially if they're leaning forwards.
            Vector3 hmdForwardFlat = Vector3.Cross(HMD.Hmd.transform.right, new Vector3(0, 1, 0)); // HMD direction on horizontal plane
            hmdPos += hmdForwardFlat * -0.2f;

            return hmdPos;
        }


        public void updateRoomscale()
        {
            // Calculates vector between HMD and player position, and decides if we can 
            // move player to HMD without messing anything up.
            // If we can, player is moved with ManualMoveToWithCollision(), and the origin
            // is shifted so the player doesn't notice that they moved.

            if (m_agent.PlayerCharacterController == null)
            {
                return;
            }

            switch (m_agent.Locomotion.m_currentStateEnum)
            {
                // These are basically the only normal locomotion states
                // If you're not running and not crouched, you're standing.
                // that includes walking
                case PlayerLocomotion.PLOC_State.Stand:
                case PlayerLocomotion.PLOC_State.Run:
                case PlayerLocomotion.PLOC_State.Crouch:
                {
                    break;
                }

                default:
                {
                    return;
                }
            }

            // If a player is teleported, the player model may be far far awy from the
            // VR origin when this is called, so ensure origin is updated first.
            UpdateOrigin();

            Vector3 playerPos = m_agent.PlayerCharacterController.SmoothPosition; // Player center
            Vector3 hmdPos = HMD.Hmd.transform.position; // This takes into account existing offset

            #if DEBUG_GTFO_VR
            if (VRConfig.configDebugOrigin.Value)
            {
                // original HMD position at player height. We shift it backwards a bit below, but still refer to it as HMD.
                GTFODebugDraw3D.DrawSphere(new Vector3(hmdPos.x, playerPos.y, hmdPos.z), .015f, ColorExt.Green(0.5f), renderOntop: true);
                // Player position
                GTFODebugDraw3D.DrawSphere(playerPos, .05f, ColorExt.Red(0.5f));
            }
            #endif

            // Offset to roughly where the player torso would actually be
            hmdPos = getTorsoPosition();

            // Vector from player to HMD
            Vector3 playerHmdOffset = hmdPos - playerPos;
            float hmdHeightFromPlayer = playerHmdOffset.y;  // Need this for ground raycast
            playerHmdOffset.y = 0;

            // Player movement.
            // This includes gravity, so need to clear that
            Vector3 playerVelocity = m_agent.PlayerCharacterController.m_lastAttemptedFixedMove;
            playerVelocity.y = 0;

            // Threshold for player to be moved to HMD location is higher
            // if player is not moving, to prevent accidental movement.
            bool playerMoving = playerVelocity.magnitude > 0.01f;
            float distanceThreshold = playerMoving ? 0.05f : 1f;
            bool shouldUpdate = playerHmdOffset.magnitude > distanceThreshold;

            if (playerHmdOffset.magnitude > 15.0f)
            {
                // Sanity check, move HMD to player if we're impossibly far away.
                // This shouldn't happen.
                Log.Error("Roomscale tried to move player by more than 15m!: " + playerHmdOffset.magnitude);
                CenterPlayerToOrigin();
                return;
            }

            #if DEBUG_GTFO_VR
            if (VRConfig.configDebugOrigin.Value)
            {
                // HMD position again, but pushed a bit backwards to it's closer to the person's actual center.
                GTFODebugDraw3D.DrawSphere(playerPos + playerHmdOffset, .025f, ColorExt.Green(0.5f), renderOntop: true);

                // Cube representing distance at which we will move the player
                Vector3 thresholdPosition = playerPos + (playerHmdOffset.normalized * distanceThreshold);
                GTFODebugDraw3D.DrawCube(thresholdPosition, Quaternion.identity, new Vector3(.05f, .05f, .05f), ColorExt.Blue(0.5f), renderOntop: true);
            }
            #endif

            if (shouldUpdate)
            {
                // Moving the player should ideally never result in them falling off a ledge.
                // However, we also don't want them to be able to walk in the air.
                bool validHmdTarget = false;
                // Raycast donwards from HMD location, and only attempt to move player 
                // if the drop is less than 0.3m, or HMD more than 1.5m from player

                // We cast from the HMD position down to player position ( feet ) + max fall distance
                float castDistance = Math.Abs(hmdHeightFromPlayer) + 0.3f;
                RaycastHit hit;
                if (Physics.SphereCast(
                    hmdPos,
                    .1f,                // size, change to same as character collider
                    new Vector3(0, -1, 0),// down
                    out hit,
                    castDistance,               // distance 
                    LayerManager.MASK_WORLD,
                    QueryTriggerInteraction.Ignore))
                {
                    // Not much of a drop, go ahead
                    validHmdTarget = true;
                }
                else
                {
                    float dropOffCliffThreshold = 1.75f;

                    #if DEBUG_GTFO_VR
                    if (VRConfig.configDebugOrigin.Value)
                    {
                        // Tip of raycast. You will only see this if it didn't hit anything.
                        Vector3 raycastPosition = hmdPos + new Vector3(0, -castDistance, 0);
                        GTFODebugDraw3D.DrawSphere(raycastPosition, .025f, ColorExt.Red(0.25f), renderOntop: true);

                        // Marker for 1.75m, where we move even if there's a ledge
                        Vector3 thresholdPosition = playerPos + (playerHmdOffset.normalized * dropOffCliffThreshold);
                        GTFODebugDraw3D.DrawCube(thresholdPosition, Quaternion.identity, new Vector3(.05f, .05f, .05f), ColorExt.Blue(0.5f), renderOntop: true);
                    }
                    #endif

                    // general, we're good as long as we hit something.
                    // However, to avoid the player walking on air, we should move them if
                    // they're more than 1.75m away from 
                    if (playerHmdOffset.magnitude > dropOffCliffThreshold)
                    {
                        validHmdTarget = true;
                    }
                }

                if (validHmdTarget)
                {

                    // Ask game to move player, check how far they actually moved, and use that to
                    // set that roomscale offset that will shift the origin by the opposite amount.
                    // This way the player viewpoint stays in the exact same location.

                    // While ManualMoveToWithCollision() effectively teleports the player,
                    // sleepers are still alerted, as detection is based on the distance the 
                    // player transform moves, calculated in the respective PLOC states.

                    // ManualMoveToWithCollision() will zero out the delta, so store it first
                    Vector3 originalMoveDelta = m_agent.PlayerCharacterController.m_moveDelta;

                    Vector3 origPlayerPos = m_agent.PlayerCharacterController.m_smoothPosition;
                    m_agent.PlayerCharacterController.ManualMoveToWithCollision(playerHmdOffset);
                    Vector3 newPlayerPos = m_agent.PlayerCharacterController.m_smoothPosition;

                    // Restore original delta
                    m_agent.PlayerCharacterController.m_moveDelta = originalMoveDelta;

                    Vector3 newOriginOffset = (origPlayerPos - newPlayerPos);
                    newOriginOffset.y = 0; // Just in case

                    // Add new offset to existing offset
                    m_roomscaleOffset += newOriginOffset;
                }

            }
        }

        private Vector3 CalculateCrouchOffset()
        {
            if (m_agent && m_agent.Locomotion.m_currentStateEnum.Equals(PlayerLocomotion.PLOC_State.Crouch))
            {
                float goalCrouchHeight = VRConfig.configCrouchHeight.Value / 100f;

                float diff = Mathf.Max(0f, HMD.GetPlayerHeight() - goalCrouchHeight + VRConfig.configFloorOffset.Value / 100f);
                return new Vector3(0, diff, 0);
            }
            return Vector3.zero;
        }

        public void CenterPlayerToOrigin()
        {
            Vector3 pos = HMD.Hmd.transform.localPosition;
            pos.y = 0f;
            pos = PlayerRotationOffset * pos;
            m_offsetFromPlayerToHMD = pos;
            // Apply offset so we center the torso rather than the head.
            // We use torso position when roomscale moving player, so this need to match.
            m_roomscaleOffset = HMD.Hmd.transform.position - getTorsoPosition();
        }

        private void OnDestroy()
        {
            Controllers.OnOriginDestroyed();
            HMD.OnOriginDestroyed();
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
            Snapturn.OnSnapTurn += HandleOriginShift;
        }
    }
}