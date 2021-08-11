using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class PathEditorGUI : EditorWindow {
        [HideInInspector]
        Vector2 scrollPosition;
        [HideInInspector]
        public static Path Target;
        [HideInInspector]
        public static PathGroup TargetPathGroup;
        [HideInInspector]
        private GameObject lastSelect = null;
        [HideInInspector]
        Texture LogoTex;
        [HideInInspector]
        Texture EndTex;
        [HideInInspector]
        private int TargetID = 0;

        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(PathEditorGUI), new Rect(0, 0, 500, 400),false, "Path Editor");
        }

        public void OnEnable() {
            LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
            EndTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo2", typeof(Texture2D));
        }

        // Start is called before the first frame update
        private void OnGUI() {
            if (Selection.objects.Length == 0)
                if (lastSelect != null)
                    Selection.objects = new UnityEngine.Object[] { lastSelect };

            if (Target == null) {
                try {
                    Path group = Selection.activeGameObject.GetComponent<Path>();
                    if (group != null) Target = group;
                    else Target = null;
                } catch (System.Exception e) { e.ToString(); }
            } else {
                try {
                    TargetPathGroup = Target.transform.parent.parent.gameObject.GetComponent<PathGroup>();
                } catch (System.Exception e) { e.ToString(); }
            }

            GUIStyle centerStyle = new GUIStyle("Label");
            centerStyle.alignment = TextAnchor.MiddleCenter;
            centerStyle.fixedHeight = 280;

            GUIStyle centerStyle2 = new GUIStyle("Label");
            centerStyle2.alignment = TextAnchor.MiddleCenter;

            GUIStyle centerStyle3 = new GUIStyle("Label");
            centerStyle3.alignment = TextAnchor.MiddleCenter;
            centerStyle3.fixedHeight = 120;

            GUILayout.Label(LogoTex,GUILayout.Width(300f));
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Path Group Editor Open", GUILayout.Width(240), GUILayout.Height(25)))
                PathGroupEditorButtonClick();
            GUILayout.Space(17f);
            GUILayout.Label("Path", EditorStyles.boldLabel, GUILayout.Width(70), GUILayout.Height(25));
            if (Target = EditorGUILayout.ObjectField("", Target, typeof(Path), true, GUILayout.Width(160), GUILayout.Height(25)) as Path) {
                if (TargetID != Target.GetInstanceID()) {
                    TargetID = Target.GetInstanceID();
                    Selection.objects = new UnityEngine.Object[] { Target.gameObject };
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            Node finalStart = null, finalEnd = null, finalEnd2 = null ;
            if (!TargetPathGroup && Target)
                TargetPathGroup = Target.ParentPathGroup;

           for (int i = 0 ; i < TargetPathGroup.NodeList.Count;i ++) {
                if (TargetPathGroup.NodeList[i].ID == Target.Connections[0].StartID)
                    finalStart = TargetPathGroup.NodeList[i];
                if (TargetPathGroup.NodeList[i].ID == Target.Connections[Target.Connections.Count-1].EndID)
                    finalEnd = TargetPathGroup.NodeList[i];
                if (Target.Connections.Count >= 2 &&
                    TargetPathGroup.NodeList[i].ID == Target.Connections[Target.Connections.Count - 2].EndID) {
                    finalEnd2 = TargetPathGroup.NodeList[i];
                }
            }

            Target.Start = finalStart.Name;
            Target.StartID = finalStart.ID;
            Target.End = finalEnd.Name;
            Target.EndID = finalEnd.ID;
            GUI.enabled = Target != null;
            GUILayout.Label("Start node", GUILayout.Width(75));
            if (Target) { Node node_temp1 = EditorGUILayout.ObjectField("", finalStart, typeof(Node), true, GUILayout.Width(160)) as Node; }
            GUILayout.Space(15);
            GUILayout.Label("End node", GUILayout.Width(70));
            if (Target) { Node node_temp2 = EditorGUILayout.ObjectField("", finalEnd, typeof(Node), true, GUILayout.Width(160)) as Node; }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUI.enabled = true;
            GuiLine();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            List<Connection> EndConnections = new List<Connection>();
            List<Connection> StartConnections = new List<Connection>();

            if (Target != null && TargetPathGroup != null) {
                for(int i = 0; i < TargetPathGroup.ConnectionList.Count; i++) {
                    if (finalEnd.ID == TargetPathGroup.ConnectionList[i].StartID)
                        StartConnections.Add(TargetPathGroup.ConnectionList[i]);

                    if (finalEnd2 != null && finalEnd2.ID == TargetPathGroup.ConnectionList[i].StartID)
                        EndConnections.Add(TargetPathGroup.ConnectionList[i]);
                }

            }

            GUILayout.Label("Connection list", EditorStyles.boldLabel, GUILayout.Width(350));
            GUI.enabled = GUI.enabled ? StartConnections.Count>0 : GUI.enabled;
            if (GUILayout.Button("Add connection",GUILayout.Width(140)))
                AddConnectionButton(StartConnections);
            GUILayout.EndHorizontal();
            GUI.enabled = Target != null;

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Connection", EditorStyles.toolbarButton, GUILayout.Width(120f));
            GUILayout.Label("Start", EditorStyles.toolbarButton, GUILayout.Width(150f));
            GUILayout.Label("End", EditorStyles.toolbarButton, GUILayout.Width(150f));
            GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(80f));
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(500), GUILayout.Height(110));
            if(Target == null || TargetPathGroup == null) {
                Rect rect1 = new Rect(0, 0, 500, 110);
                GUILayout.BeginArea(rect1);
                if (Target == null) GUILayout.Label("Path not found", centerStyle);
                if (TargetPathGroup == null) GUILayout.Label("PathGroup not found", centerStyle);
                else GUILayout.Label("Connection not found", centerStyle);
                GUILayout.EndArea();
            } else {
                try {
                    Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.Connections.Count);
                    Rect rect1 = new Rect(rect0.x, rect0.y, 500, 21 * Target.Connections.Count);
                    GUILayout.BeginArea(rect1);

                    for(int i = 0; i < Target.Connections.Count;i++) {
                        GUILayout.BeginHorizontal(EditorStyles.toolbar);

                        if(i == 0 && Target.Connections.Count == 1) {
                            string[] options = new string[TargetPathGroup.ConnectionList.Count];
                            int currentIndex = 0;
                            for(int j = 0; j < TargetPathGroup.ConnectionList.Count; j++) {
                                options[j] = "[" + j + "] " + TargetPathGroup.ConnectionList[j].Name;
                                if (Target.Connections[0].ID == TargetPathGroup.ConnectionList[j].ID)
                                    currentIndex = j;
                            }
                            Target.Connections[0] = TargetPathGroup.
                                ConnectionList[EditorGUILayout.Popup(currentIndex, options, GUILayout.Width(116f))];
                        } else if(i == Target.Connections.Count - 1) {
                            string[] options = new string[EndConnections.Count];
                            int currentIndex = 0;
                            for (int j = 0; j < EndConnections.Count; j++) {
                                options[j] = "[" + EndConnections[j].Index + "] " 
                                    + EndConnections[j].Name;
                                if (Target.Connections[Target.Connections.Count - 1].ID == EndConnections[j].ID) {
                                    currentIndex = j;
                                }
                            }
                           
                            Target.Connections[Target.Connections.Count - 1] = EndConnections
                                [EditorGUILayout.Popup(currentIndex, options, GUILayout.Width(116f))];
                        } else {
                            GUILayout.Label("[" + Target.Connections[i].Index + "] " + Target.Connections[i].Name, GUILayout.Width(115));
                        }

                        Node startNode = null, endNod = null;
                        for(int k = 0; k < TargetPathGroup.NodeList.Count; k++) {
                            if (TargetPathGroup.NodeList[k].ID == Target.Connections[i].StartID)
                                startNode = TargetPathGroup.NodeList[k];
                            else if (TargetPathGroup.NodeList[k].ID == Target.Connections[i].EndID)
                                endNod = TargetPathGroup.NodeList[k];
                            if (startNode != null && endNod != null) break;
                        }

                        GUILayout.Label("[" + startNode.Index + "] " + startNode.Name, GUILayout.Width(145));
                        GUILayout.Label("[" + endNod.Index + "] " + endNod.Name, GUILayout.Width(145));
                        if(Target.Connections.Count > 5) {
                            bool saveBool = GUI.enabled;
                            GUI.enabled = GUI.enabled ? i != 0 : false;
                            if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f)))
                                RemoveConnectionButton(i);
                            GUI.enabled = saveBool;
                        } else {
                            bool saveBool = GUI.enabled;
                            GUI.enabled = GUI.enabled ? i != 0 : false;
                            if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(80f)))
                                RemoveConnectionButton(i);
                            GUI.enabled = saveBool;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndArea();

                } catch (System.Exception e) {
                    e.ToString();
                    Rect rect1 = new Rect(0, 0, 500, 110);
                    GUILayout.BeginArea(rect1);
                    if (Target == null) GUILayout.Label("Path not found", centerStyle);
                    if (TargetPathGroup == null) GUILayout.Label("PathGroup not found", centerStyle);
                    else GUILayout.Label("Connection not found", centerStyle);
                    GUILayout.EndArea();
                }
            }
            GUILayout.EndScrollView();

            GUI.enabled = GUI.enabled ? Target.Connections.Count > 1 : GUI.enabled;
            if (GUILayout.Button("Clear connection list"))
                ClearConnectionButton();
            GUI.enabled = true;
            GuiLine();
            GUILayout.Label(EndTex);
            if (GUI.changed) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

        public void AddConnectionButton(List<Connection> conList) {
            Target.Connections.Add(conList[0]);
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        public void RemoveConnectionButton(int index) {
            Target.Connections.RemoveRange(index, ( Target.Connections.Count - index ));
        }

        public void ClearConnectionButton() {
            RemoveConnectionButton(1);
        }


        public void PathGroupEditorButtonClick() {
            if(Target != null) {
                Selection.objects = new UnityEngine.Object[] { TargetPathGroup.gameObject };
                PathGroupEditorGUI.Target = TargetPathGroup;
            } else {
                PathGroupEditorGUI.Target = null;
            }
            PathGroupEditorGUI.ShowWindow();
            this.Close();
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        void OnSelectionChange() {
            try {
                bool flag = false;
                foreach (Object ob in Selection.objects) {
                    GameObject go = ob as GameObject;
                    Path temp = go.GetComponent<Path>();
                    if (temp != null) { lastSelect = go; flag = true; break; }
                }

                if (!flag && Selection.objects.Length > 0) lastSelect = null;

                Path group = Selection.activeGameObject.GetComponent<Path>();
                if (group != null) {
                    Target = group;
                } else { Target = null; TargetID = 0; }

            } catch (System.Exception e) {
                e.ToString(); TargetID = 0;
            }
            this.Repaint();
        }
    }
}


