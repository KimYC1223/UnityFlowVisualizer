using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    public class CornerChild : MonoBehaviour {
        public Connection con;

        [CustomEditor(typeof(CornerChild))]
        public class ConerChildGUI : Editor {
            public void OnSceneGUI() {
                CornerChild component = target as CornerChild;
                if (component.con != null) {
                    Selection.objects = new UnityEngine.Object[] { component.con.gameObject };
                }
            }
        }
    }
}
