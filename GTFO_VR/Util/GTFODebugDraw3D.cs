using GTFO_VR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace GTFO_VR.Util
{

    // Provides debug drawing of colliders through various static methods.
    // Note that this component will only be active and added to VRSystem in the DEBUG build
    public class GTFODebugDraw3D : MonoBehaviour
    {
        public GTFODebugDraw3D(IntPtr value) : base(value)
        {
        }

        private static Material IntersectMaterial;
        private static Material OverlayMaterial;

        private static Queue<DebugShape> SpherePool = new Queue<DebugShape>();
        private static Queue<DebugShape> MeshPool = new Queue<DebugShape>();
        private static Queue<DebugShape> CubePool = new Queue<DebugShape>();
        private static Queue<DebugShape> CylinderPool = new Queue<DebugShape>();

        private static List<DebugShape> DrawQueue = new List<DebugShape>();

        private static GameObject DebugDrawContainer;

        private class DebugShape
        {
            public GameObject go;
            public MeshRenderer renderer;
            public float duration;
            public DebugShapeType shapeType;

            public DebugShape(  GameObject go, MeshRenderer renderer, float duration, DebugShapeType shapeType)
            {
                this.go = go;
                this.renderer = renderer;
                this.duration = duration;
                this.shapeType = shapeType;
            }   
        }

        private enum DebugShapeType
        {
            Sphere, Cube, Cylinder, Mesh
        }

        private static Material GetMaterial()
        {
            if (IntersectMaterial == null)
            {
                IntersectMaterial = new Material(Shader.Find("UI/Default"));
            }

            return IntersectMaterial;
        }

        private static Material GetOverlayMaterial(Color color)
        {
            if (OverlayMaterial == null)
            {
                //Mesh colliders will often be buried in geometry, so use fancy draw-over-everything material
                OverlayMaterial = new Material(Shader.Find("UI/Default"));
                OverlayMaterial.renderQueue = (int)RenderQueue.Overlay + 1;
                OverlayMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            }

            return OverlayMaterial;
        }

        private static DebugShape GetDebugShape( DebugShapeType type ) 
        {
            DebugShape shape = null;
            Queue<DebugShape> pool = GetPool(type);

            if (pool.Count > 1)
            {
                shape = pool.Dequeue();
            }
            else
            {
                shape = GenerateNewDebugShape(type);
            }

            shape.go.SetActive(true);
            return shape;
        }

        private static Queue<DebugShape> GetPool(DebugShapeType type)
        {
            switch (type)
            {
                case DebugShapeType.Sphere:
                {
                    return SpherePool;
                }
                case DebugShapeType.Cube:
                {
                    return CubePool;
                }
                case DebugShapeType.Cylinder:
                {
                    return CylinderPool;
                }
                case DebugShapeType.Mesh:
                {
                    return MeshPool;
                }
                default:
                {
                    return SpherePool;
                }
            }
        }


        private static void ReturnDebugShape( DebugShape shape )
        {
            shape.go.SetActive(false);
            GetPool(shape.shapeType).Enqueue(shape);
        }

        private static DebugShape GenerateNewDebugShape( DebugShapeType type  )
        {
            GameObject newShape;

            switch (type)
            {
                case DebugShapeType.Sphere:
                {
                    newShape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                }
                case DebugShapeType.Cube:
                {
                    newShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                }
                case DebugShapeType.Cylinder:
                {
                    newShape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
                }
                case DebugShapeType.Mesh:
                {
                    newShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                }
                default:
                {
                    newShape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                }
            }

            newShape.transform.SetParent(DebugDrawContainer.transform);
            GameObject.Destroy(newShape.gameObject.GetComponent<Collider>());
            return new DebugShape(newShape, newShape.GetComponent<MeshRenderer>(), 0, type);
        }


        public void Awake()
        {
            DebugDrawContainer = new GameObject("debugDraw");
            DebugDrawContainer.transform.SetParent(this.gameObject.transform);
        }

        public void LateUpdate()
        {
            DrawQueuedShapes();
        }

        private static bool SetupComplete()
        {
            if (DebugDrawContainer == null)
            {
                Log.Error("Tried to use GTFODebugDraw3D without adding the component to a GO! It should be added to VRSystems in debug builds.");
                return false;
            }

            return true;
        }

        public static void DrawSphere(Vector3 vector, float radius, Color color, float duration = 0, bool renderOntop = false)
        {
            if (!SetupComplete())
                return;

            DebugShape shape = GetDebugShape(DebugShapeType.Sphere);

            shape.duration = duration;

            shape.go.transform.position = vector;
            shape.go.transform.transform.localScale = new Vector3(radius *2, radius*2, radius*2);
            shape.go.transform.rotation = Quaternion.identity;

            AddToDrawQueue(shape, color, duration, renderOntop);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0, bool renderOntop = false)
        {
            if (!SetupComplete())
                return;

            DebugShape shape = GetDebugShape(DebugShapeType.Mesh);

            shape.duration = duration;

            shape.go.transform.position = position;
            shape.go.transform.rotation = rotation;
            shape.go.transform.localScale = scale;

            shape.go.GetComponent<MeshFilter>().mesh = mesh;

            AddToDrawQueue(shape, color, duration, renderOntop);
        }

        public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0, bool renderOntop = false)
        {
            if (!SetupComplete())
                return;

            DebugShape shape = GetDebugShape(DebugShapeType.Cube);

            shape.duration = duration;

            shape.go.transform.position = position;
            shape.go.transform.transform.localScale = scale;
            shape.go.transform.rotation = rotation;

            AddToDrawQueue(shape, color, duration, renderOntop);
        }


        public static void DrawCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0, bool renderOntop = false)
        {
            if (!SetupComplete())
                return;

            DebugShape shape = GetDebugShape(DebugShapeType.Cylinder);

            shape.duration = duration;

            shape.go.transform.position = position;
            shape.go.transform.transform.localScale = scale;
            shape.go.transform.rotation = rotation;

            AddToDrawQueue(shape, color, duration, renderOntop);
        }

        private static void AddToDrawQueue( DebugShape shape, Color color, float duration, bool renderOntop)
        {
            // Common tasks so perform them here
            shape.renderer.sharedMaterial = renderOntop ? GetOverlayMaterial(color) : GetMaterial();
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", color);
            shape.renderer.SetPropertyBlock(propertyBlock);

            DrawQueue.Add(shape);
        }

        public static void DrawCollider(Collider collider, Color color, float duration = 0, bool renderOntop = true)
        {

            // Colliders are scaled along with their gameobject, 
            // so they'll be bigger, but their radius values will remain unchanged.
            // Compensate for this.
            Vector3 lossyScale = collider.transform.lossyScale;
            float uniformScale = new[] { lossyScale.x, lossyScale.y, lossyScale.z }.Max();

            // Not just checking type and casting because it... doesn't work. 
            // Everything is a Collider, unless I do this.
            if (collider.GetComponent<SphereCollider>() != null)
            {
                SphereCollider spherCol = collider.GetComponent<SphereCollider>();

                DrawSphere(spherCol.transform.TransformPoint(spherCol.center), spherCol.radius * uniformScale, color, duration, renderOntop);
                return;
            }

            if (collider.GetComponent<BoxCollider>() != null)
            {
                BoxCollider boxCollider = collider.GetComponent<BoxCollider>();

                Vector3 colliderScale = boxCollider.transform.lossyScale;

                Vector3 drawScale = new Vector3(
                    boxCollider.size.x * colliderScale.x, 
                    boxCollider.size.y * colliderScale.y, 
                    boxCollider.size.z * colliderScale.z);

                DrawCube(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.transform.rotation, drawScale, color, duration, renderOntop);

                return;
            }

            if (collider.GetComponent<CapsuleCollider>() != null)
            {

                CapsuleCollider capsuleCol = collider.GetComponent<CapsuleCollider>();

                // Capsule colliders scale weirdly with their parent.
                // The vertical axis will determine height, while the highest of the other axes will determine width.
                // The axis used will depend on the direction of the capsule collider, which in GTFO's case is always X ( 0 )
                // This also affects the orientation of the capsule, so we have to rotate our visualization by 90 degrees along the Z axis ( For x-oriented colliders )

                // default y orientation
                Quaternion directionOffset = Quaternion.identity;
                float radiusScale = new[] { lossyScale.x, lossyScale.z }.Max();
                float heightScale = lossyScale.y;

                if (capsuleCol.direction == 0)  // X oriented, used for everything
                {
                    // y/z determine width, x determines height
                    radiusScale = new[] { lossyScale.y, lossyScale.z }.Max();
                    heightScale = lossyScale.x;

                    // Rotated so our normal cylinder will point in the same direction as the collider
                    directionOffset = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
                }
                else if (capsuleCol.direction == 2)   // z oriented
                {
                    // y/z determine width, x determines height
                    radiusScale = new[] { lossyScale.x, lossyScale.y }.Max();
                    heightScale = lossyScale.z;

                    // Rotated so our normal cylinder will point in the same direction as the collider
                    directionOffset = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
                }


                float scaledRadius = capsuleCol.radius * radiusScale;
                float scaledDiameter = scaledRadius * 2;
                float scaledHeight = capsuleCol.height * heightScale;

                Vector3 cylinderScale = new Vector3(
                                          scaledDiameter,
                        (((scaledHeight - scaledDiameter) * 0.5f)),
                                          scaledDiameter
                    );

                Vector3 center = capsuleCol.transform.TransformPoint(capsuleCol.center);
                Quaternion rotation = capsuleCol.transform.rotation * directionOffset;

                DrawCylinder(center, rotation, cylinderScale, color, duration, renderOntop);

                Vector3 endcapVector = rotation * new Vector3(0, cylinderScale.y, 0);  // A cylinder is 2 high at a scale of 1, so use as-is

                DrawSphere(center + endcapVector, scaledRadius, color, duration, renderOntop);
                DrawSphere(center - endcapVector, scaledRadius, color, duration, renderOntop);

                return;
            }

            if (collider.GetComponent<MeshCollider>() != null)
            {
                MeshCollider meshCollider = collider.GetComponent<MeshCollider>();

                Transform transform = meshCollider.transform;

                DrawMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale, color, duration, renderOntop);

                return;
            }
        }

        private void DrawQueuedShapes()
        {
            foreach (var debugShape in DrawQueue.ToList())
            {
                // Remove any expired meshes
                if (debugShape.duration < 0)
                {
                    DrawQueue.Remove(debugShape);
                    ReturnDebugShape(debugShape);
                }
                else
                {
                    // keep track of remaining time to keep drawing
                    debugShape.duration -= Time.deltaTime;
                }
            }
        }

        public static void DrawNearbyColliders( Vector3 position, float radius, int layerMask, Color color, float duration = 0, bool renderOntop = true)
        {
            var colliders = Physics.OverlapSphere(position, radius, layerMask, QueryTriggerInteraction.Ignore);
            foreach (var collider in colliders)
            {
                DrawCollider(collider, color, duration, renderOntop);
            }
        }
    }
}
