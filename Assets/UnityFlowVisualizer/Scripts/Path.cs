using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class Path : MonoBehaviour
    {
        public int Index;
        public int ID;
        public string Name;
        public string Start;
        public int StartID;
        public string End;
        public int EndID;
        public List<Connection> Connections;
    }
}
