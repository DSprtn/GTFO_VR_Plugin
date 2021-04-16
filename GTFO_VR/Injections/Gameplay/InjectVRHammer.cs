namespace GTFO_VR.Injections
{
    // TODO - Design/Implement this
    /*
    /// <summary>
    /// Do not play any animations
    /// </summary>
    [HarmonyPatch(typeof(MeleeWeaponFirstPerson), "PlayAnim")]
    static class InjectVRHammer
    {
        static bool Prefix()
        {
            return false;
        }
    }
    /*

    [HarmonyPatch(typeof(MWS_ChargeUp), "Update")]
    static class InjectVRHammerChargup
    {
        static MeleeWeaponFirstPerson weapon;

        static void Postfix(MWS_ChargeUp __instance)
        {
            if (GTFO_VR.Util.Utils.CheckEnemyOverlap(__instance.AttackData.m_damageRef.position, .3f))
            {
                 AccessTools.FieldRefAccess<MWS_Base, MeleeWeaponFirstPerson>((MWS_Base)__instance, "m_weapon").ChangeState(eMeleeWeaponState.AttackChargeReleaseRight);
            }
        }
    }
    */
}