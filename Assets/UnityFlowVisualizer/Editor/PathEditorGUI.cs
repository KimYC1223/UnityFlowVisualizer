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
            EditorWindow.GetWindowWithRect(typeof(PathEditorGUI), new Rect(0, 0, 300, 500), false, "Path Editor");
        }

        public void OnEnable() {
            LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
            EndTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo2", typeof(Texture2D));
            // Remove delegate listener if it has previously
            // been assigned.
        }



        private void OnGUI() {
            GUILayout.Label(LogoTex);
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GuiLine();
            GUILayout.Space(5);
            GUI.enabled = Target != null;
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("gg");
            GUILayout.EndHorizontal();

            GUI.enabled = true;
            GUILayout.Label(EndTex);
        }
        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 1);
        }

        private void OnFocus() {
            SceneView.onSceneGUIDelegate = this.OnSceneGUI;
            Debug.Log("asd1");
            if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
        }

        private void OnLostFocus() {
            Debug.Log("asd2");
            SceneView.onSceneGUIDelegate = null;
            if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        void OnSceneGUI(SceneView sceneView) {
            //Debug.Log("asdasd");
            //// Do your drawing here using Handles.
            //Handles.BeginGUI();
            //Handles.Slider(targetPosition, Vector3.right); //X 축
            //Handles.Slider(targetPosition, Vector3.up); //Y 축
            //Handles.Slider(targetPosition, Vector3.forward); //Z 축
            //SceneView.lastActiveSceneView.Repaint();
            //HandleUtility.Repaint();
            //Handles.EndGUI();

            Handles.BeginGUI();
            EditorGUILayout.LabelField("SceneView GUI Test");
            if (GUILayout.Button("Disable SceneView Button", GUILayout.Width(228))) {
                Debug.Log("SceneView ButtonClick");
            }
            Handles.EndGUI();
        }
    }
}
