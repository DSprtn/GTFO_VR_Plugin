
using GTFO_VR.Events;
using UnityEngine;

namespace GTFO_VR
{
    public class LaserPointer : MonoBehaviour
    {
        public bool active = true;
        public float thickness = 1f / 400f;

        public Color color = Color.red;
        public GameObject holder;
        public GameObject pointer;
        public GameObject dot;
        public int layerMask = 0;

        public Vector3 dotScale = new Vector3(0.04f, 0.01f, 0.016f);

        void Awake()
        {
            ItemEquippableEvents.OnPlayerWieldItem += PlayerChangedItem;
        }

        private void Start()
        {
            CreatePointerObjects();
        }

        private void Update()
        {
            if (pointer.activeSelf)
            {
                float hitDistance = 35f;
                RaycastHit hitInfo;
                bool raycastHit = Physics.Raycast(transform.position, transform.forward, out hitInfo, hitDistance, layerMask);

                if (raycastHit && hitInfo.distance < hitDistance)
                {
                    hitDistance = hitInfo.distance;
                    dot.SetActive(true);
                    dot.transform.position = hitInfo.point;
                    dot.transform.rotation = Quaternion.LookRotation(pointer.transform.up);
                }
                else
                {
                    dot.SetActive(false);
                }

                pointer.transform.localScale = new Vector3(this.thickness, this.thickness, hitDistance);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, hitDistance / 2f);
            }
        }


        public void PlayerChangedItem(ItemEquippable item)
        {
            
            if (item.HasFlashlight && item.AmmoType != Player.AmmoType.None)
            {
                EnablePointer();
            }
            else
            {
                DisablePointer();
            }

            SetHolderTransform(item.MuzzleAlign);
        }

        void EnablePointer()
        {
            pointer.SetActive(true);
            dot.SetActive(true);
        }

        void DisablePointer()
        {
            pointer.SetActive(false);
            dot.SetActive(false);
        }

        void SetHolderTransform(Transform t) 
        {
            holder.transform.SetParent(t);
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;
        }
        private void CreatePointerObjects()
        {
            holder = new GameObject();
            holder.transform.parent = transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dot.transform.localScale = dotScale;
            dot.transform.SetParent(holder.transform);
            dot.GetComponent<Collider>().enabled = false;
            pointer.GetComponent<Collider>().enabled = false;

            pointer.transform.parent = this.holder.transform;
            pointer.transform.localScale = new Vector3(this.thickness, this.thickness, 100f);
            pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            dot.transform.localRotation = Quaternion.identity;
            Material material = new Material(Shader.Find("Unlit/Color"));
            material.SetColor("_Color", this.color);
            pointer.GetComponent<MeshRenderer>().material = material;
            dot.GetComponent<MeshRenderer>().material = material;
        }

        void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= PlayerChangedItem;
        }
    }
}
