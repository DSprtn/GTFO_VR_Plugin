using Gear;
using GTFO_VR.Core.PlayerBehaviours.Melee;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using Il2CppSystem.Collections.Generic;
using LevelGeneration;
using System;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Disable animations, tweak hitbox size and position
    /// </summary>
    public class VRMeleeWeapon : MonoBehaviour
    {
        public VRMeleeWeapon(IntPtr value) : base(value)
        {
        }

        public static VRMeleeWeapon Current;

        public static float WeaponHitboxSize = .061f;

        public VelocityTracker m_damageRefTipPositionTracker = new VelocityTracker();
        public VelocityTracker m_damageRefBasePositionTracker = new VelocityTracker();
        public VelocityTracker m_handPositionTracker = new VelocityTracker();
        private MeleeWeaponFirstPerson m_weapon;
        private Transform m_animatorRoot;
        private Light m_chargeupIndicatorLight;

        private Quaternion m_rotationOffset = Quaternion.EulerAngles(new Vector3(0.78f, 0, 0)); // Weapon up is about 45 degrees off
        private Vector3 m_offsetTip = new Vector3(0, 0, .6f);      
        private Vector3 m_offsetBase = new Vector3(0, 0, .3f);
        private bool m_elongatedHitbox = false; // If hitbox is elongated ( knife, bat, hammer ) or a single sphere ( spear )

#if DEBUG_GTFO_VR
        private static readonly float DEBUG_HIT_DRAW_DURATION = 10;
#endif

        public void Setup(MeleeWeaponFirstPerson weapon)
        {
            Current = this;

            m_weapon = weapon;
            m_animatorRoot = m_weapon.ModelData.m_damageRefAttack.parent;
            m_chargeupIndicatorLight = new GameObject("VR_Weapon_Chargeup_Light").AddComponent<Light>();
            m_chargeupIndicatorLight.color = Color.white;

            m_chargeupIndicatorLight.enabled = false;
            m_chargeupIndicatorLight.shadows = LightShadows.None;
            VRMeleeWeaponEvents.OnHammerFullyCharged += WeaponFullyCharged;
            VRMeleeWeaponEvents.OnHammerHalfCharged += WeaponHalfCharged;

            switch (weapon.ArchetypeName)
            {
                case "Spear":
                    m_offsetTip = m_rotationOffset * new Vector3(0, 1.3f, 0f );
                    WeaponHitboxSize = 0.02f;
                    m_elongatedHitbox = false;
                    break;
                case "Knife":
                    m_offsetTip = m_rotationOffset * new Vector3(0, 0.35f, 0.01f);
                    m_offsetBase = m_rotationOffset * new Vector3(0, 0.2f, 0.01f);
                    WeaponHitboxSize = 0.022f;
                    m_elongatedHitbox = true;
                    break;
                case "Bat":
                    WeaponHitboxSize = 0.035f;
                    m_offsetTip = m_rotationOffset * new Vector3(0, 0.49f, 0f);
                    m_offsetBase = m_rotationOffset * new Vector3(0, 0.2f, 0.0f);
                    m_elongatedHitbox = true;
                    break;
                case "Sledgehammer":
                    WeaponHitboxSize = .061f;
                    m_offsetTip = m_rotationOffset * new Vector3(0, 0.74f, 0.13f);  // Front-facing hammer head
                    m_offsetBase = m_rotationOffset * new Vector3(0, 0.74f, -0.13f);
                    m_elongatedHitbox = true;
                    break;
                default:
                    Log.Error($"Unknown melee weapon detected {weapon.name}");
                    return;
            }
        }

        private void WeaponHalfCharged()
        {
            if (!VRConfig.configUseVisualHammerIndicator.Value)
            {
                return;
            }

            if (m_chargeupIndicatorLight != null)
            {
                m_chargeupIndicatorLight.range = 1.75f;
                m_chargeupIndicatorLight.intensity = .75f;
                m_chargeupIndicatorLight.enabled = true;
            }
            Invoke(nameof(VRMeleeWeapon.TurnChargeLightOff), 0.10f);
        }

        private void WeaponFullyCharged()
        {
            if (!VRConfig.configUseVisualHammerIndicator.Value)
            {
                return;
            }

            if (m_chargeupIndicatorLight != null)
            {
                m_chargeupIndicatorLight.range = 1.75f;
                m_chargeupIndicatorLight.intensity = 1.75f;
                m_chargeupIndicatorLight.enabled = true;
            }
            Invoke(nameof(VRMeleeWeapon.TurnChargeLightOff), 0.15f);
        }

        private void TurnChargeLightOff()
        {
            m_chargeupIndicatorLight.enabled = false;
        }

        private void Update()
        {
            if (VRConfig.configUseOldHammer.Value || !VRConfig.configUseControllers.Value)
            {
                return;
            }
            if (m_weapon.Owner && m_weapon.Owner.IsLocallyOwned)
            {
                ForceDamageRefPosition();
                m_chargeupIndicatorLight.transform.position = m_weapon.ModelData.m_damageRefAttack.transform.position;
                TrackPositionAndVelocity();
            }

#if DEBUG_GTFO_VR
            if (VRConfig.configDebugShowHammerHitbox.Value)
            {
                DebugDraw3D.DrawSphere(m_damageRefBasePositionTracker.GetLatestPosition(), VRMeleeWeapon.WeaponHitboxSize, ColorExt.Red(0.2f));
                DebugDraw3D.DrawSphere(m_damageRefTipPositionTracker.GetLatestPosition(), VRMeleeWeapon.WeaponHitboxSize, ColorExt.Red(0.2f));
            }
#endif
        }

        private void ForceDamageRefPosition()
        {
            if (VRConfig.configUseControllers.Value && !VRConfig.configUseOldHammer.Value)
            {
                if (m_weapon.ModelData != null)
                {
                    // We never access this directly anymore, but still set it incase we are missing some paths
                    m_weapon.ModelData.m_damageRefAttack.transform.position = Controllers.MainController.transform.TransformPoint( m_offsetTip);
                }
            }
        }

        public void TrackPositionAndVelocity()
        {
            if (m_weapon.ModelData != null)
            {
                // Position of melee damage ref is used to calculate velocity.
                // This must be done in local space as the units are tiny and rounding errors throw off the velocity otherwise.
                // This is basically a reapeat of ForceDamageRefPosition() but in local space of the controller

                // Global position of the tip of the attack ref. Used for accurate hit detection.
                // Reuse the damageRefAttack position we set earlier.
                // Rotation is not used
                m_damageRefTipPositionTracker.AddPosition(m_weapon.ModelData.m_damageRefAttack.position, m_weapon.ModelData.m_damageRefAttack.rotation, Time.deltaTime);

                // Global position of the base, used when we need multiple spheres to cover attack surface
                if (m_elongatedHitbox)
                {
                    // Rotation is not used
                    m_damageRefBasePositionTracker.AddPosition(Controllers.MainController.transform.TransformPoint(m_offsetBase), m_weapon.ModelData.m_damageRefAttack.rotation, Time.deltaTime);
                }

                // Use controller transform to calculate velocity needed for triggering bonk.
                // Actual position doesn't matter, just difference between frames.
                // Note that we are using the local position, so thumbstick movement is ignored.
                // However, add the vertical component of player position so the player can do silly things like drop onto scouts
                Vector3 velocityPosition = Controllers.MainControllerPose.transform.localPosition + new Vector3( 0, m_weapon.Owner.Position.y, 0);
                m_handPositionTracker.AddPosition(velocityPosition, Controllers.MainControllerPose.transform.localRotation, Time.deltaTime);
            }
        }

        private bool VelocityAboveThreshold( float positionalThreshold, float angularThreshold )
        {
            return m_handPositionTracker.GetSmoothVelocity() > positionalThreshold || m_handPositionTracker.GetSmoothAngularVelocity() > angularThreshold;
        }

        public bool VelocityAboveThreshold()
        {
            return VelocityAboveThreshold(0.8f, 200f);
        }

        private void LateUpdate()
        {
            if (VRConfig.configUseOldHammer.Value || !VRConfig.configUseControllers.Value)
            {
                return;
            }

            if (m_weapon.Owner && m_weapon.Owner.IsLocallyOwned)
            {
                if (FocusStateEvents.currentState == eFocusState.FPS)
                {
                    m_animatorRoot.transform.localPosition = Vector3.zero;
                    m_animatorRoot.transform.localRotation = Quaternion.identity;
                }
            }
        }

        public static List<MeleeWeaponDamageData> sortHits(List<MeleeWeaponDamageData> hits, Vector3 damageRefPos)
        {
            List<MeleeWeaponDamageData> sortedHits = new List<MeleeWeaponDamageData>();

            while (hits.Count > 0)
            {
                float lowest = 999999f;
                MeleeWeaponDamageData closestData = null;
                foreach (var hit in hits)
                {
                    float sqrDst = (hit.hitPos - damageRefPos).sqrMagnitude;
                    if (sqrDst <= lowest)
                    {
                        closestData = hit;
                        lowest = sqrDst;
                    }
                }
                sortedHits.Add(closestData);
                hits.Remove(closestData);
            }

            return sortedHits;
        }


        private bool ConsiderDamageable(IDamageable damagable)
        {
            // SearchID is increment at the beginning of CheckForAttackTarget.
            // If we encounter a collider that is already set to DamageUtil.SearchID
            // then we have already hit it during this search, and shuold skip it.
            if (damagable != null)
            {
                if (damagable.GetBaseDamagable().TempSearchID == DamageUtil.SearchID )
                {
                    return false;
                }
                else
                {
                    damagable.GetBaseDamagable().TempSearchID = DamageUtil.SearchID;
                    return true;
                }
            }

            // No damagable means it's something static.
            // We only check for them once ( SphereCast ) so this should never happen.
            return true;
        }

        private void HandleSkinnedDoor()
        {
            // This logic is from the original CheckForAttackTargets, where it is run before the rest of the function.
            // It used to be run manually as a MeleeWeaponFirstPerson.DoAttackDamage() prefix.
            // This would work, but the first hit would be ignored and not damage the door. 
            if (m_weapon.Owner.FPSCamera.CameraRayDist <= 3f && m_weapon.Owner.FPSCamera.CameraRayObject != null && m_weapon.Owner.FPSCamera.CameraRayObject.layer == LayerManager.LAYER_DYNAMIC)
            {
                iLG_WeakDoor_Destruction componentInParent = m_weapon.Owner.FPSCamera.CameraRayObject.GetComponentInParent<iLG_WeakDoor_Destruction>();
                if (componentInParent != null && !componentInParent.SkinnedDoorEnabled)
                {
                    componentInParent.EnableSkinnedDoor();
                }
            }
        }

        private record MeleeAttackData(Vector3 weaponPosCurrent, Vector3 weaponPosPrev);


        
        public bool CheckForAttackTarget( out List<MeleeWeaponDamageData> hits)
        {
            HandleSkinnedDoor();

            DamageUtil.IncrementSearchID();

            hits = new List<MeleeWeaponDamageData>();

            System.Collections.Generic.List<MeleeAttackData> hitboxes = new System.Collections.Generic.List<MeleeAttackData>();

            Vector3 weaponTipPosCurrent = m_damageRefTipPositionTracker.GetLatestPosition();
            Vector3 weaponTipPosPrev = m_damageRefTipPositionTracker.getPreviousPosition();

            hitboxes.Add(new MeleeAttackData(weaponTipPosCurrent, weaponTipPosPrev));

            if (m_elongatedHitbox)
            {
                // Knife and Bat need multiple hitboxes, because we can't really cast a capsule properly.
                // An extra 2 spheres is enough for these
                Vector3 baseCurrent = m_damageRefBasePositionTracker.GetLatestPosition();
                Vector3 basePrev = m_damageRefBasePositionTracker.getPreviousPosition();

                // Add base position
                hitboxes.Add(new MeleeAttackData(baseCurrent, basePrev));

                // And one inbetween tip and base
                hitboxes.Add(new MeleeAttackData((baseCurrent + weaponTipPosCurrent) * 0.5f, (weaponTipPosPrev + basePrev) * 0.5f ));
            }

            bool castHit = false;
            foreach (var hitbox in hitboxes)
            {
                var ( weaponPosCurrent, weaponPosPrev )= (hitbox.weaponPosCurrent, hitbox.weaponPosPrev);
                Vector3 velocity = (weaponPosCurrent - weaponPosPrev);

#if DEBUG_GTFO_VR
                DebugDraw3D.DrawCone(weaponPosCurrent, weaponPosPrev, VRMeleeWeapon.WeaponHitboxSize * 0.3f, ColorExt.Blue(0.5f), 0.5f);
#endif

                // cast a sphere from where the the hitbox was, to where it is, and get the first thing it collides with along the way
                RaycastHit rayHit;
                castHit = Physics.SphereCast(weaponPosPrev, VRMeleeWeapon.WeaponHitboxSize, velocity.normalized, out rayHit, velocity.magnitude, LayerManager.MASK_MELEE_ATTACK_TARGETS_WITH_STATIC, QueryTriggerInteraction.Ignore);
                if (castHit)
                {
                    IDamageable damagable = rayHit.collider.GetComponent<IDamageable>();

                    // Check if we've already hit this collider ( this is the first check, so no )
                    // and also sets identifier so we can make sure we don't double-tap it below
                    if (ConsiderDamageable(damagable))
                    {
#if DEBUG_GTFO_VR
                        DebugDraw3D.DrawCone(weaponPosCurrent, weaponPosPrev, VRMeleeWeapon.WeaponHitboxSize * 0.3f, ColorExt.Blue(0.5f), DEBUG_HIT_DRAW_DURATION);
#endif
                        hits.Add(new MeleeWeaponDamageData
                        {
                            damageGO = rayHit.collider.gameObject,
                            hitPos = rayHit.point, // vector from sourcePos to hitPos, and source to enemy spine used to determine backstab
                            hitNormal = rayHit.normal, // Used for gore
                            sourcePos = rayHit.point - (velocity.normalized), // positioned used to calculate backstab
                            damageTargetFound = true, // Not actually used for anything
                            damageComp = damagable
                        });
                    }

                    // Unless we are looking for multiple hits, break on the first hit
                    if (!m_weapon.MeleeArchetypeData.CanHitMultipleEnemies)
                        break;
                }
            }

            // If there are no hits, it is possible that we are already inside of a collider. 
            // Also do this if we are allowed to hit multiple enemies ( e.g. spear )
            if (!castHit || m_weapon.MeleeArchetypeData.CanHitMultipleEnemies)
            {
                foreach (var hitbox in hitboxes)
                {
                    var (weaponPosCurrent, weaponPosPrev) = (hitbox.weaponPosCurrent, hitbox.weaponPosPrev);
                    Vector3 velocity = (weaponPosCurrent - weaponPosPrev);

                    var colliders = Physics.OverlapSphere(weaponPosCurrent, VRMeleeWeapon.WeaponHitboxSize, LayerManager.MASK_MELEE_ATTACK_TARGETS, QueryTriggerInteraction.Ignore);
                    foreach (var collider in colliders)
                    {
                        IDamageable damagable = collider.GetComponent<IDamageable>();
                        if (ConsiderDamageable(damagable))
                        {
                            hits.Add(new MeleeWeaponDamageData
                            {
                                damageGO = collider.gameObject,
                                hitPos = weaponPosCurrent, // vector from sourcePos to hitPos, and source to enemy spine used to determine backstab
                                hitNormal = velocity.normalized * -1f, // / Used for gore. Expected to be surface normal of thing we hit
                                sourcePos = weaponPosCurrent - (velocity.normalized), // positioned used to calculate backstab
                                damageTargetFound = true, // Not actually used for anything
                                damageComp = damagable
                            });
                        }
                    }
                }
            }


#if DEBUG_GTFO_VR

            if (VRConfig.configDebugShowHammerHitbox.Value)
            {
                foreach(var hit in hits)
                {
                    Collider collider = hit.damageGO.GetComponent<Collider>();

                    // Draw hit, line between prev/curr melee position, and name of collider hit
                    DebugDraw3D.DrawSphere(hit.hitPos, VRMeleeWeapon.WeaponHitboxSize, ColorExt.Green(0.3f), DEBUG_HIT_DRAW_DURATION);
                    DebugDraw3D.DrawText(hit.hitPos, collider.name, 1f, ColorExt.Green(0.3f), DEBUG_HIT_DRAW_DURATION);

                    // Draw collider hit
                    GTFODebugDraw3D.drawCollider(collider, ColorExt.Red(0.2f), DEBUG_HIT_DRAW_DURATION);
                }
            }
#endif

            return hits.Count > 0;
        }


        private void OnDestroy()
        {
            VRMeleeWeaponEvents.OnHammerHalfCharged -= WeaponHalfCharged;
            VRMeleeWeaponEvents.OnHammerFullyCharged -= WeaponFullyCharged;
        }
    }
}