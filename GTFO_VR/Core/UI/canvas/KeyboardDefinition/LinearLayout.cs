using GTFO_VR.UI.CANVAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.KeyboardDefinition
{
    public enum LinearOrientation { VERTICAL, HORIZONTAL };

    public class LinearLayout : KeyboardLayout
    {
        private KeyboardLayoutParameters m_layoutParameters;
        private LinearOrientation m_orientation;
        private TextAnchor m_gravity;  // Turns out they reuse this for Auto Layout.
        private string m_name;
        private KeyboardStyle m_style;

        private List<KeyboardLayout> m_children = new List<KeyboardLayout>();
        
        public LinearLayout(LinearOrientation orientation=LinearOrientation.VERTICAL) : this(orientation, TextAnchor.UpperCenter, new KeyboardLayoutParameters(1, 1, true, true)) { }

        public LinearLayout(LinearOrientation orientation, string name) : this(orientation, TextAnchor.UpperCenter, new KeyboardLayoutParameters(1, 1, true, true), name) { }

        public LinearLayout(LinearOrientation orientation, TextAnchor gravity) : this(orientation, gravity, new KeyboardLayoutParameters(1,1,true,true)){ }
        
        public LinearLayout(LinearOrientation orientation, TextAnchor gravity, KeyboardLayoutParameters layoutParameters) : this(orientation, gravity, layoutParameters, null) { }

        public LinearLayout(LinearOrientation orientation,TextAnchor gravity, KeyboardLayoutParameters layoutParams, string name )
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
            m_layoutParameters.populateLayoutElement(element, inheritedStyle);

            // preferred height/width is only respected in the orientation of the layout.
            // constrain the other direction using a ContentSizeFitter, I guess.
            ContentSizeFitter sizeFitter = panel.AddComponent<ContentSizeFitter>();
            
            HorizontalOrVerticalLayoutGroup layoutGroup = null;
            switch (m_orientation)
            {
                case LinearOrientation.VERTICAL:
                    layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
                    layoutGroup.spacing = inheritedStyle.SpacingVertical;
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
                case LinearOrientation.HORIZONTAL:
                    layoutGroup = panel.AddComponent<HorizontalLayoutGroup>();
                    layoutGroup.spacing = inheritedStyle.SpacingHorizontal;
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
