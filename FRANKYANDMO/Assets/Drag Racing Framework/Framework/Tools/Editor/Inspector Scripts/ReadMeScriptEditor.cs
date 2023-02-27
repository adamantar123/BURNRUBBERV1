using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using UnityEditorInternal;

namespace GercStudio.DragRacingFramework
{
	[InitializeOnLoad]
	[CustomEditor(typeof(ReadmeScript))]
	public class ReadMeScriptEditor : Editor
	{
		static string kShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";

		private static string key = "DRF1.3.1";
		static float kSpace = 16f;
		private static bool firstStart;

		private ReorderableList items;
		private ReadmeScript script;


		private void Awake()
		{
			script = (ReadmeScript) target;
		}


		static ReadMeScriptEditor()
		{
			EditorApplication.update += SelectReadmeAutomatically;
		}

		private void OnEnable()
		{
			items = new ReorderableList(serializedObject, serializedObject.FindProperty("sections"), true, false, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Items"); },

				onAddCallback = items => { script.sections.Add(null); },

				onRemoveCallback = items => { script.sections.Remove(script.sections[items.index]); }
			};
		}

		static void SelectReadmeAutomatically()
		{
			if (!SessionState.GetBool(kShowedReadmeSessionStateName, false) && !PlayerPrefs.HasKey(key))
			{
				var readme = SelectReadme();
				SessionState.SetBool(kShowedReadmeSessionStateName, true);

				PlayerPrefs.SetInt(key, 1);
				PlayerPrefs.Save();

				if (readme && !readme.loadedLayout)
				{
					LoadLayout();
					readme.loadedLayout = true;
				}
			}

		}

		static void LoadLayout()
		{
			// var assembly = typeof(EditorApplication).Assembly;
			// var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
			// var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
			// method?.Invoke(null, new object[] {Path.Combine(Application.dataPath, "Drag Racing Framework/Framework/Tools/Editor/Layout.wlt"), false});
			
			var inspectorWindow = EditorWindow.GetWindow( typeof( Editor ).Assembly.GetType( "UnityEditor.InspectorWindow" ) );
			inspectorWindow.Focus();
		}
		
		static ReadmeScript SelectReadme()
		{
			var ids = AssetDatabase.FindAssets("!Welcome t:ReadmeScript");
			if (ids.Length == 1)
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

				Selection.objects = new[] {readmeObject};

				return (ReadmeScript) readmeObject;
			}
			else
			{
				Debug.Log("Couldn't find a readme");
				return null;
			}
		}

		protected override void OnHeaderGUI()
		{
			var readme = (ReadmeScript) target;

			Init();

			var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

			GUILayout.BeginHorizontal("In BigTitle");
			{
				GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
				GUILayout.Label(readme.title, TitleStyle);
			}
			GUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI()
		{
			var readme = (ReadmeScript) target;
			Init();

			foreach (var section in readme.sections)
			{
				if (!string.IsNullOrEmpty(section.heading))
				{
					GUILayout.Label("<b>" + section.heading + "</b>", HeadingStyle);
				}

				if (!string.IsNullOrEmpty(section.text))
				{
					GUILayout.Label(section.text, BodyStyle);
				}

				if (!string.IsNullOrEmpty(section.linkText))
				{
					if (LinkLabel(new GUIContent(section.linkText)))
					{
						Application.OpenURL(section.url);
					}
				}

				GUILayout.Space(kSpace);
			}

			// items.DoLayoutList();

			// DrawDefaultInspector();
		}


		bool m_Initialized;

		GUIStyle LinkStyle
		{
			get { return m_LinkStyle; }
		}

		[SerializeField] GUIStyle m_LinkStyle;

		GUIStyle TitleStyle
		{
			get { return m_TitleStyle; }
		}

		[SerializeField] GUIStyle m_TitleStyle;

		GUIStyle HeadingStyle
		{
			get { return m_HeadingStyle; }
		}

		[SerializeField] GUIStyle m_HeadingStyle;

		GUIStyle BodyStyle
		{
			get { return m_BodyStyle; }
		}

		[SerializeField] GUIStyle m_BodyStyle;

		void Init()
		{
			if (m_Initialized)
				return;
			
			m_BodyStyle = new GUIStyle(EditorStyles.label) {wordWrap = true, fontSize = 14, richText = true};

			m_TitleStyle = new GUIStyle(m_BodyStyle) {fontSize = 26};

			m_HeadingStyle = new GUIStyle(m_BodyStyle) {fontSize = 18, richText = true};

			m_LinkStyle = new GUIStyle(m_BodyStyle) {wordWrap = false, normal = {textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f)}, stretchWidth = false};

			m_Initialized = true;
		}

		bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
		{
			var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

			Handles.BeginGUI();
			Handles.color = LinkStyle.normal.textColor;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();

			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

			return GUI.Button(position, label, LinkStyle);
		}
	}
}
