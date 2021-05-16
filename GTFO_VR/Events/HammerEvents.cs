using GTFO_VR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Events
{
    class HammerEvents
    {
        public static event Action<float> OnHammerSmack;
        public static event Action OnHammerFullyCharged;
        public static event Action OnHammerHalfCharged;


        public static void HammerSmacked(float damage)
        {
            OnHammerSmack?.Invoke(damage);
        }

        public static void HammerFullyCharged()
        {
            OnHammerFullyCharged?.Invoke();
        }

        public static void HammerHalfCharged()
        {
            OnHammerHalfCharged?.Invoke();
        }
    }
}
