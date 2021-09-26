using System;

namespace GTFO_VR.Events
{
    public static class PlayerTriggerReloadEvents
    {
        public static event Action OnTriggerWeaponReloaded;

        public static void TriggerWeaponReloaded()
        {
            OnTriggerWeaponReloaded?.Invoke();
        }
    }
}