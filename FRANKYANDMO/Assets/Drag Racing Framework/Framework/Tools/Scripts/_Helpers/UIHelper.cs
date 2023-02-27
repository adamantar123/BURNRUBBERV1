using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GD.MinMaxSlider;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GercStudio.DragRacingFramework
{
	public static class UIHelper
	{
		public enum MenuPages
		{
			MainMenu,
			Settings,
			
			Lobby,
			
			LobbyMainMenu,
			GameOptionsMenu,
			ShopMenu,
			UpgradesMenu,
			ProfileMenu,
			Avatars,
			
			Game,
			
			LoadingGameMenu, 
			RaceUI,
			PauseMenu, 
			GameOverMenu
		}

		[Serializable]
		public class Dashboard
		{
			public string name;

			public GameObject mainObject;
			
			public RawImage nitrometerArrow;
			public RawImage speedometerArrow;
			public RawImage tachometerArrow;
			
			public Text currentGearText;
			
			public Button nextGearButton;
			public Button previousGearButton;
			public Button useNitroButton;
			public Button gasButton;
			
			[MinMaxSlider(-180, 180)] public Vector2 perfectStartRange = new Vector2(-50, 50);
			[MinMaxSlider(-180, 180)] public Vector2 perfectShiftRange = new Vector2(-50, 50);

			[MinMaxSlider(-180, 180)] public Vector2 tachometerLimits = new Vector2(-100, 100);

			[MinMaxSlider(-180, 180)] public Vector2 speedometerLimits = new Vector2(-100, 100);

			[MinMaxSlider(-180, 180)] public Vector2 nitrometerLimits = new Vector2(-100, 100);
			
			public float maxSpeed = 280;
			public float minSpeed = 20;

			public void DisableAll()
			{
				if(mainObject)
					mainObject.SetActive(false);
				
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}

			public void ActivateAll()
			{
				if(mainObject)
					EnableAllParents(mainObject);
				
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
				}
			}
		}
		
		[Serializable]
		public class GameUI
		{
			public GameObject mainObject;
			
			public RawImage roadPlaceholder;
			public RawImage playerCarPlaceholder;
			public RawImage opponentCarPlaceholder;
			
			public Text countdownText;
			public Text gamePopUp;
			
			public Button pauseButton;
			
			public List<Dashboard> dashboards = new List<Dashboard>{new Dashboard{name = "Dashboard 1"}};

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}

				if (mainObject)
					mainObject.SetActive(false);

				foreach (var dashboard in dashboards)
				{
					dashboard.DisableAll();
				}
			}

			public void ActivateAll(int selectedDashboard)
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
				}
				
				if(countdownText)
					EnableAllParents(countdownText.gameObject);

				if (mainObject)
					mainObject.SetActive(true);

				if (dashboards.Count > 0 && dashboards[selectedDashboard] != null)
				{
					dashboards[selectedDashboard].ActivateAll();
				}
			}
		}

		[Serializable]
		public class GameOver
		{
			public GameObject mainObject;

			public Text result;
			public Text firstPlayerStats;
			public Text secondPlayerStats;
			public Text generalRaceProfit;
			public Text newLevelPopup;

			public Text raceProfit;
			public Text perfectStart;
			public Text perfectShifts;
			public Text distanceBonus;

			public RawImage firstPlayerBackground;
			public RawImage secondPlayerBackground;

			public RawImage firstPlayerAvatarPlaceholder;
			public RawImage secondPlayerAvatarPlaceholder;

			public Color currentPlayerHighlight;

			public Button exit;
			public Button restart;

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) EnableAllParents(go);
					}
				}
			}

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.SetActive(false);
					}
				}
			}

		}

		[Serializable]
		public class Pause
		{
			public GameObject mainObject;

			public Button exitButton;
			public Button resumeButton;
			public Button optionsButton;

			public void DisableAll()
			{
				if (mainObject)
					mainObject.SetActive(false);

				if (exitButton)
					exitButton.gameObject.SetActive(false);

				if (resumeButton)
					resumeButton.gameObject.SetActive(false);
				
				if (optionsButton)
					optionsButton.gameObject.SetActive(false);

			}

			public void ActivateAll()
			{
				if (mainObject)
					EnableAllParents(mainObject);

				if (exitButton)
					EnableAllParents(exitButton.gameObject);

				if (resumeButton)
					EnableAllParents(resumeButton.gameObject);
				
				if (optionsButton)
					EnableAllParents(optionsButton.gameObject);
			}

		}

		[Serializable]
		public class PreGameTimer
		{
			public GameObject mainObject;
			public Text status;
			public Text timer;
			public Button exitButton;

			public void DisableAll()
			{
				if (mainObject)
					mainObject.SetActive(false);

				if (status)
					status.gameObject.SetActive(false);

				if (timer)
					timer.gameObject.SetActive(false);

				if (exitButton)
					exitButton.gameObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if (mainObject)
					EnableAllParents(mainObject);

				if (status)
					EnableAllParents(status.gameObject);

				if (timer)
					EnableAllParents(timer.gameObject);

				if (exitButton)
					EnableAllParents(exitButton.gameObject);
			}

		}

		[Serializable]
		public class SelectCarMenu
		{
			public GameObject mainObject;

			public Button previousCar;
			public Button nextCar;
			public Button exitButton;
			public Button selectCarButton;

			public Text carStatus;
			public Text nameText;
			public Text selectButtonText;

			public Text maxSpeedText;
			public Text accelerationText;
			public Text powerText;
			public Text nitroTimeText;
			public Text massText;

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}

				if (mainObject)
					mainObject.SetActive(false);
			}

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
				}

				if (mainObject)
					mainObject.SetActive(true);
			}
		}

		[Serializable]
		public class ProfileMenu
		{
			public GameObject mainObject;

			public Text currentMoney;
			public Text currentScore;
			public Text currentLevel;
			public Text nextLevel;

			public Image currentLevelFill;

			public InputField nickname;

			public Button backButton;
			public Button changeAvatar;

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
				}

				if (mainObject)
					EnableAllParents(mainObject);

				if (currentLevelFill)
					EnableAllParents(currentLevelFill.gameObject);

				if (nickname)
					EnableAllParents(nickname.gameObject);
			}

			public void DisableAll()
			{
				if (mainObject)
					mainObject.SetActive(false);

				if (currentLevelFill)
					currentLevelFill.gameObject.SetActive(false);

				if (nickname)
					nickname.gameObject.SetActive(false);

				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}
		}

		[Serializable]
		public class SettingsMenu
		{
			public GameObject mainObject;

			public Button backButton;
			public Button resetButton;
			public SettingsButton fullscreenMode;
			public SettingsButton windowedMode;
			
			public Button currentSelectedSettingsButton;

			public List<SettingsButton> resolutionButtons = new List<SettingsButton>();
			public List<SettingsButton> graphicsButtons = new List<SettingsButton>();
			
			public SettingsButton resolutionButtonPlaceholder;
			public ScrollRect resolutionsScrollRect;
			
			public bool firstTimeMenuOpened;

			[Serializable]
			public class SettingsButton
			{
				public Button button;
				public int indexNumber;

				public Resolution resolution;
				public int qualitySettings;
				public int frameRate;

				public Text textPlaceholder;

				public Color normColor;
				public Sprite normSprite;
			}

			public List<Button> GetAllOptionButtons()
			{

				var buttons = (from button in graphicsButtons where button.button select button.button).ToList();
				buttons.AddRange(from button in resolutionButtons where button.button select button.button);

				if (fullscreenMode.button)
					buttons.Add(fullscreenMode.button);

				if (windowedMode.button)
					buttons.Add(windowedMode.button);

				return buttons;
			}

			public void ActivateAll()
			{
				foreach (var button in graphicsButtons.Where(button => button.button))
				{
					EnableAllParents(button.button.gameObject);
				}

				foreach (var button in resolutionButtons.Where(button => button.button))
				{
					EnableAllParents(button.button.gameObject);
				}

				if (backButton)
					EnableAllParents(backButton.gameObject);

				if (mainObject)
					EnableAllParents(mainObject);

				if (fullscreenMode.button)
					EnableAllParents(fullscreenMode.button.gameObject);

				if (windowedMode.button)
					EnableAllParents(windowedMode.button.gameObject);	
				
				if (resetButton)
					EnableAllParents(resetButton.gameObject);
			}

			public void DisableAll()
			{
				foreach (var button in graphicsButtons.Where(button => button.button))
				{
					button.button.gameObject.SetActive(false);
				}

				foreach (var button in resolutionButtons.Where(button => button.button))
				{
					button.button.gameObject.SetActive(false);
				}

				if (backButton)
					backButton.gameObject.SetActive(false);

				if (fullscreenMode.button)
					fullscreenMode.button.gameObject.SetActive(false);

				if (windowedMode.button)
					windowedMode.button.gameObject.SetActive(false);

				if (mainObject)
					mainObject.SetActive(false);
				
				if(resetButton)
					resetButton.gameObject.SetActive(false);
			}
		}

		public static List<Resolution> GetResolutions(out List<string> stringResolutions)
		{
			var resolutions = Screen.resolutions;
			var isWindowed = Screen.fullScreenMode == FullScreenMode.Windowed;
			var isFullScreenWindow = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
			var systemWidth = Display.main.systemWidth;
			var systemHeight = Display.main.systemHeight;

			stringResolutions = new List<string>();

			var finalResolutions = new List<Resolution>();

			foreach (var res in resolutions)
			{
				var resParts = res.ToString().Split(new char[] {'x', '@', 'H'});
				var width = int.Parse(resParts[0].Trim());
				var height = int.Parse(resParts[1].Trim());

				// skip resolutions that won't fit in windowed modes
				if (isWindowed && (width >= systemWidth || height >= systemHeight))
					continue;
				if (isFullScreenWindow && (width > systemWidth || height > systemHeight))
					continue;

				var resString = GetResolutionString(width, height);

				finalResolutions.Add(res);
				stringResolutions.Add(resString);
			}

			return finalResolutions;
		}

		static string GetResolutionString(int w, int h)
		{
			return string.Format("{0}x{1}", w, h);
		}

		public static void SetResolution(Resolution resolution, FullScreenMode mode, int hz)
		{
			Screen.SetResolution(resolution.width, resolution.height, mode, hz);
		}

		public static void SetQuality(int value)
		{
			QualitySettings.SetQualityLevel(value, !Application.isMobilePlatform);

			if (!Application.isMobilePlatform)
				QualitySettings.vSyncCount = 0;
		}

		public static void SetWindowMode(int index, Resolution currentResolution)
		{
			if (index == 0) // full screen mode
			{
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
			}
			else if (index == 1) // windowed mode
			{
				Screen.fullScreenMode = FullScreenMode.Windowed;

				var screenWidth = Display.main.systemWidth;
				var screenHeight = Display.main.systemHeight;

				if (currentResolution.width >= screenWidth || currentResolution.height >= screenHeight)
				{
					var closestWidth = screenWidth;
					var closestHeight = screenHeight;
					foreach (var res in Screen.resolutions)
					{
						if (res.width < screenWidth && res.height < screenHeight)
						{
							closestWidth = res.width;
							closestHeight = res.height;
						}
					}

					SetResolution(new Resolution {width = closestWidth, height = closestHeight}, FullScreenMode.Windowed, QualitySettings.vSyncCount);
				}
				else
				{
					SetResolution(currentResolution, FullScreenMode.Windowed, QualitySettings.vSyncCount);
				}
			}
		}

		public static void ResetSettingsButtons(List<SettingsMenu.SettingsButton> buttons, int currentButtonIndex)
		{
			Button currentButton = null;

			for (var i = 0; i < buttons.Count; i++)
			{
				var button = buttons[i];
				if (!button.button) continue;

				ChangeButtonColor(button.button, button.normColor, button.normSprite);

				if (i == currentButtonIndex)
				{
					currentButton = button.button;
				}
			}

			if (buttons.Count > 0 && currentButton != null)
			{
				var color = currentButton.colors.selectedColor;
				var sprite = currentButton.spriteState.selectedSprite;

				ChangeButtonColor(currentButton, color, sprite);
			}
		}

		public static void ManageSettingsButtons(UIManager uiManager)
		{
			var allButtons = new List<SettingsMenu.SettingsButton>();
			allButtons.AddRange(uiManager.menuUI.settingsMenu.graphicsButtons);
			allButtons.AddRange(uiManager.menuUI.settingsMenu.resolutionButtons);
			allButtons.Add(uiManager.menuUI.settingsMenu.fullscreenMode);
			allButtons.Add(uiManager.menuUI.settingsMenu.windowedMode);

			foreach (var button in allButtons)
			{
				if (button.button)
				{
					switch (button.button.transition)
					{
						case Selectable.Transition.ColorTint:
							button.normColor = button.button.colors.normalColor;
							break;
						case Selectable.Transition.SpriteSwap:
							button.normSprite = button.button.GetComponent<Image>().sprite;
							break;
					}
				}
			}

			for (var i = 0; i < uiManager.menuUI.settingsMenu.graphicsButtons.Count; i++)
			{
				var button = uiManager.menuUI.settingsMenu.graphicsButtons[i];

				if (button.button)
				{
					button.indexNumber = i;
					button.button.onClick.AddListener(delegate { uiManager.SetSettingsParameter(uiManager.menuUI.settingsMenu.graphicsButtons, button.indexNumber, "CurrentQualityButton", "quality"); });
				}
			}

			if (!Application.isMobilePlatform)
			{
				for (var i = 0; i < uiManager.menuUI.settingsMenu.resolutionButtons.Count; i++)
				{
					var button = uiManager.menuUI.settingsMenu.resolutionButtons[i];
					if (button.button)
					{
						button.indexNumber = i;
						button.button.onClick.AddListener(delegate { uiManager.SetSettingsParameter(uiManager.menuUI.settingsMenu.resolutionButtons, button.indexNumber, "CurrentResolutionButton", "resolution"); });
					}
				}

				uiManager.menuUI.settingsMenu.fullscreenMode.button.onClick.AddListener(delegate { uiManager.SetWindowMode("CurrentWindowModeButton", 0); });
				uiManager.menuUI.settingsMenu.windowedMode.button.onClick.AddListener(delegate { uiManager.SetWindowMode("CurrentWindowModeButton", 1); });
			}
			else
			{
				foreach (var button in uiManager.menuUI.settingsMenu.resolutionButtons)
				{
					button.button.interactable = false;
				}
				
				uiManager.menuUI.settingsMenu.fullscreenMode.button.interactable = false;
				uiManager.menuUI.settingsMenu.windowedMode.button.interactable = false;
			}
		}

		[Serializable]
		public class UpgradeMenu
		{
			public GameObject mainObject;

			public Button backButton;
			public Button buyButton;

			public Button engineButton;
			public Button turboButton;
			public Button transmissionButton;
			public Button nitroButton;
			public Button weightButton;

			public Text priceAndLevelText;
			public Text nameText;

			public Text maxSpeedText;
			public Text accelerationText;
			public Text powerText;
			public Text nitroTimeText;
			public Text massText;

			public Text maxSpeedAdditionalText;
			public Text accelerationAdditionalText;
			public Text powerAdditionalText;
			public Text nitroTimeAdditionalText;
			public Text massAdditionalText;

			public Color[] normButtonsColors = new Color[5];
			public Sprite[] normButtonsSprites = new Sprite[5];

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
				}

				if (mainObject)
					EnableAllParents(mainObject);
			}

			public void DisableAll()
			{
				if (mainObject)
					mainObject.SetActive(false);

				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}
		}

		[Serializable]
		public class AvatarsMenu
		{
			public GameObject mainObject;

			public Button backButton;

			public RawImage avatarPlaceholder;
			public RawImage avatarSelectionIndicator;

			public ScrollRect scrollRect;

			public void ActivateAll()
			{
				if (mainObject)
					EnableAllParents(mainObject);

				if (backButton)
					EnableAllParents(backButton.gameObject);

				if (avatarPlaceholder)
					EnableAllParents(avatarPlaceholder.gameObject);

				if (scrollRect && scrollRect.content)
					EnableAllParents(scrollRect.content.gameObject);
			}

			public void DisableAll()
			{
				if (mainObject)
					mainObject.SetActive(false);

				if (backButton)
					backButton.gameObject.SetActive(false);

				if (avatarPlaceholder)
					avatarPlaceholder.gameObject.SetActive(false);

				if (scrollRect && scrollRect.content)
					scrollRect.content.gameObject.SetActive(false);
			}
		}

		[Serializable]
		public class MainMenu
		{
			public GameObject mainObject;

			public Button selectCarButton;
			public Button startGameButton;
			public Button profileButton;
			public Button upgradeButton;
			public Button settingsButton;

			public Button exitButton;

			public Text connectionStatus;

			public Dropdown regionsDropdown;

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) EnableAllParents(go.gameObject);
					}
				}

				if (mainObject)
					EnableAllParents(mainObject);

				if (connectionStatus)
					EnableAllParents(connectionStatus.gameObject);

				if (regionsDropdown)
					EnableAllParents(regionsDropdown.gameObject);
			}

			public void DisableAll()
			{
				if (mainObject)
					mainObject.SetActive(false);

				if (connectionStatus)
					connectionStatus.gameObject.SetActive(false);

				if (regionsDropdown)
					regionsDropdown.gameObject.SetActive(false);

				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}
		}

		public static void EnableAllParents(GameObject childObject)
		{
			var t = childObject.transform;
			childObject.SetActive(true);

			while (t.parent != null)
			{
				t.parent.gameObject.SetActive(true);
				t = t.parent;
			}
		}

		public static void setButtonEvent(GameObject button, GameManager gameManager)
		{
			var eventTrigger = button.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};

			entry.callback.AddListener(data => { gameManager.PressGasUIButton("+"); });
			eventTrigger.triggers.Add(entry);

			entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerUp};
			entry.callback.AddListener(data => { gameManager.PressGasUIButton("-"); });

			eventTrigger.triggers.Add(entry);
		}

		public static void AddListeners(UIManager currentUIManager, MenuManager menuManager)
		{
			if (currentUIManager.menuUI.profileMenu.nickname)
				currentUIManager.menuUI.profileMenu.nickname.onValueChanged.AddListener(menuManager.SetName);

			if (currentUIManager.menuUI.avatarsMenu.backButton)
				currentUIManager.menuUI.avatarsMenu.backButton.onClick.AddListener(delegate { menuManager.OpenMenu("profile"); });

			if (currentUIManager.menuUI.selectCarMenu.exitButton)
				currentUIManager.menuUI.selectCarMenu.exitButton.onClick.AddListener(delegate { menuManager.OpenMenu("menu"); });

			if (currentUIManager.menuUI.selectCarMenu.nextCar)
				currentUIManager.menuUI.selectCarMenu.nextCar.onClick.AddListener(delegate { menuManager.ChangeCar("+"); });

			if (currentUIManager.menuUI.selectCarMenu.previousCar)
				currentUIManager.menuUI.selectCarMenu.previousCar.onClick.AddListener(delegate { menuManager.ChangeCar("-"); });

			if (currentUIManager.menuUI.mainMenu.startGameButton)
				currentUIManager.menuUI.mainMenu.startGameButton.onClick.AddListener(menuManager.StartRace);

			if (currentUIManager.menuUI.mainMenu.exitButton)
				currentUIManager.menuUI.mainMenu.exitButton.onClick.AddListener(menuManager.CloseApp);

			if (currentUIManager.menuUI.mainMenu.profileButton)
				currentUIManager.menuUI.mainMenu.profileButton.onClick.AddListener(delegate { menuManager.OpenMenu("profile"); });

			if (currentUIManager.menuUI.mainMenu.settingsButton)
				currentUIManager.menuUI.mainMenu.settingsButton.onClick.AddListener(delegate { menuManager.OpenMenu("settings"); });

			if (currentUIManager.menuUI.mainMenu.upgradeButton)
				currentUIManager.menuUI.mainMenu.upgradeButton.onClick.AddListener(delegate { menuManager.OpenMenu("upgrade"); });

			if (currentUIManager.menuUI.mainMenu.selectCarButton)
				currentUIManager.menuUI.mainMenu.selectCarButton.onClick.AddListener(delegate { menuManager.OpenMenu("cars"); });

			if (currentUIManager.menuUI.profileMenu.backButton)
				currentUIManager.menuUI.profileMenu.backButton.onClick.AddListener(delegate { menuManager.OpenMenu("menu"); });

			if (currentUIManager.menuUI.profileMenu.changeAvatar)
				currentUIManager.menuUI.profileMenu.changeAvatar.onClick.AddListener(delegate { menuManager.OpenMenu("avatars"); });

			if (currentUIManager.menuUI.upgradeMenu.backButton)
				currentUIManager.menuUI.upgradeMenu.backButton.onClick.AddListener(delegate { menuManager.OpenMenu("menu"); });

			if (currentUIManager.menuUI.upgradeMenu.engineButton)
				currentUIManager.menuUI.upgradeMenu.engineButton.onClick.AddListener(delegate { menuManager.ChooseUpgrade("engine", true); });

			if (currentUIManager.menuUI.upgradeMenu.turboButton)
				currentUIManager.menuUI.upgradeMenu.turboButton.onClick.AddListener(delegate { menuManager.ChooseUpgrade("turbo", true); });

			if (currentUIManager.menuUI.upgradeMenu.transmissionButton)
				currentUIManager.menuUI.upgradeMenu.transmissionButton.onClick.AddListener(delegate { menuManager.ChooseUpgrade("transmission", true); });

			if (currentUIManager.menuUI.upgradeMenu.nitroButton)
				currentUIManager.menuUI.upgradeMenu.nitroButton.onClick.AddListener(delegate { menuManager.ChooseUpgrade("nitro", true); });

			if (currentUIManager.menuUI.upgradeMenu.weightButton)
				currentUIManager.menuUI.upgradeMenu.weightButton.onClick.AddListener(delegate { menuManager.ChooseUpgrade("weight", true); });

			if (currentUIManager.menuUI.upgradeMenu.buyButton)
				currentUIManager.menuUI.upgradeMenu.buyButton.onClick.AddListener(delegate { menuManager.BuyUpgrade(); });

			if (currentUIManager.menuUI.settingsMenu.backButton)
				currentUIManager.menuUI.settingsMenu.backButton.onClick.AddListener(delegate { menuManager.OpenMenu("menu"); });

			if (currentUIManager.menuUI.settingsMenu.resetButton)
				currentUIManager.menuUI.settingsMenu.resetButton.onClick.AddListener(menuManager.ResetAllData);
		}

		public static void SetUpgradeValues(UIManager currentUIManager, GameHelper.UpgradeParameter upgradeParameter, CarHelper.CarInfo currentParameters, GameHelper.UpgradesType upgradesType)
		{
			if (currentUIManager.menuUI.upgradeMenu.maxSpeedAdditionalText)
				SetValue(currentUIManager.menuUI.upgradeMenu.maxSpeedAdditionalText, upgradeParameter.addSpeedValue, currentParameters.MaxSpeed, false, upgradesType);

			if (currentUIManager.menuUI.upgradeMenu.accelerationAdditionalText)
				SetValue(currentUIManager.menuUI.upgradeMenu.accelerationAdditionalText, upgradeParameter.addAccelerationValue, currentParameters.Acceleration, false, upgradesType);

			if (currentUIManager.menuUI.upgradeMenu.powerAdditionalText)
				SetValue(currentUIManager.menuUI.upgradeMenu.powerAdditionalText, upgradeParameter.addPowerValue, currentParameters.Power, false, upgradesType);

			if (currentUIManager.menuUI.upgradeMenu.nitroTimeAdditionalText)
				SetValue(currentUIManager.menuUI.upgradeMenu.nitroTimeAdditionalText, upgradeParameter.addNitroValue, currentParameters.nitroTime, false, upgradesType);

			if (currentUIManager.menuUI.upgradeMenu.massAdditionalText)
				SetValue(currentUIManager.menuUI.upgradeMenu.massAdditionalText, upgradeParameter.addMassValue, currentParameters.Mass, true, upgradesType);

			EnableAllAdditionalTexts(currentUIManager);
		}

		static void SetValue(Text text, int upgradeValue, float currentValue, bool inverseColor, GameHelper.UpgradesType upgradesType)
		{
			if (upgradesType == GameHelper.UpgradesType.AddPercent)
			{
				var percent = (float) upgradeValue / 100;
				var addedValue = currentValue * percent;
				upgradeValue = (int) addedValue;
			}

			SetValue(text, upgradeValue, inverseColor);
		}

		static void SetValue(Text text, int value, bool inverseColor)
		{
			if (value > 0)
			{
				text.color = !inverseColor ? Color.green : Color.red;
				text.text = "+" + value;
			}
			else if (value < 0)
			{
				text.color = !inverseColor ? Color.red : Color.green;
				text.text = value + "";
			}
			else
			{
				text.color = Color.white;
				text.text = value + "";
			}
		}

		public static void UpdateCarStats(UIManager uiManager, VehicleController carController)
		{
			if (uiManager.menuUI.selectCarMenu.maxSpeedText)
				uiManager.menuUI.selectCarMenu.maxSpeedText.text = "Max Speed: " + carController.carInfo.MaxSpeed;

			if (uiManager.menuUI.selectCarMenu.accelerationText)
				uiManager.menuUI.selectCarMenu.accelerationText.text = "Acceleration: " + carController.carInfo.Acceleration;

			if (uiManager.menuUI.selectCarMenu.powerText)
				uiManager.menuUI.selectCarMenu.powerText.text = "Power: " + carController.carInfo.Power;

			if (uiManager.menuUI.selectCarMenu.nitroTimeText)
				uiManager.menuUI.selectCarMenu.nitroTimeText.text = "Nitro Time: " + carController.carInfo.nitroTime;

			if (uiManager.menuUI.selectCarMenu.massText)
				uiManager.menuUI.selectCarMenu.massText.text = "Mass: " + carController.carInfo.Mass;
		}

		public static void DisableAllAdditionalTexts(UIManager currentUIManager)
		{
			if (currentUIManager.menuUI.upgradeMenu.maxSpeedAdditionalText)
				currentUIManager.menuUI.upgradeMenu.maxSpeedAdditionalText.gameObject.SetActive(false);

			if (currentUIManager.menuUI.upgradeMenu.accelerationAdditionalText)
				currentUIManager.menuUI.upgradeMenu.accelerationAdditionalText.gameObject.SetActive(false);

			if (currentUIManager.menuUI.upgradeMenu.powerAdditionalText)
				currentUIManager.menuUI.upgradeMenu.powerAdditionalText.gameObject.SetActive(false);

			if (currentUIManager.menuUI.upgradeMenu.nitroTimeAdditionalText)
				currentUIManager.menuUI.upgradeMenu.nitroTimeAdditionalText.gameObject.SetActive(false);

			if (currentUIManager.menuUI.upgradeMenu.massAdditionalText)
				currentUIManager.menuUI.upgradeMenu.massAdditionalText.gameObject.SetActive(false);
		}

		private static void EnableAllAdditionalTexts(UIManager currentUIManager)
		{
			if (currentUIManager.menuUI.upgradeMenu.maxSpeedAdditionalText)
				EnableAllParents(currentUIManager.menuUI.upgradeMenu.maxSpeedAdditionalText.gameObject);

			if (currentUIManager.menuUI.upgradeMenu.accelerationAdditionalText)
				EnableAllParents(currentUIManager.menuUI.upgradeMenu.accelerationAdditionalText.gameObject);

			if (currentUIManager.menuUI.upgradeMenu.powerAdditionalText)
				EnableAllParents(currentUIManager.menuUI.upgradeMenu.powerAdditionalText.gameObject);

			if (currentUIManager.menuUI.upgradeMenu.nitroTimeAdditionalText)
				EnableAllParents(currentUIManager.menuUI.upgradeMenu.nitroTimeAdditionalText.gameObject);

			if (currentUIManager.menuUI.upgradeMenu.massAdditionalText)
				EnableAllParents(currentUIManager.menuUI.upgradeMenu.massAdditionalText.gameObject);

		}

		public static void ChangeButtonColor(Button button, Color color, Sprite sprite)
		{
			switch (button.transition)
			{
				case Selectable.Transition.ColorTint:
					var buttonColors = button.colors;
					buttonColors.normalColor = color;
					button.colors = buttonColors;
					break;
				case Selectable.Transition.SpriteSwap:
					if (sprite)
						button.GetComponent<Image>().sprite = sprite;
					break;
			}
		}

