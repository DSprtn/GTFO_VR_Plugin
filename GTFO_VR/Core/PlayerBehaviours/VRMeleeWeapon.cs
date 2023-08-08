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

        private float m_hitboxSize = .061f;

        public VelocityTracker m_damageRefTipPositionTracker = new VelocityTracker();
        public VelocityTracker m_damageRefBasePositionTracker = new VelocityTracker();
        public VelocityTracker m_handPositionTracker = new VelocityTracker();
        private MeleeWeaponFirstPerson m_weapon;
        private Transform m_animatorRoot;
        private Light m_chargeupIndicatorLight;

        private Vector3 m_offsetTip = new Vector3(0, 0, .6f);
        private Vector3 m_offsetBase = new Vector3(0, 0, .3f);
        private bool m_elongatedHitbox = false; // If hitbox is elongated ( knife, bat, hammer ) or a single sphere ( spear )
        private bool m_centerHitbox = false;    // If a center hitbox should be generated when using an elongated hitbox

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
                    m_hitboxSize = 0.035f;
                    m_offsetTip = new Vector3(0, 0.95f, 0f);
                    m_offsetBase = m_offsetTip;
                    m_elongatedHitbox = false;
                    m_centerHitbox = false;
                    break;
                case "Knife":
                    m_hitboxSize = 0.025f;
                    m_offsetTip = new Vector3(0, 0.28f, 0.01f);
                    m_offsetBase = new Vector3(0, 0.12f, 0.01f);
                    m_elongatedHitbox = true;
                    m_centerHitbox = true;
                    break;
                case "Bat":
                    m_hitboxSize = 0.04f;
                    m_offsetTip = new Vector3(0, 0.4f, 0f);
                    m_offsetBase = new Vector3(0, 0.15f, 0.0f);
                    m_elongatedHitbox = true;
                    m_centerHitbox = true;
                    break;
                case "Sledgehammer":
                    m_hitboxSize = .07f;
                    m_offsetTip = new Vector3(0, 0.42f, 0.1f);  // Front-facing hammer head
                    m_offsetBase = new Vector3(0, 0.42f, -0.1f);
                    m_elongatedHitbox = true;
                    m_centerHitbox = false;
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
                UpdateDamagePositionAndVelocity();
                ForceDamageRefPosition();
                m_chargeupIndicatorLight.transform.position = m_damageRefTipPositionTracker.GetLatestPosition();
            }

#if DEBUG_GTFO_VR
            if (VRConfig.configDebugShowHammerHitbox.Value)
            {
                DebugDraw3D.DrawSphere(m_damageRefTipPositionTracker.GetLatestPosition(), m_hitboxSize, ColorExt.Red(0.2f));
                if (m_elongatedHitbox)
                {
                    DebugDraw3D.DrawSphere(m_damageRefBasePositionTracker.GetLatestPosition(), m_hitboxSize, ColorExt.Red(0.2f));
                    if (m_centerHitbox)
                    {
                        Vector3 centerHitbox = (m_damageRefTipPositionTracker.GetLatestPosition() + m_damageRefBasePositionTracker.GetLatestPosition()) * 0.5f;
                        DebugDraw3D.DrawSphere(centerHitbox, m_hitboxSize, ColorExt.Red(0.2f));
                    }
                }
            }
