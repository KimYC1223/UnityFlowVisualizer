using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class Path : MonoBehaviour
    {
        public int Index;
        public string Name;
        public int Start;
        public int StartID;
        public int End;
        public int EndID;
        public List<Node> Nodes;
    }
}
