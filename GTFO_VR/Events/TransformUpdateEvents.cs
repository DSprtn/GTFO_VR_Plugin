using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{
    public static class TransformUpdateEvents
    {


        public static event Action OnHMDPositionUpdated;


        public static event Action OnControllersPositionUpdated;

        public static void HmdPositionUpdated()
        {
            if(OnHMDPositionUpdated != null)
            {
                OnHMDPositionUpdated.Invoke();
            }
        }

        public static void ControllerPositionsUpdated()
        {
            if (OnControllersPositionUpdated != null)
            {
                OnControllersPositionUpdated.Invoke();
            }
        }

        


    }
}
