using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    [ExecuteInEditMode]
    public class PathInfo : MonoBehaviour
    {
        public string PathName = "";
        [Range(0,1000)]
        public float Thickness = 1;
        public Color PathColor;

        public List<Node> NodeList;
        public List<Connection> ConnectionList;
        public List<Path> PathList;

        public GameObject NodeParent;
        public GameObject ConnectionParent;
        public GameObject PathParent;

        Coroutine myRoutine = null;

        public void OnEnable() {
            if(myRoutine != null) {
                StopCoroutine(myRoutine);
                myRoutine = null;
            }
            bool isNodeParent = false, isConnectionParent = false, isPathParent = false;
            for(int i = 0; i < this.transform.childCount; i++) {
                if (isNodeParent && isConnectionParent && isPathParent) break;

                string childName = this.transform.GetChild(i).name;
                if (childName == "Nodes") isNodeParent = true;
                else if (childName == "Connections") isConnectionParent = true;
                else if (childName == "Paths") isPathParent = true;
            }
            if(!( isNodeParent && isConnectionParent && isPathParent )) {
                NodeList = new List<Node>();
                ConnectionList = new List<Connection>();
                PathList = new List<Path>();

                if(!isNodeParent) {
                    GameObject newOb = new GameObject();
                    newOb.transform.parent = this.transform;
                    newOb.name = "Nodes";
                    NodeParent = newOb;
                } else {
                    Node[] array = NodeParent.GetComponentsInChildren<Node>();
                    foreach (Node element in array)
                        NodeList.Add(element);
                }

                if (!isConnectionParent) {
                    GameObject newOb = new GameObject();
                    newOb.transform.parent = this.transform;
                    newOb.name = "Connections";
                    ConnectionParent = newOb;
                } else {
                    Connection[] array = ConnectionParent.GetComponentsInChildren<Connection>();
                    foreach (Connection element in array)
                        ConnectionList.Add(element);
                }

                if (!isPathParent) {
                    GameObject newOb = new GameObject();
                    newOb.transform.parent = this.transform;
                    newOb.name = "Paths";
                    PathParent = newOb;
                } else {
                    Path[] array = PathParent.GetComponentsInChildren<Path>();
                    foreach (Path element in array)
                        PathList.Add(element);
                }
            }
            myRoutine = StartCoroutine(CheckParent());
        }

        public IEnumerator CheckParent() {
            while (true) {
                yield return 0.5f;
                bool isNodeParent = false, isConnectionParent = false, isPathParent = false;
                for (int i = 0; i < this.transform.childCount; i++) {
                    if (isNodeParent && isConnectionParent && isPathParent) break;

                    string childName = this.transform.GetChild(i).name;
                    if (childName == "Nodes") isNodeParent = true;
                    else if (childName == "Connections") isConnectionParent = true;
                    else if (childName == "Paths") isPathParent = true;
                }
                if (!( isNodeParent && isConnectionParent && isPathParent )) {
                    if (!isNodeParent) {
                        GameObject newOb = new GameObject();
                        newOb.transform.parent = this.transform;
                        newOb.name = "Nodes";
                        NodeParent = newOb;
                        NodeList = new List<Node>();
                    }
                    if (!isConnectionParent) {
                        GameObject newOb = new GameObject();
                        newOb.transform.parent = this.transform;
                        newOb.name = "Connections";
                        ConnectionParent = newOb;
                        ConnectionList = new List<Connection>();
                    }
                    if (!isPathParent) {
                        GameObject newOb = new GameObject();
                        newOb.transform.parent = this.transform;
                        newOb.name = "Paths";
                        PathParent = newOb;
                        PathList = new List<Path>();
                    }
                }
            }
        }
    }
}
