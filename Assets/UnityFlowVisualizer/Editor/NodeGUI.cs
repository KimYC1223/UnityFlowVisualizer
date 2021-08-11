using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(Node))]
    public class NodeGUI : Editor {
        [HideInInspector]
        private Tool LastTool = Tool.None;


        [HideInInspector]
        Node component;

        public void OnEnable() {
            component = target as Node;
            if(component.PinnedObject) {
                LastTool = Tools.current;
                Tools.current = Tool.None;
            }
        }
        public void OnDisable() {
            if (component.PinnedObject) {
                Tools.current = LastTool;
                LastTool = Tool.None;
            }
        }

        public override void OnInspectorGUI() {
            try {
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
                PathGroup temp = EditorGUILayout.ObjectField("", component.group, typeof(PathGroup), true) as PathGroup;
                EditorGUILayout.LabelField("", PathManagerEditorGUI.BackgroundStyle.Get(component.group.PathColor), GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Node name ",EditorStyles.boldLabel,GUILayout.Width(100));
                component.Name = GUILayout.TextField(component.Name);
                component.transform.gameObject.name = component.Name;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Pinned object ", EditorStyles.boldLabel, GUILayout.Width(100));
                component.PinnedObject = EditorGUILayout.ObjectField("", component.PinnedObject, typeof(Transform), true) as Transform;
                GUILayout.EndHorizontal();
                GUI.enabled = component.PinnedObject == null;
                GUILayout.BeginHorizontal();
                GUILayout.Label("World position ", EditorStyles.boldLabel, GUILayout.Width(100));
                component.transform.position = EditorGUILayout.Vector3Field("", component.transform.position) ;
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                GUILayout.Space(10);
                GuiLine();
                GUILayout.Space(10);
                if (GUILayout.Button("Open Path group editor", GUILayout.Height(35))) {
                    PathGroupEditorGUI.ShowWindow();
                    PathGroupEditorGUI.Target = component.group;
                }
                GUILayout.Space(10);
            } catch (System.Exception e) { e.ToString(); }

        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}