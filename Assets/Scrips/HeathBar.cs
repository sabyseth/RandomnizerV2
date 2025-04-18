using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using UnityEngine.WSA;

namespace HudEelments
{
    public class HeathBar : VisualElement
    {

        public int width { get; set; }
        public int height { get; set; }

        private VisualElement hbParent; 
        private VisualElement hbBackground;
        private VisualElement hbForground; 

        public new class UxmlFactory: UxmlFactory<HeathBar, UxmlTraits>{ }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription m_width = new UxmlIntAttributeDescription(){name = "width", defaultValue =  300};
            UxmlIntAttributeDescription m_hight = new UxmlIntAttributeDescription(){name = " height", defaultValue = 50};
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as HeathBar; 
                ate.width = m_width.GetValueFromBag(bag, cc);
                ate.height = m_hight.GetValueFromBag(bag, cc);

                ate.Clear();
                VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/Heathbar");
                VisualElement healthbar = vt.Instantiate();
                ate.hbParent = healthbar.Q<VisualElement>("healthbar");
                ate.hbBackground = healthbar.Q<VisualElement>("background");
                ate.hbForground = healthbar.Q<VisualElement>("forground");
                ate.Add(healthbar);

            } 
        }
    }
}