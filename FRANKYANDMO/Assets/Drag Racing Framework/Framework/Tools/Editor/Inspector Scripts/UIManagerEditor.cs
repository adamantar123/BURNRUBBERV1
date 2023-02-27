using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.DragRacingFramework
{
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        public UIManager script;
        private ReorderableList graphicsButtons;
        
        private Texture2D backButton;
        private Texture2D settingsButton;

        private string path;

        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle windowStyle;
        private GUIStyle toolbarStyle;
        private GUIStyle toolbarSmallStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle infoStyle;
        
        private UIHelper.MenuPages targetPage;
        private bool switchMenu;

        private GUISkin customSkin;
        
        private void Awake()
        {
            script = (UIManager) target;
            
            if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
	            UIHelper.OpenItem(script, script.currentMenuPage, ref script.previousMenuPage);
        }

        private void OnEnable()
        {
            
            customSkin = (GUISkin) Resources.Load("EditorSkin");

			buttonStyle = customSkin.GetStyle("Button");
			labelStyle = customSkin.GetStyle("Label");
			toolbarStyle = customSkin.GetStyle("TabSmall");
			toolbarSmallStyle = customSkin.GetStyle("TabSmallest");
			descriptionStyle = customSkin.GetStyle("Description");
			infoStyle = customSkin.GetStyle("Info");
            
			backButton = (Texture2D) Resources.Load("left arrow");
			settingsButton = (Texture2D) Resources.Load("settings");

			descriptionStyle.normal.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
			infoStyle.normal.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;

			labelStyle.normal.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
			labelStyle.hover.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
			labelStyle.active.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;

			if (EditorGUIUtility.isProSkin)
			{
				customSkin.customStyles[1].onNormal.background = Resources.Load("TabSelected") as Texture2D;
				customSkin.customStyles[1].onNormal.textColor = Color.white;

				customSkin.customStyles[2].onNormal.background = Resources.Load("TabSelected") as Texture2D;
				customSkin.customStyles[2].onNormal.textColor = Color.white;
			}
			else
			{
				customSkin.customStyles[1].onNormal.background = Resources.Load("TabSelected2") as Texture2D;
				customSkin.customStyles[1].onNormal.textColor = Color.black;

				customSkin.customStyles[2].onNormal.background = Resources.Load("TabSelected2") as Texture2D;
				customSkin.customStyles[2].onNormal.textColor = Color.black;
			}

			buttonStyle.normal.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
			buttonStyle.hover.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
			buttonStyle.active.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;

			if (EditorGUIUtility.isProSkin)
			{
				customSkin.customStyles[0].onNormal.background = Resources.Load("TabSelected") as Texture2D;
				customSkin.customStyles[0].onNormal.textColor = Color.white;
			}
			else
			{
				customSkin.customStyles[0].onNormal.background = Resources.Load("TabSelected2") as Texture2D;
				customSkin.customStyles[0].onNormal.textColor = Color.black;
			}
            
            graphicsButtons = new ReorderableList(serializedObject, serializedObject.FindProperty("menuUI.settingsMenu.graphicsButtons"), false, true,
                true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Graphic Settings Button");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), "Quality Level");
                },

                onAddCallback = items => { script.menuUI.settingsMenu.graphicsButtons.Add(new UIHelper.SettingsMenu.SettingsButton()); },

                onRemoveCallback = items => { script.menuUI.settingsMenu.graphicsButtons.Remove(script.menuUI.settingsMenu.graphicsButtons[items.index]); },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.menuUI.settingsMenu.graphicsButtons[index].button = (Button)
                        EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), script.menuUI.settingsMenu.graphicsButtons[index].button, typeof(Button), true);

                    script.menuUI.settingsMenu.graphicsButtons[index].qualitySettings = EditorGUI.Popup(
                        new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), script.menuUI.settingsMenu.graphicsButtons[index].qualitySettings, QualitySettings.names);
                }
            };
            
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
	        EditorApplication.update -= Update;
        }


        void DrawBackButton(Rect lastRect, UIHelper.MenuPages menuPage)
		{
			var rect = new Rect(new Vector2(lastRect.x, lastRect.y - 2), new Vector2(30, 30));

			EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
			EditorGUI.LabelField(rect, new GUIContent(backButton));

			if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
			{
				switchMenu = true;
				targetPage = menuPage;
			}
		}

		void DrawSettingsButton(Rect lastRect)
		{
			var rect = new Rect(new Vector2(lastRect.x, lastRect.y - 1), new Vector2(26, 26));

			EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
			EditorGUI.LabelField(rect, new GUIContent(settingsButton));

			if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
			{
				switchMenu = true;
				targetPage = UIHelper.MenuPages.Settings;
			}
		}

		void DrawPath(Rect lastRect, UIHelper.MenuPages[] items, string[] names)
		{
			if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
			{
				GUILayout.Space(20);

				// var lastItemLenght = labelStyle.CalcSize(new GUIContent(" / " + names[0] + " / ")).x;
				var lastItemPosition = lastRect.x + 40;

				EditorGUI.LabelField(new Rect(new Vector2(lastItemPosition, lastRect.y + 2), new Vector2(10, labelStyle.lineHeight * 2)), "/ ", labelStyle);

				lastItemPosition += 11;

				for (var i = 1; i <= items.Length; i++)
				{
					var lastItemLenght = labelStyle.CalcSize(new GUIContent(names[i - 1] + " / ")).x;

					var rect = new Rect(new Vector2(lastItemPosition, lastRect.y + 10), new Vector2(lastItemLenght, labelStyle.lineHeight * 2));
					DrawPathItem(rect, items[i - 1], names[i - 1], true, i != items.Length);

					lastItemPosition += lastItemLenght + 2;
				}
			}
			else
			{
				labelStyle.fontSize = 16;
				labelStyle.alignment = TextAnchor.MiddleCenter;
				labelStyle.fontStyle = FontStyle.Bold;


				EditorGUILayout.LabelField("- " + names[names.Length - 1] + " -", labelStyle);

				if (script.currentMenuInGame >= script.currentMenusInGame.Count)
					script.currentMenuPage = 0;

				script.currentMenuPage = script.currentMenusInGame[script.currentMenuInGame];
			}
		}

		void DrawPathItem(Rect rect, UIHelper.MenuPages item, string name, bool drawBrake, bool isLink)
		{
			if (UIHelper.LinkLabel(new GUIContent(name), rect, labelStyle.CalcSize(new GUIContent(name)).x, labelStyle, false, isLink, isLink, true))
			{
				script.currentMenuPage = item;
			}

			if (drawBrake)
			{
				var labelLenght = labelStyle.CalcSize(new GUIContent(name)).x + 2;
				EditorGUI.LabelField(new Rect(new Vector2(rect.x + labelLenght, rect.y - 8), new Vector2(labelStyle.CalcSize(new GUIContent(" / ")).x, labelStyle.lineHeight * 2)), " / ", labelStyle);
			}
		}

		void DrawButton(string text, UIHelper.MenuPages menuPage, string textureName)
		{
			if (GUILayout.Button("   " + text, buttonStyle))
				script.currentMenuPage = menuPage;

			var lastRect = GUILayoutUtility.GetLastRect();
			var tex = (Texture2D) Resources.Load(textureName);
			var lenght = buttonStyle.CalcSize(new GUIContent(text)).x;

			EditorGUI.LabelField(new Rect(new Vector2(lastRect.x + lastRect.width / 2 - lenght / 2 - 20, lastRect.y + (lastRect.height - 25) / 2), new Vector2(25, 25)), new GUIContent(tex, ""));
		}

		void DrawDescription(string text)
		{
			if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();

				EditorGUILayout.LabelField(text, descriptionStyle);
				EditorGUILayout.Space();
			}
		}
		
		void AddEmptyMainObject(int hierarchyIndex, GameObject mainObject)
		{
			if (!script.hierarchy[hierarchyIndex] && mainObject)
				script.hierarchy[hierarchyIndex] = mainObject;
		}

		void Update()
		{
			if (!script)
				return;

			if (Application.isPlaying)
			{
				if (script.currentMenusInGame.Count > 1)
					script.currentMenuPage = script.currentMenusInGame[0];
				
				return;
			}

			if (script.currentMenuPage != script.previousMenuPage)
			{
				UIHelper.OpenItem(script, script.currentMenuPage, ref script.previousMenuPage);
			}

			if (script.currentDashboard != script.previousDashboard)
			{
				if (script.previousDashboard <= script.inGameUI.gameUI.dashboards.Count - 1)
				{
					if (script.inGameUI.gameUI.dashboards[script.previousDashboard].mainObject)
						script.inGameUI.gameUI.dashboards[script.previousDashboard].mainObject.SetActive(false);
				}

				if(script.inGameUI.gameUI.dashboards[script.currentDashboard].mainObject)
					script.inGameUI.gameUI.dashboards[script.currentDashboard].mainObject.SetActive(true);

				script.previousDashboard = script.currentDashboard;
			}

			if (script.inGameUI.gameUI.dashboards.Count == 0)
			{
				script.inGameUI.gameUI.dashboards.Add(new UIHelper.Dashboard{name = "Dashboard 1"});
			}
			if (script.currentDashboard > script.inGameUI.gameUI.dashboards.Count - 1)
			{
				script.currentDashboard = script.inGameUI.gameUI.dashboards.Count - 1;
			}
			
			AddEmptyMainObjects();
		}
		

		public override void OnInspectorGUI()
		{
			if (!script)
				return;
			
			serializedObject.Update();
			
			if (!script.gameObject.activeInHierarchy)
			{
				EditorGUILayout.HelpBox("Open this prefab or add it on a scene to adjust the UI.", MessageType.Info);
				return;
			}

			if (switchMenu)
			{
				if (Event.current.type == EventType.Layout)
				{
					script.currentMenuPage = targetPage;
					switchMenu = false;
				}
			}

			if (script.currentMenuPage == UIHelper.MenuPages.MainMenu)
			{
				labelStyle.fontSize = 17;
				labelStyle.alignment = TextAnchor.MiddleCenter;
				labelStyle.fontStyle = FontStyle.Bold;

				buttonStyle.fontSize = 17;
				buttonStyle.fontStyle = FontStyle.Bold;
				buttonStyle.margin.top = 7;
				buttonStyle.margin.bottom = 7;
			}
			else
			{
				labelStyle.fontSize = 15;
				labelStyle.alignment = TextAnchor.MiddleLeft;
				labelStyle.fontStyle = FontStyle.Bold;

				buttonStyle.fontStyle = FontStyle.Bold;
				buttonStyle.fontSize = 15;
				buttonStyle.margin.top = 4;
				buttonStyle.margin.bottom = 4;
			}
			
			EditorGUILayout.Space();

			if (script.currentMenuPage == UIHelper.MenuPages.MainMenu)
			{
				labelStyle.fontSize = 15;
				GUILayout.Label("Use this component to manage all in-game UI", labelStyle);
			}

			EditorGUILayout.Space();

			var lastRect = GUILayoutUtility.GetLastRect();
			
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20);
				EditorGUILayout.BeginVertical();

				// path
				switch (script.currentMenuPage)
				{
					case UIHelper.MenuPages.MainMenu:
						break;
					case UIHelper.MenuPages.Settings:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Settings}, new[] {"Main Menu", "Settings"});
						break;
					case UIHelper.MenuPages.Lobby:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Lobby}, new[] {"Main Menu", "Lobby"});
						break;
					case UIHelper.MenuPages.LobbyMainMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Lobby, UIHelper.MenuPages.LobbyMainMenu}, new[] {"Main Menu", "Lobby", "Main"});
						break;
					case UIHelper.MenuPages.ShopMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Lobby, UIHelper.MenuPages.ShopMenu}, new[] {"Main Menu", "Lobby", "Shop"});
						break;
					case UIHelper.MenuPages.UpgradesMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Lobby, UIHelper.MenuPages.UpgradesMenu}, new[] {"Main Menu", "Lobby", "Upgrades"});
						break;
					case UIHelper.MenuPages.ProfileMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Lobby, UIHelper.MenuPages.ProfileMenu}, new[] {"Main Menu", "Lobby", "Profile"});
						break;
					case UIHelper.MenuPages.Avatars:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Lobby, UIHelper.MenuPages.Avatars}, new[] {"Main Menu", "Lobby", "Avatars"});
						break;
					case UIHelper.MenuPages.GameOptionsMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.GameOptionsMenu}, new[] {"Main Menu", "Options"});
						break;
					case UIHelper.MenuPages.Game:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Game}, new[] {"Main Menu", "Game"});
						break;
					case UIHelper.MenuPages.LoadingGameMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Game, UIHelper.MenuPages.LoadingGameMenu}, new[] {"Main Menu", "Game", "Loading Screen"});
						break;
					case UIHelper.MenuPages.RaceUI:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Game, UIHelper.MenuPages.RaceUI}, new[] {"Main Menu", "Game", "Race UI"});
						break;
					case UIHelper.MenuPages.PauseMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Game, UIHelper.MenuPages.PauseMenu}, new[] {"Main Menu", "Game", "Pause"});
						break;
					case UIHelper.MenuPages.GameOverMenu:
						DrawPath(lastRect, new[] {UIHelper.MenuPages.MainMenu, UIHelper.MenuPages.Game, UIHelper.MenuPages.GameOverMenu}, new[] {"Main Menu", "Game", "Game Over"});
						break;
				}


				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				lastRect = GUILayoutUtility.GetLastRect();

				if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
				{

					// back buttons
					switch (script.currentMenuPage)
					{
						case UIHelper.MenuPages.Settings:
						case UIHelper.MenuPages.Lobby:
						case UIHelper.MenuPages.Game:
						case UIHelper.MenuPages.GameOptionsMenu:
							DrawBackButton(lastRect, UIHelper.MenuPages.MainMenu);
							break;

						case UIHelper.MenuPages.PauseMenu:
						case UIHelper.MenuPages.RaceUI:
						case UIHelper.MenuPages.GameOverMenu:
						case UIHelper.MenuPages.LoadingGameMenu:
							DrawBackButton(lastRect, UIHelper.MenuPages.Game);
							break;

						case UIHelper.MenuPages.ProfileMenu:
						case UIHelper.MenuPages.Avatars:
						case UIHelper.MenuPages.ShopMenu:
						case UIHelper.MenuPages.UpgradesMenu:
						case UIHelper.MenuPages.LobbyMainMenu:
							DrawBackButton(lastRect, UIHelper.MenuPages.Lobby);
							break;
					}


					if (script.currentMenuPage != UIHelper.MenuPages.MainMenu)
					{
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					}

					switch (script.currentMenuPage)
					{
						case UIHelper.MenuPages.MainMenu:
							DrawButton("Lobby", UIHelper.MenuPages.Lobby, "garage");
							DrawButton("Game", UIHelper.MenuPages.Game, "race");
							EditorGUILayout.Space();
							DrawButton("Game Options", UIHelper.MenuPages.GameOptionsMenu, "options");
							break;

						case UIHelper.MenuPages.Lobby:

							if (GUILayout.Button("Main", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.LobbyMainMenu;

							if (GUILayout.Button("Shop", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.ShopMenu;

							if (GUILayout.Button("Upgrades", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.UpgradesMenu;

							if (GUILayout.Button("Profile", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.ProfileMenu;

							if (GUILayout.Button("Avatars", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.Avatars;

							break;

						case UIHelper.MenuPages.Game:

							if (GUILayout.Button("Loading Screen", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.LoadingGameMenu;

							if (GUILayout.Button("Race UI", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.RaceUI;

							if (GUILayout.Button("Pause", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.PauseMenu;


							if (GUILayout.Button("Game Over", buttonStyle))
								script.currentMenuPage = UIHelper.MenuPages.GameOverMenu;

							break;
					}
				}


				// body
			switch (script.currentMenuPage)
			{
				case UIHelper.MenuPages.Settings:

					// EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("gridTexture"), new GUIContent("Grid Image"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("gridOpacity"), new GUIContent("Grid Opacity"));

					break;

				case UIHelper.MenuPages.LobbyMainMenu:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.startGameButton"), new GUIContent("Start Game"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.selectCarButton"), new GUIContent("Select Car"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.upgradeButton"), new GUIContent("Upgrade Car"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.profileButton"), new GUIContent("Profile"));
					
					EditorGUILayout.Space();

					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.settingsButton"), new GUIContent("Settings"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.exitButton"), new GUIContent("Exit"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.connectionStatus"), new GUIContent("Connection Status"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.mainMenu.regionsDropdown"), new GUIContent("Regions"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.ShopMenu:

					EditorGUILayout.BeginVertical("helpbox");

					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.nameText"), new GUIContent("Car Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.carStatus"), new GUIContent("Car Status"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Car Parameters", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.maxSpeedText"), new GUIContent("Max Speed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.accelerationText"), new GUIContent("Acceleration"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.powerText"), new GUIContent("Power"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.nitroTimeText"), new GUIContent("Nitro Time"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.massText"), new GUIContent("Mass"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.previousCar"), new GUIContent("Previous Car"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.nextCar"), new GUIContent("Next Car"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.exitButton"), new GUIContent("Back"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.selectCarButton"), new GUIContent("Select/Buy Car"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.selectCarMenu.selectButtonText"), new GUIContent("Button Text"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.UpgradesMenu:

					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.nameText"), new GUIContent("Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.priceAndLevelText"), new GUIContent("Required Price & Level"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.engineButton"), new GUIContent("Engine"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.turboButton"), new GUIContent("Turbo"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.transmissionButton"), new GUIContent("Transmission"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.nitroButton"), new GUIContent("Nitro"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.weightButton"), new GUIContent("Weight"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.backButton"), new GUIContent("Back"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.buyButton"), new GUIContent("Buy & Install"));
					// EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.buyButtonText"), new GUIContent("Text"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("Car Parameters", EditorStyles.boldLabel);
					// EditorGUILayout.BeginVertical("helpbox");
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.maxSpeedText"), new GUIContent("Max Speed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.maxSpeedAdditionalText"), new GUIContent("Additional°", "The 'Additional' text will display upgrade values (+10, -5, etc)."));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.accelerationText"), new GUIContent("Acceleration"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.accelerationAdditionalText"), new GUIContent("Additional°", "The 'Additional' text will display upgrade values (+10, -5, etc)."));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.powerText"), new GUIContent("Power"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.powerAdditionalText"), new GUIContent("Additional°", "The 'Additional' text will display upgrade values (+10, -5, etc)."));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.nitroTimeText"), new GUIContent("Nitro Time"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.nitroTimeAdditionalText"), new GUIContent("Additional°", "The 'Additional' text will display upgrade values (+10, -5, etc)."));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.massText"), new GUIContent("Weight"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.upgradeMenu.massAdditionalText"), new GUIContent("Additional°", "The 'Additional' text will display upgrade values (+10, -5, etc)."));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.EndVertical();
// EditorGUILayout.EndVertical();

					break;

				case UIHelper.MenuPages.ProfileMenu:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.currentMoney"), new GUIContent("Current Money"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.currentScore"), new GUIContent("Current Score"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.currentLevel"), new GUIContent("Current Level"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.nextLevel"), new GUIContent("Next Level"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.backButton"), new GUIContent("Back"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.changeAvatar"), new GUIContent("Change Avatar"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.currentLevelFill"), new GUIContent("Current Level Fill"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.profileMenu.nickname"), new GUIContent("Nickname"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.Avatars:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.avatarsMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.avatarsMenu.avatarPlaceholder"), new GUIContent("Avatar Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.avatarsMenu.avatarSelectionIndicator"), new GUIContent("Selection Indicator"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.avatarsMenu.scrollRect"), new GUIContent("Scroll Rect"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.avatarsMenu.backButton"), new GUIContent("Back"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.GameOptionsMenu:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.backButton"), new GUIContent("Back"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.resetButton"), new GUIContent("Reset Game Data"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.LabelField("Screen Resolution", EditorStyles.boldLabel);
					EditorGUILayout.HelpBox("At the game start, all the screen resolutions will appear in the Scroll Rect", MessageType.Info);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.resolutionButtonPlaceholder.button"), new GUIContent("Button Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.resolutionsScrollRect"), new GUIContent("Scroll Rect"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.fullscreenMode.button"), new GUIContent("Full Screen Mode"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUI.settingsMenu.windowedMode.button"), new GUIContent("Windowed Mode"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					graphicsButtons.DoLayoutList();
					
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.LoadingGameMenu:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.preGameTimer.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.preGameTimer.status"), new GUIContent("Status"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.preGameTimer.timer"), new GUIContent("Timer"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.preGameTimer.exitButton"), new GUIContent("Exit Button"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.RaceUI:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.countdownText"), new GUIContent("Countdown"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.gamePopUp"), new GUIContent("Game Pop-up"));
					
					EditorGUILayout.Space();
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.pauseButton"), new GUIContent("Pause Button"));

					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("Placeholders", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.roadPlaceholder"), new GUIContent("Road"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.playerCarPlaceholder"), new GUIContent("Player's car"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.opponentCarPlaceholder"), new GUIContent("Opponent's car"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					//dashboards selector

					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
					{
						EditorGUILayout.LabelField(new GUIContent("Dashboards°", "You can create several dashboards and then select one of them in the 'VehicleController' script."), EditorStyles.boldLabel);
						BeginGreenHelpBox();

						var names = script.inGameUI.gameUI.dashboards.Select(dashboard => dashboard.name).ToList();
						script.currentDashboard = EditorGUILayout.Popup("", script.currentDashboard, names.ToArray());

						EditorGUILayout.BeginHorizontal();

						EditorGUI.BeginDisabledGroup(script.rename);
						if (GUILayout.Button("Rename"))
						{
							script.rename = true;
							script.curName = "";
						}

						EditorGUI.EndDisabledGroup();
						EditorGUI.BeginDisabledGroup(script.inGameUI.gameUI.dashboards.Count <= 1 || script.delete);

						if (GUILayout.Button("Delete"))
						{
							script.delete = true;
						}


						EditorGUI.EndDisabledGroup();


						if (GUILayout.Button("Create a new one"))
						{

							if (!script.inGameUI.gameUI.dashboards.Exists(dashboard => dashboard.name == "Dashboard " + (script.inGameUI.gameUI.dashboards.Count + 1)))
							{
								script.inGameUI.gameUI.dashboards.Add(new UIHelper.Dashboard {name = "Dashboard " + (script.inGameUI.gameUI.dashboards.Count + 1)});
							}
							else
							{
								script.inGameUI.gameUI.dashboards.Add(new UIHelper.Dashboard {name = "Dashboard " + Random.Range(10, 100)});
							}

							script.currentDashboard = script.inGameUI.gameUI.dashboards.Count - 1;

							EditorUtility.SetDirty(script);

							return;
						}

						EditorGUILayout.EndHorizontal();

						if (script.delete)
						{
							BeginBlueHelpbox();
							EditorGUILayout.LabelField("Are you sure?");
							EditorGUILayout.BeginHorizontal();


							if (GUILayout.Button("No"))
							{
								script.delete = false;
							}

							if (GUILayout.Button("Yes"))
							{
								script.inGameUI.gameUI.dashboards.Remove(script.inGameUI.gameUI.dashboards[script.currentDashboard]);
								script.currentDashboard = script.inGameUI.gameUI.dashboards.Count - 1;
								script.delete = false;

								return;
							}

							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
						else if (script.rename)
						{
							BeginBlueHelpbox();
							script.curName = EditorGUILayout.TextField("New name", script.curName);

							EditorGUILayout.BeginHorizontal();

							if (GUILayout.Button("Cancel"))
							{
								script.rename = false;
								script.curName = "";
								script.renameError = false;
							}

							if (GUILayout.Button("Save"))
							{
								if (!script.inGameUI.gameUI.dashboards.Exists(dashboard => dashboard.name == script.curName))
								{
									script.rename = false;
									script.inGameUI.gameUI.dashboards[script.currentDashboard].name = script.curName;
									script.curName = "";
									script.renameError = false;
								}
								else
								{
									script.renameError = true;
								}
							}

							EditorGUILayout.EndHorizontal();

							if (script.renameError)
								EditorGUILayout.HelpBox("This name already exist.", MessageType.Warning);

							EditorGUILayout.EndVertical();
						}

						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();

					}
					else
					{
						EditorGUILayout.LabelField(new GUIContent("Dashboard"), EditorStyles.boldLabel);
					}

					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.Space();
					// EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("currentGearText"), new GUIContent("Current Gear"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("nextGearButton"), new GUIContent("Next Gear Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("previousGearButton"), new GUIContent("Previous Gear Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("useNitroButton"), new GUIContent("Use Nitro Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("gasButton"), new GUIContent("Gas Pedal Button"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					
					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
						EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("speedometerArrow"), new GUIContent("Speedometer Arrow"));

					var backgroundColor = GUI.backgroundColor;
					
					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
					{
						EditorGUILayout.Space();

						EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("minSpeed"), new GUIContent("Min Speed"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("maxSpeed"), new GUIContent("Max Speed"));

						EditorGUILayout.Space();
						EditorGUILayout.Space();

						GUI.backgroundColor = new Color(0, 0.6f, 0.9f, 0.3f);
						script.setSpeedometerArrowRange = GUILayout.Toggle(script.setSpeedometerArrowRange, new GUIContent("Set Range°", "These limits are needed so that your arrow moves within the values on the speedometer"), EditorStyles.miniButton);
						GUI.backgroundColor = backgroundColor;

						if (script.setSpeedometerArrowRange)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("speedometerLimits"), new GUIContent("-180 to 180"));
						}
					}
					
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
						EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("nitrometerArrow"), new GUIContent("Nitrometer Arrow"));

					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
					{
						EditorGUILayout.Space();

						backgroundColor = GUI.backgroundColor;
						GUI.backgroundColor = new Color(0, 0.6f, 0.9f, 0.3f);
						script.setNitrometerArrowRange = GUILayout.Toggle(script.setNitrometerArrowRange, new GUIContent("Set Range°", "These limits are needed so that your arrow moves within the values on the nitrometer"), EditorStyles.miniButton);
						GUI.backgroundColor = backgroundColor;

						if (script.setNitrometerArrowRange)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("nitrometerLimits"), new GUIContent("-180 to 180"));
						}
					}

					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
						EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("tachometerArrow"), new GUIContent("Tachometer Arrow"));

					if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI MANAGER")
					{
						EditorGUILayout.Space();

						backgroundColor = GUI.backgroundColor;
						GUI.backgroundColor = new Color(0, 0.6f, 0.9f, 0.3f);
						script.setTachometerArrowRange = GUILayout.Toggle(script.setTachometerArrowRange, new GUIContent("Set Range°", "These limits are needed so that your arrow moves within the values on the tachometer"), EditorStyles.miniButton);
						GUI.backgroundColor = backgroundColor;

						if (script.setTachometerArrowRange)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("tachometerLimits"), new GUIContent("-180 to 180"));
						}


						EditorGUILayout.Space();

						EditorGUILayout.BeginHorizontal();

						backgroundColor = GUI.backgroundColor;
						GUI.backgroundColor = new Color(1, 0.8f, 0, 0.3f);
						script.setPerfectStartRange = GUILayout.Toggle(script.setPerfectStartRange, new GUIContent("Set Perfect Start Range°", "If at the start the arrow is within these limits, there will be a perfect start"), EditorStyles.miniButton);

						GUI.backgroundColor = new Color(1, 0.1f, 1, 0.3f);
						script.setPerfectShiftRange = GUILayout.Toggle(script.setPerfectShiftRange, new GUIContent("Set Perfect Shift Range°", "If, when changing gear, the arrow is within these limits, there will be a perfect shift"), EditorStyles.miniButton);
						GUI.backgroundColor = backgroundColor;

						EditorGUILayout.EndHorizontal();
						if (script.setPerfectStartRange)
						{
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("perfectStartRange"), new GUIContent("Start"));
						}

						if (script.setPerfectShiftRange)
						{
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameUI.dashboards").GetArrayElementAtIndex(script.currentDashboard).FindPropertyRelative("perfectShiftRange"), new GUIContent("Shifts"));
						}
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

				case UIHelper.MenuPages.PauseMenu:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.pauseMenu.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.pauseMenu.exitButton"), new GUIContent("Exit"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.pauseMenu.resumeButton"), new GUIContent("Resume"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.pauseMenu.optionsButton"), new GUIContent("Options"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;


				case UIHelper.MenuPages.GameOverMenu:
					EditorGUILayout.BeginVertical("helpbox");
					BeginGreenHelpBox();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.mainObject"), new GUIContent("Main Object"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.result"), new GUIContent("Result"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.exit"), new GUIContent("Exit"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.restart"), new GUIContent("Restart"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.currentPlayerHighlight"), new GUIContent("Player Stats Color°", "This color will highlight the player's statistics"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("1st Place", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.firstPlayerStats"), new GUIContent("Stats"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.firstPlayerAvatarPlaceholder"), new GUIContent("Avatar Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.firstPlayerBackground"), new GUIContent("Background"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("2nd Place", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.secondPlayerStats"), new GUIContent("Stats"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.secondPlayerAvatarPlaceholder"), new GUIContent("Avatar Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.secondPlayerBackground"), new GUIContent("Background"));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("Race Profit", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.raceProfit"), new GUIContent("Race Profit"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.perfectStart"), new GUIContent("Perfect Start"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.perfectShifts"), new GUIContent("Perfect Shifts"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.distanceBonus"), new GUIContent("Distance Bonus"));
					EditorGUILayout.Space();

					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.generalRaceProfit"), new GUIContent("General Profit"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("inGameUI.gameOver.newLevelPopup"), new GUIContent("New Level pop-up"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					break;

			}
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			Repaint();

			serializedObject.ApplyModifiedProperties();


            // DrawDefaultInspector();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);

                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
		}
		
		void BeginGreenHelpBox()
		{
			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			EditorGUILayout.BeginVertical("helpbox");
			GUI.backgroundColor = backgroundColor;
		}
		
		void BeginBlueHelpbox()
		{
			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
			EditorGUILayout.BeginVertical("helpbox");
			GUI.backgroundColor = backgroundColor;
		}
		
		void AddEmptyMainObjects()
		{
			AddEmptyMainObject(1, script.menuUI.mainMenu.mainObject);
			AddEmptyMainObject(2, script.menuUI.selectCarMenu.mainObject);
			AddEmptyMainObject(3, script.menuUI.upgradeMenu.mainObject);
			AddEmptyMainObject(4, script.menuUI.profileMenu.mainObject);
			AddEmptyMainObject(5, script.menuUI.avatarsMenu.mainObject);
			AddEmptyMainObject(7, script.inGameUI.preGameTimer.mainObject);
			AddEmptyMainObject(8, script.inGameUI.gameUI.mainObject);
			AddEmptyMainObject(9, script.inGameUI.pauseMenu.mainObject);
			AddEmptyMainObject(10, script.inGameUI.gameOver.mainObject);
			AddEmptyMainObject(11, script.menuUI.settingsMenu.mainObject);
		}
    }
}
