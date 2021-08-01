using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public enum CON_TYPE { NONE, ZERO, ONE, TWO, TREE };
    public class Connection : MonoBehaviour
    {
        public int Index;
        public string Name;
        public int Start;
        public int End;
        public CON_TYPE Type;
    }
}