#if UNITY_EDITOR
		public static void InitStyles(ref GUIStyle style, Color32 color)
		{
			if (style == null)
			{
				style = new GUIStyle(EditorStyles.helpBox) {normal = {background = MakeTex(2, 2, color)}};
			}
		}

		private static Texture2D MakeTex(int width, int height, Color col)
		{
			var pix = new Color[width * height];
			for (int i = 0; i < pix.Length; ++i)
			{
				pix[i] = col;
			}

			var result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}
		
		public static Canvas NewCanvas(string name, Transform parent)
		{
			var canvas = new GameObject(name);
			var rect = canvas.AddComponent<RectTransform>();
			rect.sizeDelta = new Vector2(10, 10);
			var script = canvas.AddComponent<Canvas>();
			script.renderMode = RenderMode.WorldSpace;
			canvas.AddComponent<GraphicRaycaster>();
			canvas.transform.SetParent(parent);
			canvas.transform.localPosition = Vector3.zero;
			return script;
		}
		
		static void CanMenuBeOpen(GameObject mainObject, MenuPages menuPage, UIManager script)
		{
			if (mainObject.activeInHierarchy)
			{
				if (!script.currentMenusInGame.Exists(page => page == menuPage))
					script.currentMenusInGame.Add(menuPage);
			}
			else
			{
				if (script.currentMenusInGame.Exists(page => page == menuPage))
					script.currentMenusInGame.Remove(menuPage);
			}
		}
		
		public static void OpenPagesInPlayMode(UIManager script)
		{
			CanMenuBeOpen(script.hierarchy[1], MenuPages.LobbyMainMenu, script);
			CanMenuBeOpen(script.hierarchy[2], MenuPages.ShopMenu, script);
			CanMenuBeOpen(script.hierarchy[3], MenuPages.UpgradesMenu, script);
			CanMenuBeOpen(script.hierarchy[4], MenuPages.ProfileMenu, script);
			CanMenuBeOpen(script.hierarchy[5], MenuPages.Avatars, script);
			CanMenuBeOpen(script.hierarchy[7], MenuPages.LoadingGameMenu, script);
			CanMenuBeOpen(script.hierarchy[8], MenuPages.RaceUI, script);
			CanMenuBeOpen(script.hierarchy[9], MenuPages.PauseMenu, script);
			CanMenuBeOpen(script.hierarchy[10], MenuPages.GameOverMenu, script);
			CanMenuBeOpen(script.hierarchy[11], MenuPages.GameOptionsMenu, script);
			
			if (script.currentMenuPage != script.previousMenuPage)
			{
				OpenItem(script, script.currentMenuPage, ref script.previousMenuPage);
			}
		}
		
		public static bool LinkLabel(GUIContent label, Rect rect, float lenght, GUIStyle style, bool isLabel, bool isActive, bool isLink, bool uiManager, params GUILayoutOption[] options)
		{
			var position = rect;

			if (isLink)
			{
				Handles.BeginGUI();
				Handles.color = style.normal.textColor;
				Handles.DrawLine(new Vector3(position.xMin, position.yMin + style.lineHeight + (isLabel ? 1 : 0)), new Vector3(position.xMin + lenght, position.yMin + style.lineHeight + (isLabel ? 1 : 0)));
				Handles.color = Color.white;
				Handles.EndGUI();
			}

			var _rect = new Rect(position.xMin, !uiManager ? position.yMin : position.yMin - style.lineHeight / 2, lenght, !uiManager ? style.lineHeight : style.lineHeight * 2);
            
			if(isActive)
				EditorGUIUtility.AddCursorRect(_rect, MouseCursor.Link);

			return GUI.Button(_rect, label, style);
		}
		
		public static void OpenItem(UIManager script, MenuPages currentMenuPage, ref MenuPages previousMenuPage)
		{
			previousMenuPage = currentMenuPage;

			script.HideAllHierarchy();

			switch (currentMenuPage)
			{
				case UIHelper.MenuPages.MainMenu:
				case MenuPages.Settings:
					break;
				case UIHelper.MenuPages.Lobby:
					EnableHierarchyItem(new[] {0}, script);
					break;
				
				case UIHelper.MenuPages.LobbyMainMenu:
					EnableHierarchyItem(new[] {0, 1}, script);
					break;
				
				case UIHelper.MenuPages.ShopMenu:
					EnableHierarchyItem(new[] {0, 2}, script);
					break;
				
				case UIHelper.MenuPages.UpgradesMenu:
					EnableHierarchyItem(new[] {0, 3}, script);
					break;
				
				case UIHelper.MenuPages.ProfileMenu:
					EnableHierarchyItem(new[] {0, 4}, script);
					break;
				
				case UIHelper.MenuPages.Avatars:
					EnableHierarchyItem(new[] {0, 5}, script);
					break;
				
				case UIHelper.MenuPages.Game:
					EnableHierarchyItem(new[] {6}, script);
					break;
				
				case UIHelper.MenuPages.LoadingGameMenu:
					EnableHierarchyItem(new[] {6, 7}, script);
					break;
				
				case UIHelper.MenuPages.RaceUI:
					EnableHierarchyItem(new[] {6, 8}, script);
					break;
				
				case UIHelper.MenuPages.PauseMenu:
					EnableHierarchyItem(new[] {6, 9}, script);
					break;
				
				case UIHelper.MenuPages.GameOverMenu:
					EnableHierarchyItem(new[] {6, 10}, script);
					break;
				
				case UIHelper.MenuPages.GameOptionsMenu:
					EnableHierarchyItem(new[] {11}, script);
					break;
			}
		}

		private static void EnableHierarchyItem(int[] pathItems, UIManager script)
		{
			foreach (var index in pathItems)
			{
				if (!script.hierarchy[index]) continue;

				script.hierarchy[index].hideFlags = HideFlags.None;
				script.hierarchy[index].SetActive(true);

				if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI Manager")
					SetExpanded(script.hierarchy[index], true);
			}

			if (!Application.isPlaying || Application.isPlaying && script.gameObject.scene.name == "UI Manager")
				SetExpanded(script.gameObject, true);
		}

		private static void SetExpanded(GameObject go, bool expand)
		{
			var sceneHierarchy = GetSceneHierarchy();
			var methodInfo = sceneHierarchy.GetType().GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

			methodInfo.Invoke(sceneHierarchy, new object[] {go.GetInstanceID(), expand});
		}
		
		private static object GetSceneHierarchy()
		{
			EditorWindow window = GetHierarchyWindow();

			object sceneHierarchy = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow").GetProperty("sceneHierarchy")?.GetValue(window);

			return sceneHierarchy;
		}

		private static EditorWindow GetHierarchyWindow()
		{
			EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
			return EditorWindow.focusedWindow;
		}
#endif
	}
}
