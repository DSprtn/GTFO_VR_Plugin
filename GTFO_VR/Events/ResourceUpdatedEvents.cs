using System;

namespace GTFO_VR.Events
{
    public static class ResourceUpdatedEvents
    {
        public static event Action<float, float, float> OnAmmoGained;
        public static event Action<float> OnInfectionUpdated;
        public static event Action<float> OnHealthUpdated;

        public static void AmmoGained(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            OnAmmoGained?.Invoke(ammoStandardRel, ammoSpecialRel, ammoClassRel);
        }

        public static void InfectionUpdated(float infection)
        {
            OnInfectionUpdated?.Invoke(infection);
        }

        public static void HealthUpdated(float health)
        {
            OnHealthUpdated?.Invoke(health);
        }
    }
}