using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityFlowVisualizer {
    [CustomEditor(typeof(Connection))]
    public class ConnectionGUI : Editor {
        [HideInInspector]
        private Tool LastTool = Tool.None;

        private bool showPosition = true;

        private void OnSceneGUI() {
            Connection component = target as Connection;

            try {
                if (component.ConType == CON_TYPE.PLAIN) {
                    for (int i = 0; i < component.CornerList.Count; i++) {
                        Vector3 temp =
                        PositionHandlePlane(component.CornerList[i].transform, component.StartPos.position, component.EndPos.position);
                        if(temp != component.CornerList[i].transform.position) {
                            Undo.RecordObject(component.CornerList[i].transform, "Change connection");
                            component.CornerList[i].transform.position = temp;
                            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                        }
                    }
                } else if (component.ConType == CON_TYPE.FREE) {
                    for (int i = 0; i < component.CornerList.Count; i++) {
                        Vector3 temp = PositionHandle(component.CornerList[i].transform);
                        if (temp != component.CornerList[i].transform.position) {
                            Undo.RecordObject(component.CornerList[i].transform, "Change connection");
                            component.CornerList[i].transform.position = temp;
                            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                        }
                    }
                } else if (component.ConType == CON_TYPE.PRESET&& component.CornerList.Count == 2) {
                    component.Preset2Handler = ( component.CornerList[0].transform.position + component.CornerList[1].transform.position )/2;
                    Vector3 temp = PositionHandleAxis(component.Preset2Handler, component.PresetType2);
                    if (component.Preset2Handler != temp) {
                        Undo.RecordObjects(new Object[] { component.CornerList[0].transform ,
                                                          component.CornerList[1].transform }, "Change connection");
                        component.Preset2Handler = temp;
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                        UnityEditor.SceneView.RepaintAll();
                    }
                } else if (component.ConType == CON_TYPE.PRESET && component.CornerList.Count == 3) {
                    Vector3 temp = PositionHandle_XYZPlane(component.CornerList[1].transform.position, component.PresetType3);
                    if (component.Preset3Handler != temp) {
                        Undo.RecordObject(component.CornerList[1].transform, "Change connection");
                        component.Preset3Handler = temp;
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                        UnityEditor.SceneView.RepaintAll();
                    }
                }
            } catch(System.Exception e) {
                e.ToString();
                Corner[] childs = component.transform.GetComponentsInChildren<Corner>();
                component.CornerList = new List<Corner>();
                if (childs != null && childs.Length > 0)
                    for (int i = 0; i < childs.Length; i++)
                        component.CornerList.Add(childs[i]);
                component.CornerCount = component.CornerList.Count;
                Repaint();
            }

            if (GUI.changed) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

        public void OnEnable() {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        public void OnDisable() {
            Tools.current = LastTool;
        }

        public override void OnInspectorGUI() {
            try {
                Connection component = target as Connection;

                GUIStyle centerStyle = new GUIStyle("Label");
                centerStyle.alignment = TextAnchor.MiddleCenter;
                Texture LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
                GUILayout.Label(LogoTex, GUILayout.Width(250));
                GUI.enabled = false;
                GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
                GUILayout.Label("  Developed by KimYC1223");
                GUI.enabled = true;
                GUILayout.Space(10);
                GuiLine();
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Path group", EditorStyles.boldLabel, GUILayout.Width(110));
                PathGroup temp = EditorGUILayout.ObjectField("", component.ParentPathGroup, typeof(PathGroup), true) as PathGroup;
                EditorGUILayout.LabelField("", PathManagerEditorGUI.BackgroundStyle.Get(component.ParentPathGroup.PathColor), GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Connection name", EditorStyles.boldLabel, GUILayout.Width(110));
                component.Name = GUILayout.TextField(component.Name);
                component.transform.gameObject.name = component.Name;
                GUILayout.EndHorizontal();
                Node startNode = null;
                Node endNode = null;
                for(int i = 0; i < component.ParentPathGroup.NodeList.Count; i++) {
                    if (component.ParentPathGroup.NodeList[i].ID == component.StartID)
                        startNode = component.ParentPathGroup.NodeList[i];
                    if (component.ParentPathGroup.NodeList[i].ID == component.EndID)
                        endNode = component.ParentPathGroup.NodeList[i];
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Start node", GUILayout.Width(110));
                Node temp2 = EditorGUILayout.ObjectField("", startNode, typeof(Node), true) as Node;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("End node", GUILayout.Width(110));
                Node temp3 = EditorGUILayout.ObjectField("", endNode, typeof(Node), true) as Node;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Connection type", GUILayout.Width(110));

                string str = "[" + component.ConType.ToString() + "] ";
                string[] options1 = new string[] { "Perpendicular to the start node", "Perpendicular to the end node" };
                string[] options2 = new string[] { "Parallel to X-axis", "Parallel to Y-axis", "Parallel to Z-axis" };
                string[] options3 = new string[] { "Three corners in XY plane (1)", "Three corners in XY plane (2)",
                                                    "Three corners in YZ plane (1)", "Three corners in YZ plane (2)",
                                                    "Three corners in XZ plane (1)", "Three corners in XZ plane (2)"};

                GUILayout.Label(str,EditorStyles.boldLabel);
                GUILayout.EndHorizontal();

                GUI.enabled = component.CornerList.Count > 0;
                showPosition =  EditorGUILayout.Foldout(showPosition,"Corner count : " + component.CornerList.Count);
                if(showPosition) {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (component.ConType == CON_TYPE.FREE || component.ConType == CON_TYPE.PLAIN)
                         GUILayout.Label("Index", EditorStyles.toolbarButton, GUILayout.Width(60f));
                    else GUILayout.Label("Type", EditorStyles.toolbarButton, GUILayout.Width(60f));
                    GUILayout.Label("Corner", EditorStyles.toolbarButton);
                    GUILayout.EndHorizontal();
                    if(component.ConType == CON_TYPE.FREE || component.ConType == CON_TYPE.PLAIN) {
                        for (int i = 0; i < component.CornerList.Count; i++) {
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);
                            GUILayout.Label(i.ToString(),centerStyle, GUILayout.Width(55));
                            Vector3 temp4 = EditorGUILayout.Vector3Field("",component.CornerList[i].transform.position);
                            GUILayout.EndHorizontal();
                        }
                    } else {
                        GUILayout.BeginHorizontal(EditorStyles.toolbar);
                        GUILayout.Label("TYPE", centerStyle, GUILayout.Width(55));
                        if (component.ConType == CON_TYPE.PRESET && component.CornerList.Count == 1)
                            GUILayout.Label(options1[(int)component.PresetType1], centerStyle);
                        else if (component.ConType == CON_TYPE.PRESET && component.CornerList.Count == 2)
                            GUILayout.Label(options2[(int)component.PresetType2], centerStyle);
                        else if (component.ConType == CON_TYPE.PRESET && component.CornerList.Count == 3)
                            GUILayout.Label(options3[(int)component.PresetType3], centerStyle);
                        else GUILayout.Label("-", centerStyle);
                        GUILayout.EndHorizontal();
                    }
                }
                GUI.enabled = true;

                GUILayout.Space(10);
                GuiLine();
                GUILayout.Space(10);
                if (GUILayout.Button("Open Path group editor", GUILayout.Height(35))) {
                    ConnectionEditorGUI.ShowWindow();
                    ConnectionEditorGUI.Target = component;
                }
                GUILayout.Space(10);
                
            } catch(System.Exception e) { e.ToString(); }
        }

        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        Vector3 PositionHandlePlane(Transform transform,Vector3 start,Vector3 end) {
            if((start.x == end.x) && (start.z == end.z))
                return PositionHandle(transform);
            

            var position = transform.position;
            start = new Vector3(start.x, 0f, start.z);
            end = new Vector3(end.x, 0f, end.z);
            Vector3 newDir = new Vector3(end.x - start.x, 0f, end.z - start.z);
            newDir = newDir.normalized;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, newDir); //X축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, Vector3.up); //Z축

            return position;
        }

        Vector3 PositionHandlePlane(Vector3 pos, Vector3 start, Vector3 end) {
            if (( start.x == end.x ) && ( start.z == end.z ))
                return PositionHandle(pos);


            var position = pos;
            start = new Vector3(start.x, 0f, start.z);
            end = new Vector3(end.x, 0f, end.z);
            Vector3 newDir = new Vector3(end.x - start.x, 0f, end.z - start.z);
            newDir = newDir.normalized;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, newDir); //X축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, Vector3.up); //Z축

            return position;
        }

        Vector3 PositionHandleAxis(Vector3 pos, PRESET_TYPE_2_CORNER Axis = PRESET_TYPE_2_CORNER.Z) {
            var position = pos;
                        
            if(Axis != PRESET_TYPE_2_CORNER.X) {
                Handles.color = Handles.xAxisColor;
                position = Handles.Slider(position, new Vector3(1, 0, 0)); //X축
            }
            if (Axis != PRESET_TYPE_2_CORNER.Y) {
                Handles.color = Handles.yAxisColor;
                position = Handles.Slider(position, new Vector3(0, 1, 0)); //Y축
            }
            if (Axis != PRESET_TYPE_2_CORNER.Z) {
                Handles.color = Handles.zAxisColor;
                position = Handles.Slider(position, new Vector3(0, 0, 1)); //Z축
            }

            return position;
        }

        Vector3 PositionHandle_XYZPlane(Vector3 pos, PRESET_TYPE_3_CORNER plane = PRESET_TYPE_3_CORNER.XZ1) {
            var position = pos;

            if (plane == PRESET_TYPE_3_CORNER.YZ1 || plane == PRESET_TYPE_3_CORNER.YZ2) {
                Handles.color = Color.red;
                position = Handles.Slider(position, new Vector3(1, 0, 0)); //X축
                Handles.color = Color.blue;
                position = Handles.Slider(position, new Vector3(-1, 0, 0)); //-X축
            }
            if (plane == PRESET_TYPE_3_CORNER.XZ1 || plane == PRESET_TYPE_3_CORNER.XZ2) {
                Handles.color = Color.red;
                position = Handles.Slider(position, new Vector3(0, 1, 0)); //Y축
                Handles.color = Color.blue;
                position = Handles.Slider(position, new Vector3(0, -1, 0)); //-Y축
            }
            if (plane == PRESET_TYPE_3_CORNER.XY1 || plane == PRESET_TYPE_3_CORNER.XY2) {
                Handles.color = Color.red;
                position = Handles.Slider(position, new Vector3(0, 0, 1)); //Z축
                Handles.color = Color.blue;
                position = Handles.Slider(position, new Vector3(0, 0, -1)); //-Z축
            }

            return position;
        }

        Vector3 PositionHandle(Transform transform) {
            var position = transform.position;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, new Vector3(1, 0, 0)); //X축
            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, new Vector3(0, 1, 0)); //Y축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, new Vector3(0, 0, 1)); //Z축

            return position;
        }

        Vector3 PositionHandle(Vector3 pos) {
            var position = pos;

            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, new Vector3(1, 0, 0)); //X축
            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, new Vector3(0, 1, 0)); //Y축
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, new Vector3(0, 0, 1)); //Z축

            return position;
        }
    }
}
