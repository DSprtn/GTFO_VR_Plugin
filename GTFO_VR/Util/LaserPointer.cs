
using GTFO_VR.Events;
using UnityEngine;

namespace GTFO_VR
{
    public class LaserPointer : MonoBehaviour
    {
        public float thickness = 1f / 400f;

        public Color color = Color.red;
        public GameObject pointer;
        public GameObject dot;

        public Vector3 dotScale = new Vector3(0.04f, 0.01f, 0.016f);
        public float dotMultiplierByDistance = 2.5f;

        bool setup = false;

        void Awake()
        {
            ItemEquippableEvents.OnPlayerWieldItem += PlayerChangedItem;
        }

        private void Start()
        {
            CreatePointerObjects();
        }

        private void LateUpdate()
        {
            if(transform.parent == null)
            {
                return;
            }
            float dist = 50f;

            Ray raycast = new Ray(transform.parent.position, transform.parent.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit, 51f, LayerManager.MASK_WEAPON_TARGETS, QueryTriggerInteraction.Ignore);

            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
                dot.transform.rotation = Quaternion.LookRotation(pointer.transform.up);
                dot.transform.position = hit.point;
                dot.transform.localScale = Vector3.Lerp(dotScale, dotScale * 3f, dist / 51f);
            }
            else
            {
                dot.SetActive(false);
            }

            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);

        }


        public void PlayerChangedItem(ItemEquippable item)
        {
            if(!setup)
            {
                return;
            }
            if (item.HasFlashlight && item.AmmoType != Player.AmmoType.None)
            {
                EnablePointer();
                SetHolderTransform(item.MuzzleAlign);
            }
            else
            {
                DisablePointer();
            }
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
            transform.SetParent(t);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            dot.transform.localRotation = Quaternion.identity;
            dot.transform.localScale = dotScale;
        }
        private void CreatePointerObjects()
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dot.transform.localScale = dotScale;
            dot.transform.SetParent(transform);
            dot.GetComponent<Collider>().enabled = false;
            pointer.GetComponent<Collider>().enabled = false;

            pointer.transform.parent = transform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            dot.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            dot.transform.localRotation = Quaternion.identity;
            Material material = new Material(Shader.Find("Unlit/Color"));
            material.SetColor("_Color", this.color);
            pointer.GetComponent<MeshRenderer>().material = material;
            dot.GetComponent<MeshRenderer>().material = material;
            setup = true;
        }

        void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= PlayerChangedItem;
        }
    }
}
