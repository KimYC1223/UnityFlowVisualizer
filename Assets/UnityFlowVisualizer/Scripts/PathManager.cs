using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    [ExecuteInEditMode]
    public class PathManager : MonoBehaviour
    {
        public static PathManager Instance;
        public List<PathInfo> list;
        public void OnEnable() {
            this.gameObject.name = "PathManager";
            Instance = this;
        }
    }

}

