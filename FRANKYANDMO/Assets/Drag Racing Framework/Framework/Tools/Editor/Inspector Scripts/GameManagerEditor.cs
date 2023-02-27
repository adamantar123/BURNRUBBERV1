using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace GercStudio.DragRacingFramework
{
	[CustomEditor(typeof(GameManager))]
	public class GameManagerEditor : Editor
	{

		public GameManager script;
		private ReorderableList roadPrefabs;

		private GUIStyle grayBackground;
		private GUIStyle style;

		public void Awake()
		{
			script = (GameManager) target;
		}

		private void OnEnable()
		{
			roadPrefabs = new ReorderableList(serializedObject, serializedObject.FindProperty("roadPrefabs"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Road Prefabs°", "These prefabs will be randomly generated in the game")); },

				onAddCallback = items => { script.roadPrefabs.Add(null); },

				onRemoveCallback = items => { script.roadPrefabs.Remove(script.roadPrefabs[items.index]); },

				drawElementCallback = (rect, index, isActive, isFocused) => { script.roadPrefabs[index] = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.roadPrefabs[index], typeof(GameObject), true); }
			};

			EditorApplication.update += Update;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		void Update()
		{
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			style = new GUIStyle(EditorStyles.helpBox) {richText = true, fontSize = 10};
			UIHelper.InitStyles(ref grayBackground, new Color32(160, 160, 160, 200));

			EditorGUILayout.Space();

			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Road Parameters", "Camera", "Events"});//, "Other"});

			EditorGUILayout.Space();

			switch (script.inspectorTab)
			{
				case 0:
					EditorGUILayout.BeginVertical(grayBackground);
					EditorGUILayout.BeginVertical("helpbox");
					script.roadType = (GameHelper.RoadType) EditorGUILayout.EnumPopup("Road Type", script.roadType);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					switch (script.roadType)
					{
						case GameHelper.RoadType.AutoGeneration:
							EditorGUILayout.PropertyField(serializedObject.FindProperty("finishFlagDistance"), new GUIContent("Distance"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("roadLength"), new GUIContent("Offset°", "At what distance the new part of the road will be generated" + "\n" +
							                                                                                                              "Usually this value is equal to the length of one part of the road"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("roadImage"), new GUIContent("Image°", "This image will be used in the UI"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("lastRoadPart"), new GUIContent("1st Road Part°", "Place one part of road on the scene and set here; All remaining parts will be added relative to the first"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("finishFlag"), new GUIContent("Finish Flag°", "(Prefab/Model) The prefab will be spawned at the end of the road"));
							EditorGUILayout.Space();
							roadPrefabs.DoLayoutList();
							break;
						case GameHelper.RoadType.SetManually:
							EditorGUILayout.PropertyField(serializedObject.FindProperty("finishFlag"), new GUIContent("Finish Flag°", "(Scene Object) This flag is also a finish line"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("roadImage"), new GUIContent("Image°", "This image will be used in the UI"));
							break;
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.LabelField(new GUIContent("Cars Spawn Points°", "Cars will appear at these points; And also the cars will drive in the direction indicated in these points"), EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("playerSpawnPoint"), new GUIContent("Player"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("opponentSpawnPoint"), new GUIContent("Opponent"));

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();


					break;

				case 1:
					EditorGUILayout.BeginVertical(grayBackground);
#if !DR_CINEMACHINE
				EditorGUILayout.LabelField("The Cinemachine integration is <b>inactive</b>. " + "\n" + 
				                           "To activate it, download the <b><color=green>Cinemachine package</color></b> from the <b><color=blue>Package Manager</color></b> and then press <b><color=green>[Tools/Drag Racing Framework/Cinemachine Integration -> Enable]</color></b>." + "\n\n" +
				                           "Set a default camera here and it will smoothly follow the player's car during the game.", style);
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical("helpbox");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCamera"), new GUIContent("Camera"));
				EditorGUILayout.EndVertical();


#else
					EditorGUILayout.LabelField("<color=green>The Cinemachine integration is <b>active</b></color>. " + "\n\n" +
					                           "Set a <b>cinemachine virtual camera</b> here and adjust its parameters (the <b>target</b> for it <b>will be set automatically</b> at the start of the game).", style);
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("virtualCamera"), new GUIContent("Virtual Camera"));
					EditorGUILayout.EndVertical();

#endif
					EditorGUILayout.EndVertical();

					break;

				case 2:

					EditorGUILayout.HelpBox("When one of these actions happens in the game, all the events that you added for it will be triggered.", MessageType.Info);
					EditorGUILayout.Space();

					script.currentEventsMenu = GUILayout.Toolbar(script.currentEventsMenu, new[] {"Game", "Player Car"});
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical(grayBackground);
					switch (script.currentEventsMenu)
					{
						case 0:

							EditorGUILayout.PropertyField(serializedObject.FindProperty("raceStartedEvent"), new GUIContent("Game Started"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("victoryEvent"), new GUIContent("Victory"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("lossEvent"), new GUIContent("Defeat"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("playerFinishedEvent"), new GUIContent("Player Finished"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("opponentFinishedEvent"), new GUIContent("Opponent Finished"));
							break;

						case 1:
							EditorGUILayout.PropertyField(serializedObject.FindProperty("gearUpEvent"), new GUIContent("Increase Gear"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("gearDownEvent"), new GUIContent("Reduce Gear"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("startUsingNitroEvent"), new GUIContent("Start Using Nitro"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("endUsingNitroEvent"), new GUIContent("End Using Nitro"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("perfectStartEvent"), new GUIContent("Perfect Start"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("perfectShiftEvent"), new GUIContent("Perfect Shift"));
							break;
					}

					EditorGUILayout.EndVertical();

					break;
				
				case 3:
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("currentUIManager"), new GUIContent("UI Manager"));
					EditorGUILayout.EndVertical();
					break;
			}

			serializedObject.ApplyModifiedProperties();

			// DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

		}
	}
}
