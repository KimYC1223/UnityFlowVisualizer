using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class PathGeneratorGUI : EditorWindow {
        public PathManager Env;
        Texture LogoTex;
        Texture EndTex;

        [MenuItem("/Flow Visualizer/Total Path Manager")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(PathGeneratorGUI));
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
            GUILayout.Space(10);
            if (GUILayout.Button("Button", GUILayout.Height(30)))
                EnvInitButton();

            GUILayout.Label(EndTex);
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public void EnvInitButton() {
            if (Env.gameObject != null) Debug.Log("Hello");
            else Debug.Log("Null");
        }
    }
}


