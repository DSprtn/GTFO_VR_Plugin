using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.GameHooks
{

    /// <summary>
    /// Remove or disable the matrials of objects that cause significant framerate drops
    /// </summary>

    // Fires when player exits the elevator, so all assets have been loaded.
    [HarmonyPatch(typeof(WardenObjectiveManager), nameof(WardenObjectiveManager.OnLocalPlayerStartExpedition))]
    internal class InjectWaterTankShaderRemoval
    {
        private static void Postfix(WardenObjectiveManager __instance)
        {
            // Can't really disable materials easily, so need to get GameObjects and their renderers instead.
            // All of this takes this a split second and should ideally be run off the main thread.
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = renderer.sharedMaterial;
                    if (mat != null)
                    {
                        // TankGlass is used for animated water surfaces (Material: service_water_plane_2x2),
                        // and a number of other objects. Primarily murky tanks housing specimens,  
                        // but also some glass panels and for the floor in at least one location.
                        if (mat.shader.name.Equals("GTFO/TankGlass"))
                        {
                            // Want to keep the hitbox, but not render.
                            renderer.forceRenderingOff = true;
                        }
                    }
                }
            }
        }
    }
}
