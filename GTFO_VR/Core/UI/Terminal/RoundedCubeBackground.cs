using System;
using System.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal
{
    class RoundedCubeBackground : MonoBehaviour
    {
        private static readonly float DEGREES_90_IN_RADS = 90f * 0.0174532924F;
        
        public float width = 10;
        public float height = 10;
        public float radius = 3;
        public float padding = 0.01f;   // Negative padding works too
        public int cornerVertices = 4;
        public bool autoSize = true;
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            ensureInit();
        }

        void OnEnable()
        {
            meshRenderer.enabled = true;
        }

        private void Start()
        {
            GenerateMesh();
        }
        
        private void ensureInit()
        {
            if (this.meshFilter == null)
            {
                this.meshFilter = GetComponent<MeshFilter>();
                if (this.meshFilter == null)
                {
                    meshFilter = this.gameObject.AddComponent<MeshFilter>();
                    meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.material = new Material(Shader.Find("Unlit/Color"));
                }
            }
        }

        void OnDisable()
        {
            meshRenderer.enabled = false;
        }

        /*
        private void OnValidate()
        {
            ensureInit();
            GenerateMesh();
        }
        */

        public void setSize(float width, float height)
        {
            this.width = width;
            this.height = height;
            GenerateMesh();
        }

        public void setMaterial( Material mat )
        {
            if (meshRenderer != null)
            {
                meshRenderer.sharedMaterial = mat;
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            if (autoSize)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    setSize(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
                }
            }
        }

        public void GenerateMesh()
        {
            ensureInit();

            float halfWidth = (width * 0.5f) - padding;
            float halfHeight = (height * 0.5f) - padding;

            // Radius can't be greater than half the width/height, as it'll begin intersecting with itself
            float viableRadius = radius;
            float minRadius = Math.Min( halfWidth, halfHeight);
            if (minRadius < viableRadius)
                viableRadius = minRadius; 

            // A cube with a cube the size of radius cut out of each corner, starting with the top left section
            List<Vector3> vertices = new List<Vector3>();

            // Left corner
            vertices.Add( new Vector3( -halfWidth,                  halfHeight - viableRadius) );
            vertices.Add( new Vector3( -halfWidth + viableRadius,   halfHeight - viableRadius) );
            vertices.Add( new Vector3( -halfWidth + viableRadius,   halfHeight               ) );

            // Flip on x f.Add( ght corner
            vertices.Add(  new Vector3(vertices[2].x * -1f, vertices[2].y) );
            vertices.Add(  new Vector3(vertices[1].x * -1f, vertices[1].y) );
            vertices.Add(  new Vector3(vertices[0].x * -1f, vertices[0].y) );
                                                                                             
            // Now flip bo.Add(  the above on y for bottom right and left                    
            vertices.Add(  new Vector3(vertices[5].x, vertices[5].y * -1f) );
            vertices.Add(  new Vector3(vertices[4].x, vertices[4].y * -1f) );
            vertices.Add(  new Vector3(vertices[3].x, vertices[3].y * -1f) );
                                                                                             
            vertices.Add(  new Vector3(vertices[2].x, vertices[2].y * -1f) );
            vertices.Add(  new Vector3(vertices[1].x, vertices[1].y * -1f) );
            vertices.Add(  new Vector3(vertices[0].x, vertices[0].y * -1f) );

            //      2______________3
            //       |            |
            //  0 ___|            |___ 5
            //   |    1          4    |
            //   |                    |
            //   |                    |
            //   |___10          7____|
            // 11    |            |    6
            //       |____________|
            //      9              8
            //

            // Triangles for these will always be the same, going left to right starting from the top, then the center area between everything.
            List<int> triangles = new List<int> 
            {
                1,2,3,
                3,4,1,

                4,5,6,
                6,7,4,

                7,8,9,
                9,10,7,

                10,11,0,
                0,1,10,

                // Center
                1,4,7,
                7,10,1
            };

            addCurvedCorner(vertices, triangles, 0, 1, 2, cornerVertices);
            addCurvedCorner(vertices, triangles, 3, 4, 5, cornerVertices);
            addCurvedCorner(vertices, triangles, 6, 7, 8, cornerVertices);
            addCurvedCorner(vertices, triangles, 9, 10, 11, cornerVertices);

            meshFilter.mesh = new Mesh { vertices = vertices.ToArray(), triangles = triangles.ToArray() };
        }

        [HideFromIl2Cpp]
        private void addCurvedCorner(List<Vector3> vertices, List<int> triangles, int start, int center, int end, int cornerVertexCount)
        {
            if (cornerVertexCount < 1)
                cornerVertexCount = 1;

            // So we're rotating 90 degrees. 
            // We will add a bunch of vertex along a curve, then draw triangles between them and start/end/center.
            float degreesPerTriangle = (DEGREES_90_IN_RADS) / (cornerVertexCount);

            // We'll be rotating around the center, so translate it to origin, and then start and end by the same amount.

            Vector3 startOrigin = vertices[start] - vertices[center];
            Vector3 endOrigin = vertices[end] - vertices[center];

            int previousVertex = start;       
            for(int i = 0; i < cornerVertexCount; i++ )
            {
                vertices.Add( vertices[center] + Vector3.RotateTowards(startOrigin, endOrigin, degreesPerTriangle * i, 1));
                triangles.Add(previousVertex);
                triangles.Add( vertices.Count - 1);
                triangles.Add(center);

                previousVertex = vertices.Count - 1;
            }

            // then draw the final triangle connecting it to the other end
            triangles.Add(vertices.Count - 1);
            triangles.Add(end);
            triangles.Add(center);
        }
        
        
    }
}
