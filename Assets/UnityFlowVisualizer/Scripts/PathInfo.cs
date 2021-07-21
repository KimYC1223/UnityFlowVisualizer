using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class PathInfo : MonoBehaviour
    {
        public string PathName = "";
        public float Tickness = 1;
        public Color PathColor;

        public class Node {
            public int index;
            public Vector3 Pos;
            public string Name;
        }

        public enum CON_TYPE { NONE, ZERO, ONE, TWO, TREE};

        public class Connection {
            public int index;
            public int Start;
            public int End;
            public CON_TYPE Type;
        }

        public List<Node> NodeList;
        public List<Connection> ConnectionList;

    }
}
