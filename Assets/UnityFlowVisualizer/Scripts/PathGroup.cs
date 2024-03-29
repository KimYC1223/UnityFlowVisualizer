﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    [ExecuteInEditMode]
    public class PathGroup : MonoBehaviour
    {
        public string PathName = "";

        [Range(0,1000)]
        public float Thickness = 0.5f;
        public Color PathColor;

        [HideInInspector]
        public List<Node> NodeList;
        [HideInInspector]
        public List<Connection> ConnectionList;
        [HideInInspector]
        public List<Path> PathList;

        [HideInInspector]
        public GameObject NodeParent;
        [HideInInspector]
        public GameObject ConnectionParent;
        [HideInInspector]
        public GameObject PathParent;
        [HideInInspector]
        public GameObject CornerParent;

        [HideInInspector]
        public Material PathMat;

        Coroutine myRoutine = null;

        public void OnEnable() {
            Material PathMatOrigin = (Material)Resources.Load("UnityFlowVisualizer/Materials/PathMat", typeof(Material));
            PathMat = new Material(PathMatOrigin);
            PathMat.color = PathColor;

            MeshRenderer[] renderers = this.transform.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer renderer in renderers)
                renderer.sharedMaterial = PathMat;

            if (myRoutine != null) {
                StopCoroutine(myRoutine);
                myRoutine = null;
            }
            bool isNodeParent = false, isConnectionParent = false, isPathParent = false, isCornerParent = false;
            for(int i = 0; i < this.transform.childCount; i++) {
                if (isNodeParent && isConnectionParent && isPathParent && isCornerParent) break;

                string childName = this.transform.GetChild(i).name;
                if (childName == "Nodes") isNodeParent = true;
                else if (childName == "Connections") isConnectionParent = true;
                else if (childName == "Paths") isPathParent = true;
                else if (childName == "Corners") isCornerParent = true;
            }
            if(!( isNodeParent && isConnectionParent && isPathParent && isCornerParent)) {
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

                if (!isCornerParent) {
                    GameObject newOb = new GameObject();
                    newOb.transform.parent = this.transform;
                    newOb.name = "Corners";
                    CornerParent = newOb;
                }
            }
            myRoutine = StartCoroutine(CheckParent());
        }

        public IEnumerator CheckParent() {
            while (true) {
                yield return 0.5f;
                bool flag = true;
                try {
                    PathManager pm = this.transform.parent.GetComponent<PathManager>();
                } catch(System.Exception e) { e.ToString();
                    Debug.LogError("The Path Group script should have a parent with a Path Manager component.");
                    flag = false;
                }
                if(flag) {
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
}
