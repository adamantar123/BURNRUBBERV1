using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.DragRacingFramework
{
	[CustomEditor(typeof(VehicleController))]
	public class VehicleControllerEditor : Editor
	{
		public VehicleController script;
		private ReorderableList wheelsList;
		private ReorderableList gearsList;

		private GUIStyle grayBackground;

		public void Awake()
		{
			script = (VehicleController) target;
		}

		private void OnEnable()
		{
			wheelsList = new ReorderableList(serializedObject, serializedObject.FindProperty("wheels"), false, true,
				true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Game Objects°", "Set all the wheels of your vehicle here.")); },

				onAddCallback = items => { script.wheels.Add(null); },

				onRemoveCallback = items => { script.wheels.Remove(script.wheels[items.index]); },

				drawElementCallback = (rect, index, isActive, isFocused) => { script.wheels[index] = (Transform) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.wheels[index], typeof(Transform), true); }
			};

			gearsList = new ReorderableList(serializedObject, serializedObject.FindProperty("gears"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Gears");
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 10 + 10, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), new GUIContent("Max Speed°", "• This is the approximate maximum speed at which the car can move with the selected gear." + "\n\n" +

					                                                                                                                                                     "• You need to adjust these values to distribute all gears up to the car maximum speed." + "\n\n" +
					                                                                                                                                                     "• Each following value should be higher than the previous one." + "\n\n" +
					                                                                                                                                                     "[Example]" + "\n" +
					                                                                                                                                                     "1st gear - 30 km/h" + "\n" +
					                                                                                                                                                     "2nd gear - 60 km/h" + "\n" +
					                                                                                                                                                     "3rd gear - 90 km/h" + "\n" +
					                                                                                                                                                     "4th gear - 120 km/h" + "\n" +
					                                                                                                                                                     "Max Speed - 130 km/h"));
						
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 10 + rect.width / 4 + 30, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("Engine Sounds°", "• Optional you can set special engine sounds for each gear. " + "\n" +
					                                                                                                                                                                          "• Leave the fields blank to use the general sound (the one set in the [Sounds] tab)."));
					
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 10  + rect.width / 4 + rect.width / 3 + 50, rect.y, rect.width - (rect.width / 10  + rect.width / 4 + rect.width / 3 + 50), EditorGUIUtility.singleLineHeight), new GUIContent("Audio Pitch°", "Switching gears will also change the pitch of the Audio Controller component."));
				},

				onAddCallback = items => { script.gears.Add(null); },

				onRemoveCallback = items =>
				{
					if(script.gears.Count > 1)
						script.gears.Remove(script.gears[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), (index + 1).ToString());
					script.gears[index].maxTransferSpeed = EditorGUI.IntField(new Rect(rect.x + rect.width / 10 + 10, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), script.gears[index].maxTransferSpeed);
					script.gears[index].specificEngineSound = (AudioClip) EditorGUI.ObjectField(new Rect(rect.x + (rect.width / 10 + rect.width / 4) + 30, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), script.gears[index].specificEngineSound, typeof(AudioClip), false);
					script.gears[index].enginePitch = EditorGUI.FloatField(new Rect(rect.x + (rect.width / 10 + rect.width / 4 + rect.width / 3) + 50, rect.y, rect.width - (rect.width / 10 + rect.width / 4 + rect.width / 3) - 50, EditorGUIUtility.singleLineHeight), script.gears[index].enginePitch);
				}
			};

			
			EditorApplication.update += Update;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		void Update()
		{
			if (!Application.isPlaying && script)
			{
				if (script.carId == "")
				{
					script.carId = MultiplayerHelper.GenerateRandomName();
					EditorUtility.SetDirty(script.gameObject);
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}

				if (!script.firstInstance && script.gears.Count == 0)
				{
					script.gears.Add(new CarHelper.Gear{maxTransferSpeed = 30, enginePitch = 1.1f});
					script.gears.Add(new CarHelper.Gear{maxTransferSpeed = 60, enginePitch = 1.2f});
					script.gears.Add(new CarHelper.Gear{maxTransferSpeed = 90, enginePitch = 1.4f});
					script.gears.Add(new CarHelper.Gear{maxTransferSpeed = 120, enginePitch = 1.6f});
					script.gears.Add(new CarHelper.Gear{maxTransferSpeed = 160, enginePitch = 1.8f});
					script.gears.Add(new CarHelper.Gear{maxTransferSpeed = 200, enginePitch = 2f});

					script.firstInstance = true;
				}

				if (!script.uiManagerAsset)
				{
					script.uiManagerAsset = Resources.Load("UI MANAGER", typeof(UIManager)) as UIManager;
				}

				if (!script.engineAudioSource)
				{
					var tempCar = !script.gameObject.activeInHierarchy && script.gameObject.activeSelf ? (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject) : script.gameObject;

					var controller = tempCar.GetComponent<VehicleController>();

					var audioController = new GameObject("Audio Controller").transform;
					audioController.transform.SetParent(tempCar.transform);
					audioController.transform.localPosition = Vector3.zero;

					controller.engineAudioSource = CarHelper.CreateAudioSourse(audioController, "Engine");
					controller.nitroAudioSource = CarHelper.CreateAudioSourse(audioController, "Nitro");
					controller.switchGearAudioSource = CarHelper.CreateAudioSourse(audioController, "Switch Transmission");

#if !UNITY_2018_3_OR_NEWER
				PrefabUtility.ReplacePrefab(tempCar, PrefabUtility.GetPrefabParent(tempCar), ReplacePrefabOptions.ConnectToPrefab);
#else
					if(tempCar && PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCar) != "")
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempCar, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCar), InteractionMode.AutomatedAction);
#endif

					if (!script.gameObject.activeInHierarchy && script.gameObject.activeSelf)
						DestroyImmediate(tempCar);
				}
			}
		}


		public override void OnInspectorGUI()
		{
			UIHelper.InitStyles(ref grayBackground, new Color32(160, 160, 160, 200));

			serializedObject.Update();

			EditorGUILayout.Space();

			script.currentInspectorTab = GUILayout.Toolbar(script.currentInspectorTab, new[] {"Parameters", "Physics", "Sounds"});

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			switch (script.currentInspectorTab)
			{
				case 0:
					// EditorGUILayout.BeginVertical("helpbox");
					
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("carImage"), new GUIContent("Image°", "This image will be used in the UI"));
					EditorGUILayout.EndVertical();

					if (script.uiManagerAsset)
					{
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginVertical("helpbox");

						var names = script.uiManagerAsset.inGameUI.gameUI.dashboards.Select(dashboard => dashboard.name).ToList();
						script.selectedDashboard = EditorGUILayout.Popup(new GUIContent( "Dashboard°", "You can create several dashboards in the [UIManager] and then set one of them for this vehicle."), script.selectedDashboard, names.ToArray());
						EditorGUILayout.EndVertical();
					}


					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.LabelField(new GUIContent("Specifications"), EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("carInfo.MaxSpeed"), new GUIContent("Maximum Speed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("carInfo.Acceleration"), new GUIContent("Acceleration"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("carInfo.Power"), new GUIContent("Power"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("carInfo.nitroTime"), new GUIContent("Nitro Time (sec)"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("carInfo.Mass"), new GUIContent("Mass"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					gearsList.DoLayoutList();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					
					EditorGUILayout.LabelField(new GUIContent("Placeholders°", "These placeholders are needed to display the nickname and avatar of opponents."), EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("avatarPlaceholder"), new GUIContent("Avatar"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("nicknamePlaceholder"), new GUIContent("Nickname"));
					
					if (!script.avatarPlaceholder)
					{
						EditorGUILayout.Space();

						if (GUILayout.Button("Create Placeholders"))
						{
							CreatePlaceholders();
						}
					}

					EditorGUILayout.EndVertical();

					// EditorGUILayout.EndVertical();
					break;

				case 1:
					// EditorGUILayout.BeginVertical("helpbox");
					
					EditorGUILayout.LabelField(new GUIContent("Body"), EditorStyles.boldLabel);

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationOffset"), new GUIContent("Direction°", "The green arrow on the prefab should point forward."));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationPart"), new GUIContent("Rotation Part°", "The car body"));
					// EditorGUILayout.Space();
					if (script.rotationPart)
					{
						EditorGUILayout.BeginVertical("helpbox");

						script.bodyRotationAxis = (CarHelper.BodyRotationAxis) EditorGUILayout.EnumPopup(new GUIContent("Rotation Axis°", "Choose the axis that suits your model"), script.bodyRotationAxis);
						EditorGUILayout.Space();

						EditorGUILayout.PropertyField(serializedObject.FindProperty("bodyRotationSpeed"), new GUIContent("Rotation Speed°", "How fast the car body will deflect"));
						EditorGUILayout.Space();
						// EditorGUILayout.LabelField("Rotation Values:", EditorStyles.label);
						// EditorGUILayout.BeginVertical("helpbox");
						EditorGUILayout.PropertyField(serializedObject.FindProperty("drivingRotation"), new GUIContent("Driving Rotation Value°", "The base value by which the car body will deflect when driving"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("brakingRotation"), new GUIContent("Braking Rotation Value°", "The base value by which the car body will deflect when braking"));
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndVertical();
					
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField(new GUIContent("Wheels"), EditorStyles.boldLabel);

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelsRotationAxis"), new GUIContent("Rotation Axis"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					wheelsList.DoLayoutList();
					// EditorGUILayout.EndVertical();

					break;

				case 2:
					// EditorGUILayout.BeginVertical(grayBackground);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("engineAudioClip"), new GUIContent("Engine°", "Other movement sounds (acceleration, deceleration, etc) will be generated from this audio clip."));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("nitroAudioClip"), new GUIContent("Nitro"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchGearAudioClip"), new GUIContent("Switch Gear"));

					EditorGUILayout.EndVertical();
					// EditorGUILayout.EndVertical();
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
		
		void CreatePlaceholders()
		{
			var canvas = UIHelper.NewCanvas("Canvas", script.transform);
			
			var avatarPlaceholder = new GameObject("Avatar Placeholder");
			avatarPlaceholder.transform.SetParent(canvas.transform);
			var rect = avatarPlaceholder.AddComponent<RectTransform>();
			rect.sizeDelta = new Vector2(1.72f, 1.72f);
			avatarPlaceholder.transform.localPosition = Vector3.zero;
			var avatarImage = avatarPlaceholder.AddComponent<RawImage>();
			
			var textPlaceholder = new GameObject("Nickname Placeholder");
			textPlaceholder.transform.SetParent(canvas.transform);
			textPlaceholder.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
			rect = textPlaceholder.AddComponent<RectTransform>();
			rect.sizeDelta = new Vector2(575, 112);
			textPlaceholder.transform.localPosition = Vector3.zero;
			var text = textPlaceholder.AddComponent<Text>();
			text.text = "Nickname";
			text.fontSize = 90;
			text.alignment = TextAnchor.MiddleCenter;

			script.avatarPlaceholder = avatarImage;
			script.nicknamePlaceholder = text;
		}
	}
}
