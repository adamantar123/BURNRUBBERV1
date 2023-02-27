using System.Collections;
using System.Collections.Generic;
using GercStudio.DragRacingFramework;
using UnityEditor;
using UnityEngine;

namespace GercStudio.DragRacingFramework
{
	[CustomEditor(typeof(InputSettings))]
	public class InputSettingsEditor : Editor
	{

		private InputSettings script;


		public void Awake()
		{
			script = (InputSettings) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();
			
#if DR_INPUT_SYSTEM
			EditorGUILayout.LabelField("Gas Pedal", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("HelpBox");
			script.keyboardButtonsInProjectSettings[0] = (InputHelper.KeyboardCodes) EditorGUILayout.EnumPopup("Keyboard", script.keyboardButtonsInProjectSettings[0]);
			script.gamepadButtonsInProjectSettings[0] = (InputHelper.GamepadButtons) EditorGUILayout.EnumPopup("Gamepad", script.gamepadButtonsInProjectSettings[0]);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Previous Gear", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("HelpBox");
			script.keyboardButtonsInProjectSettings[2] = (InputHelper.KeyboardCodes) EditorGUILayout.EnumPopup("Keyboard", script.keyboardButtonsInProjectSettings[2]);
			script.gamepadButtonsInProjectSettings[2] = (InputHelper.GamepadButtons) EditorGUILayout.EnumPopup("Gamepad", script.gamepadButtonsInProjectSettings[2]);
			EditorGUILayout.EndVertical();
			
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Next Gear", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("HelpBox");
			script.keyboardButtonsInProjectSettings[1] = (InputHelper.KeyboardCodes) EditorGUILayout.EnumPopup("Keyboard", script.keyboardButtonsInProjectSettings[1]);
			script.gamepadButtonsInProjectSettings[1] = (InputHelper.GamepadButtons) EditorGUILayout.EnumPopup("Gamepad", script.gamepadButtonsInProjectSettings[1]);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Activate Nitro", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("HelpBox");
			script.keyboardButtonsInProjectSettings[3] = (InputHelper.KeyboardCodes) EditorGUILayout.EnumPopup("Keyboard", script.keyboardButtonsInProjectSettings[3]);
			script.gamepadButtonsInProjectSettings[3] = (InputHelper.GamepadButtons) EditorGUILayout.EnumPopup("Gamepad", script.gamepadButtonsInProjectSettings[3]);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Pause", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("HelpBox");
			script.keyboardButtonsInProjectSettings[4] = (InputHelper.KeyboardCodes) EditorGUILayout.EnumPopup("Keyboard", script.keyboardButtonsInProjectSettings[4]);
			script.gamepadButtonsInProjectSettings[4] = (InputHelper.GamepadButtons) EditorGUILayout.EnumPopup("Gamepad", script.gamepadButtonsInProjectSettings[4]);
			EditorGUILayout.EndVertical();
#else
			
			EditorGUILayout.HelpBox("To use the keyboard and gamepads, you must first import the 'Input System' and then activate it by pressing [Tools/DRF/Integrations/Input System].", MessageType.Info);
			
			if (LinkLabel(new GUIContent("More info can be found in Documentation"))) 
			{
				Application.OpenURL("https://gerc-studio.gitbook.io/drag-racing/project-settings/input-manager"); 
			}

#endif
			
			serializedObject.ApplyModifiedProperties();

			// DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
			}

		}
		
		bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
		{
			var position = GUILayoutUtility.GetRect(label, EditorStyles.linkLabel, options);

			Handles.BeginGUI();
			Handles.color = Color.blue;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();

			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

			return GUI.Button(position, label, EditorStyles.linkLabel);
		}
	}
}
