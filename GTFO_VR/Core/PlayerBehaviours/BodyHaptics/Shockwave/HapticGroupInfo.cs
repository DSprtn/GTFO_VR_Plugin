namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave
{
    public struct HapticGroupInfo
    {
        public ShockwaveManager.HapticGroup group;
        public float intensity;

        public HapticGroupInfo(ShockwaveManager.HapticGroup group, float intensity)
        {
            this.group = group;
            this.intensity = intensity;
        }
    }
}