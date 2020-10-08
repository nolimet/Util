﻿using UnityEngine;
using UnityEngine.UI;

namespace Util.UI
{
    /// <summary>
    /// A classes that is used to create a slider for the Scrollrect thatdoes not change size
    /// </summary>
    public class SliderSetter : MonoBehaviour
    {
        public Slider slider;
        public ScrollRect scrollRect;
        public float StartValue;

        private void Start()
        {
            slider.value = StartValue;
            scrollRect.verticalNormalizedPosition = StartValue;
        }
    }
}