#endif
        }

        private void ForceDamageRefPosition()
        {
            if (VRConfig.configUseControllers.Value && !VRConfig.configUseOldHammer.Value)
            {
                if (m_weapon.ModelData != null)
                {
                    // We are patching anywhere this would be used, so probably redunant
                    m_weapon.ModelData.m_damageRefAttack.transform.position = m_damageRefTipPositionTracker.GetLatestPosition();
                }
            }
        }

        public void UpdateDamagePositionAndVelocity()
        {
            if (m_weapon.ModelData != null)
            {

                // Attack position ( tip ) is offset from the transform of the wielded item ( the melee weapon )
                m_damageRefTipPositionTracker.AddPosition( VRPlayer.PlayerAgent.FPItemHolder.WieldedItem.transform.TransformPoint(m_offsetTip), Time.deltaTime);

                // If we are using an elongated hitbox we want to do this for the base ( bottom ) of the hitbox area too.
                if (m_elongatedHitbox)
                {
                    m_damageRefBasePositionTracker.AddPosition(VRPlayer.PlayerAgent.FPItemHolder.WieldedItem.transform.TransformPoint(m_offsetBase), Time.deltaTime);
                }

                // Use local position ( ignoring thumbstick movement ) to determine velocity required for bonk.
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

        private record MeleeAttackData(Vector3 weaponPosCurrent, Vector3 weaponPosPrev)
        {
            public Vector3 Velocity
            {
                get { return (weaponPosCurrent - weaponPosPrev); }
            }
        }

        private record MeleeAttackHit(RaycastHit rayHit, Vector3 weaponPosCurrent, Vector3 weaponPosPrev, float hitTime)
        {
            public Vector3 Velocity
            {
                get { return (weaponPosCurrent - weaponPosPrev); }
            }
        }


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

                // And one inbetween tip and base, maybe
                if (m_centerHitbox)
                {
                    hitboxes.Add(new MeleeAttackData((baseCurrent + weaponTipPosCurrent) * 0.5f, (weaponTipPosPrev + basePrev) * 0.5f));
                }
            }

            float earliestHitTime = float.MaxValue; // Temporal distance of nearest hit
            float earliestObstacleHitTime = float.MaxValue; // Temporal distance of nearest obstacle hit

            // May be populated by a single hit ( earliest SphereCast hit ) or a bunch of hits from SphereCastAll ( spear ) 
            System.Collections.Generic.List<MeleeAttackHit> attackHits = new System.Collections.Generic.List<MeleeAttackHit>();
            foreach (var hitbox in hitboxes)
            {

#if DEBUG_GTFO_VR
                DebugDraw3D.DrawCone(hitbox.weaponPosCurrent, hitbox.weaponPosPrev, m_hitboxSize * 0.3f, ColorExt.Blue(0.5f), 0.5f);
#endif
                // cast a sphere from where the the hitbox was, to where it is, and get the first thing it collides with along the way
                if (m_weapon.MeleeArchetypeData.CanHitMultipleEnemies)
                {
                    // Spear  is able to hit multiple enemies 
                    RaycastHit[] rayHits = Physics.SphereCastAll(hitbox.weaponPosPrev, m_hitboxSize, hitbox.Velocity.normalized, hitbox.Velocity.magnitude, LayerManager.MASK_MELEE_ATTACK_TARGETS_WITH_STATIC, QueryTriggerInteraction.Ignore);

                    // Order is not guaranteed, so sort them nearest-to-furthest.
                    // This matters because each enemy can only be hit once
                    if (rayHits.Length > 1)
                    {
                        System.Array.Sort(rayHits, (a, b) => a.distance.CompareTo(b.distance));
                    }

                    // Remove any hits that came after hitting an obstacle, so we can't stab through obstacles.
                    foreach (var rayHit in rayHits)
                    {
                        float hitTime = rayHit.distance / hitbox.Velocity.magnitude;
                        attackHits.Add(new MeleeAttackHit(rayHit, hitbox.weaponPosCurrent, hitbox.weaponPosPrev, hitTime));

                        // If there are multiple hitboxes we have to check them all before filtering ones that
                        // collided after hitting an bstacle. We do this below.
                        if (rayHit.collider.GetComponent<IDamageable>() == null)
                        {
                            if (hitTime < earliestObstacleHitTime)
                                earliestObstacleHitTime = hitTime;

                            break;
                        }
                    }
                }
                else
                {
                    // Other weapons only care about the first hit
                    RaycastHit rayHit;
                    if (Physics.SphereCast(hitbox.weaponPosPrev, m_hitboxSize, hitbox.Velocity.normalized, out rayHit, hitbox.Velocity.magnitude, LayerManager.MASK_MELEE_ATTACK_TARGETS_WITH_STATIC, QueryTriggerInteraction.Ignore))
                    {
                        // Hitboxes may impact different colliders on the same frame
                        // % it travelled along the ray should correspond roughly to what time the impact was made,
                        // so we can figure out whether we hit something before or after the other hitboxes.
                        float hitTime = rayHit.distance / hitbox.Velocity.magnitude;
                        if (hitTime < earliestHitTime)
                        {
                            // While we check multiple hitboxes, we only ever care for the earliest hit
                            // so we can just replace the existing hit here.
                            attackHits.Clear();
                            attackHits.Add(new MeleeAttackHit(rayHit, hitbox.weaponPosCurrent, hitbox.weaponPosPrev, hitTime));
                            earliestHitTime = hitTime;
                        }
                    }
                }
            }

            // There will only be one hit here, unless spear
            foreach (MeleeAttackHit attackHit in attackHits)
            {
                IDamageable damagable = attackHit.rayHit.collider.GetComponent<IDamageable>();

                // Remove hits that occurred after we hit an obstacle when using SphereCastAll
                if (attackHit.hitTime > earliestObstacleHitTime)
                    continue;

                // Check if we've already hit this collider ( this is the first check, so no )
                // and also sets identifier so we can make sure we don't double-tap it below
                if (ConsiderDamageable(damagable))
                {
#if DEBUG_GTFO_VR
                    DebugDraw3D.DrawCone(attackHit.weaponPosCurrent, attackHit.weaponPosPrev, m_hitboxSize * 0.3f, ColorExt.Blue(0.5f), DEBUG_HIT_DRAW_DURATION);
#endif

                    hits.Add(new MeleeWeaponDamageData
                    {
                        damageGO = attackHit.rayHit.collider.gameObject,
                        hitPos = attackHit.rayHit.point, // vector from sourcePos to hitPos, and source to enemy spine used to determine backstab
                        hitNormal = attackHit.rayHit.normal, // Used for gore
                        sourcePos = attackHit.rayHit.point - (attackHit.Velocity.normalized), // positioned used to calculate backstab
                        damageTargetFound = true, // Not actually used for anything
                        damageComp = damagable
                    });
                }
            }

            // If there are no hits, it is possible that we are already inside of a collider. 
            // Also do this if we are allowed to hit multiple enemies ( e.g. spear ), but not if we already impact some obstacle
            if (hits.Count < 1 || earliestObstacleHitTime == float.MaxValue && m_weapon.MeleeArchetypeData.CanHitMultipleEnemies ) 
            {
                foreach (var hitbox in hitboxes)
                {
                    var colliders = Physics.OverlapSphere(hitbox.weaponPosCurrent, m_hitboxSize, LayerManager.MASK_MELEE_ATTACK_TARGETS, QueryTriggerInteraction.Ignore);
                    foreach (var collider in colliders)
                    {
                        IDamageable damagable = collider.GetComponent<IDamageable>();
                        if (ConsiderDamageable(damagable))
                        {
                            hits.Add(new MeleeWeaponDamageData
                            {
                                damageGO = collider.gameObject,
                                hitPos = hitbox.weaponPosCurrent, // vector from sourcePos to hitPos, and source to enemy spine used to determine backstab
                                hitNormal = hitbox.Velocity.normalized * -1f, // / Used for gore. Expected to be surface normal of thing we hit
                                sourcePos = hitbox.weaponPosCurrent - (hitbox.Velocity.normalized), // positioned used to calculate backstab
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
                    DebugDraw3D.DrawSphere(hit.hitPos, m_hitboxSize, ColorExt.Green(0.3f), DEBUG_HIT_DRAW_DURATION);
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