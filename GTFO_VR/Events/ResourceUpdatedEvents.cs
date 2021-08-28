using System;

namespace GTFO_VR.Events
{
    public static class ResourceUpdatedEvents
    {
        public static event Action<float> OnHealthGained;
        public static event Action<float, float, float> OnAmmoGained;
        public static event Action<float> OnDisinfectionGained;
        public static event Action<float> OnHealthUpdated;

        public static void HealthGained(float amountRel)
        {
            OnHealthGained?.Invoke(amountRel);
        }

        public static void AmmoGained(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            OnAmmoGained?.Invoke(ammoStandardRel, ammoSpecialRel, ammoClassRel);
        }

        public static void DisinfectionGained(float amountRel)
        {
            OnDisinfectionGained?.Invoke(amountRel);
        }

        public static void HealthUpdated(float health)
        {
            OnHealthUpdated?.Invoke(health);
        }
    }
}