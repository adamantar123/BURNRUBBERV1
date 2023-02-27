using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GercStudio.DragRacingFramework
{
    [CustomEditor(typeof(GameAssets))]
    public class GameAssetsEditor : Editor
    {
        public GameAssets script;

        public void Awake()
        {
            script = (GameAssets) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component stores all the player data (do not delete it)", MessageType.Info);

            // DrawDefaultInspector();
        }
    }
}
