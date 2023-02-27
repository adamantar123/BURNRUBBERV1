using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GercStudio.DragRacingFramework
{
    public class About : EditorWindow
    {
        private GUIStyle LabelStyle;
        private Vector2 scrollPos;
        private const string Version = "1.3.2";

        [MenuItem("Tools/Drag Racing Framework/About", false, -10000)]
        public static void ShowWindow()
        {
            GetWindowWithRect(typeof(About), new Rect(Vector2.zero, new Vector2(400, 200)),true, "About DRF").ShowUtility();
        }


        private void Awake()
        {
            if (LabelStyle != null) return;

            LabelStyle = new GUIStyle
            {
                normal = {textColor = Color.black},
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            LabelStyle.fontStyle = FontStyle.Bold;
            LabelStyle.fontSize = 15;
            GUILayout.Label("Drag Racing Framework", LabelStyle);
            GUILayout.Label(Version + " version", LabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            LabelStyle.fontSize = 13;
            GUILayout.Label("Support email: gercstudio@gmail.com", LabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            LabelStyle.fontStyle = FontStyle.Normal;
            LabelStyle.fontSize = 12;
            GUILayout.Label("Copyright © 2022 GercStudio " + "\n" + "All rights reserved", LabelStyle);

        }
    }
}
