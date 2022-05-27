using UnityEngine.UI;

namespace GTFO_VR.Core.UI.Terminal.KeyboardDefinition
{

    public class LayoutParameters
    {
        public static readonly float FILL_PARENT = -1;
        public static readonly float WRAP_CONTENT = -2;

        public static LayoutParameters FillParent()
        {
            return new LayoutParameters(FILL_PARENT, FILL_PARENT);
        }

        public static LayoutParameters WrapContent()
        {
            return new LayoutParameters(WRAP_CONTENT, WRAP_CONTENT);
        }

        public float width = 1;     // Size of element multiplied by tile size defined in KeyboardStyle.
        public float height = 1;

        public float weight = 1;

        public LayoutParameters() : this(1, 1, 1) { }
        public LayoutParameters(float width) : this(width, 1, 1) { }
        public LayoutParameters(float width, float height) : this(width, height, 1) { }

        public LayoutParameters( float width, float height, float weight)
        {
            this.width = width;
            this.height = height;
            this.weight = weight;
        }

        public LayoutElement populateLayoutElement(LayoutElement element, KeyboardStyle style)
        {
            if ( width == FILL_PARENT )
            {
                element.preferredWidth = -1;
                element.flexibleWidth = weight;
            }
            else if (width == WRAP_CONTENT)
            {
                element.preferredWidth = -1;
                element.flexibleWidth = -1;
            }
            else
            {
                element.preferredWidth = style.TileSize * this.width;
                element.flexibleWidth = -1;
            }

            if (height == FILL_PARENT)
            {
                element.preferredHeight = -1;
                element.flexibleHeight = weight;
            }
            else if (width == WRAP_CONTENT)
            {
                element.preferredHeight = -1;
                element.flexibleHeight = -1;
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
