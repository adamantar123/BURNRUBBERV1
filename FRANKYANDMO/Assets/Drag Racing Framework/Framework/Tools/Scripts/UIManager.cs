using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GercStudio.DragRacingFramework
{
	public class UIManager : MonoBehaviour
	{
		public int currentDashboard;
		
		#region Inspector Variables
		
		public int previousDashboard;
		public int currentMenuInGame;
		
		public bool delete;
		public bool rename;
		public bool renameError;
		
		public string curName;

		#endregion
		
		public bool setTachometerArrowRange;
		public bool setSpeedometerArrowRange;
		public bool setNitrometerArrowRange;
		public bool setPerfectStartRange;
		public bool setPerfectShiftRange;

		
		public List<UIHelper.MenuPages> currentMenusInGame;
		public UIHelper.MenuPages currentMenuPage;
		public UIHelper.MenuPages previousMenuPage;

		public GameObject[] hierarchy;

		[Serializable]
		public class InGameUI
		{
			public UIHelper.GameUI gameUI;
			public UIHelper.GameOver gameOver;
			public UIHelper.Pause pauseMenu;
			public UIHelper.PreGameTimer preGameTimer;
		}

		[Serializable]
		public class MenuUI
		{
			public UIHelper.MainMenu mainMenu;
			public UIHelper.SelectCarMenu selectCarMenu;
			public UIHelper.ProfileMenu profileMenu;
			public UIHelper.SettingsMenu settingsMenu;
			public UIHelper.UpgradeMenu upgradeMenu;
			public UIHelper.AvatarsMenu avatarsMenu;
		}

		public InGameUI inGameUI;
		public MenuUI menuUI;

		public void HideAllInGameUI()
		{
			inGameUI.gameUI.DisableAll();
			inGameUI.preGameTimer.DisableAll();
			inGameUI.pauseMenu.DisableAll();
			inGameUI.gameOver.DisableAll();
		}

		public void HideAllMenuUI()
		{
			menuUI.mainMenu.DisableAll();
			menuUI.selectCarMenu.DisableAll();
			menuUI.profileMenu.DisableAll();
			menuUI.settingsMenu.DisableAll();
			menuUI.upgradeMenu.DisableAll();
			menuUI.avatarsMenu.DisableAll();
		}
		
		private void InstantiateResolutionButtons()
		{
			if(!menuUI.settingsMenu.resolutionButtonPlaceholder.button) return;
			
			var resolutions = UIHelper.GetResolutions(out var stringResolutions);

			foreach (var resolutionButton in menuUI.settingsMenu.resolutionButtons)
			{
				if(resolutionButton.button && resolutionButton.button.gameObject.GetInstanceID() != menuUI.settingsMenu.resolutionButtonPlaceholder.button.gameObject.GetInstanceID())
					Destroy(resolutionButton.button.gameObject);
			}
			
			menuUI.settingsMenu.resolutionButtons.Clear();
			menuUI.settingsMenu.resolutionButtonPlaceholder.button.gameObject.SetActive(false);

			for (var i = 0; i < resolutions.Count; i++)
			{
				var resolution = resolutions[i];
				var instantiatedButton = Instantiate(menuUI.settingsMenu.resolutionButtonPlaceholder.button.gameObject, menuUI.settingsMenu.resolutionsScrollRect.content).GetComponent<Button>();
				var buttonItem = new UIHelper.SettingsMenu.SettingsButton {button = instantiatedButton, resolution = new Resolution {height = resolution.height, width = resolution.width}, textPlaceholder = instantiatedButton.transform.GetChild(0).GetComponent<Text>()};
				buttonItem.textPlaceholder.text = stringResolutions[i];
				menuUI.settingsMenu.resolutionButtons.Add(buttonItem);
				instantiatedButton.gameObject.SetActive(true);
			}
		}

		public void SetWindowMode(string value, int index)
		{
			if(menuUI.settingsMenu.resolutionButtons.Count == 0) return;
			
			if (!PlayerPrefs.HasKey(value))
			{
				PlayerPrefs.SetInt(value, 0);
				index = 0;
			}
			else
			{
				if(index == - 1)
					index = PlayerPrefs.GetInt(value);
					
				PlayerPrefs.SetInt(value, index);
			}

			UIHelper.SetWindowMode(index, menuUI.settingsMenu.resolutionButtons[PlayerPrefs.GetInt("CurrentResolutionButton")].resolution);
			UIHelper.ResetSettingsButtons(new List<UIHelper.SettingsMenu.SettingsButton>{menuUI.settingsMenu.fullscreenMode, menuUI.settingsMenu.windowedMode}, index);
		}

		public void SetSettingsParameter(List<UIHelper.SettingsMenu.SettingsButton> settingsButtons, int index, string value, string type)
		{
			if(settingsButtons.Count == 0) return;
			
			if (!PlayerPrefs.HasKey(value))
			{
				var newValue = 0;

				if (!Application.isMobilePlatform)
					newValue = settingsButtons.Count - 1;
				
				PlayerPrefs.SetInt(value, newValue);
				index = newValue;
			}
			else
			{
				if (index == -1)
				{
					index = PlayerPrefs.GetInt(value);
				}
			}
			
			if (index > settingsButtons.Count - 1)
				index = settingsButtons.Count - 1;
			
			if(index != -1)
				PlayerPrefs.SetInt(value, index);

			if (type == "resolution")
			{
				var windowMode = PlayerPrefs.HasKey("CurrentWindowModeButton") ? (PlayerPrefs.GetInt("CurrentWindowModeButton") == 0 ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed) : FullScreenMode.ExclusiveFullScreen;
				UIHelper.SetResolution(settingsButtons[index].resolution, windowMode, QualitySettings.vSyncCount);
			}
			else if(type == "quality") UIHelper.SetQuality(settingsButtons[index].qualitySettings);
			
			UIHelper.ResetSettingsButtons(settingsButtons, index);
		}

#if UNITY_EDITOR
		public void HideAllHierarchy()
		{
			foreach (var obj in hierarchy)
			{
				if (obj)
				{
					obj.hideFlags = HideFlags.HideInHierarchy;
					
					if (!Application.isPlaying || Application.isPlaying && gameObject.scene.name == "UI Manager")
						obj.SetActive(false);
				}
			}
		}
#endif
		
		private void HideAllHierarchyInGame()
		{
			foreach (var obj in hierarchy)
			{
				if (obj)
				{
					obj.SetActive(false);
				}
			}
		}

		private void Awake()
		{
			gameObject.name = GameHelper.CorrectName(gameObject.name);
			
#if UNITY_EDITOR
			HideAllHierarchyInGame();
#endif
			
			InstantiateResolutionButtons();
			UIHelper.ManageSettingsButtons(this);
			
			SetSettingsParameter(menuUI.settingsMenu.graphicsButtons, -1, "CurrentQualityButton", "quality");

			if (!Application.isMobilePlatform)
			{
				SetSettingsParameter(menuUI.settingsMenu.resolutionButtons, -1, "CurrentResolutionButton", "resolution");
				SetWindowMode("CurrentWindowModeButton", -1);
			}

			currentMenuPage = UIHelper.MenuPages.LobbyMainMenu;
			previousMenuPage = UIHelper.MenuPages.Avatars;
		}

		void Update()
		{
#if UNITY_EDITOR
			foreach (var item in hierarchy)
			{
				if (!item) continue;

				if (!item.activeInHierarchy && item.hideFlags != HideFlags.HideInHierarchy)
					item.hideFlags = HideFlags.HideInHierarchy;
			}
			
			UIHelper.OpenPagesInPlayMode(this);
#endif

		}
		
		public static Vector2 GetMainGameViewSize()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
			return (Vector2)Res;
		}
		
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
				return;

			var size = GetMainGameViewSize();
			
			var screenDelta =  size.x / 1920;
			
			if(inGameUI.gameUI.dashboards[currentDashboard].tachometerArrow && inGameUI.gameUI.dashboards[currentDashboard].tachometerArrow.gameObject.activeInHierarchy)
			{
				if (setTachometerArrowRange)
				{
					Handles.color = new Color32(0, 160, 255, 50);

					var minRotation = Quaternion.AngleAxis(inGameUI.gameUI.dashboards[currentDashboard].tachometerLimits.x, Vector3.forward) *
					                  Vector3.up;
					var angle = Mathf.Abs(inGameUI.gameUI.dashboards[currentDashboard].tachometerLimits.y - inGameUI.gameUI.dashboards[currentDashboard].tachometerLimits.x);

					Handles.DrawSolidArc(inGameUI.gameUI.dashboards[currentDashboard].tachometerArrow.transform.position, Vector3.forward, minRotation, angle, 200 * screenDelta);
				}

				if (setPerfectStartRange)
				{
					Handles.color = new Color32(255, 200, 0, 50);

					var minRotation = Quaternion.AngleAxis(inGameUI.gameUI.dashboards[currentDashboard].perfectStartRange.x, Vector3.forward) * Vector3.up;
					var angle = Mathf.Abs(inGameUI.gameUI.dashboards[currentDashboard].perfectStartRange.y - inGameUI.gameUI.dashboards[currentDashboard].perfectStartRange.x);

					Handles.DrawSolidArc(inGameUI.gameUI.dashboards[currentDashboard].tachometerArrow.transform.position, Vector3.forward, minRotation, angle, 200 * screenDelta);
				}

				if (setPerfectShiftRange)
				{
					Handles.color = new Color32(255, 50, 200, 50);

					var minRotation = Quaternion.AngleAxis(inGameUI.gameUI.dashboards[currentDashboard].perfectShiftRange.x, Vector3.forward) * Vector3.up;
					var angle = Mathf.Abs(inGameUI.gameUI.dashboards[currentDashboard].perfectShiftRange.y - inGameUI.gameUI.dashboards[currentDashboard].perfectShiftRange.x);

					Handles.DrawSolidArc(inGameUI.gameUI.dashboards[currentDashboard].tachometerArrow.transform.position, Vector3.forward, minRotation, angle, 200 * screenDelta);
				}

			}

			if (inGameUI.gameUI.dashboards[currentDashboard].speedometerArrow && inGameUI.gameUI.dashboards[currentDashboard].speedometerArrow.gameObject.activeInHierarchy && setSpeedometerArrowRange)
			{
				Handles.color = new Color32(0, 160, 255, 50);

				var minRotation = Quaternion.AngleAxis(inGameUI.gameUI.dashboards[currentDashboard].speedometerLimits.x, Vector3.forward) * Vector3.up;
				var angle = Mathf.Abs(inGameUI.gameUI.dashboards[currentDashboard].speedometerLimits.y - inGameUI.gameUI.dashboards[currentDashboard].speedometerLimits.x);

				Handles.DrawSolidArc(inGameUI.gameUI.dashboards[currentDashboard].speedometerArrow.transform.position, Vector3.forward, minRotation, angle, 200 * screenDelta);
			}

			if (inGameUI.gameUI.dashboards[currentDashboard].nitrometerArrow && inGameUI.gameUI.dashboards[currentDashboard].nitrometerArrow.gameObject.activeInHierarchy && setNitrometerArrowRange)
			{
				Handles.color = new Color32(0, 160, 255, 50);

				var minRotation = Quaternion.AngleAxis(inGameUI.gameUI.dashboards[currentDashboard].nitrometerLimits.x, Vector3.forward) * Vector3.up;
				var angle = Mathf.Abs(inGameUI.gameUI.dashboards[currentDashboard].nitrometerLimits.y - inGameUI.gameUI.dashboards[currentDashboard].nitrometerLimits.x);

				Handles.DrawSolidArc(inGameUI.gameUI.dashboards[currentDashboard].nitrometerArrow.transform.position, Vector3.forward, minRotation, angle, 100 * screenDelta);
			}
		}
#endif
	}
}
