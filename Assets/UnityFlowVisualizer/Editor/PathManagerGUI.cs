using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class PathManagerGUI : EditorWindow {
        Vector2 scrollPosition;
        public PathManager Env;
        Texture LogoTex;
        Texture EndTex;

        [MenuItem("/Flow Visualizer/Total Path Manager")]
        public static void ShowWindow() {
            EditorWindow.GetWindowWithRect(typeof(PathManagerGUI), new Rect(0, 0, 300, 435),false, "Total Path Manager");
        }

        public void OnEnable() {
            LogoTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo", typeof(Texture2D));
            EndTex = (Texture2D)Resources.Load("UnityFlowVisualizer/Logo/FlowVisualizerLogo2", typeof(Texture2D));
            Env = PathManager.Instance;
        }

        // Start is called before the first frame update
        private void OnGUI() {
            Env = PathManager.Instance;
            try {
                GameObject go = Env.gameObject;
            } catch (System.Exception e) { e.ToString(); Env = null; }

            GUILayout.Label(LogoTex);
            GUILayout.Label("  Unity Flow Visualizer   |   유니티 유량 시각화 도구", EditorStyles.boldLabel);
            GUILayout.Label("  Developed by KimYC1223");
            GuiLine();
            GUILayout.Space(5);
            GUI.enabled = Env == null;
            if (GUILayout.Button("Instantiate Path Manager", GUILayout.Height(30)))
                EnvInitButton();
            GUI.enabled = true;
            if (Env == null)
                EditorGUILayout.HelpBox("  Create an Path Manager object first!", MessageType.Warning);
            GUILayout.Space(5);
            GuiLine();
            GUILayout.Space(5);

            GUI.enabled = Env != null;
            GUILayout.Label("Path group info list");
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Color", EditorStyles.toolbarButton,GUILayout.Width(70f));
            GUILayout.Label("Path group name", EditorStyles.toolbarButton, GUILayout.Width(170f));
            GUILayout.Label("Edit", EditorStyles.toolbarButton, GUILayout.Width(60f));
            GUILayout.EndHorizontal();
            if (Env != null) 
                 scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(300), GUILayout.Height(190));
            else scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(300), GUILayout.Height(150));
            
            try {
                // Scroll View Area
                if(Env == null) {
                    Rect rect1 = new Rect (0,0,300,150);
                    GUIStyle centerStyle = new GUIStyle("Label");
                    centerStyle.alignment = TextAnchor.MiddleCenter;
                    centerStyle.fixedHeight = 140;
                    GUILayout.BeginArea(rect1);
                    GUILayout.Label("Path Manager not found", centerStyle);
                    GUILayout.EndArea();
                } else {
                    try {
                        Rect rect0 = EditorGUILayout.GetControlRect(false, 21 * Env.PathInfoList.Count);
                        Rect rect1 = new Rect(rect0.x, rect0.y, 300, 21 * Env.PathInfoList.Count);
                        GUILayout.BeginArea(rect1);

                        for(int i = 0; i < Env.PathInfoList.Count; i++) {

                            Color targetColor = Env.PathInfoList[i].PathColor;
                            string targetName = Env.PathInfoList[i].PathName;
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);
                            GUILayout.Label(ColorToStr(targetColor), BackgroundStyle.GetToolbar(targetColor), GUILayout.Width(69f));
                            Env.PathInfoList[i].PathName = GUILayout.TextField(targetName, EditorStyles.toolbarTextField, GUILayout.Width(165f));
                            Env.PathInfoList[i].gameObject.name = Env.PathInfoList[i].PathName;
                            if (Env.PathInfoList.Count >= 9) { if (GUILayout.Button("Edit", EditorStyles.toolbarButton, GUILayout.Width(49f))) EditButtonClick(i); }
                            else { if (GUILayout.Button("Edit", EditorStyles.toolbarButton, GUILayout.Width(59f))) EditButtonClick(i); }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndArea();
                    } catch(System.Exception e) {
                        Rect rect1 = new Rect(0, 0, 300, 150);
                        GUIStyle centerStyle = new GUIStyle("Label");
                        centerStyle.alignment = TextAnchor.MiddleCenter;
                        centerStyle.fixedHeight = 140;
                        GUILayout.BeginArea(rect1);
                        GUILayout.Label("Path Manager not found", centerStyle);
                        GUILayout.EndArea();
                        e.ToString();
                    }
                }
            }catch (System.Exception e) {
                e.ToString();
                Rect rect1 = new Rect(0, 0, 300, 150);
                GUILayout.BeginArea(rect1);
                GUILayout.EndArea();
            }

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if(GUILayout.Button("New group", EditorStyles.toolbarButton)) NewPathButtonClick();
            if(GUILayout.Button("Group editor", EditorStyles.toolbarButton)) PathEditButtonClick();
            GUILayout.EndHorizontal();


            GUI.enabled = true;
            GuiLine();
            GUILayout.Label(EndTex);
            if (GUI.changed) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }


        void GuiLine(int i_height = 1) {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public void EnvInitButton() {
            if (Env == null) {
                GameObject go = new GameObject();
                Env = go.AddComponent<PathManager>();
                go.name = "PathManager";
            }
        }

        public void EditButtonClick(int index) {
            Selection.objects = new UnityEngine.Object[] { Env.PathInfoList[index].gameObject};
            PathGroupEditorGUI.Target = Env.PathInfoList[index];
            PathGroupEditorGUI.ShowWindow();
            this.Close();
        }

        public void PathEditButtonClick() {
            PathGroupEditorGUI.Target = null;
            PathGroupEditorGUI.ShowWindow();
            this.Close();
        }

        public void NewPathButtonClick() {
            if (Env == null) return;
            GameObject newOb = new GameObject();
            newOb.name = "New path group name";
            newOb.transform.parent = Env.transform;
            PathInfo newInfo = newOb.AddComponent<PathInfo>();
            newInfo.PathColor = new Color(UnityEngine.Random.Range(0f, 1f),
                                          UnityEngine.Random.Range(0f, 1f),
                                          UnityEngine.Random.Range(0f, 1f), 0.5f);
            Material PathMatOrigin = (Material)Resources.Load("UnityFlowVisualizer/Materials/PathMat", typeof(Material));
            newInfo.PathMat = new Material(PathMatOrigin);
            newInfo.PathName = "New path group name";
            newInfo.NodeList = new List<Node>();
            newInfo.ConnectionList = new List<Connection>();

            Env.PathInfoList.Add(newInfo);
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        public static string ColorToStr(Color color) {
            string r = ( (int)( color.r * 255 ) ).ToString("X2");
            string g = ( (int)( color.g * 255 ) ).ToString("X2");
            string b = ( (int)( color.b * 255 ) ).ToString("X2");

            string result = string.Format("#{0}{1}{2}", r, g, b);
            return result;
        }

        void OnSelectionChange() {
            this.Repaint();
        }

        public static class BackgroundStyle {
            private static GUIStyle style = new GUIStyle();
            private static GUIStyle style2 = new GUIStyle("ToolbarButton");
            private static GUIStyle style3 = new GUIStyle("ToolbarTextField");
            private static Texture2D texture = new Texture2D(1, 1);
            public static GUIStyle Get(Color color) {
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
                return style;
            }

            public static GUIStyle GetToolbar(Color color) {
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style2.normal.background = texture;
                return style2;
            }

            public static GUIStyle GetToolbarTextField(Color color) {
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style3.normal.background = texture;
                return style3;
            }
        }
    }
}


