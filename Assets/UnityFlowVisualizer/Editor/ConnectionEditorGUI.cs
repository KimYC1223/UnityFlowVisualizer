using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class ConnectionEditorGUI : EditorWindow {
        Vector2 scrollPosition;
        Vector2 scrollPosition2;
        public static Connection Target;
        public static PathInfo TargetPathInfo;
        private GameObject lastSelect = null;
        Texture LogoTex;
        Texture EndTex;
        private int TargetID = 0;

        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(ConnectionEditorGUI), new Rect(0, 0, 300, 540),false, "Connection Editor");
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
                    Connection info = Selection.activeGameObject.GetComponent<Connection>();
                    if (info != null) Target = info;
                    else Target = null;
                } catch (System.Exception e) { e.ToString(); }
            } else {
                try {
                    TargetPathInfo = Target.transform.parent.parent.gameObject.GetComponent<PathInfo>();
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
            if (GUILayout.Button("Path Group Editor Open", GUILayout.Width(295), GUILayout.Height(30f)))
                PathGroupEditorButtonClick();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Connection", EditorStyles.boldLabel, GUILayout.Width(70));
            if (Target = EditorGUILayout.ObjectField("", Target, typeof(Connection), true) as Connection) {
                if (TargetID != Target.GetInstanceID()) {
                    TargetID = Target.GetInstanceID();
                    Selection.objects = new UnityEngine.Object[] { Target.gameObject };
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GuiLine();
            GUILayout.Space(10);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(300), GUILayout.Height(290f));
            if(Target == null) {
                GUI.enabled = false;
                Rect rect0 = EditorGUILayout.GetControlRect(false, 280);
                Rect rect1 = new Rect(rect0.x, rect0.y, 295, 280);
                GUILayout.BeginArea(rect1);
                GUILayout.Label("Connection not found ", centerStyle);
                GUILayout.EndArea();
            } else {
                try {
                    string[] options = new string[TargetPathInfo.NodeList.Count];
                    int[] optionsID = new int[TargetPathInfo.NodeList.Count];

                    for(int i = 0; i < TargetPathInfo.NodeList.Count; i ++) {
                        options[i] = "[" + i + "] " + TargetPathInfo.NodeList[i].Name;
                        optionsID[i] = TargetPathInfo.NodeList[i].ID;
                    }
                
                    int StartIndex = 0, EndIndex = 0;
                    for(int i = 0; i < TargetPathInfo.NodeList.Count; i++) {
                        if (optionsID[i] == Target.StartID) StartIndex = i;
                        else if (optionsID[i] == Target.EndID) EndIndex = i;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Start Node ",GUILayout.Width(70f));
                    int NewStartIndex = EditorGUILayout.Popup(StartIndex,options,GUILayout.Width(215f));
                    Target.Start = TargetPathInfo.NodeList[NewStartIndex].Name;
                    Target.StartID = TargetPathInfo.NodeList[NewStartIndex].ID;
                    Target.StartPos = TargetPathInfo.NodeList[NewStartIndex].transform;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("End Node ",GUILayout.Width(70f));
                    int NewEndIndex = EditorGUILayout.Popup(EndIndex,options,GUILayout.Width(215f));
                    Target.End = TargetPathInfo.NodeList[NewEndIndex].Name;
                    Target.EndID = TargetPathInfo.NodeList[NewEndIndex].ID;
                    Target.EndPos = TargetPathInfo.NodeList[NewEndIndex].transform;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Corner count", GUILayout.Width(110f));
                    int NewType = EditorGUILayout.IntField(Target.CornerCount, GUILayout.Width(175f));

                    if(NewType != Target.CornerCount) {
                        CreateCorner(NewType);
                    }
                    Target.CornerCount = NewType;
                    GUILayout.EndHorizontal();
                    GUI.enabled = Target.CornerCount > 0;
                    string[] con_type = new string[] { "PRESET", "PLAIN", "FREE" };
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Connection type", GUILayout.Width(110f));
                    int NewConType = EditorGUILayout.Popup((int)Target.ConType,con_type,GUILayout.Width(175f));
                    if( ( NewConType == (int)CON_TYPE.PLAIN  && Target.ConType == CON_TYPE.PRESET ) ||
                        ( NewConType == (int)CON_TYPE.FREE   && Target.ConType == CON_TYPE.PRESET ) ||
                        ( NewConType == (int)CON_TYPE.PRESET && Target.ConType == CON_TYPE.PLAIN  ) ||
                        ( NewConType == (int)CON_TYPE.PRESET && Target.ConType == CON_TYPE.FREE   ) ) {
                        CreateCorner(Target.CornerCount);
                    } else if(NewConType == 0 && Target.CornerCount > 2) {
                        Target.CornerCount = 2;
                        CreateCorner(Target.CornerCount);
                    }
                    Target.ConType = (CON_TYPE)NewConType;
                    GUILayout.EndHorizontal();
                    GUI.enabled = true;

                    GUILayout.Space(10);
                    GuiLine();
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Corner list", EditorStyles.boldLabel);
                    GUI.enabled = !(Target.ConType == CON_TYPE.PRESET && Target.CornerList.Count >= 2);
                    if (GUILayout.Button("Add corner")) { CreateCornerContinue(++Target.CornerCount); Repaint(); }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    GUILayout.Label("No", EditorStyles.toolbarButton, GUILayout.Width(50f));
                    GUILayout.Label("Corner", EditorStyles.toolbarButton, GUILayout.Width(179f));
                    GUILayout.Label("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f));
                    GUILayout.EndHorizontal();

                    if(Target.CornerList.Count <= 0) {
                        scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, false, GUILayout.Width(300), GUILayout.Height(120f));
                        Rect rect0 = EditorGUILayout.GetControlRect(false, 115);
                        Rect rect1 = new Rect(rect0.x, rect0.y, 300, 115);
                        GUILayout.BeginArea(rect1);
                        GUI.enabled = false;
                        GUILayout.Label("There is no corner in this connection", centerStyle3, GUILayout.Width(295), GUILayout.Height(120f));
                        GUI.enabled = true;
                    } else if(Target.ConType == CON_TYPE.FREE || Target.ConType == CON_TYPE.PLAIN) {
                        scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, false, GUILayout.Width(300), GUILayout.Height(120f));
                        Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.CornerList.Count);
                        Rect rect1 = new Rect(rect0.x, rect0.y, 300, 21 * Target.CornerList.Count);
                        GUILayout.BeginArea(rect1);
                        for (int i = 0; i < Target.CornerList.Count; i++) {
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);
                            GUILayout.Label(i.ToString(),centerStyle2, GUILayout.Width(43f));
                            EditorGUILayout.Vector3Field("",Target.CornerList[i].transform.position, GUILayout.Width(176f));
                            if (Target.CornerList.Count <= 5) {
                                if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f)))
                                    { DeleteCorner(i); }
                            } else {
                                if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(55f)))
                                    { DeleteCorner(i); }
                            }
                            GUILayout.EndHorizontal();
                        }
                    } else {
                        scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, false, GUILayout.Width(300), GUILayout.Height(120f));
                        Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Target.CornerList.Count);
                        Rect rect1 = new Rect(rect0.x, rect0.y, 300, 21 * Target.CornerList.Count);
                        GUILayout.BeginArea(rect1);

                        if (Target.CornerList.Count == 1) {
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);
                            GUILayout.Label(0.ToString(),centerStyle2, GUILayout.Width(43f));

                            string[] con_type_1 = new string[] { "START", "END" };
                            int presetType1 = EditorGUILayout.Popup((int)Target.PresetType1, con_type_1, GUILayout.Width(176f));

                            if(presetType1 != (int)Target.PresetType1) {
                                if(presetType1 == (int)PRESET_TYPE_1_CORNER.START) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          Target.StartPos.position.y,
                                                                                          Target.EndPos.position.z);
                                }
                                else if (presetType1 == (int)PRESET_TYPE_1_CORNER.END) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          Target.EndPos.position.y,
                                                                                          Target.StartPos.position.z);
                                }
                                Target.PresetType1 = (PRESET_TYPE_1_CORNER)presetType1;
                                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                                UnityEditor.SceneView.RepaintAll();
                            }

                            if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f)))
                                { DeleteCorner(0); }
                            
                            GUILayout.EndHorizontal();

                        } else {
                            for (int i = 0; i < Target.CornerList.Count; i++) {
                                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                                GUILayout.Label(i.ToString(), centerStyle2, GUILayout.Width(43f));
                                EditorGUILayout.Vector3Field("", Target.CornerList[i].transform.position, GUILayout.Width(176f));
                                if (Target.CornerList.Count <= 5) {
                                    if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f))) { DeleteCorner(i); }
                                } else {
                                    if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(55f))) { DeleteCorner(i); }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                        
                    }

                    GUILayout.EndArea();
                    GUILayout.EndScrollView();

                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    //GUILayout.Label(Target.NodeList[i].Index.ToString(), centerStyle2, GUILayout.Width(75f));
                    GUILayout.EndHorizontal();

                } catch (System.Exception e) { e.ToString();
                    GUI.enabled = false;
                    Rect rect1 = new Rect(0, 0, 295, 280);
                    GUILayout.BeginArea(rect1);
                    GUILayout.Label("Connection not found ", centerStyle);
                    GUILayout.EndArea();
                }
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Clear corner list")) { ClearCorner(); Repaint(); }
            GUI.enabled = true;
            GuiLine();
            GUILayout.Label(EndTex);
        }

        public void ClearCorner() {
            for (int i = 0; i < Target.CornerList.Count; i++) {
                GameObject go = Target.CornerList[i].gameObject;
                DestroyImmediate(go);
            }
            Target.CornerList = new List<Corner>();
            Target.CornerCount = 0;
        }

        public void CreateCorner(int NewType) {
            if (Target.ConType == CON_TYPE.PRESET && NewType > 2)
                NewType = 2;

            for (int i = 0; i < Target.CornerList.Count; i++) {
                GameObject go = Target.CornerList[i].gameObject;
                DestroyImmediate(go);
            }
            Target.CornerList = new List<Corner>();
            Vector3 Amount = ( Target.EndPos.position - Target.StartPos.position ) / ( NewType + 1 );

            for (int k = 1; k <= NewType; k++) {
                GameObject go = new GameObject();
                go.transform.parent = TargetPathInfo.CornerParent.transform;
                go.transform.position = Target.StartPos.position + k * ( Amount );
                go.name = Target.name + " Corner_" + k;


                Corner newCorner = go.AddComponent<Corner>();
                newCorner.ParentID = Target.GetInstanceID();
                GameObject line = new GameObject();
                line.transform.parent = go.transform;
                line.transform.position = go.transform.position;
                newCorner.Line = line.transform;
                newCorner.pathInfo = TargetPathInfo;
                newCorner.Connection = Target;

                Mesh Cylinder = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Cylinder",typeof(Mesh));
                Mesh Sphere   = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Sphere", typeof(Mesh));
                Material PathMat = (Material)Resources.Load("UnityFlowVisualizer/Materials/PathMat",typeof(Material));

                MeshFilter mesh = go.AddComponent<MeshFilter>();
                mesh.mesh = Sphere;
                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = PathMat;
                renderer.sharedMaterial.color = TargetPathInfo.PathColor;

                mesh = line.AddComponent<MeshFilter>();
                mesh.mesh = Cylinder;
                renderer = line.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = PathMat;
                renderer.sharedMaterial.color = TargetPathInfo.PathColor;

                Target.CornerList.Add(newCorner);
            }

            for(int i = 0; i < Target.CornerList.Count; i++) {
                if (i == Target.CornerList.Count - 1)
                    Target.CornerList[i].Destination = Target.EndPos;
                else Target.CornerList[i].Destination = Target.CornerList[i + 1].transform;
            }

            Repaint();
        }

        public void CreateCornerContinue(int NewType) {
            if (Target.CornerList.Count == 0) {
                CreateCorner(NewType);
                return;
            }

            int count = Target.CornerList.Count;
            Vector3 Amount = ( Target.EndPos.position - Target.CornerList[count-1].transform.position ) / 2;

            GameObject go = new GameObject();
            go.transform.parent = TargetPathInfo.CornerParent.transform;
            go.transform.position = Target.CornerList[count - 1].transform.position + Amount;
            go.name = Target.name + " Corner_" + count;

            Corner newCorner = go.AddComponent<Corner>();
            newCorner.ParentID = Target.GetInstanceID();
            GameObject line = new GameObject();
            line.transform.parent = go.transform;
            line.transform.position = go.transform.position;
            newCorner.Line = line.transform;
            newCorner.pathInfo = TargetPathInfo;
            newCorner.Connection = Target;

            Mesh Cylinder = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Cylinder", typeof(Mesh));
            Mesh Sphere = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Sphere", typeof(Mesh));
            Material PathMat = (Material)Resources.Load("UnityFlowVisualizer/Materials/PathMat", typeof(Material));

            MeshFilter mesh = go.AddComponent<MeshFilter>();
            mesh.mesh = Sphere;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = PathMat;
            renderer.sharedMaterial.color = TargetPathInfo.PathColor;

            mesh = line.AddComponent<MeshFilter>();
            mesh.mesh = Cylinder;
            renderer = line.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = PathMat;
            renderer.sharedMaterial.color = TargetPathInfo.PathColor;

            Target.CornerList.Add(newCorner);
            
            Target.CornerList[count].Destination = Target.EndPos;
            Target.CornerList[count-1].Destination = Target.CornerList[count].transform;

            Repaint();
        }

        public void DeleteCorner(int index) {
            if (Target.CornerList.Count == 0)
                return;

            GameObject go = Target.CornerList[index].gameObject;
            
            if(index == 0) {
                Target.CornerList.RemoveAt(0);
                DestroyImmediate(go);
            } else if (index == Target.CornerList.Count -1) {
                Target.CornerList[index - 1].Destination = Target.EndPos;
                Target.CornerList.RemoveAt(index);
                DestroyImmediate(go);
            } else {
                Target.CornerList[index - 1].Destination = Target.CornerList[index + 1].transform;
                Target.CornerList.RemoveAt(index);
                DestroyImmediate(go);
            }
            Repaint();
        }

        public void PathGroupEditorButtonClick() {
            if(Target != null) {
                Selection.objects = new UnityEngine.Object[] { TargetPathInfo.gameObject };
                PathGroupEditorGUI.Target = TargetPathInfo;
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
                    Connection temp = go.GetComponent<Connection>();
                    if (temp != null) { lastSelect = go; flag = true; break; }
                }

                if (!flag && Selection.objects.Length > 0) lastSelect = null;

                Connection info = Selection.activeGameObject.GetComponent<Connection>();
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


