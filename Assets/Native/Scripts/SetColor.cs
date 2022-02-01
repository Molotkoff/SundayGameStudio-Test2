using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Molotkoff.Test2App
{
    public class SetColor : MonoBehaviour
    {
        [SerializeField]
        private Drawable drawable;
        [SerializeField]
        private Color color;

        public void Set()
        {
            drawable.Color = color;
        }
    }
}