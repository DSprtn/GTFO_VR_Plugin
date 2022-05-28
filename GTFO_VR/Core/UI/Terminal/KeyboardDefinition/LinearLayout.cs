using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GTFO_VR.Core.UI.Terminal.KeyboardDefinition
{
    public enum LinearOrientation { VERTICAL, HORIZONTAL };

    /// <summary>
    /// Wrapped for Vertical/HorizontalLayoutGroup
    /// </summary>
    public class LinearLayout : KeyboardLayout
    {
        private string m_name;
        private TextAnchor m_gravity;  // Turns out they reuse this for Auto Layout.
        private KeyboardStyle m_style;
        private LinearOrientation m_orientation;
        private LayoutParameters m_layoutParameters;

        private List<KeyboardLayout> m_children = new List<KeyboardLayout>();


        public bool ShowBackground = false;
        
        public LinearLayout(LinearOrientation orientation=LinearOrientation.VERTICAL) : this(orientation, TextAnchor.UpperCenter, LayoutParameters.FillParent() ) { }

        public LinearLayout(LinearOrientation orientation, string name) : this(orientation, TextAnchor.UpperCenter, LayoutParameters.FillParent(), name) { }

        public LinearLayout(LinearOrientation orientation, TextAnchor gravity) : this(orientation, gravity, LayoutParameters.FillParent()) { }
        
        public LinearLayout(LinearOrientation orientation, TextAnchor gravity, LayoutParameters layoutParameters) : this(orientation, gravity, layoutParameters, null) { }

        public LinearLayout(LinearOrientation orientation,TextAnchor gravity, LayoutParameters layoutParams, string name )
        {
            m_orientation = orientation;
            m_gravity = gravity;
            m_name = name;
            m_layoutParameters = layoutParams;
        }

        public void AddChild(KeyboardLayout layout)
        {
            m_children.Add(layout);
        }

        public GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle inheritedStyle)
        {
            
            if (m_style != null)
                inheritedStyle = m_style;

            GameObject panel = new GameObject();
            panel.name = GetName();
            panel.AddComponent<RectTransform>();

            LayoutElement element = panel.AddComponent<LayoutElement>();
            m_layoutParameters.PopulateLayoutElement(element, inheritedStyle);

            if ( ShowBackground )
            {
                RoundedCubeBackground background = panel.AddComponent<RoundedCubeBackground>();
                background.SetMaterial(inheritedStyle.GetBackgroundMaterial());
                background.Radius = inheritedStyle.keyboardBackgroundStyle.radius;
                background.CornerVertices = inheritedStyle.keyboardBackgroundStyle.cornerVertices;
                background.Padding = inheritedStyle.keyboardBackgroundStyle.padding;
                background.AutoSize = true;

                background.GetComponent<MeshRenderer>().sharedMaterial = inheritedStyle.GetBackgroundMaterial();
            }

            // preferred height/width is only respected in the orientation of the layout.
            // constrain the other direction using a ContentSizeFitter, I guess.
            ContentSizeFitter sizeFitter = panel.AddComponent<ContentSizeFitter>();
            
            HorizontalOrVerticalLayoutGroup layoutGroup = null;
            switch (m_orientation)
            {
                case LinearOrientation.VERTICAL:
                    layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
                case LinearOrientation.HORIZONTAL:
                    layoutGroup = panel.AddComponent<HorizontalLayoutGroup>();
                    sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
            }

            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;

            layoutGroup.childAlignment = m_gravity;

            foreach ( KeyboardLayout layout in m_children )
            {
                GameObject child = layout.GenerateLayout(keyboardRoot, inheritedStyle);
                child.transform.SetParent(panel.transform);
            }
            
            return panel;
        }

        public string GetName()
        {
            if (m_name != null)
                return m_name;
            return "LinearLayout-" + m_orientation.ToString();
        }

        public void SetStyle(KeyboardStyle style)
        {
            throw new NotImplementedException();
        }
    }
}
