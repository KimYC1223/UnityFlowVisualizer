using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    public enum CON_TYPE {PRESET, PLAIN, FREE};
    public enum PRESET_TYPE_2_CORNER {X, Y, Z};
    public enum PRESET_TYPE_1_CORNER {START,END};
    public enum PRESET_TYPE_3_CORNER {XY1, XY2, YZ1, YZ2, XZ1, XZ2};
    [ExecuteInEditMode]
    public class Connection : MonoBehaviour
    {
        [HideInInspector]
        public int Index;

        public string Name;
        [HideInInspector]
        public string Start;
        [HideInInspector]
        public Transform StartPos;
        [HideInInspector]
        public int StartID;
        [HideInInspector]
        public string End;
        [HideInInspector]
        public int EndID;
        [HideInInspector]
        public Transform EndPos;
        [HideInInspector]
        public int CornerCount;
        [HideInInspector]
        public int ID;
        [HideInInspector]
        public CON_TYPE ConType;
        [HideInInspector]
        public PRESET_TYPE_1_CORNER PresetType1 = PRESET_TYPE_1_CORNER.START;
        [HideInInspector]
        public PRESET_TYPE_2_CORNER PresetType2 = PRESET_TYPE_2_CORNER.Y;
        [HideInInspector]
        public PRESET_TYPE_3_CORNER PresetType3 = PRESET_TYPE_3_CORNER.XY1;
        [HideInInspector]
        public Vector3 Preset2Handler;
        [HideInInspector]
        public Vector3 Preset3Handler;
        [HideInInspector]
        public List<Corner> CornerList;
        public PathGroup ParentPathGroup;

        private Tool LastTool = Tool.None;

        Coroutine myRoutine = null;

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;

            if (myRoutine != null) {
                StopCoroutine(myRoutine);
                myRoutine = null;
            }
            try {
                if (ParentPathGroup == null)
                    ParentPathGroup = this.transform.parent.parent.GetComponent<PathGroup>();

                Corner[] childs = ParentPathGroup.CornerParent.GetComponentsInChildren<Corner>();
                CornerList = new List<Corner>();
                if (childs != null && childs.Length > 0)
                    for (int i = 0; i < childs.Length; i++)
                        if (this == childs[i].Connection)
                            CornerList.Add(childs[i]);
                CornerCount = CornerList.Count;
            } catch(System.Exception e) { e.ToString(); } 
            this.gameObject.name = Name;
            myRoutine = StartCoroutine(checkList());
        }

        public void OnDisable() {
            Tools.current = LastTool;
        }

        public void OnDestroy() {
            for (int i = 0; i < CornerList.Count; i++) {
                GameObject go = CornerList[i].gameObject;
                DestroyImmediate(go);
            }
            CornerList.Clear();
        }

        public void LateUpdate() {
            if (Start == null || End == null) return;
            if (ParentPathGroup == null) return;
            //this.transform.localScale = new Vector3(1f, 1f, 1f);
            //this.transform.position = new Vector3(0f, 0f, 0f);
            //this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            //MeshRenderer renderer = GetComponent<MeshRenderer>();
            //renderer.sharedMaterial.color = ParentPathGroup.PathColor;

            if (CornerList.Count == 0) {
                float distance = Vector3.Distance(StartPos.position, EndPos.position);
                this.transform.localScale = new Vector3(ParentPathGroup.Thickness, ParentPathGroup.Thickness,distance);
                this.transform.position = StartPos.position;
                this.transform.LookAt(EndPos.position);
            } else {
                float distance = Vector3.Distance(StartPos.position, CornerList[0].transform.position);
                this.transform.localScale = new Vector3(ParentPathGroup.Thickness, ParentPathGroup.Thickness, distance);
                this.transform.position = StartPos.position;
                this.transform.LookAt(CornerList[0].transform);
            }
        }


        public IEnumerator checkList() {
            while (true) {
                if(ParentPathGroup == null) 
                    ParentPathGroup = this.transform.parent.parent.GetComponent<PathGroup>();
                
                this.gameObject.name = Name;
                yield return new WaitForSeconds(1f);
                Corner[] childs = ParentPathGroup.CornerParent.GetComponentsInChildren<Corner>();
                CornerList = new List<Corner>();
                if (childs != null && childs.Length > 0)
                    for (int i = 0; i < childs.Length; i++)
                        if (this == childs[i].Connection)
                            CornerList.Add(childs[i]);
                CornerCount = CornerList.Count;
            }
        }
    }
}
