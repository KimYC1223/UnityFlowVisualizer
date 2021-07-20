using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class PathGeneratorGUI : EditorWindow {
        Vector2 scrollPosition;
        public PathManager Env;
        Texture LogoTex;
        Texture EndTex;

        [MenuItem("/Flow Visualizer/Total Path Manager")]
        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(PathGeneratorGUI), new Rect(0, 0, 300, 450),false, "Total Path Manager");
        }

        public void OnEnable() {
            LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
            EndTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo2", typeof(Texture2D));
            Env = PathManager.Instance;
        }

        // Start is called before the first frame update
        private void OnGUI() {
            Env = PathManager.Instance;
            try {
                GameObject go = Env.gameObject;
            } catch (System.Exception e) { e.ToString(); Env = null; }

            GUILayout.Label(LogoTex);
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GuiLine();
            GUILayout.Space(5);
            GUI.enabled = Env == null;
            if (GUILayout.Button("Instantiate Path Manager", GUILayout.Height(30)))
                EnvInitButton();
            GUI.enabled = true;
            if (Env == null)
                EditorGUILayout.HelpBox("  Create an Path Manager object first!", MessageType.Warning);
            GUILayout.Space(5);
            GuiLine();
            GUILayout.Space(5);

            GUI.enabled = Env != null;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton);
            GUILayout.Label("PathColor", EditorStyles.toolbarButton);
            GUILayout.Label("Edit", EditorStyles.toolbarButton);
            GUILayout.EndHorizontal();
            if (Env != null) 
                 scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(300), GUILayout.Height(190));
            else scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(300), GUILayout.Height(150));

            // Scroll View Area
            if(Env == null) {
                Rect rect1 = new Rect (0,0,300,150);
                GUIStyle centerStyle = new GUIStyle("Label");
                centerStyle.alignment = TextAnchor.MiddleCenter;
                centerStyle.fixedHeight = 140;
                GUILayout.BeginArea(rect1);
                GUILayout.Label("Path Manager not found", centerStyle);
                GUILayout.EndArea();
            } else {
                Rect rect0 = EditorGUILayout.GetControlRect(false, 300);
                Rect rect1 = new Rect(rect0.x, rect0.y, 300, 150);
                GUILayout.BeginArea(rect1);
                GUILayout.Label("hello,World");
                GUILayout.EndArea();
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Button("New Path", EditorStyles.toolbarButton);
            GUILayout.Button("PathColor", EditorStyles.toolbarButton);
            GUILayout.EndHorizontal();


            GUI.enabled = true;
            GuiLine();
            GUILayout.Label(EndTex);
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public void EnvInitButton() {
            if (Env == null) {
                GameObject go = new GameObject();
                Env = go.AddComponent<PathManager>();
                go.name = "PathManager";
            }
        }
    }
}


