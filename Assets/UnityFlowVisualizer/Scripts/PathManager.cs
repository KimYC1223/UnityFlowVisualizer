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
        public List<PathGroup> PathGroupList;

        Coroutine myRoutine = null;

        public void Awake() {
            Instance = this;
        }

        public void OnEnable() {
            Instance = this;
            LastTool = Tools.current;
            Tools.current = Tool.None;

            if (myRoutine != null) {
                StopCoroutine(myRoutine);
                myRoutine = null;
            }
            PathGroup[] childs = this.transform.GetComponentsInChildren<PathGroup>();
            PathGroupList = new List<PathGroup>();
            if(childs != null && childs.Length > 0)
                for (int i = 0; i < childs.Length; i++)
                    PathGroupList.Add(childs[i]);
            this.gameObject.name = "PathManager";
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
                yield return new WaitForSeconds(1f);
                PathGroup[] childs = this.transform.GetComponentsInChildren<PathGroup>();
                PathGroupList = new List<PathGroup>();
                if (childs != null && childs.Length > 0)
                    for (int i = 0; i < childs.Length; i++)
                        PathGroupList.Add(childs[i]);
            }
        }
    }

}

