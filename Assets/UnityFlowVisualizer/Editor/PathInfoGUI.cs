using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(PathInfo))]
    public class PathInfoGUI : Editor {
        private Tool LastTool = Tool.None;

        private void OnSceneGUI() {
            PathInfo component = target as PathInfo;

            for(int i = 0; i < component.NodeList.Count; i++) {
                component.NodeList[i].Pos = PositionHandle(component.NodeList[i].transform);
                component.NodeList[i].transform.position = component.NodeList[i].Pos;
            }

            if (GUI.changed) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        public void OnDisable() {
            Tools.current = LastTool;
        }


        Vector3 PositionHandle(Transform transform) {
            var position = transform.position;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, transform.right); //X축
            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, transform.up); //Y축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, transform.forward); //Z축

            return position;
        }
    }
}
