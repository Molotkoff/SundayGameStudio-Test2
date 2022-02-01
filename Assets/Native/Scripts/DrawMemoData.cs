using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Molotkoff.Test2App
{
    [CreateAssetMenu(menuName = "Paint/DrawMemoData")]
    public class DrawMemoData : ScriptableObject
    {
        public Stack<DrawMemo> memos = new Stack<DrawMemo>();
    }
}