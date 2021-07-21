using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    [ExecuteInEditMode]
    public class PathManager : MonoBehaviour
    {
        public static PathManager Instance;
        public List<PathInfo> PathInfoList;

        public void OnEnable() {
            StopAllCoroutines();
            PathInfo[] childs = this.transform.GetComponentsInChildren<PathInfo>();
            PathInfoList = new List<PathInfo>();
            if(childs != null && childs.Length > 0)
                for (int i = 0; i < childs.Length; i++)
                    PathInfoList.Add(childs[i]);
            this.gameObject.name = "PathManager";
            Instance = this;
            StartCoroutine(checkList());
        }


        public IEnumerator checkList() {
            while(true) {
                yield return 0.5f;
                PathInfo[] childs = this.transform.GetComponentsInChildren<PathInfo>();
                PathInfoList = new List<PathInfo>();
                if (childs != null && childs.Length > 0)
                    for (int i = 0; i < childs.Length; i++)
                        PathInfoList.Add(childs[i]);
            }
        }
    }

}

