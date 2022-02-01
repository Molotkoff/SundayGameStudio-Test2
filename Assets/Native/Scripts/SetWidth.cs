using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Molotkoff.Test2App
{
    public class SetWidth : MonoBehaviour
    {
        [SerializeField]
        private Drawable drawble;
        [SerializeField]
        private Slider slider;

        public void Set(float _)
        {
            drawble.BrushWidth = (int)slider.value;
        }
    }
}