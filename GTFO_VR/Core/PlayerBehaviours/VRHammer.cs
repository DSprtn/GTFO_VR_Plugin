using Gear;
using GTFO_VR.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class VRHammer : MonoBehaviour
    {
        public VRHammer(IntPtr value) : base(value)
        {
        }

        MeleeWeaponFirstPerson m_weapon;
        Transform m_animatorRoot;

        public void Setup(MeleeWeaponFirstPerson weapon)
        {
            m_weapon = weapon;
            m_animatorRoot = m_weapon.ModelData.m_damageRefAttack.parent;
        }

        void LateUpdate()
        {
            if(FocusStateEvents.currentState == eFocusState.FPS) {
                m_animatorRoot.transform.localPosition = Vector3.zero;
                m_animatorRoot.transform.localRotation = Quaternion.identity;
            }
        }

    }
}
