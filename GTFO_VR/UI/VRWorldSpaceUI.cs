using GTFO_VR.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.UI
{
    public class VRWorldSpaceUI : MonoBehaviour
    {
        public static PUI_InteractionPrompt statusBar;
        public static PUI_InteractionPrompt interactionBar;

        GameObject statusBarHolder;
        GameObject interactionBarHolder;

        public static InteractionGuiLayer interactGUI;

        void Awake()
        {
            SteamVR_Events.NewPosesApplied.AddListener(() => OnNewPoses());
        }

        private void OnNewPoses()
        {
            UpdateWorldSpaceUI();
        }

        public static void SetRef(PUI_InteractionPrompt status, PUI_InteractionPrompt interact)
        {
            statusBar = status;
            interactionBar = interact;
            //statusBar.RectTrans.anch
        }

        void Start()
        {

            Debug.Log("Creating status and interaction prompt VR UI");
            statusBarHolder = new GameObject("VR_StatusUI");
            interactionBarHolder = new GameObject("VR_InteractionUI");
            //statusBarHolder.transform.localScale = Vector3.one * 0.005f;
            //interactionBarHolder.transform.localScale = Vector3.one * 0.005f;


            //GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //testCube.transform.SetParent(statusBarHolder.transform);
            //testCube.GetComponent<Collider>().enabled = false;
            //testCube.transform.localScale = Vector3.one * .2f;
            //testCube.transform.localPosition = Vector3.zero;




            //UnityEngine.GameObject PUI_Interact_Prefab = Resources.Load("Gui/Player/PUI_InteractionPrompt_CellUI") as GameObject;
            //statusBar = Instantiate(PUI_Interact_Prefab, statusBarHolder.transform, false).GetComponent<PUI_InteractionPrompt>();
            //interactionBar = Instantiate(PUI_Interact_Prefab, interactionBarHolder.transform, false).GetComponent<PUI_InteractionPrompt>();



            //UpdateWorldSpaceUI();
            // typeof(InteractionGuiLayer).GetField("m_message", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(interactGUI, statusBar);
            //typeof(InteractionGuiLayer).GetField("m_interactPrompt", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(interactGUI, interactionBar);

            //Debug.Log((typeof(InteractionGuiLayer).GetField("m_message", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(interactGUI) as PUI_InteractionPrompt).transform.root.name);




            Invoke(nameof(VRWorldSpaceUI.Setup), 1f);

        }

        void Setup()
        {
            foreach (TextMeshPro p in statusBarHolder.GetComponentsInChildren<TextMeshPro>())
            {
                //p.font = Watch.Debug_GetTextMeshFontAsset();
                // if (p.GetComponent<MeshRenderer>())
                // {
                //p.GetComponent<MeshRenderer>().sharedMaterial = Watch.Debug_GetTextMeshProMat();
                // }


                if (p.canvas)
                {
                    p.canvas.renderMode = RenderMode.WorldSpace;
                    p.canvas.worldCamera = UI_Core.UIPassHUD.Camera;
                }

                // p.fontSize = 1f;
                //p.rectTransform.sizeDelta = Vector2.one;
                p.alignment = TextAlignmentOptions.Center;
            }

            foreach (TextMeshPro p in interactionBar.GetComponentsInChildren<TextMeshPro>())
            {
                // p.font = Watch.Debug_GetTextMeshFontAsset();
                // if (p.GetComponent<MeshRenderer>())
                // {
                //     p.GetComponent<MeshRenderer>().sharedMaterial = Watch.Debug_GetTextMeshProMat();
                // }
                if (p.canvas)
                {
                    p.canvas.renderMode = RenderMode.WorldSpace;
                }
                p.fontSize = 1f;
                //p.rectTransform.sizeDelta = Vector2.one;
                //p.alignment = TextAlignmentOptions.Center;

            }

            foreach (Transform t in statusBar.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = 0;
            }
            foreach (Transform t in interactionBar.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = 0;
            }

            //statusBarHolder.transform.SetParent(Watch.Debug_GetTransform());
            // interactionBarHolder.transform.SetParent(Watch.Debug_GetTransform());
            //statusBarHolder.transform.localPosition = Vector3.up * .2f;
            // interactionBarHolder.transform.localPosition = Vector3.up * .4f;
            // statusBarHolder.transform.rotation = Quaternion.identity;
            //interactionBarHolder.transform.rotation = Quaternion.identity;

            //statusBar.gameObject.layer = 0;
            //interactionBar.gameObject.layer = 0;

            //statusBar.transform.localScale = Vector3.one * .001f;
            //interactionBar.transform.localScale = Vector3.one * .001f;

            //statusBarHolder.transform.localPosition = Vector3.zero + Vector3.up * .1f;
            //interactionBarHolder.transform.localPosition = Vector3.zero + Vector3.up * .45f;

            //statusBar.transform.localPosition = Vector3.zero + Vector3.up * .1f;
            //interactionBar.transform.localPosition = Vector3.zero + Vector3.up * .45f;
            foreach (TextMeshPro textMeshPro in statusBar.GetComponentsInChildren<TextMeshPro>())
            {
                textMeshPro.fontSize = 1f;
                //textMeshPro.rectTransform.sizeDelta = Vector2.one * .5f;
            }
            foreach (TextMeshPro textMeshPro in interactionBar.GetComponentsInChildren<TextMeshPro>())
            {
                textMeshPro.fontSize = 1f;
                //textMeshPro.rectTransform.sizeDelta = Vector2.one * .5f;
            }

            statusBar.m_headerText.transform.SetParent(null);
            statusBar.m_headerText.transform.localScale = Vector3.one;

            interactionBar.m_headerText.transform.SetParent(null);
            interactionBar.m_headerText.transform.localScale = Vector3.one;
            //interactGUI.GuiLayerBase.m_layerCanvas.renderMode = RenderMode.WorldSpace;
        }

        void Update()
        {
            DebugKeys();
            //UpdateWorldSpaceUI();
        }

        private void DebugKeys()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
            {
                if (statusBar)
                {

                    foreach (MeshRenderer m in statusBar.GetComponentsInChildren<MeshRenderer>())
                    {
                        foreach (Material mat in m.sharedMaterials)
                        {
                            Debug.Log("Status m " + mat + " s " + mat.shader);
                        }
                        Debug.Log("Status renderer - " + m + " enabled? " + m.enabled);
                    }
                }

                if (interactionBar)
                {
                    foreach (MeshRenderer m in interactionBar.GetComponentsInChildren<MeshRenderer>())
                    {
                        foreach (Material mat in m.sharedMaterials)
                        {
                            Debug.Log("Interaction m " + mat + " s " + mat.shader);

                        }
                        Debug.Log("Interaction renderer - " + m + " enabled? " + m.enabled);
                    }
                }


                //statusBar.gameObject.layer = 0;
                //interactionBar.gameObject.layer = 0;

                //statusBar.transform.localScale = Vector3.one * .001f;
                //interactionBar.transform.localScale = Vector3.one * .001f;

                statusBar.transform.localPosition = Vector3.zero + Vector3.up * .1f;
                interactionBar.transform.localPosition = Vector3.zero + Vector3.up * .45f;

                foreach (TextMeshPro textMeshPro in statusBar.GetComponentsInChildren<TextMeshPro>())
                {
                    textMeshPro.fontSize = 1f;
                    textMeshPro.rectTransform.sizeDelta = Vector2.one;
                }
                foreach (TextMeshPro textMeshPro in interactionBar.GetComponentsInChildren<TextMeshPro>())
                {
                    textMeshPro.fontSize = 1f;
                    textMeshPro.rectTransform.sizeDelta = Vector2.one;
                }

            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F7))
            {
                statusBarHolder.transform.localScale = Vector3.one;
                interactionBarHolder.transform.localScale = Vector3.one;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F8))
            {
                statusBarHolder.transform.localScale *= 10f;
                interactionBarHolder.transform.localScale *= 10f;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
            {
                statusBarHolder.transform.localScale *= .1f;
                interactionBarHolder.transform.localScale *= .1f;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
            {
                foreach (TextMeshPro p in statusBarHolder.GetComponentsInChildren<TextMeshPro>())
                {



                    if (p.canvas)
                    {
                        p.canvas.renderMode = RenderMode.WorldSpace;
                    }

                    p.fontSize = 1f;
                    p.alignment = TextAlignmentOptions.Center;
                }

                foreach (TextMeshPro p in interactionBar.GetComponentsInChildren<TextMeshPro>())
                {
                    //p.font = Watch.Debug_GetTextMeshFontAsset();
                    if (p.GetComponent<MeshRenderer>())
                    {
                        //p.GetComponent<MeshRenderer>().sharedMaterial = Watch.Debug_GetTextMeshProMat();
                    }
                    if (p.canvas)
                    {
                        p.canvas.renderMode = RenderMode.WorldSpace;
                    }
                    p.fontSize = 1f;
                    p.alignment = TextAlignmentOptions.Center;
                }

                foreach (Transform t in statusBarHolder.GetComponentInChildren<Transform>())
                {
                    t.gameObject.layer = 0;
                }
                foreach (Transform t in interactionBar.GetComponentInChildren<Transform>())
                {
                    t.gameObject.layer = 0;
                }

                //statusBarHolder.transform.SetParent(Watch.Debug_GetTransform());
                //interactionBarHolder.transform.SetParent(Watch.Debug_GetTransform());

                //statusBar.gameObject.layer = 0;
                //interactionBar.gameObject.layer = 0;

                //statusBar.transform.localScale = Vector3.one * .001f;
                //interactionBar.transform.localScale = Vector3.one * .001f;

                statusBar.transform.localPosition = Vector3.zero + Vector3.up * .1f;
                interactionBar.transform.localPosition = Vector3.zero + Vector3.up * .45f;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
            {
                if (statusBar)
                {
                    foreach (MeshRenderer m in statusBar.GetComponentsInChildren<MeshRenderer>())
                    {
                        //m.sharedMaterial = Watch.Debug_GetTextMeshProMat();
                    }
                }
                if (interactionBar)
                {
                    foreach (MeshRenderer m in interactionBar.GetComponentsInChildren<MeshRenderer>())
                    {
                        //m.sharedMaterial = Watch.Debug_GetTextMeshProMat();
                    }
                }

            }

        }

        void UpdateWorldSpaceUI()
        {
            interactionBar.m_headerText.enabled = interactGUI.IsVisible() && interactGUI.InteractPromptVisible;
            if (interactionBar.m_headerText.enabled)
            {
                interactionBar.m_headerText.transform.localScale = Vector3.one * 0.0012f;
                interactionBar.m_headerText.transform.position = GetInteractionPromptPosition();
                interactionBar.m_headerText.transform.rotation = Quaternion.LookRotation(PlayerVR.fpsCamera.transform.forward);

            }
            statusBar.m_headerText.enabled = interactGUI.IsVisible() && interactGUI.MessageVisible;
            if (statusBar.m_headerText.enabled)
            {
                statusBar.m_headerText.transform.localScale = Vector3.one * 0.0012f;
                statusBar.m_headerText.transform.position = PlayerVR.fpsCamera.transform.position - new Vector3(0, -0.25f, 0) + PlayerVR.fpsCamera.transform.forward * .75f;
                statusBar.m_headerText.transform.rotation = Quaternion.LookRotation(PlayerVR.fpsCamera.transform.forward);
            }
        }


        Vector3 GetInteractionPromptPosition()
        {
            if(PlayerVR.fpsCamera.CameraRayObject && PlayerVR.fpsCamera.CameraRayDist < 1f)
            {
                return PlayerVR.fpsCamera.CameraRayPos - PlayerVR.fpsCamera.transform.forward * .1f;
            }
            return PlayerVR.fpsCamera.transform.position - new Vector3(0, 0.25f, 0) + PlayerVR.fpsCamera.CameraRayDir * .5f;
        }
    }
}


