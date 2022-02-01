using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Molotkoff.Test2App
{
    public class DrawMemo
    {
        public Color[] Colors { get; private set; }

        public DrawMemo(Color[] colors)
        {
            this.Colors = (Color[])colors.Clone();
        }
    }
}