using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace Assets.scripts.canvas
{
    class PhysicalButton : MonoBehaviour
    {
        public BoxCollider m_collider;
        public RectTransform m_rectTrans;
        public RoundedCubeBackground m_background;

        public ButtonClickedEvent onClick = new ButtonClickedEvent();

        public ColorBlock colors = new ColorBlock();

        private void Awake()
        {
            m_collider = this.gameObject.AddComponent<BoxCollider>();
            m_rectTrans = this.gameObject.AddComponent<RectTransform>();
            m_background = this.gameObject.AddComponent<RoundedCubeBackground>();
        }

        public void setSize( float width, float height, float depth )
        {
            m_collider.size = new Vector3(width, height, depth);
            m_background.setSize(width, height);
        }

    }
}
