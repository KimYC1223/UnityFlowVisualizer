using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [InitializeOnLoad]
    public class PathEditorGUI : EditorWindow {
        public static PathInfo Target;
        Vector3 targetPosition = new Vector3(0, 0, 0);
        Texture LogoTex;
        Texture EndTex;

        [MenuItem("/Flow Visualizer/Path Editor")]
        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(PathEditorGUI), new Rect(0, 0, 800, 600), true, "Path Editor");
        }

        public void OnEnable() {
            LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
            EndTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo2", typeof(Texture2D));
        }

        private void OnGUI() {
            try { 
                PathInfo info = Selection.activeGameObject.GetComponent<PathInfo>();
                if (info != null) Target = info;
                else Target = null;
            } catch(System.Exception e) { e.ToString(); }

            GUILayout.Label(LogoTex);
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GuiLine();
            GUILayout.Space(5);
            GUI.enabled = Target != null;
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name", GUILayout.Width(40f));
            if(Target != null) Target.name = GUILayout.TextField(Target.name, GUILayout.Width(165f));
            else GUILayout.TextField("", GUILayout.Width(165f));
            GUILayout.Space(30);

            GUILayout.Label("Path color", GUILayout.Width(60f));
            if (Target != null) Target.PathColor = EditorGUILayout.ColorField(Target.PathColor, GUILayout.Width(80f));
            else EditorGUILayout.ColorField(Color.white, GUILayout.Width(80f));
            GUILayout.Space(30);

            GUILayout.Label("Path thickness ", GUILayout.Width(90f));
            if (Target != null) {
                Target.Thickness = EditorGUILayout.FloatField(Target.Thickness, GUILayout.Width(80f));
                Target.Thickness = ( Target.Thickness <= 0f ) ? 0.001f : ( Target.Thickness >= 1000f ) ? 1000f : Target.Thickness;
            } else EditorGUILayout.FloatField(0f, GUILayout.Width(80f));
            GUILayout.Space(30);

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            GUILayout.Label(EndTex);
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }


        void OnSelectionChange() {
            try {
                PathInfo info = Selection.activeGameObject.GetComponent<PathInfo>();
                if (info != null) Target = info;
                else Target = null;
            } catch (System.Exception e) { e.ToString(); }
        }
    }
}
