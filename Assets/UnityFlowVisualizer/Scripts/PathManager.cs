using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [ExecuteInEditMode]
    public class PathManager : MonoBehaviour
    {
        private Tool LastTool = Tool.None;
        public static PathManager Instance;
        public List<PathInfo> PathInfoList;

        Coroutine myRoutine = null;

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;

            if (myRoutine != null) {
                StopCoroutine(myRoutine);
                myRoutine = null;
            }
            PathInfo[] childs = this.transform.GetComponentsInChildren<PathInfo>();
            PathInfoList = new List<PathInfo>();
            if(childs != null && childs.Length > 0)
                for (int i = 0; i < childs.Length; i++)
                    PathInfoList.Add(childs[i]);
            this.gameObject.name = "PathManager";
            Instance = this;
            myRoutine = StartCoroutine(checkList());
        }

        public void OnDisable() {
            Tools.current = LastTool;
        }

        public void Update() {
            this.transform.position = new Vector3(0f,0f,0f);
            this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            this.transform.localScale = new Vector3(1f, 1f, 1f);
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

