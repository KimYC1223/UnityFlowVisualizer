using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(PathManager))]
    public class PathManagerGUI : Editor
    {
        [HideInInspector]
        private Tool LastTool = Tool.None;

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

        public void OnSceneGUI() {
            PathManager component = target as PathManager;
            component.transform.position = Vector3.zero;
            component.transform.rotation = Quaternion.identity;
            component.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public override void OnInspectorGUI() {
            try {
                Texture LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
                PathManager component = target as PathManager;
                GUILayout.Label(LogoTex, GUILayout.Width(250));
                GUI.enabled = false;
                GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
                GUILayout.Label("  Developed by KimYC1223");
                GUI.enabled = true;
                GUILayout.Space(10);

                List<PathGroup> list = component.PathGroupList;

                if(list != null) {
                    for(int i = 0; i < list.Count;i++ ) {
                        GUILayout.BeginHorizontal();
                        if(GUILayout.Button("Edit",GUILayout.Width(45))) {
                            PathGroupEditorGUI.ShowWindow();
                            PathGroupEditorGUI.Target = list[i];
                        }
                        PathGroup temp = EditorGUILayout.ObjectField("", list[i], typeof(PathGroup), true) as PathGroup;
                        EditorGUILayout.LabelField("", BackgroundStyle.Get(list[i].PathColor),GUILayout.Width(50));
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(10);
                }

                if (GUILayout.Button("Open Path manager editor",GUILayout.Height(35))) {
                    PathManagerEditorGUI.ShowWindow();
                }
                GUILayout.Space(10);
            } catch (System.Exception e) { e.ToString(); }

        }
    }

}
