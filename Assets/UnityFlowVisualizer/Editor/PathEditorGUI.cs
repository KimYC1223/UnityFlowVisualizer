using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [InitializeOnLoad]
    public class PathEditorGUI : EditorWindow {
        public static PathInfo Target;
        private GameObject lastSelect = null;

        Vector2 scrollPosition1;
        Vector2 scrollPosition2;
        Vector2 scrollPosition3;

        Vector3 targetPosition = new Vector3(0, 0, 0);
        Texture LogoTex;
        Texture EndTex;

        [MenuItem("/Flow Visualizer/Path Editor")]
        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(PathEditorGUI), new Rect(0, 0, 800, 605), true, "Path Editor");
        }

        public void OnEnable() {
            LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
            EndTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo2", typeof(Texture2D));
            OnSelectionChange();
        }

        private void OnGUI() {
            if(Selection.objects.Length == 0) {
                if (lastSelect != null) {
                    Selection.objects = new UnityEngine.Object[] { lastSelect };
                }
            }

            try { 
                PathInfo info = Selection.activeGameObject.GetComponent<PathInfo>();
                if (info != null) Target = info;
                else Target = null;
            } catch(System.Exception e) { e.ToString(); }

            GUILayout.Label(LogoTex);
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            GUI.enabled = Target != null;
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name", GUILayout.Width(40f));
            if(Target != null) Target.name = GUILayout.TextField(Target.name, GUILayout.Width(165f));
            else GUILayout.TextField("", GUILayout.Width(165f));
            GUILayout.Space(30);

            GUILayout.Label("Path color", GUILayout.Width(60f));
            if (Target != null) Target.PathColor = EditorGUILayout.ColorField(Target.PathColor, GUILayout.Width(80f));
            else EditorGUILayout.ColorField(Color.white, GUILayout.Width(80f));
            GUILayout.Space(30);

            GUILayout.Label("Path thickness ", GUILayout.Width(90f));
            if (Target != null) {
                Target.Thickness = EditorGUILayout.FloatField(Target.Thickness, GUILayout.Width(80f));
                Target.Thickness = ( Target.Thickness <= 0f ) ? 0.001f : ( Target.Thickness >= 1000f ) ? 1000f : Target.Thickness;
            } else EditorGUILayout.FloatField(0f, GUILayout.Width(80f));
            GUILayout.Space(30);

            if(GUILayout.Button("Delete path",GUILayout.Width(100f))) {
                DestroyImmediate(Target.gameObject);
                Target = null;
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Node list",EditorStyles.boldLabel, GUILayout.Width(200f)); GUILayout.Space(400f);
            GUILayout.Space(5);
            if (GUILayout.Button("New node button")) NewNodeButton();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(200f));
            GUILayout.Label("Position", EditorStyles.toolbarButton, GUILayout.Width(270f));
            GUILayout.Label("Lock object", EditorStyles.toolbarButton, GUILayout.Width(150f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(100f));
            GUILayout.EndHorizontal();

            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, false, GUILayout.Width(800), GUILayout.Height(150));

            // Scroll View Area
            if (Target == null || Target.NodeList.Count == 0) {
                Rect rect1 = new Rect(0, 0, 800, 150);
                GUIStyle centerStyle = new GUIStyle("Label");
                centerStyle.alignment = TextAnchor.MiddleCenter;
                centerStyle.fixedHeight = 150;
                GUILayout.BeginArea(rect1);
                if(Target == null)GUILayout.Label("Path info component not found", centerStyle);
                else GUILayout.Label("Node not found", centerStyle);
                GUILayout.EndArea();
            } else {
                Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.NodeList.Count);
                Rect rect1 = new Rect(rect0.x, rect0.y, 800, 21 * Target.NodeList.Count);
                GUILayout.BeginArea(rect1);

                for (int i = 0; i < Target.NodeList.Count; i++) {
                    string targetName = Target.NodeList[i].Name;
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    Target.NodeList[i].Index = i;
                    GUILayout.Label(Target.NodeList[i].ID.ToString(), GUILayout.Width(75f));
                    Target.NodeList[i].Name = GUILayout.TextField(targetName, EditorStyles.toolbarTextField, GUILayout.Width(197f));
                    Target.NodeList[i].transform.localPosition = EditorGUILayout.Vector3Field("",Target.NodeList[i].transform.localPosition, GUILayout.Width(265f));
                    Target.NodeList[i].PinnedObject = EditorGUILayout.ObjectField("",Target.NodeList[i].PinnedObject,typeof(Transform),true , GUILayout.Width(147f)) as Transform;
                    if (Target.NodeList.Count >= 7) { if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(89f))) DeleteNodeButton(i); }
                    else { if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(99f))) DeleteNodeButton(i); }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            GUILayout.EndScrollView();
            GUILayout.Space(5);
            GuiLine();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Connection list", EditorStyles.boldLabel);

            GUI.enabled = !( Target == null || Target.NodeList.Count < 2);
            if (GUILayout.Button("New Connection button", GUILayout.Width(150f))) NewConnectionButton();
            GUI.enabled = Target != null;
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(55f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(100f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.EndHorizontal();

            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, false, GUILayout.Width(400), GUILayout.Height(150));
            if (Target == null || Target.NodeList.Count <  2 || Target.ConnectionList.Count <= 0) {
                Rect rect1 = new Rect(0, 0, 400, 150);
                GUIStyle centerStyle = new GUIStyle("Label");
                centerStyle.alignment = TextAnchor.MiddleCenter;
                centerStyle.fixedHeight = 150;
                GUILayout.BeginArea(rect1);
                if (Target == null) GUILayout.Label("Path info component not found", centerStyle);
                else if (Target.NodeList.Count < 2) GUILayout.Label("The number of nodes must be at least two", centerStyle);
                else GUILayout.Label("Connection not found", centerStyle);
                GUILayout.EndArea();
            } else {
                Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.NodeList.Count);
                Rect rect1 = new Rect(rect0.x, rect0.y, 800, 21 * Target.NodeList.Count);
                GUILayout.BeginArea(rect1);

                for (int i = 0; i < Target.NodeList.Count; i++) {
                    string targetName = Target.ConnectionList[i].Name;
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    Target.ConnectionList[i].Index = i;
                    GUILayout.Label(Target.ConnectionList[i].ID.ToString(), GUILayout.Width(75f));
                    Target.ConnectionList[i].Name = GUILayout.TextField(targetName, EditorStyles.toolbarTextField, GUILayout.Width(197f));
                    Target.NodeList[i].transform.localPosition = EditorGUILayout.Vector3Field("", Target.NodeList[i].transform.localPosition, GUILayout.Width(265f));
                    Target.NodeList[i].PinnedObject = EditorGUILayout.ObjectField("", Target.NodeList[i].PinnedObject, typeof(Transform), true, GUILayout.Width(147f)) as Transform;
                    if (Target.NodeList.Count >= 7) { if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(89f))) DeleteNodeButton(i); } else { if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(99f))) DeleteNodeButton(i); }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            GUILayout.Label("Path List", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(55f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(100f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.EndHorizontal();




            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GuiLine();
            GUI.enabled = true;
            GUILayout.Label(EndTex);
        }



        public void NewNodeButton() {
            GameObject newGo = new GameObject();
            newGo.transform.parent = Target.NodeParent.transform;
            newGo.transform.position = Target.NodeParent.transform.position;
            newGo.transform.rotation = new Quaternion(0, 0, 0, 0);
            newGo.transform.localScale = new Vector3(1, 1, 1);
            newGo.name = "New Node";
            Node newNode = newGo.AddComponent<Node>();
            newNode.Name = "New Node";
            newNode.PinnedObject = null;
            newNode.Pos = Vector3.zero;
            newNode.ID = newNode.GetInstanceID();
            Target.NodeList.Add(newNode);
        }

        public void NewConnectionButton() {

        }

        public void NewPathButton() {

        }

        public void DeleteNodeButton(int index) {
            GameObject DelOb = Target.NodeList[index].gameObject;
            Target.NodeList.RemoveAt(index);
            DestroyImmediate(DelOb);
        }

        public void DeleteConnectionButton(int index) {

        }

        public void EditPathButton(int index) {

        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }


        void OnSelectionChange() {
            try {
                bool flag = false;
                foreach(Object ob in Selection.objects) {
                    GameObject go = ob as GameObject;
                    PathInfo temp = go.GetComponent<PathInfo>();
                    if(temp != null) { lastSelect = go; flag = true;  break; }
                }

                if(!flag && Selection.objects.Length > 0) lastSelect = null;

                PathInfo info = Selection.activeGameObject.GetComponent<PathInfo>();
                if (info != null) {
                    Target = info;
                } else Target = null;

            } catch (System.Exception e) {
                e.ToString();
            }
            
            this.Repaint();
        }
    }
}
