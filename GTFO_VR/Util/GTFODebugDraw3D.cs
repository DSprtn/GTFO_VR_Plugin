using GTFO_VR.Core.PlayerBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR.Util
{

    // Provides debug drawing of colliders through various static methods.
    // Note that this component will only be active and added to VRSystem in the DEBUG build
    public class GTFODebugDraw3D : MonoBehaviour
    {
        public GTFODebugDraw3D(IntPtr value) : base(value)
        {
        }

        private static Material DebugMaterial;
        private static Color DebugColor = ColorExt.Red(0.1f);   // Meshes share a single color because I am lazy

        private static List<DebugMesh> DebugMeshes = new List<DebugMesh>();

        private class DebugMesh
        {
            public Mesh colliderMesh;
            public Matrix4x4 colliderTransform;
            public float duration;
        }

        public void Update()
        {
            drawMeshes();
        }

        private static void setupMaterial()
        {
            if (DebugMaterial == null)
            {
                //Mesh colliders will often be buried in geometry, so use fancy draw-over-everything material
                DebugMaterial = new Material(Shader.Find("UI/Default"));
                DebugMaterial.renderQueue = (int)RenderQueue.Overlay + 1;
                DebugMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
                DebugMaterial.color = ColorExt.Red(0.1f);
            }
        }

        public static void drawCollider(Collider collider, Color color, float duration)
        {
            // gets garbage collected so ensure it still exists
            setupMaterial();

            GameObject go = collider.gameObject;

            if (go.GetComponent<SphereCollider>() != null)
            {
                SphereCollider spherCol = go.GetComponent<SphereCollider>();

                DebugDraw3D.DrawSphere(spherCol.transform.TransformPoint(spherCol.center), spherCol.radius, color, duration);
                return;
            }

            if (go.GetComponent<BoxCollider>() != null)
            {
                BoxCollider boxCollider = go.GetComponent<BoxCollider>();

                DebugMeshes.Add(new DebugMesh
                {
                    colliderMesh = GenerateBoxColliderMesh(boxCollider),
                    colliderTransform = boxCollider.gameObject.transform.localToWorldMatrix,
                    duration = duration

                });

                // Update mesh material color if it changes.
                // All mesh visualizations share the same color
                if (color.Equals(DebugColor))
                {
                    DebugColor = color;
                    DebugMaterial.color = color;
                }

                return;
            }

            if (go.GetComponent<CapsuleCollider>() != null)
            {
                CapsuleCollider capsuleCol = go.GetComponent<CapsuleCollider>();

                // For some reason height corresponds to the X axis
                Vector3 top = capsuleCol.transform.TransformPoint(capsuleCol.center - (new Vector3(capsuleCol.height * 0.5f, 0, 0)));
                Vector3 bottom = capsuleCol.transform.TransformPoint(capsuleCol.center + (new Vector3(capsuleCol.height * 0.5f, 0, 0 )));

                // We can't draw actual capsules, and we can't draw lines ( easily ), but we still have cones.
                DebugDraw3D.DrawCone(top, bottom, capsuleCol.radius, color, duration);
                DebugDraw3D.DrawCone(bottom, top, capsuleCol.radius, color, duration);

                //And spheres for the caps
                DebugDraw3D.DrawSphere(capsuleCol.transform.TransformPoint(top), capsuleCol.radius, color, duration);
                DebugDraw3D.DrawSphere(capsuleCol.transform.TransformPoint(bottom), capsuleCol.radius, color, duration);

                return;
            }

            if (go.GetComponent<MeshCollider>() != null)
            {
                MeshCollider meshCollider = go.GetComponent<MeshCollider>();

                DebugMeshes.Add(new DebugMesh
                {
                    colliderMesh = meshCollider.sharedMesh,
                    colliderTransform = meshCollider.gameObject.transform.localToWorldMatrix,
                    duration = duration

                });

                // Update mesh material color if it changes.
                // All mesh visualizations share the same color
                if (color.Equals(DebugColor))
                {
                    DebugColor = color;
                    DebugMaterial.color = color;
                }

                return;
            }
        }

        private void drawMeshes()
        {   
            foreach(var debugMesh in DebugMeshes.ToList())
            {
                // Draw whatever is in the list
                Graphics.DrawMesh(debugMesh.colliderMesh, debugMesh.colliderTransform, DebugMaterial, LayerManager.LAYER_DEFAULT);

                // Remove any expired meshes
                if (debugMesh.duration <= 0)
                {
                    DebugMeshes.Remove(debugMesh);
                }

                // keep track of remaining time to keep drawing
                debugMesh.duration -= Time.deltaTime;
            }
        }

        public static Mesh GenerateBoxColliderMesh(BoxCollider collider)
        {
            Mesh mesh = new Mesh();

            float xSize = collider.size.x * 0.5f;
            float ySize = collider.size.y * 0.5f;
            float zSize = collider.size.z * 0.5f;
            Vector3 center = collider.center;

            // Box representing box collider
            Vector3[] vertices = new Vector3[] 
            {
                new Vector3(center.x - xSize, center.y - ySize, center.z - zSize),
                new Vector3(center.x + xSize, center.y - ySize, center.z - zSize),
                new Vector3(center.x + xSize, center.y + ySize, center.z - zSize),
                new Vector3(center.x - xSize, center.y + ySize, center.z - zSize),
                new Vector3(center.x - xSize, center.y - ySize, center.z + zSize),
                new Vector3(center.x + xSize, center.y - ySize, center.z + zSize),
                new Vector3(center.x + xSize, center.y + ySize, center.z + zSize),
                new Vector3(center.x - xSize, center.y + ySize, center.z + zSize),
            };

            int[] triangles = new int[] {
                // Front face.
                0, 1, 2,
                0, 2, 3,
                // Back face.
                4, 5, 6,
                4, 6, 7,
                // Left face.
                0, 4, 5,
                0, 5, 1,
                // Right face.
                3, 2, 6,
                3, 6, 7,
                // Top face.
                1, 5, 6,
                1, 6, 2,
                // Bottom face.
                0, 3, 7,
                0, 7, 4,
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}
