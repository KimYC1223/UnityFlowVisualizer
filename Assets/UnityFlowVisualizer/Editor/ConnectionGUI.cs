using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(Connection))]
    public class ConnectionGUI : Editor {
        private Tool LastTool = Tool.None;

        private void OnSceneGUI() {
            Connection component = target as Connection;

            try {
                if (component.ConType == CON_TYPE.PLAIN) {
                    for (int i = 0; i < component.CornerList.Count; i++) {
                        component.CornerList[i].transform.position =
                        PositionHandlePlane(component.CornerList[i].transform, component.StartPos.position, component.EndPos.position);
                    }
                } else if (component.ConType == CON_TYPE.FREE) {
                    for (int i = 0; i < component.CornerList.Count; i++) {
                        component.CornerList[i].transform.position = PositionHandle(component.CornerList[i].transform);
                    }
                }
            } catch(System.Exception e) {
                e.ToString();
                Corner[] childs = component.transform.GetComponentsInChildren<Corner>();
                component.CornerList = new List<Corner>();
                if (childs != null && childs.Length > 0)
                    for (int i = 0; i < childs.Length; i++)
                        component.CornerList.Add(childs[i]);
                component.CornerCount = component.CornerList.Count;
                Repaint();
            }
        }

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        public void OnDisable() {
            Tools.current = LastTool;
        }

        Vector3 PositionHandlePlane(Transform transform,Vector3 start,Vector3 end) {
            if((start.x == end.x) && (start.z == end.z))
                return PositionHandle(transform);
            

            var position = transform.position;
            start = new Vector3(start.x, 0f, start.z);
            end = new Vector3(end.x, 0f, end.z);
            Vector3 newDir = new Vector3(end.x - start.x, 0f, end.z - start.z);
            newDir = newDir.normalized;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, newDir); //X축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, Vector3.up); //Z축

            return position;
        }

        Vector3 PositionHandle(Transform transform) {
            var position = transform.position;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, new Vector3(1, 0, 0)); //X축
            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, new Vector3(0, 1, 0)); //Y축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, new Vector3(0, 0, 1)); //Z축

            return position;
        }
    }
}
