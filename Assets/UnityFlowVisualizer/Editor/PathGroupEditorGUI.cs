using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [InitializeOnLoad]
    public class PathGroupEditorGUI : EditorWindow {
        public static PathInfo Target;
        private GameObject lastSelect = null;

        Vector2 scrollPosition1;
        Vector2 scrollPosition2;
        Vector2 scrollPosition3;

        Vector3 targetPosition = new Vector3(0, 0, 0);
        Texture LogoTex;
        Texture EndTex;

        private int TargetID = 0;

        [MenuItem("/Flow Visualizer/Path Group Editor")]
        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(PathGroupEditorGUI), new Rect(0, 0, 900, 615), true, "Path Group Editor");
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
            
            if(Target == null) {
                try { 
                    PathInfo info = Selection.activeGameObject.GetComponent<PathInfo>();
                    if (info != null) Target = info;
                    else Target = null;
                } catch(System.Exception e) { e.ToString(); }
            }

            if(Target != null) {
                if(Target.NodeList.Count < 2) {
                    for(int i = 0; i < Target.ConnectionList.Count; i++) {
                        GameObject go = Target.ConnectionList[i].gameObject;
                        DestroyImmediate(go);
                    }
                    Target.ConnectionList.Clear();
                }

                if (Target.ConnectionList.Count < 1) {
                    for (int i = 0; i < Target.PathList.Count; i++) {
                        GameObject go = Target.PathList[i].gameObject;
                        DestroyImmediate(go);
                    }
                    Target.PathList.Clear();
                }
            }

            GUIStyle centerStyle = new GUIStyle("Label");
            centerStyle.alignment = TextAnchor.MiddleCenter;
            centerStyle.fixedHeight = 150;

            GUIStyle centerStyle2 = new GUIStyle("Label");
            centerStyle2.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label(LogoTex);
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Path Generator Open", GUILayout.Width(170f))) {
                PathGeneratorButtonClick();
                Target = null;
            }
            GUILayout.Space(30);
            GUILayout.Label("Path Group", GUILayout.Width(70f));

            if (Target = EditorGUILayout.ObjectField("", Target, typeof(PathInfo), true, GUILayout.Width(165f)) as PathInfo) {
                if (TargetID != Target.GetInstanceID()) {
                    TargetID = Target.GetInstanceID();
                    Selection.objects = new UnityEngine.Object[] { Target.gameObject };
                }
            }

            GUI.enabled = Target != null;


            GUILayout.Space(10);

            GUILayout.Label("Path color", GUILayout.Width(60f));
            if (Target != null) Target.PathColor = EditorGUILayout.ColorField(Target.PathColor, GUILayout.Width(80f));
            else EditorGUILayout.ColorField(Color.white, GUILayout.Width(80f));
            GUILayout.Space(10);

            GUILayout.Label("Path thickness ", GUILayout.Width(90f));
            if (Target != null) {
                Target.Thickness = EditorGUILayout.FloatField(Target.Thickness, GUILayout.Width(60f));
                Target.Thickness = ( Target.Thickness <= 0f ) ? 0.001f : ( Target.Thickness >= 1000f ) ? 1000f : Target.Thickness;
            } else EditorGUILayout.FloatField(0f, GUILayout.Width(60f));

            GUILayout.Space(10);

            GUILayout.Space(10);

            if(GUILayout.Button("Delete path",GUILayout.Width(100f))) {
                DestroyImmediate(Target.gameObject);
                Target = null;
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Node list",EditorStyles.boldLabel, GUILayout.Width(200f)); GUILayout.Space(550f);
            GUILayout.Space(5);
            if (GUILayout.Button("New node button")) NewNodeButton();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(200f));
            GUILayout.Label("Position", EditorStyles.toolbarButton, GUILayout.Width(320f));
            GUILayout.Label("Lock object", EditorStyles.toolbarButton, GUILayout.Width(200f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(100f));
            GUILayout.EndHorizontal();

            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, false, GUILayout.Width(900), GUILayout.Height(150));

            // Scroll View Area
            if (Target == null || Target.NodeList.Count == 0) {
                Rect rect1 = new Rect(0, 0, 900, 150);
                GUILayout.BeginArea(rect1);
                if(Target == null)GUILayout.Label("Path info component not found", centerStyle);
                else GUILayout.Label("Node not found", centerStyle);
                GUILayout.EndArea();
            } else {
                Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.NodeList.Count);
                Rect rect1 = new Rect(rect0.x, rect0.y, 900, 21 * Target.NodeList.Count);
                GUILayout.BeginArea(rect1);

                for (int i = 0; i < Target.NodeList.Count; i++) {
                    string targetName = Target.NodeList[i].Name;
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    Target.NodeList[i].Index = i;
                    GUILayout.Label(Target.NodeList[i].Index.ToString(), centerStyle2, GUILayout.Width(75f));
                    Target.NodeList[i].Name = GUILayout.TextField(targetName, EditorStyles.toolbarTextField, GUILayout.Width(197f));
                    Target.NodeList[i].name = Target.NodeList[i].Name;
                    Target.NodeList[i].transform.localPosition = EditorGUILayout.Vector3Field("",Target.NodeList[i].transform.localPosition, GUILayout.Width(315f));
                    Target.NodeList[i].PinnedObject = EditorGUILayout.ObjectField("",Target.NodeList[i].PinnedObject,typeof(Transform),true , GUILayout.Width(197f)) as Transform;
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

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(40f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(90f));
            GUILayout.Label("Start", EditorStyles.toolbarButton, GUILayout.Width(90f));
            GUILayout.Label("End", EditorStyles.toolbarButton, GUILayout.Width(90f));
            GUILayout.Label("Edit", EditorStyles.toolbarButton, GUILayout.Width(60f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f));
            GUILayout.EndHorizontal();

            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, false, GUILayout.Width(440), GUILayout.Height(150));
            if (Target == null || Target.NodeList.Count <  2 || Target.ConnectionList.Count <= 0) {
                Rect rect1 = new Rect(0, 0, 440, 150);
                GUILayout.BeginArea(rect1);
                if (Target == null) GUILayout.Label("Path info component not found", centerStyle);
                else if (Target.NodeList.Count < 2) GUILayout.Label("The number of nodes must be at least two", centerStyle);
                else GUILayout.Label("Connection not found", centerStyle);
                GUILayout.EndArea();
            } else {
                Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.ConnectionList.Count);
                Rect rect1 = new Rect(rect0.x, rect0.y, 440, 21 * Target.ConnectionList.Count);
                GUILayout.BeginArea(rect1);

                string[] options = new string[Target.NodeList.Count];
                if (Target.NodeList.Count > 0) {
                    options = new string[Target.NodeList.Count];
                    for (int i = 0; i < Target.NodeList.Count; i++) {
                        options[i] = "[" + Target.NodeList[i].Index + "] " + Target.NodeList[i].Name;
                    }
                }

                for (int i = 0; i < Target.ConnectionList.Count; i++) {
                    string targetName = Target.ConnectionList[i].Name;
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    Target.ConnectionList[i].Index = i;
                    GUILayout.Label(Target.ConnectionList[i].Index.ToString(), centerStyle2, GUILayout.Width(35f));
                    Target.ConnectionList[i].Name = GUILayout.TextField(targetName, EditorStyles.toolbarTextField, GUILayout.Width(85f));
                    int oldSelect = 0;
                    for (int j = 0; j < options.Length; j++) {
                        if (Target.ConnectionList[i].StartID == Target.NodeList[j].ID)
                            oldSelect = j;
                    }
                    int nodeSelect = EditorGUILayout.Popup(oldSelect, options, GUILayout.Width(87f));
                    Target.ConnectionList[i].StartID = Target.NodeList[nodeSelect].ID;
                    Target.ConnectionList[i].Start = Target.NodeList[nodeSelect].name;
                    Target.ConnectionList[i].StartPos = Target.NodeList[nodeSelect].transform;

                    oldSelect = 0;
                    for (int j = 0; j < options.Length; j++) {
                        if (Target.ConnectionList[i].EndID == Target.NodeList[j].ID)
                            oldSelect = j;
                    }
                    nodeSelect = EditorGUILayout.Popup(oldSelect, options, GUILayout.Width(87f));
                    Target.ConnectionList[i].EndID = Target.NodeList[nodeSelect].ID;
                    Target.ConnectionList[i].End = Target.NodeList[nodeSelect].name;
                    Target.ConnectionList[i].EndPos = Target.NodeList[nodeSelect].transform;

                    if (GUILayout.Button("Edit", EditorStyles.toolbarButton, GUILayout.Width(60f))) EditConnectionButton(i);
                    if (Target.ConnectionList.Count >= 7) {
                        if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(60f))) DeleteConnectionButton(i);
                    } else { 
                        if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f))) DeleteConnectionButton(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Path list", EditorStyles.boldLabel);

            GUI.enabled = !( Target == null || Target.ConnectionList.Count < 1 );
            if (GUILayout.Button("New Path button", GUILayout.Width(140f))) NewPathButton();
            GUI.enabled = Target != null;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(40f));
            GUILayout.Label("Name", EditorStyles.toolbarButton, GUILayout.Width(90f));
            GUILayout.Label("Start", EditorStyles.toolbarButton, GUILayout.Width(90f));
            GUILayout.Label("End", EditorStyles.toolbarButton, GUILayout.Width(90f));
            GUILayout.Label("Edit", EditorStyles.toolbarButton, GUILayout.Width(60f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f));
            GUILayout.EndHorizontal();

            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, false, false, GUILayout.Width(440), GUILayout.Height(150));
            if (Target == null || Target.ConnectionList.Count < 1 || Target.PathList.Count <= 0) {
                Rect rect1 = new Rect(0, 0, 440, 150);
                GUILayout.BeginArea(rect1);
                if (Target == null) GUILayout.Label("Path info component not found", centerStyle);
                else if (Target.ConnectionList.Count < 1) GUILayout.Label("The number of connection must be at least one", centerStyle);
                else GUILayout.Label("Path not found", centerStyle);
                GUILayout.EndArea();
            } else {
                Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.PathList.Count);
                Rect rect1 = new Rect(rect0.x, rect0.y, 440, 21 * Target.PathList.Count);
                GUILayout.BeginArea(rect1);

                for (int i = 0; i < Target.PathList.Count; i++) {
                    string targetName = Target.PathList[i].Name;
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    Target.PathList[i].Index = i;
                    GUILayout.Label(Target.PathList[i].Index.ToString(), centerStyle2, GUILayout.Width(35f));
                    Target.PathList[i].Name = GUILayout.TextField(targetName, EditorStyles.toolbarTextField, GUILayout.Width(86f));

                    int StartIndex = 0, EndIndex = 0;
                    for (int j = 0; j < Target.NodeList.Count; j++) {
                        if (Target.PathList[i].StartID == Target.NodeList[j].ID)
                            StartIndex = j;
                        else if (Target.PathList[i].EndID == Target.NodeList[j].ID)
                            EndIndex = j;
                    }

                    string StartString = "[" + StartIndex + "] " + Target.NodeList[StartIndex].Name;
                    string EndString = "[" + EndIndex + "] " + Target.NodeList[EndIndex].Name;
                    GUILayout.Label(StartString, GUILayout.Width(85f));
                    GUILayout.Label(EndString, GUILayout.Width(85f));

                    if (GUILayout.Button("Edit", EditorStyles.toolbarButton, GUILayout.Width(60f))) EditPathButton(i);
                    if (Target.PathList.Count >= 7) {
                        if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(60f))) DeletePathButton(i);
                    } else {
                        if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f))) DeletePathButton(i);
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndArea();
            }
            GUILayout.EndScrollView();


            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GuiLine();
            GUI.enabled = true;
            GUILayout.Label(EndTex);
        }

        public void PathGeneratorButtonClick() {
            PathManagerGUI.ShowWindow();
            this.Close();
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
            newNode.Pos = newGo.transform.position;
            newNode.ID = newNode.GetInstanceID();
            Target.NodeList.Add(newNode);
        }

        public void NewConnectionButton() {
            GameObject newGo = new GameObject();
            newGo.transform.parent = Target.ConnectionParent.transform;
            newGo.transform.position = Target.ConnectionParent.transform.position;
            newGo.transform.rotation = new Quaternion(0, 0, 0, 0);
            newGo.transform.localScale = new Vector3(1, 1, 1);
            newGo.name = "New Connection";

            MeshFilter mesh = newGo.AddComponent<MeshFilter>();
            mesh.mesh = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Cylinder", typeof(Mesh));
            MeshRenderer renderer = newGo.AddComponent<MeshRenderer>();
            renderer.material = (Material)Resources.Load("UnityFlowVisualizer/Materials/PathMat", typeof(Material));

            Connection newCon = newGo.AddComponent<Connection>();
            newCon.Name = "New Connection";
            newCon.Start = Target.NodeList[0].Name;
            newCon.End = Target.NodeList[1].Name;
            newCon.StartID = Target.NodeList[0].ID;
            newCon.EndID = Target.NodeList[1].ID;   
            newCon.StartPos = Target.NodeList[0].transform;
            newCon.EndPos = Target.NodeList[1].transform;
            newCon.CornerList = new List<Corner>();
            newCon.CornerCount = 0;
            newCon.ConType = CON_TYPE.PRESET;
            newCon.ParentPathInfo = Target;
            newCon.ID = newCon.GetInstanceID();
            Target.ConnectionList.Add(newCon);
        }

        public void NewPathButton() {
            GameObject newGo = new GameObject();
            newGo.transform.parent = Target.PathParent.transform;
            newGo.transform.position = Target.PathParent.transform.position;
            newGo.transform.rotation = new Quaternion(0, 0, 0, 0);
            newGo.transform.localScale = new Vector3(1, 1, 1);
            newGo.name = "New Path";
            Path newPath = newGo.AddComponent<Path>();
            newPath.Name = "New Path";
            newPath.Connections = new List<Connection>();
            newPath.Connections.Add(Target.ConnectionList[0]);
            newPath.Start = Target.ConnectionList[0].Start;
            newPath.End = Target.ConnectionList[0].End;
            newPath.StartID = Target.ConnectionList[0].StartID;
            newPath.EndID = Target.ConnectionList[0].EndID;
            newPath.ID = newPath.GetInstanceID();
            Target.PathList.Add(newPath);
        }

        public void DeleteNodeButton(int index) {
            GameObject DelOb = Target.NodeList[index].gameObject;
            Target.NodeList.RemoveAt(index);
            DestroyImmediate(DelOb);
        }

        public void DeleteConnectionButton(int index) {
            GameObject DelOb = Target.ConnectionList[index].gameObject;
            Target.ConnectionList.RemoveAt(index);
            DestroyImmediate(DelOb);
        }

        public void DeletePathButton(int index) {
            GameObject DelOb = Target.PathList[index].gameObject;
            Target.PathList.RemoveAt(index);
            DestroyImmediate(DelOb);
        }

        public void EditConnectionButton(int index) {
            Selection.objects = new UnityEngine.Object[] { Target.ConnectionList[index].gameObject };
            ConnectionEditorGUI.TargetPathInfo = Target;
            ConnectionEditorGUI.Target = Target.ConnectionList[index];
            ConnectionEditorGUI.ShowWindow();
            this.Close();
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
                } else { Target = null; TargetID = 0; }

                } catch (System.Exception e) {
                e.ToString(); TargetID = 0;
            }
            
            this.Repaint();
        }
    }
}
