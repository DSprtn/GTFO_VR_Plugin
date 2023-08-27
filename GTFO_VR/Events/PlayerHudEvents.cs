using System;

namespace GTFO_VR.Events
{
    public static class PlayerHudEvents
    {
        public static event Action OnLiquidSplat;

        public static void LiquidSplat()
        {
            OnLiquidSplat?.Invoke();
        }
    }
}