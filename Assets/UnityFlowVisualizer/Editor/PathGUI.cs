using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(Path))]
    public class PathGUI : Editor
    {
        [HideInInspector]
        private Tool LastTool = Tool.None;

        [HideInInspector]
        Path component;

        GUIStyle centerStyle;

        public void OnEnable() {
            component = target as Path;
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }
        public void OnDisable() {
            Tools.current = LastTool;
            LastTool = Tool.None;
        }

        public override void OnInspectorGUI() {
            try {
                centerStyle = new GUIStyle("Label");
                centerStyle.alignment = TextAnchor.MiddleCenter;
                Texture LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
                GUILayout.Label(LogoTex, GUILayout.Width(250));
                GUI.enabled = false;
                GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
                GUILayout.Label("  Developed by KimYC1223");
                GUI.enabled = true;
                GUILayout.Space(10);
                GuiLine();
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Path group", EditorStyles.boldLabel, GUILayout.Width(100));
                PathGroup temp = EditorGUILayout.ObjectField("", component.ParentPathGroup, typeof(PathGroup), true) as PathGroup;
                EditorGUILayout.LabelField("", PathManagerEditorGUI.BackgroundStyle.Get(component.ParentPathGroup.PathColor), GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Path name ", EditorStyles.boldLabel, GUILayout.Width(100));
                component.Name = GUILayout.TextField(component.Name);
                component.transform.gameObject.name = component.Name;
                GUILayout.EndHorizontal();

                Node startNode = null;
                Node endNode = null;
                for (int i = 0; i < component.ParentPathGroup.NodeList.Count; i++) {
                    if (component.ParentPathGroup.NodeList[i].ID == component.StartID)
                        startNode = component.ParentPathGroup.NodeList[i];
                    if (component.ParentPathGroup.NodeList[i].ID == component.EndID)
                        endNode = component.ParentPathGroup.NodeList[i];
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Start node",EditorStyles.boldLabel, GUILayout.Width(100));
                Node temp2 = EditorGUILayout.ObjectField("", startNode, typeof(Node), true) as Node;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("End node", EditorStyles.boldLabel, GUILayout.Width(100));
                Node temp3 = EditorGUILayout.ObjectField("", endNode, typeof(Node), true) as Node;
                GUILayout.EndHorizontal();

                if(component.Connections.Count > 0) {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    GUILayout.Label("Order", EditorStyles.toolbarButton, GUILayout.Width(60f));
                    GUILayout.Label("Visit node", EditorStyles.toolbarButton);
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < component.Connections.Count; i++) {
                        GUILayout.BeginHorizontal(EditorStyles.toolbar);
                        GUILayout.Label(i.ToString(), centerStyle, GUILayout.Width(60f));
                        GUILayout.Label(component.Connections[i].Start, centerStyle);
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label(component.Connections.Count.ToString(), centerStyle, GUILayout.Width(60f));
                GUILayout.Label(component.Connections[component.Connections.Count-1].End, centerStyle);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                GuiLine();
                GUILayout.Space(10);
                if (GUILayout.Button("Open Path editor", GUILayout.Height(35))) {
                    PathEditorGUI.ShowWindow();
                    PathEditorGUI.Target = component;
                }
                GUILayout.Space(10);
            } catch(System.Exception e) { e.ToString(); }
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
