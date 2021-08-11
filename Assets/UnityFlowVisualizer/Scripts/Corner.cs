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
        public PathGroup pathGroup;
        public Connection Connection;

        [CustomEditor(typeof(Corner))]
        public class ConerGUI : Editor {
            public void OnSceneGUI() {
                Corner component = target as Corner;
                if (component.Connection != null) {
                    Selection.objects = new UnityEngine.Object[] { component.Connection.gameObject };
                }
            }
        }

        public void Update() {
            if (Connection.CornerList.Count == 1 && Connection.ConType == CON_TYPE.PRESET && Connection.PresetType1 == PRESET_TYPE_1_CORNER.END) {
                this.transform.position = new Vector3(Destination.position.x, Connection.StartPos.position.y, Destination.position.z);
            } else if (Connection.CornerList.Count == 1 && Connection.ConType == CON_TYPE.PRESET && Connection.PresetType1 == PRESET_TYPE_1_CORNER.START) {
                this.transform.position = new Vector3(Connection.StartPos.position.x, Destination.position.y, Connection.StartPos.position.z);
            }
            
            else if (Connection.CornerList.Count == 2 && Connection.ConType == CON_TYPE.PRESET && Connection.PresetType2 == PRESET_TYPE_2_CORNER.X) {
                if(Connection.CornerList[0] == this)
                    this.transform.position = new Vector3(Connection.StartPos.position.x, Connection.Preset2Handler.y, Connection.Preset2Handler.z);
                else this.transform.position = new Vector3(Connection.EndPos.position.x, Connection.Preset2Handler.y, Connection.Preset2Handler.z);
            } else if (Connection.CornerList.Count == 2 && Connection.ConType == CON_TYPE.PRESET && Connection.PresetType2 == PRESET_TYPE_2_CORNER.Y) {
                if (Connection.CornerList[0] == this)
                    this.transform.position = new Vector3(Connection.Preset2Handler.x, Connection.StartPos.position.y, Connection.Preset2Handler.z);
                else this.transform.position = new Vector3(Connection.Preset2Handler.x, Connection.EndPos.position.y, Connection.Preset2Handler.z);
            } else if (Connection.CornerList.Count == 2 && Connection.ConType == CON_TYPE.PRESET && Connection.PresetType2 == PRESET_TYPE_2_CORNER.Z) {
                if (Connection.CornerList[0] == this)
                    this.transform.position = new Vector3(Connection.Preset2Handler.x, Connection.Preset2Handler.y, Connection.StartPos.position.z);
                else this.transform.position = new Vector3(Connection.Preset2Handler.x, Connection.Preset2Handler.y, Connection.EndPos.position.z);
            } 
            
            
            else if (Connection.CornerList.Count == 3 && Connection.ConType == CON_TYPE.PRESET &&
                        ( Connection.PresetType3 == PRESET_TYPE_3_CORNER.XY1 || Connection.PresetType3 == PRESET_TYPE_3_CORNER.XY2 )) {
                if (Connection.CornerList[0] == this)
                    this.transform.position = new Vector3(Connection.StartPos.position.x, Connection.StartPos.position.y, Connection.Preset3Handler.z);
                else if (Connection.CornerList[1] == this)
                    this.transform.position = new Vector3(Connection.Preset3Handler.x, Connection.Preset3Handler.y, Connection.Preset3Handler.z);
                else this.transform.position = new Vector3(Connection.EndPos.position.x, Connection.EndPos.position.y, Connection.Preset3Handler.z);
            } else if (Connection.CornerList.Count == 3 && Connection.ConType == CON_TYPE.PRESET &&
                        ( Connection.PresetType3 == PRESET_TYPE_3_CORNER.YZ1 || Connection.PresetType3 == PRESET_TYPE_3_CORNER.YZ2 )) {
                if (Connection.CornerList[0] == this)
                    this.transform.position = new Vector3(Connection.Preset3Handler.x, Connection.StartPos.position.y, Connection.StartPos.position.z);
                else if (Connection.CornerList[1] == this)
                    this.transform.position = new Vector3(Connection.Preset3Handler.x, Connection.Preset3Handler.y, Connection.Preset3Handler.z);
                else this.transform.position = new Vector3(Connection.Preset3Handler.x, Connection.EndPos.position.y, Connection.EndPos.position.z);
            } else if (Connection.CornerList.Count == 3 && Connection.ConType == CON_TYPE.PRESET &&
                        ( Connection.PresetType3 == PRESET_TYPE_3_CORNER.XZ1 || Connection.PresetType3 == PRESET_TYPE_3_CORNER.XZ2 )) {
                if (Connection.CornerList[0] == this)
                    this.transform.position = new Vector3(Connection.StartPos.position.x, Connection.Preset3Handler.y, Connection.StartPos.position.z);
                else if (Connection.CornerList[1] == this)
                    this.transform.position = new Vector3(Connection.Preset3Handler.x, Connection.Preset3Handler.y, Connection.Preset3Handler.z);
                else this.transform.position = new Vector3(Connection.EndPos.position.x, Connection.Preset3Handler.y, Connection.EndPos.position.z);
            }
        }

        public void LateUpdate() {

            if (pathGroup != null) {
                this.transform.LookAt(Destination);
                this.transform.localScale = new Vector3(pathGroup.Thickness, pathGroup.Thickness, pathGroup.Thickness);
                float distance = Vector3.Distance(this.transform.position, Destination.position) / pathGroup.Thickness;
                Line.localScale = new Vector3(1f, 1f, distance);
            }
        }
    }
}
