using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {

    [ExecuteInEditMode]
    public class Node:MonoBehaviour{
        public int Index;
        public string Name;
        public Vector3 Pos;
        public Transform PinnedObject;
        public PathGroup group;
        public int ID;

        public void Update() {
            if(PinnedObject != null) {
                this.transform.position = PinnedObject.position;
                Pos = PinnedObject.position;
            }

            if(group == null) {
                try {
                    group = this.transform.parent.parent.GetComponent<PathGroup>();
                } catch (System.Exception e) {
                    e.ToString();
                }
            }
        }

    }
}
