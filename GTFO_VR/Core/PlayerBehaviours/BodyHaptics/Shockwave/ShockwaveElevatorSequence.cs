using System.Collections.Generic;
using System.Threading.Tasks;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave.Engine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave
{
    public class ShockwaveElevatorSequence : ElevatorSequenceAgent
    {
        private ElevatorState m_elevatorState = ElevatorState.None;
        private bool m_isInElevator = true;

        public void Setup()
        {
        }

        public void Update()
        {
        }

        public void ElevatorStateChanged(ElevatorState elevatorState)
        {
            m_elevatorState = elevatorState;

            // If not active update state so any while() loops below are exited,
            // but don't trigger nay new patterns
            if (!AgentActive())
            {
                return;
            }

            if (elevatorState == ElevatorState.FirstMovement
                || elevatorState == ElevatorState.CageRotating)
            {
                PlayDeployingPattern(0.2f);
            }
            else if (elevatorState == ElevatorState.TopDeploying
                     || elevatorState == ElevatorState.Deploying)
            {
                PlayDeployingPattern(0.5f);
            }
            else if (elevatorState == ElevatorState.FirstDescentPattern
                     || elevatorState == ElevatorState.SlowingDown)
            {
                // PlayRidePattern();
            }
            else if (elevatorState == ElevatorState.Descending)
            {
                // PlayRidePattern();
                PlayDescendingPattern();
            }
            else if (elevatorState == ElevatorState.PendingTopDeploying)
            {
                PlayDoorOpeningPattern();
            }
        }

        public void SetIsInElevator(bool inElevator)
        {
            m_isInElevator = inElevator;
        }

        private async void PlayDeployingPattern(float intensity)
        {
            ElevatorState startingState = m_elevatorState;
            int delay = 200;
            int[,] indices =
            {
                {
                    38, 33, 31, 24, 22, 17, // body front
                    26, 29, 19, 20, 10, 13, 3, 4, // body back
                    57, 65, 59, 67, 61, 69, 63, 71, // legs back
                    41, 43, 45, 47, 49, 51, 53, 55, // arms back
                }
            };

            while (m_elevatorState == startingState && m_isInElevator)
            {
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, intensity, delay));

                await Task.Delay(delay);
            }
        }

        private async void PlayRidePattern()
        {
            ElevatorState startingState = m_elevatorState;
            int[,] bodyIndices =
            {
                { 6, 15, 16, 25,    2, 11, 20, 29 },
                { 14, 23, 24, 33,   10, 19, 28, 37 },
                { 22, 31, 32, 1,    18, 27, 36, 5 },
                { 30, 39, 0, 9,     26, 35, 4, 13 },
                { 38, 7, 8, 17,     34, 3, 12, 21 },
            };

            int[,] legsIndices =
            {
                { 70, 60,   63, 69 },
                { 68, 58,   61, 67 },
                { 66, 56,   59, 65 },
                { 64, 62,   57, 71 },
            };

            int[,] armsIndices =
            {
                { 54, 46, 47, 55 },
                { 52, 44, 45, 53 },
                { 50, 42, 43, 51 },
                { 48, 40, 41, 49 },
            };

            int repeatDelay = 1000;
            while (m_elevatorState == startingState && m_isInElevator)
            {
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(bodyIndices, 0.05f, repeatDelay / bodyIndices.GetLength(0)));
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(legsIndices, 0.05f, repeatDelay / legsIndices.GetLength(0)));
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(armsIndices, 0.05f, repeatDelay / armsIndices.GetLength(0)));
                await Task.Delay(repeatDelay);
            }
        }

        private async void PlayDescendingPattern()
        {
            ElevatorState startingState = m_elevatorState;
            float intensity = 0.8f;

            while (m_elevatorState == startingState && m_isInElevator)
            {
                float durationScale = BodyHapticsUtils.GetElevatorRideDurationScale();
                int patternDuration = (int) (30 * durationScale);
                await ShockwaveEngine.PlayPatternFunc(new HapticIndexPattern(BodyHapticsIndices.FeetToShoulders, intensity, patternDuration));
                await ShockwaveEngine.PlayPatternFunc(new HapticIndexPattern(BodyHapticsIndices.FeetToShoulders, intensity, patternDuration));

                await Task.Delay((int) (400 * durationScale));
            }
        }

        private void PlayDoorOpeningPattern()
        {
            PlayDoorOpeningBackgroundEffects();
            PlayDoorOpeningExtraEffects();
        }

        private async void PlayDoorOpeningBackgroundEffects()
        {
            int[,] indices =
            {
                {
                    22, 17, 15, 8, 6, 1, // body front
                    34, 37, 2, 5, 27, 28, 19, 20, 11, 12, // body back
                    64, 56, 68, 60, // legs front
                    57, 65, 59, 67, 61, 69, 63, 71, // legs back
                    50, 52, 42, 44, // arms front
                    41, 43, 45, 47, 49, 51, 53, 55, // arms back
                }
            };
            await ShockwaveEngine.PlayPatternFunc(new HapticIndexPattern(indices, 0.3f, 3000));

            indices = new[,]
            {
                {
                    38, 30, 22, 14, 6,
                    33, 25, 17, 9, 1,
                    34, 26, 18, 10, 2,
                    37, 29, 21, 13, 5,
                }
            };
            await ShockwaveEngine.PlayPatternFunc(new HapticIndexPattern(indices, 1.0f, 500));
        }

        private async void PlayDoorOpeningExtraEffects()
        {
            await Task.Delay(1500);

            int[,] indices =
            {
                { 38, 33, 1, 6,    34, 37, 5, 2 },
                { 39, 25, 0, 14,   35, 29, 4, 10 },
                { 32, 17, 7, 22,   36, 21, 3, 18 },
            };
            await ShockwaveEngine.PlayPatternFunc(new HapticIndexPattern(indices, 1.0f, 250));

            await Task.Delay(750);

            List<List<int>> listIndices = BodyHapticsIndices.FeetToShoulders;
            int duration = 2000 / listIndices.Count;
            await ShockwaveEngine.PlayPatternFunc(new HapticIndexPattern(listIndices, 0.7f, duration));
        }

        public bool AgentActive()
        {
            return VRConfig.configUseShockwave.Value && ShockwaveManager.Instance.Ready && ShockwaveManager.Instance.suitConnected();
        }
    }
}