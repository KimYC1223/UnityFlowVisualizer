using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    public class CornerChildGUI : Editor {
        [HideInInspector]
        private Tool LastTool = Tool.None;

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }
        public void OnDisable() {
            Tools.current = LastTool;
        }

        public override void OnInspectorGUI() {
            try {
                Texture LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
                PathGroup component = target as PathGroup;
                GUILayout.Label(LogoTex, GUILayout.Width(250));
                GUI.enabled = false;
                GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
                GUILayout.Label("  Developed by KimYC1223");
                GUI.enabled = true;
                GUILayout.Space(10);
            } catch(System.Exception e) { e.ToString(); }
        }
    }
}