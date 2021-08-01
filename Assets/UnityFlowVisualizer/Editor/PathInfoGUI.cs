using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(PathInfo))]
    public class PathInfoGUI : Editor {
        private void OnSceneGUI() {
            PathInfo component = target as PathInfo;
            Transform transform = component.transform;
            Handles.color = Color.white;
            Handles.DrawLine(Vector3.zero, new Vector3(1f, 1f, 1f));
        }
    }
}
