using Gear;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// For now the only purpose of this class is to disable the melee weapon's animations
    /// </summary>
    public class VRHammer : MonoBehaviour
    {
        public VRHammer(IntPtr value) : base(value)
        {
        }

        public static float hammerSizeMult = 0.8f;

        MeleeWeaponFirstPerson m_weapon;
        Transform m_animatorRoot;

        public void Setup(MeleeWeaponFirstPerson weapon)
        {
            m_weapon = weapon;
            m_animatorRoot = m_weapon.ModelData.m_damageRefAttack.parent;
        }

        void LateUpdate()
        {
            if (FocusStateEvents.currentState == eFocusState.FPS) {
                m_animatorRoot.transform.localPosition = Vector3.zero;
                m_animatorRoot.transform.localRotation = Quaternion.identity;
            }
        }

    }
}
