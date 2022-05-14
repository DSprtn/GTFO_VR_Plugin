using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GTFO_VR.Core.UI.canvas.KeyboardDefinition
{
    class RectColliderSizer : UIBehaviour
    {
        protected override void OnRectTransformDimensionsChange()
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            RectTransform trans = GetComponent<RectTransform>();

            collider.size = new Vector3(trans.sizeDelta.x, trans.sizeDelta.y, collider.size.z);
        }


    }
}
