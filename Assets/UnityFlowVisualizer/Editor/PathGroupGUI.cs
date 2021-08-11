using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(PathGroup))]
    public class PathGroupGUI : Editor {
        [HideInInspector]
        private Tool LastTool = Tool.None;

        private void OnSceneGUI() {
            PathGroup component = target as PathGroup;

            for(int i = 0; i < component.NodeList.Count; i++) {
                component.NodeList[i].Pos = PositionHandle(component.NodeList[i].transform);
                component.NodeList[i].transform.position = component.NodeList[i].Pos;
            }

            if (GUI.changed) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

        public override void OnInspectorGUI() {
            try {
                Texture LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
                PathGroup component = target as PathGroup;
            
                GUI.enabled = false;
                GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
                GUILayout.Label("  Developed by KimYC1223");
                GUI.enabled = true;
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Path group name : ", EditorStyles.boldLabel, GUILayout.Width(110));
                GUILayout.Label(component.name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Path color : ", GUILayout.Width(75), GUILayout.Height(20));
                GUILayout.Label("", BackgroundStyle.Get(component.PathColor), GUILayout.Height(25));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                if (GUILayout.Button("Open Path group editor",GUILayout.Height(35))) {
                    PathGroupEditorGUI.ShowWindow();
                    PathGroupEditorGUI.Target = component;
                }
                GUILayout.Space(10);
            } catch (System.Exception e) { e.ToString(); }
        }

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        public void OnDisable() {
            Tools.current = LastTool;
        }

        public static class BackgroundStyle {
            private static GUIStyle style = new GUIStyle();
            private static GUIStyle style2 = new GUIStyle("ToolbarButton");
            private static GUIStyle style3 = new GUIStyle("ToolbarTextField");
            private static Texture2D texture = new Texture2D(1, 1);
            public static GUIStyle Get(Color color) {
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
                return style;
            }
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
