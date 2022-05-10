using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.scripts.KeyboardDefinition
{
    public class KeyboardLayoutParameters
    {
        public float width = 1;     // Size of element multiplied by tile size defined in KeyboardStyle.
        public float height = 1;    
        public bool expandWidth = false;    // Will expand to fill empty space, using width/height as weight
        public bool expandHeight = false;

        public KeyboardLayoutParameters() : this(1, 1, false, false) { }

        public KeyboardLayoutParameters(float width) : this(width, 1, false, false) { }

        public KeyboardLayoutParameters(float width, bool expandWidth) : this(width, 1, expandWidth, false) { }

        public KeyboardLayoutParameters( float width, float height, bool expandWidth, bool expandHeight )
        {
            this.width = width;
            this.height = height;
            this.expandWidth = expandWidth;
            this.expandHeight = expandHeight;
        }

        public LayoutElement populateLayoutElement(LayoutElement element, KeyboardStyle style)
        {
            if (expandWidth)
            {
                element.preferredWidth = -1;
                element.flexibleWidth = width;
            }
            else
            {
                element.preferredWidth = style.TileSize * this.width;
                element.flexibleWidth = -1;
            }

            if (expandHeight)
            {
                element.preferredHeight = -1;
                element.flexibleHeight = height;
            }
            else
            {
                element.preferredHeight = style.TileSize * this.height;
                element.flexibleHeight = -1;
            }

            return element;
        }
    }
}
