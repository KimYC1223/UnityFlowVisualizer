using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [ExecuteInEditMode]
    public class Corner : MonoBehaviour
    {
        public Transform Destination;
        public Transform Line;
        public PathInfo pathInfo;
        public Connection Connection;
        public int ParentID = 0;

        public void Update() {
            if (Connection.ConType == CON_TYPE.PRESET && Connection.PresetType1 == PRESET_TYPE_1_CORNER.END) {
                this.transform.position = new Vector3(Destination.position.x, Connection.StartPos.position.y, Destination.position.z);
            } else if (Connection.ConType == CON_TYPE.PRESET && Connection.PresetType1 == PRESET_TYPE_1_CORNER.START) {
                this.transform.position = new Vector3(Connection.StartPos.position.x, Destination.position.y, Connection.StartPos.position.z);
            }
        }

        public void LateUpdate() {

            if (pathInfo != null) {
                this.transform.LookAt(Destination);
                this.transform.localScale = new Vector3(pathInfo.Thickness, pathInfo.Thickness, pathInfo.Thickness);
                float distance = Vector3.Distance(this.transform.position, Destination.position) / pathInfo.Thickness;
                Line.localScale = new Vector3(1f, 1f, distance);
            }
        }
    }
}
