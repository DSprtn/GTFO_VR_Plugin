using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core.UI
{
    public class WeaponRadialMenu : MonoBehaviour
    {
        public WeaponRadialMenu(IntPtr value)
: base(value) { }

        RadialMenu m_radialMenu;
        public void Setup()
        {
            m_radialMenu = gameObject.AddComponent<RadialMenu>();
            m_radialMenu.Setup(VR_Input.InteractionHand.MainHand);
        }
    }
}
