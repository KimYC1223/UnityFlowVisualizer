using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class ConnectionEditorGUI : EditorWindow {
        [HideInInspector]
        Vector2 scrollPosition;
        [HideInInspector]
        Vector2 scrollPosition2;
        [HideInInspector]
        public static Connection Target;
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
                    string[] options = new string[TargetPathGroup.NodeList.Count];
                    int[] optionsID = new int[TargetPathGroup.NodeList.Count];

                    for(int i = 0; i < TargetPathGroup.NodeList.Count; i ++) {
                        options[i] = "[" + i + "] " + TargetPathGroup.NodeList[i].Name;
                        optionsID[i] = TargetPathGroup.NodeList[i].ID;
                    }
                
                    int StartIndex = 0, EndIndex = 0;
                    for(int i = 0; i < TargetPathGroup.NodeList.Count; i++) {
                        if (optionsID[i] == Target.StartID) StartIndex = i;
                        else if (optionsID[i] == Target.EndID) EndIndex = i;
                    }

                    bool isPathChangeFlag = false;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Start Node ",GUILayout.Width(70f));
                    int NewStartIndex = EditorGUILayout.Popup(StartIndex,options,GUILayout.Width(215f));
                    if (!isPathChangeFlag) isPathChangeFlag = NewStartIndex != StartIndex;
                    Target.Start = TargetPathGroup.NodeList[NewStartIndex].Name;
                    Target.StartID = TargetPathGroup.NodeList[NewStartIndex].ID;
                    Target.StartPos = TargetPathGroup.NodeList[NewStartIndex].transform;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("End Node ",GUILayout.Width(70f));
                    int NewEndIndex = EditorGUILayout.Popup(EndIndex,options,GUILayout.Width(215f));
                    if (!isPathChangeFlag) isPathChangeFlag = NewEndIndex != EndIndex;
                    Target.End = TargetPathGroup.NodeList[NewEndIndex].Name;
                    Target.EndID = TargetPathGroup.NodeList[NewEndIndex].ID;
                    Target.EndPos = TargetPathGroup.NodeList[NewEndIndex].transform;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Corner count", GUILayout.Width(110f));
                    int NewType = EditorGUILayout.IntField(Target.CornerCount, GUILayout.Width(175f));
                    bool con_type_change = false;
                    if (NewType != Target.CornerCount) {
                        con_type_change = true;
                        CreateCorner(NewType);
                    }
                    Target.CornerCount = NewType;
                    GUILayout.EndHorizontal();
                    string[] con_type = new string[] { "PRESET", "PLAIN", "FREE" };
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Connection type", GUILayout.Width(110f));
                    int NewConType = EditorGUILayout.Popup((int)Target.ConType,con_type,GUILayout.Width(175f));
                    con_type_change = ( con_type_change) ? true : (CON_TYPE)NewConType != Target.ConType;

                    if( ( NewConType == (int)CON_TYPE.PLAIN  && Target.ConType == CON_TYPE.PRESET ) ||
                        ( NewConType == (int)CON_TYPE.FREE   && Target.ConType == CON_TYPE.PRESET ) ||
                        ( NewConType == (int)CON_TYPE.PRESET && Target.ConType == CON_TYPE.PLAIN  ) ||
                        ( NewConType == (int)CON_TYPE.PRESET && Target.ConType == CON_TYPE.FREE   ) ) {
                        CreateCorner(Target.CornerCount);
                    } else if(NewConType == 0 && Target.CornerCount > 3) {
                        Target.CornerCount = 3;
                        CreateCorner(Target.CornerCount);
                    }
                    
                    if(isPathChangeFlag) {
                        ClearCorner();
                        CreateCorner(Target.CornerCount);
                        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                        UnityEditor.SceneView.RepaintAll();
                    }
                    Target.ConType = (CON_TYPE)NewConType;
                    GUILayout.EndHorizontal();

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

                            if(con_type_change || presetType1 != (int)Target.PresetType1) {
                                Undo.RecordObject(this, "Change connection");
                                con_type_change = false;
                                if (presetType1 == (int)PRESET_TYPE_1_CORNER.START) {
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

                        } else if (Target.CornerList.Count == 2) {
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);
                            GUILayout.Label(0.ToString(), centerStyle2, GUILayout.Width(43f));
                            string[] con_type2 = new string[] { "X-Axis parallel", "Y-Axis parallel", "Z-Axis parallel" };
                            int presetType2 = EditorGUILayout.Popup((int)Target.PresetType2, con_type2, GUILayout.Width(176f));

                            if (con_type_change || presetType2 != (int)Target.PresetType2) {
                                Undo.RecordObject(this, "Change connection");
                                con_type_change = false;
                                Vector3 centerPos = ( Target.StartPos.position + Target.EndPos.position ) / 2;
                                if (presetType2 == (int)PRESET_TYPE_2_CORNER.X) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          centerPos.y,
                                                                                          centerPos.z);
                                    Target.CornerList[1].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          centerPos.y,
                                                                                          centerPos.z);
                                } else if (presetType2 == (int)PRESET_TYPE_2_CORNER.Y) {
                                    Target.CornerList[0].transform.position = new Vector3(centerPos.x,
                                                                                          Target.StartPos.position.y,
                                                                                          centerPos.z);
                                    Target.CornerList[1].transform.position = new Vector3(centerPos.x,
                                                                                          Target.EndPos.position.y,
                                                                                          centerPos.z);
                                } else if (presetType2 == (int)PRESET_TYPE_2_CORNER.Z) {
                                    Target.CornerList[0].transform.position = new Vector3(centerPos.x,
                                                                                          centerPos.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[1].transform.position = new Vector3(centerPos.x,
                                                                                          centerPos.y,
                                                                                          Target.EndPos.position.z);
                                }
                                Target.Preset2Handler = ( Target.StartPos.position + Target.EndPos.position ) / 2;
                                Target.PresetType2 = (PRESET_TYPE_2_CORNER)presetType2;
                                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                                UnityEditor.SceneView.RepaintAll();
                            }
                            if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f))) { DeleteCorner(0); }
                            GUILayout.EndHorizontal();
                        } else if (Target.CornerList.Count == 3) {
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);
                            GUILayout.Label(0.ToString(), centerStyle2, GUILayout.Width(43f));
                            string[] con_type3 = new string[] { "XY-Plane parallel 1", "XY-Plane parallel 2",
                                                                "YZ-Plane parallel 1", "YZ-Plane parallel 2",
                                                                "XZ-Plane parallel 1", "XZ-Plane parallel 2" };
                            int presetType3 = EditorGUILayout.Popup((int)Target.PresetType3, con_type3, GUILayout.Width(176f));

                            if (con_type_change || presetType3 != (int)Target.PresetType3) {
                                Undo.RecordObject(this, "Change connection");
                                con_type_change = false;
                                Vector3 centerPos = ( Target.StartPos.position + Target.EndPos.position ) / 2;
                                if (presetType3 == (int)PRESET_TYPE_3_CORNER.XY1) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          Target.StartPos.position.y,
                                                                                          centerPos.z);
                                    Target.CornerList[1].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          Target.StartPos.position.y,
                                                                                          centerPos.z);
                                    Target.CornerList[2].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          Target.EndPos.position.y,
                                                                                          centerPos.z);
                                } else if (presetType3 == (int)PRESET_TYPE_3_CORNER.XY2) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          Target.StartPos.position.y,
                                                                                          centerPos.z);
                                    Target.CornerList[1].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          Target.EndPos.position.y,
                                                                                          centerPos.z);
                                    Target.CornerList[2].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          Target.EndPos.position.y,
                                                                                          centerPos.z);
                                } else if (presetType3 == (int)PRESET_TYPE_3_CORNER.YZ1) {
                                    Target.CornerList[0].transform.position = new Vector3(centerPos.x,
                                                                                          Target.StartPos.position.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[1].transform.position = new Vector3(centerPos.x,
                                                                                          Target.EndPos.position.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[2].transform.position = new Vector3(centerPos.x,
                                                                                          Target.EndPos.position.y,
                                                                                          Target.EndPos.position.z);
                                } else if (presetType3 == (int)PRESET_TYPE_3_CORNER.YZ2) {
                                    Target.CornerList[0].transform.position = new Vector3(centerPos.x,
                                                                                          Target.StartPos.position.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[1].transform.position = new Vector3(centerPos.x,
                                                                                          Target.StartPos.position.y,
                                                                                          Target.EndPos.position.z);
                                    Target.CornerList[2].transform.position = new Vector3(centerPos.x,
                                                                                          Target.EndPos.position.y,
                                                                                          Target.EndPos.position.z);
                                } else if (presetType3 == (int)PRESET_TYPE_3_CORNER.XZ1) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          centerPos.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[1].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          centerPos.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[2].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          centerPos.y,
                                                                                          Target.EndPos.position.z);
                                } else if (presetType3 == (int)PRESET_TYPE_3_CORNER.XZ2) {
                                    Target.CornerList[0].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          centerPos.y,
                                                                                          Target.StartPos.position.z);
                                    Target.CornerList[1].transform.position = new Vector3(Target.StartPos.position.x,
                                                                                          centerPos.y,
                                                                                          Target.EndPos.position.z);
                                    Target.CornerList[2].transform.position = new Vector3(Target.EndPos.position.x,
                                                                                          centerPos.y,
                                                                                          Target.EndPos.position.z);
                                }
                                Target.Preset3Handler = Target.CornerList[1].transform.position;
                                Target.PresetType3 = (PRESET_TYPE_3_CORNER)presetType3;
                                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                                UnityEditor.SceneView.RepaintAll();
                            }
                            if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(70f))) { DeleteCorner(0); }
                            GUILayout.EndHorizontal();
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
            if (Target != null)
                GUI.enabled = Target.ConType != CON_TYPE.PRESET;
            if (GUILayout.Button("Clear corner list")) { ClearCorner(); Repaint(); }
            GUI.enabled = true;
            GuiLine();
            GUILayout.Label(EndTex);
            if (GUI.changed) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

        public void ClearCorner() {
            for (int i = 0; i < Target.CornerList.Count; i++) {
                GameObject go = Target.CornerList[i].gameObject;
                DestroyImmediate(go);
            }
            Target.CornerList = new List<Corner>();
            Target.CornerCount = 0;
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        public void CreateCorner(int NewType) {
            if (Target.ConType == CON_TYPE.PRESET && NewType > 3)
                NewType = 3;

            if (Target.ConType == CON_TYPE.PRESET || Target.CornerList.Count == 2) {
                Target.Preset2Handler = ( Target.StartPos.position + Target.EndPos.position ) / 2;
            } else if (Target.ConType == CON_TYPE.PRESET || Target.CornerList.Count == 3) {
                Target.Preset3Handler = Target.CornerList[1].transform.position;
            }

            for (int i = 0; i < Target.CornerList.Count; i++) {
                GameObject go = Target.CornerList[i].gameObject;
                DestroyImmediate(go);
            }
            Target.CornerList = new List<Corner>();
            Vector3 Amount = ( Target.EndPos.position - Target.StartPos.position ) / ( NewType + 1 );

            GameObject[] newGos = new GameObject[NewType];
            for (int k = 1; k <= NewType; k++) {
                GameObject go = new GameObject();
                go.transform.parent = TargetPathGroup.CornerParent.transform;
                go.transform.position = Target.StartPos.position + k * ( Amount );
                go.name = Target.name + " Corner_" + k;


                Corner newCorner = go.AddComponent<Corner>();
                GameObject line = new GameObject();
                line.transform.parent = go.transform;
                line.transform.position = go.transform.position;
                CornerChild CornerChild = line.AddComponent<CornerChild>();
                CornerChild.con = Target;
                newCorner.Line = line.transform;
                newCorner.pathGroup = TargetPathGroup;
                newCorner.Connection = Target;

                Mesh Cylinder = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Cylinder",typeof(Mesh));
                Mesh Sphere   = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Sphere", typeof(Mesh));

                MeshFilter mesh = go.AddComponent<MeshFilter>();
                mesh.mesh = Sphere;
                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                renderer.material = Target.ParentPathGroup.PathMat;
                renderer.sharedMaterial.color = Target.ParentPathGroup.PathColor;

                mesh = line.AddComponent<MeshFilter>();
                mesh.mesh = Cylinder;
                renderer = line.AddComponent<MeshRenderer>();
                renderer.material = Target.ParentPathGroup.PathMat;
                renderer.sharedMaterial.color = Target.ParentPathGroup.PathColor;

                Target.CornerList.Add(newCorner);
            }

            

            for(int i = 0; i < Target.CornerList.Count; i++) {
                if (i == Target.CornerList.Count - 1)
                    Target.CornerList[i].Destination = Target.EndPos;
                else Target.CornerList[i].Destination = Target.CornerList[i + 1].transform;
            }

            Repaint();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        public void CreateCornerContinue(int NewType) {
            if (Target.CornerList.Count == 0) {
                CreateCorner(NewType);
                return;
            }

            if(Target.ConType == CON_TYPE.PRESET || Target.CornerList.Count == 2) {
                Target.Preset2Handler = ( Target.StartPos.position + Target.EndPos.position ) / 2;
            } else if (Target.ConType == CON_TYPE.PRESET || Target.CornerList.Count == 3) {
                Target.Preset3Handler = Target.CornerList[1].transform.position;
            }

            int count = Target.CornerList.Count;
            Vector3 Amount = ( Target.EndPos.position - Target.CornerList[count-1].transform.position ) / 2;

            GameObject go = new GameObject();
            go.transform.parent = TargetPathGroup.CornerParent.transform;
            go.transform.position = Target.CornerList[count - 1].transform.position + Amount;
            go.name = Target.name + " Corner_" + count;

            Corner newCorner = go.AddComponent<Corner>();
            GameObject line = new GameObject();
            CornerChild CornerChild = line.AddComponent<CornerChild>();
            CornerChild.con = Target;
            line.transform.parent = go.transform;
            line.transform.position = go.transform.position;
            newCorner.Line = line.transform;
            newCorner.pathGroup = TargetPathGroup;
            newCorner.Connection = Target;

            Mesh Cylinder = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Cylinder", typeof(Mesh));
            Mesh Sphere = (Mesh)Resources.Load("UnityFlowVisualizer/Meshes/Sphere", typeof(Mesh));

            MeshFilter mesh = go.AddComponent<MeshFilter>();
            mesh.mesh = Sphere;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.material = Target.ParentPathGroup.PathMat;
            renderer.sharedMaterial.color = Target.ParentPathGroup.PathColor;

            mesh = line.AddComponent<MeshFilter>();
            mesh.mesh = Cylinder;
            renderer = line.AddComponent<MeshRenderer>();
            renderer.material = Target.ParentPathGroup.PathMat;
            renderer.sharedMaterial.color = Target.ParentPathGroup.PathColor;

            Target.CornerList.Add(newCorner);
            
            Target.CornerList[count].Destination = Target.EndPos;
            Target.CornerList[count-1].Destination = Target.CornerList[count].transform;

            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
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
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
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


