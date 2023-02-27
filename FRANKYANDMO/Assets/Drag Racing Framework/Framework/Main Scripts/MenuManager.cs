using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if DR_MULTIPLAYER
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.DragRacingFramework
{
	public class MenuManager :
#if DR_MULTIPLAYER
		MonoBehaviourPunCallbacks, ILobbyCallbacks
#else
	MonoBehaviour
#endif
	{
		public GameAssets gameAssets;
		public UIManager currentUIManager;

		public List<GameHelper.Car> opponentCars = new List<GameHelper.Car>();
		public List<GameHelper.Car> cars = new List<GameHelper.Car>();
		public List<GameHelper.LevelParameters> levels = new List<GameHelper.LevelParameters>();

		public List<string> levelsNames = new List<string>();
#if UNITY_EDITOR
		public List<SceneAsset> currentMapsInEditor = new List<SceneAsset>();
		public List<SceneAsset> oldMapsInEditor = new List<SceneAsset>();
#endif

		public GameHelper.AllUpgradeParameters upgradeParameters;

		public GameHelper.ScoreAndMoneyValues scoreValues;
		public GameHelper.ScoreAndMoneyValues moneyValues;

		public GameHelper.UpgradesType upgradesType;

		public Transform carSpawnPoint;
		public Transform defaultCamera;
		public Transform carsShopCamera;
		public Transform carsUpgradesCamera;

		public GameObject currentCarInMenu;

		public GameHelper.CameraPosition currentCameraPositions;
		public GameHelper.CameraPosition mainMenuCameraPositions;

		public bool randomOpponents = true;
		public bool openAnyMenu;
		public bool checkInternetConnection = true;
		public bool switchCamerasImmediately;
		
		// public int currentQuality;
		// public int currentLevel;

		public int findingOpponentsTimer = 5;
		public int startGameTimer = 5; 

		[Range (1, 20)]public float cameraTransitionSpeed = 5;

		//inspector variables
		public int currentInspectorTab;
		public int lastInspectorTab;
		public int inspectorTabTop;

		public int inspectorTabBottom;
		//

#if DR_MULTIPLAYER
		private List<RoomInfo> allMultiplayerRooms = new List<RoomInfo>();
#endif

		public List<Texture> defaultAvatars;
		public List<Button> allDefaultAvatars;
		
		public string checkConnectionServer = "https://google.com";

		public GameHelper.DREvent carPurchaseEvent;
		public GameHelper.DREvent carSelectEvent;
		public GameHelper.DREvent upgradeInstallEvent;
		public GameHelper.DREvent startRaceEvent;
		public GameHelper.DREvent resetAllDataEvent;

		private int _currentCarInMenuIndex;
		// private int _currentAvatarIndex;
		// private int _currentMoney;
		// private int _currentScore;
		private int _currentUpgradeSlot;

		private bool _isConnected;
		private bool _firstTake = true;
		private bool shouldChangeRegion;
		
		private string nextRegionToConnect;

		private void Awake()
		{
			currentUIManager = !FindObjectOfType<UIManager>() ? Instantiate(Resources.Load("UI Manager", typeof(UIManager)) as UIManager) : FindObjectOfType<UIManager>();

			gameAssets = Resources.Load("GameAssets", typeof(GameAssets)) as GameAssets;

			if (!gameAssets) return;
			
			GameAssets.LoadDataFromFile(ref gameAssets.valuesToSave);

			if(gameAssets.valuesToSave.installedUpgrades.Count == 0)
				ResetAllData();
			
			currentUIManager.HideAllInGameUI();
			currentUIManager.HideAllMenuUI();

			currentUIManager.menuUI.mainMenu.ActivateAll();

			UIHelper.AddListeners(currentUIManager, this);
			
			PlayerPrefs.SetInt("findingOpponentsTimer", findingOpponentsTimer);
			PlayerPrefs.SetInt("startGameTimer", startGameTimer);

			if (currentUIManager.menuUI.profileMenu.currentMoney)
				currentUIManager.menuUI.profileMenu.currentMoney.text = gameAssets.valuesToSave.currentMoney.ToString();

			if (currentUIManager.menuUI.profileMenu.currentScore)
				currentUIManager.menuUI.profileMenu.currentScore.text = gameAssets.valuesToSave.currentScore.ToString();

			if (currentUIManager.menuUI.profileMenu.nickname)
				currentUIManager.menuUI.profileMenu.nickname.text = gameAssets.valuesToSave.nickname;

			for (var i = 0; i < defaultAvatars.Count; i++)
			{
				var avatar = defaultAvatars[i];

				if (avatar && currentUIManager.menuUI.avatarsMenu.avatarPlaceholder)
				{
					var placeholder = Instantiate(currentUIManager.menuUI.avatarsMenu.avatarPlaceholder.gameObject, currentUIManager.menuUI.avatarsMenu.scrollRect.content);

					if (!placeholder.GetComponent<Button>())
						placeholder.AddComponent<Button>();

					allDefaultAvatars.Add(placeholder.GetComponent<Button>());

					placeholder.SetActive(true);

					if (placeholder.GetComponent<RawImage>())
						placeholder.GetComponent<RawImage>().texture = avatar;

					var i1 = i;

					if (placeholder.GetComponent<Button>())
						placeholder.GetComponent<Button>().onClick.AddListener(delegate { SetAvatar(i1); });
				}
			}

			if (currentUIManager.menuUI.avatarsMenu.avatarPlaceholder)
				Destroy(currentUIManager.menuUI.avatarsMenu.avatarPlaceholder);

			if(carsShopCamera) carsShopCamera.gameObject.SetActive(false);
			if(carsUpgradesCamera) carsUpgradesCamera.gameObject.SetActive(false);
			
			if (currentUIManager.menuUI.mainMenu.startGameButton)
			{
				currentUIManager.menuUI.mainMenu.startGameButton.interactable = gameAssets.valuesToSave.selectedCar != -1;
			}
			
			if (currentUIManager.menuUI.mainMenu.upgradeButton)
			{
				currentUIManager.menuUI.mainMenu.upgradeButton.interactable = gameAssets.valuesToSave.selectedCar != -1;
			}
		}

		private void Start()
		{
			_currentCarInMenuIndex = -1;

			if (gameAssets.valuesToSave.selectedCar != -1)
				LoadCar(gameAssets.valuesToSave.selectedCar);

			SetAvatar(gameAssets.selectedAvatar);

			ChooseUpgrade("engine", false);

			mainMenuCameraPositions = new GameHelper.CameraPosition {position = defaultCamera.position, rotation = defaultCamera.rotation};
			currentCameraPositions = mainMenuCameraPositions;

			if (defaultCamera)
			{
				defaultCamera.position = currentCameraPositions.position;
				defaultCamera.rotation = currentCameraPositions.rotation;
			}

#if UNITY_EDITOR
			if (currentMapsInEditor.Count > 0)
			{
				foreach (var map in currentMapsInEditor.Where(map => !map))
				{
					Debug.LogWarning("<color=yellow>Not all scenes are added to the Menu Manager.</color>");
				}
			}
			else
			{
				Debug.LogWarning("<color=yellow>There are no scenes in the Menu Manager.</color>");
			}
#endif

#if DR_MULTIPLAYER

			if (currentUIManager.menuUI.mainMenu.regionsDropdown)
			{
				currentUIManager.menuUI.mainMenu.regionsDropdown.ClearOptions();
				currentUIManager.menuUI.mainMenu.regionsDropdown.AddOptions(MultiplayerHelper.PhotonRegions);
			}

			if (currentUIManager.menuUI.mainMenu.connectionStatus)
				currentUIManager.menuUI.mainMenu.connectionStatus.text = "Disconnected from Server";

			_isConnected = false;

			if (PhotonNetwork.InRoom)
				PhotonNetwork.Disconnect();

			if (checkInternetConnection)
			{
				if (MultiplayerHelper.GetHtmlFromUri(checkConnectionServer) == "")
				{
					if (currentUIManager.menuUI.mainMenu.connectionStatus)
						currentUIManager.menuUI.mainMenu.connectionStatus.text = "No Internet Connection";

					if (currentUIManager.menuUI.mainMenu.regionsDropdown)
						currentUIManager.menuUI.mainMenu.regionsDropdown.gameObject.SetActive(false);

					StartCoroutine(CheckInternetConnection());

					_isConnected = false;
				}
				else
				{
					if (!PhotonNetwork.IsConnected)
						PhotonNetwork.ConnectUsingSettings();
				}
			}
			else
			{
				if (!PhotonNetwork.IsConnected)
					PhotonNetwork.ConnectUsingSettings();
			}
			
#else
			if (currentUIManager.menuUI.mainMenu.connectionStatus)
				currentUIManager.menuUI.mainMenu.connectionStatus.gameObject.SetActive(false);
			
			if (currentUIManager.menuUI.mainMenu.regionsDropdown)
				currentUIManager.menuUI.mainMenu.regionsDropdown.gameObject.SetActive(false);
#endif
		}

		private void Update()
		{
			if (defaultCamera)
			{
				if (!switchCamerasImmediately)
				{
					defaultCamera.transform.position = Vector3.Lerp(defaultCamera.transform.position, currentCameraPositions.position, cameraTransitionSpeed * Time.deltaTime);
					defaultCamera.transform.rotation = Quaternion.Lerp(defaultCamera.transform.rotation, currentCameraPositions.rotation, cameraTransitionSpeed * Time.deltaTime);
				}
				else
				{
					defaultCamera.transform.position = currentCameraPositions.position;
					defaultCamera.transform.rotation = currentCameraPositions.rotation;
				}
			}
		}

		public void SetName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				Debug.Log("Player name is empty");
				return;
			}

			gameAssets.valuesToSave.nickname = name;
			
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);
		}

		public void LoadCar(int index)
		{
			if (cars.Count == 0 || !cars[index].vehicleController || _currentCarInMenuIndex == index) return;

			if (carSpawnPoint)
			{
				_currentCarInMenuIndex = index;

				if (currentCarInMenu) Destroy(currentCarInMenu);

				var name = cars[_currentCarInMenuIndex].vehicleController.gameObject.name;

				var isRaycast = Physics.Raycast(carSpawnPoint.transform.position, Vector3.down, out var hitInfo);
				currentCarInMenu = Instantiate(cars[_currentCarInMenuIndex].vehicleController.gameObject, isRaycast ? hitInfo.point + Vector3.up * 0.01f : carSpawnPoint.transform.position, carSpawnPoint.transform.rotation);

				currentCarInMenu.name = name;

				var carController = currentCarInMenu.GetComponent<VehicleController>();

				if (carController.avatarPlaceholder)
					carController.avatarPlaceholder.gameObject.SetActive(false);

				if (carController.nicknamePlaceholder)
					carController.nicknamePlaceholder.gameObject.SetActive(false);

				if (currentUIManager.menuUI.selectCarMenu.nameText)
					currentUIManager.menuUI.selectCarMenu.nameText.text = currentCarInMenu.name;

				GameHelper.LoadCarParameters(carController, gameAssets, upgradeParameters, upgradesType);
				GameHelper.CheckCarParameters(carController);

				UIHelper.UpdateCarStats(currentUIManager, carController);
			}
		}

		public void ChangeCar(string type)
		{
			var index = _currentCarInMenuIndex;

			if (type == "+")
			{
				index++;

				if (index > cars.Count - 1)
					index = 0;
			}
			else
			{
				index--;

				if (index < 0)
					index = cars.Count - 1;
			}

			LoadCar(index);
			UpdateCarMenu();
		}

		public void ChooseUpgrade(string type, bool showMenu)
		{
			if (currentUIManager.menuUI.upgradeMenu.engineButton)
				currentUIManager.menuUI.upgradeMenu.engineButton.interactable = true;

			if (currentUIManager.menuUI.upgradeMenu.turboButton)
				currentUIManager.menuUI.upgradeMenu.turboButton.interactable = true;

			if (currentUIManager.menuUI.upgradeMenu.transmissionButton)
				currentUIManager.menuUI.upgradeMenu.transmissionButton.interactable = true;

			if (currentUIManager.menuUI.upgradeMenu.nitroButton)
				currentUIManager.menuUI.upgradeMenu.nitroButton.interactable = true;

			if (currentUIManager.menuUI.upgradeMenu.weightButton)
				currentUIManager.menuUI.upgradeMenu.weightButton.interactable = true;

			switch (type)
			{
				case "engine":
					if (currentUIManager.menuUI.upgradeMenu.engineButton)
						currentUIManager.menuUI.upgradeMenu.engineButton.interactable = false;

					_currentUpgradeSlot = 0;
					break;

				case "turbo":
					if (currentUIManager.menuUI.upgradeMenu.turboButton)
						currentUIManager.menuUI.upgradeMenu.turboButton.interactable = false;

					_currentUpgradeSlot = 1;
					break;

				case "transmission":
					if (currentUIManager.menuUI.upgradeMenu.transmissionButton)
						currentUIManager.menuUI.upgradeMenu.transmissionButton.interactable = false;

					_currentUpgradeSlot = 2;
					break;

				case "nitro":
					if (currentUIManager.menuUI.upgradeMenu.nitroButton)
						currentUIManager.menuUI.upgradeMenu.nitroButton.interactable = false;

					_currentUpgradeSlot = 3;
					break;

				case "weight":
					if (currentUIManager.menuUI.upgradeMenu.weightButton)
						currentUIManager.menuUI.upgradeMenu.weightButton.interactable = false;

					_currentUpgradeSlot = 4;
					break;
			}

			if (showMenu)
				UpdateUpgradeMenu();
		}

		public void OpenMenu(string type)
		{
			currentUIManager.HideAllMenuUI();
			
			switch (type)
			{
				case "menu":

					currentCameraPositions = mainMenuCameraPositions;
					currentUIManager.menuUI.mainMenu.ActivateAll();

					if (gameAssets.valuesToSave.selectedCar != -1)
					{
						if (gameAssets.valuesToSave.selectedCar != _currentCarInMenuIndex)
							LoadCar(gameAssets.valuesToSave.selectedCar);

						if (currentUIManager.menuUI.mainMenu.startGameButton)
						{
							currentUIManager.menuUI.mainMenu.startGameButton.interactable = true;
						}
						
						if (currentUIManager.menuUI.mainMenu.upgradeButton)
						{
							currentUIManager.menuUI.mainMenu.upgradeButton.interactable = true;
						}
					}
					else
					{
						if (currentCarInMenu)
							Destroy(currentCarInMenu);

						if (currentUIManager.menuUI.mainMenu.startGameButton)
						{
							currentUIManager.menuUI.mainMenu.startGameButton.interactable = false;
						}
						
						if (currentUIManager.menuUI.mainMenu.upgradeButton)
						{
							currentUIManager.menuUI.mainMenu.upgradeButton.interactable = false;
						}
					}

					break;

				case "cars":

					openAnyMenu = true;

					currentCameraPositions = new GameHelper.CameraPosition{position = carsShopCamera.position, rotation = carsShopCamera.rotation};

					currentUIManager.menuUI.selectCarMenu.ActivateAll();

					if (!currentCarInMenu)
					{
						_currentCarInMenuIndex = -1;
						LoadCar(0);
					}

					UpdateCarMenu();

					break;

				case "profile":

					openAnyMenu = true;

					UpdateProfileMenu();
					currentUIManager.menuUI.profileMenu.ActivateAll();
					break;

				case "avatars":
					currentUIManager.menuUI.avatarsMenu.ActivateAll();

					break;

				case "upgrade":

					openAnyMenu = true;

					currentCameraPositions = new GameHelper.CameraPosition{position = carsUpgradesCamera.position, rotation = carsUpgradesCamera.rotation};
					currentUIManager.menuUI.upgradeMenu.ActivateAll();

					UpdateUpgradeMenu();

					break;

				case "settings":

					openAnyMenu = true;
					currentUIManager.menuUI.settingsMenu.ActivateAll();
					break;
			}
		}

		public void CloseApp()
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
		}
		private void BuyCar()
		{
			gameAssets.valuesToSave.currentMoney -= cars[_currentCarInMenuIndex].price;
			gameAssets.valuesToSave.purchasedCars.Add(currentCarInMenu.GetComponent<VehicleController>().carId);

			carPurchaseEvent.Invoke();
			
			SelectCar();
			
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);
		}

		private void SelectCar()
		{
			gameAssets.valuesToSave.selectedCar = _currentCarInMenuIndex;

			carSelectEvent.Invoke();

			UpdateCarMenu();
			
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);
		}

		private void UpdateCarMenu()
		{
			if(!currentCarInMenu) return;
			
			if (!gameAssets.valuesToSave.purchasedCars.Exists(id => id == currentCarInMenu.GetComponent<VehicleController>().carId))
			{
				if (currentUIManager.menuUI.selectCarMenu.selectButtonText)
					currentUIManager.menuUI.selectCarMenu.selectButtonText.text = "Buy";

				if (currentUIManager.menuUI.selectCarMenu.selectCarButton)
				{
					currentUIManager.menuUI.selectCarMenu.selectCarButton.onClick.RemoveAllListeners();
					currentUIManager.menuUI.selectCarMenu.selectCarButton.onClick.AddListener(BuyCar);
				}

				var statusText = "";

				if (cars[_currentCarInMenuIndex].level <= gameAssets.valuesToSave.currentLevel + 1)
				{
					statusText += "Required level " + cars[_currentCarInMenuIndex].level + " <color=#00FF21FF>(current " + (gameAssets.valuesToSave.currentLevel + 1) + ")</color>";
				}
				else
				{
					statusText += "Required level " + cars[_currentCarInMenuIndex].level + " <color=red>(current " + (gameAssets.valuesToSave.currentLevel + 1) + ")</color>";
				}

				if (cars[_currentCarInMenuIndex].price <= gameAssets.valuesToSave.currentMoney)
				{
					statusText += "\n" + "$" + cars[_currentCarInMenuIndex].price + " <color=#00FF21FF>($" + gameAssets.valuesToSave.currentMoney + ")</color>";
				}
				else
				{
					statusText += "\n" + "$" + cars[_currentCarInMenuIndex].price + " <color=red>($" + gameAssets.valuesToSave.currentMoney + ")</color>";
				}

				if (currentUIManager.menuUI.selectCarMenu.carStatus)
					currentUIManager.menuUI.selectCarMenu.carStatus.text = statusText;

				if (currentUIManager.menuUI.selectCarMenu.selectCarButton)
					currentUIManager.menuUI.selectCarMenu.selectCarButton.interactable = cars[_currentCarInMenuIndex].price <= gameAssets.valuesToSave.currentMoney && cars[_currentCarInMenuIndex].level <= gameAssets.valuesToSave.currentLevel + 1;
			}
			else
			{
				if (gameAssets.valuesToSave.selectedCar != _currentCarInMenuIndex)
				{
					if (currentUIManager.menuUI.selectCarMenu.carStatus)
						currentUIManager.menuUI.selectCarMenu.carStatus.text = "Purchased";

					if (currentUIManager.menuUI.selectCarMenu.selectCarButton)
						currentUIManager.menuUI.selectCarMenu.selectCarButton.interactable = true;

					if (currentUIManager.menuUI.selectCarMenu.selectButtonText)
						currentUIManager.menuUI.selectCarMenu.selectButtonText.text = "Select";
				}
				else
				{
					if (currentUIManager.menuUI.selectCarMenu.carStatus)
						currentUIManager.menuUI.selectCarMenu.carStatus.text = "Purchased";

					if (currentUIManager.menuUI.selectCarMenu.selectCarButton)
						currentUIManager.menuUI.selectCarMenu.selectCarButton.interactable = false;

					if (currentUIManager.menuUI.selectCarMenu.selectButtonText)
						currentUIManager.menuUI.selectCarMenu.selectButtonText.text = "Selected";
				}

				if (currentUIManager.menuUI.selectCarMenu.selectCarButton)
				{
					currentUIManager.menuUI.selectCarMenu.selectCarButton.onClick.RemoveAllListeners();
					currentUIManager.menuUI.selectCarMenu.selectCarButton.onClick.AddListener(SelectCar);
				}
			}
		}

		private void UpdateUpgradeMenu()
		{
			var carController = currentCarInMenu ? currentCarInMenu.GetComponent<VehicleController>() : null;

			if (carController == null)
				return;

			if (currentUIManager.menuUI.upgradeMenu.maxSpeedText)
				currentUIManager.menuUI.upgradeMenu.maxSpeedText.text = "Max Speed: " + (carController ? carController.carInfo.MaxSpeed.ToString() : "");

			if (currentUIManager.menuUI.upgradeMenu.accelerationText)
				currentUIManager.menuUI.upgradeMenu.accelerationText.text = "Acceleration: " + (carController ? carController.carInfo.Acceleration.ToString() : "");

			if (currentUIManager.menuUI.upgradeMenu.powerText)
				currentUIManager.menuUI.upgradeMenu.powerText.text = "Power: " + (carController ? carController.carInfo.Power.ToString() : "");

			if (currentUIManager.menuUI.upgradeMenu.nitroTimeText)
				currentUIManager.menuUI.upgradeMenu.nitroTimeText.text = "Nitro Time: " + (carController ? carController.carInfo.nitroTime.ToString() : "");

			if (currentUIManager.menuUI.upgradeMenu.massText)
				currentUIManager.menuUI.upgradeMenu.massText.text = "Mass: " + (carController ? carController.carInfo.Mass.ToString() : "");


			switch (_currentUpgradeSlot)
			{
				case 0:
					SetStage("Engine", carController ? carController.carId + "engineStage" : "", carController ? upgradeParameters.engineUpgrades : new List<GameHelper.UpgradeParameter>());
					break;

				case 1:
					SetStage("Turbo", carController ? carController.carId + "turboStage" : "", carController ? upgradeParameters.turboUpgrades : new List<GameHelper.UpgradeParameter>());
					break;

				case 2:
					SetStage("Transmission", carController ? carController.carId + "transmissionStage" : "", carController ? upgradeParameters.transmissionUpgrades : new List<GameHelper.UpgradeParameter>());
					break;

				case 3:
					SetStage("Nitro", carController ? carController.carId + "nitroStage" : "", carController ? upgradeParameters.nitroUpgrades : new List<GameHelper.UpgradeParameter>());
					break;

				case 4:
					SetStage("Weight", carController ? carController.carId + "weightStage" : "", carController ? upgradeParameters.weightUpgrades : new List<GameHelper.UpgradeParameter>());
					break;
			}
			
			UIHelper.UpdateCarStats(currentUIManager, carController);
		}

		private void SetStage(string name, string key, List<GameHelper.UpgradeParameter> parameters)
		{
			if (parameters != null && parameters.Count > 0)
			{
				if (!gameAssets.valuesToSave.installedUpgrades.ContainsKey(key))
				{
					gameAssets.valuesToSave.installedUpgrades.Add(key, -1);
					GameAssets.SaveDataToFile(gameAssets.valuesToSave);
				}
				
				var currentStage = gameAssets.valuesToSave.installedUpgrades[key];
				var nextStage = currentStage + 1;
				
				if (currentStage < parameters.Count - 1)
				{
					UIHelper.SetUpgradeValues(currentUIManager, parameters[nextStage], currentCarInMenu.GetComponent<VehicleController>().carInfo, upgradesType);

					UIHelper.EnableAllParents(currentUIManager.menuUI.upgradeMenu.priceAndLevelText.gameObject);

					if (currentUIManager.menuUI.upgradeMenu.nameText)
						currentUIManager.menuUI.upgradeMenu.nameText.text = name + " - Stage " + (nextStage + 1) + " | " + parameters.Count;

					var statusText = "";

					if (parameters[nextStage].level <= gameAssets.valuesToSave.currentLevel + 1)
					{
						statusText += "Required level " + parameters[nextStage].level + " <color=#00FF21FF>(current " + (gameAssets.valuesToSave.currentLevel + 1) + ")</color>";
					}
					else
					{
						statusText += "Required level " + parameters[nextStage].level + " <color=red>(current " + (gameAssets.valuesToSave.currentLevel + 1) + ")</color>";
					}

					if (parameters[nextStage].price <= gameAssets.valuesToSave.currentMoney)
					{
						statusText += "\n" + "$" + parameters[nextStage].price + " <color=#00FF21FF>($" + gameAssets.valuesToSave.currentMoney + ")</color>";
					}
					else
					{
						statusText += "\n" + "$" + parameters[nextStage].price + " <color=red>($" + gameAssets.valuesToSave.currentMoney + ")</color>";
					}

					if (currentUIManager.menuUI.upgradeMenu.priceAndLevelText)
						currentUIManager.menuUI.upgradeMenu.priceAndLevelText.text = statusText;

					if (currentUIManager.menuUI.upgradeMenu.buyButton)
						currentUIManager.menuUI.upgradeMenu.buyButton.interactable = parameters[nextStage].price <= gameAssets.valuesToSave.currentMoney && parameters[nextStage].level <= gameAssets.valuesToSave.currentLevel + 1;
				}
				else
				{
					if (currentUIManager.menuUI.upgradeMenu.nameText)
						currentUIManager.menuUI.upgradeMenu.nameText.text = name + " - Stage " + (currentStage + 1) + " | " + parameters.Count;

					UIHelper.EnableAllParents(currentUIManager.menuUI.upgradeMenu.priceAndLevelText.gameObject);

					if (currentUIManager.menuUI.upgradeMenu.priceAndLevelText)
						currentUIManager.menuUI.upgradeMenu.priceAndLevelText.text = "All stages are installed";

					if (currentUIManager.menuUI.upgradeMenu.buyButton)
						currentUIManager.menuUI.upgradeMenu.buyButton.interactable = false;

					UIHelper.DisableAllAdditionalTexts(currentUIManager);
				}
			}
			else
			{
				if (currentUIManager.menuUI.upgradeMenu.nameText)
					currentUIManager.menuUI.upgradeMenu.nameText.text = "No upgrades available";

				if (currentUIManager.menuUI.upgradeMenu.priceAndLevelText)
					currentUIManager.menuUI.upgradeMenu.priceAndLevelText.gameObject.SetActive(false);

				if (currentUIManager.menuUI.upgradeMenu.buyButton)
					currentUIManager.menuUI.upgradeMenu.buyButton.interactable = false;

				UIHelper.DisableAllAdditionalTexts(currentUIManager);
			}
		}

		private void AddValues(string key, List<GameHelper.UpgradeParameter> parameters)
		{
			var carController = currentCarInMenu.GetComponent<VehicleController>();

			var stage = gameAssets.valuesToSave.installedUpgrades[key];
			stage++;
			gameAssets.valuesToSave.installedUpgrades[key] = stage;

			if (upgradesType == GameHelper.UpgradesType.AddValue)
			{
				carController.carInfo.MaxSpeed += parameters[stage].addSpeedValue;
				carController.carInfo.Acceleration += parameters[stage].addAccelerationValue;
				carController.carInfo.Power += parameters[stage].addPowerValue;
				carController.carInfo.nitroTime += parameters[stage].addNitroValue;
				carController.carInfo.Mass += parameters[stage].addMassValue;
			}
			else
			{
				GameHelper.AddPercent(ref carController.carInfo.MaxSpeed, parameters[stage].addSpeedValue);
				GameHelper.AddPercent(ref carController.carInfo.Acceleration, parameters[stage].addAccelerationValue);
				GameHelper.AddPercent(ref carController.carInfo.Power, parameters[stage].addPowerValue);
				GameHelper.AddPercent(ref carController.carInfo.nitroTime, parameters[stage].addNitroValue);
				GameHelper.AddPercent(ref carController.carInfo.Mass, parameters[stage].addMassValue);
			}

			GameHelper.CheckCarParameters(carController);

			gameAssets.valuesToSave.currentMoney -= parameters[stage].price;
			
			UpdateUpgradeMenu();
			
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);
		}

		public void BuyUpgrade()
		{

			var carController = currentCarInMenu.GetComponent<VehicleController>();

			upgradeInstallEvent.Invoke();

			switch (_currentUpgradeSlot)
			{
				case 0:

					AddValues(carController.carId + "engineStage", upgradeParameters.engineUpgrades);

					break;

				case 1:

					AddValues(carController.carId + "turboStage", upgradeParameters.turboUpgrades);

					break;

				case 2:

					AddValues(carController.carId + "transmissionStage", upgradeParameters.transmissionUpgrades);

					break;

				case 3:

					AddValues(carController.carId + "nitroStage", upgradeParameters.nitroUpgrades);

					break;

				case 4:

					AddValues(carController.carId + "weightStage", upgradeParameters.weightUpgrades);
					break;
			}
		}

		public void ResetAllData()
		{
			gameAssets.valuesToSave.ResetAllData();

			ManageValues();

			resetAllDataEvent.Invoke();

			_currentCarInMenuIndex = -1;

			if (currentCarInMenu)
				Destroy(currentCarInMenu);

			foreach (var car in cars.Where(car => car.vehicleController))
			{
				gameAssets.valuesToSave.installedUpgrades.Add(car.vehicleController.carId + "engineStage", -1);
				gameAssets.valuesToSave.installedUpgrades.Add(car.vehicleController.carId + "turboStage", -1);
				gameAssets.valuesToSave.installedUpgrades.Add(car.vehicleController.carId + "transmissionStage", -1);
				gameAssets.valuesToSave.installedUpgrades.Add(car.vehicleController.carId + "nitroStage", -1);
				gameAssets.valuesToSave.installedUpgrades.Add(car.vehicleController.carId + "weightStage", -1);
			}
			
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);

			Debug.Log("All player's data and purchases have been reset.");
		}

		private void SetAvatar(int index)
		{
			if(allDefaultAvatars.Count == 0) return;
			
			allDefaultAvatars[gameAssets.selectedAvatar].interactable = true;

			gameAssets.selectedAvatar = index;
			
			allDefaultAvatars[gameAssets.selectedAvatar].interactable = false;

			if (currentUIManager.menuUI.avatarsMenu.avatarSelectionIndicator)
			{
				currentUIManager.menuUI.avatarsMenu.avatarSelectionIndicator.transform.SetParent(allDefaultAvatars[gameAssets.selectedAvatar].transform);
				currentUIManager.menuUI.avatarsMenu.avatarSelectionIndicator.rectTransform.localPosition = Vector2.zero;
			}
		}

		private void UpdateProfileMenu()
		{
			ManageValues();

			GameHelper.CalculateLevel(gameAssets);
			
			if (currentUIManager.menuUI.profileMenu.changeAvatar && currentUIManager.menuUI.profileMenu.changeAvatar.GetComponent<RawImage>() && defaultAvatars.Count > 0 && defaultAvatars[gameAssets.selectedAvatar])
			{
				currentUIManager.menuUI.profileMenu.changeAvatar.GetComponent<RawImage>().texture = defaultAvatars[gameAssets.selectedAvatar];
			}

			if (currentUIManager.menuUI.profileMenu.currentMoney)
			{
				currentUIManager.menuUI.profileMenu.currentMoney.text = gameAssets.valuesToSave.currentMoney.ToString();
			}

			if (currentUIManager.menuUI.profileMenu.currentScore)
			{
				currentUIManager.menuUI.profileMenu.currentScore.text = gameAssets.valuesToSave.currentScore.ToString();
			}

			if(levels.Count == 0) return;
			
			var scoreValue = (gameAssets.valuesToSave.currentScore - levels[gameAssets.valuesToSave.currentLevel].limits.x) / (levels[gameAssets.valuesToSave.currentLevel].limits.y - levels[gameAssets.valuesToSave.currentLevel].limits.x);

			if (currentUIManager.menuUI.profileMenu.currentLevelFill)
			{
				if (gameAssets.valuesToSave.currentLevel != levels.Count - 1)
					currentUIManager.menuUI.profileMenu.currentLevelFill.fillAmount = scoreValue;
				else currentUIManager.menuUI.profileMenu.currentLevelFill.fillAmount = 1;
			}


			if (currentUIManager.menuUI.profileMenu.currentLevel)
				currentUIManager.menuUI.profileMenu.currentLevel.text = "lvl " + (gameAssets.valuesToSave.currentLevel + 1) + "\n" + "(" + levels[gameAssets.valuesToSave.currentLevel].limits.x + ")";

			if (currentUIManager.menuUI.profileMenu.nextLevel)
			{
				if (gameAssets.valuesToSave.currentLevel + 2 <= levels.Count)
					currentUIManager.menuUI.profileMenu.nextLevel.text = "lvl " + (gameAssets.valuesToSave.currentLevel + 2) + "\n" + "(" + levels[gameAssets.valuesToSave.currentLevel].limits.y + ")";
				else currentUIManager.menuUI.profileMenu.nextLevel.text = "";
			}

		}

		public void StartRace()
		{
			startRaceEvent.Invoke();

			ManageCars();
			ManageAvatars();
			ManageValues();

#if !DR_MULTIPLAYER
        var index = Random.Range(1, levelsNames.Count + 1);
		SceneManager.LoadScene(index);
#else
			if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
				StartMultiplayerGame();
			else
			{
				var index = Random.Range(1, levelsNames.Count + 1);
				SceneManager.LoadScene(index);
			}
#endif
		}

#if DR_MULTIPLAYER
		
		IEnumerator CheckInternetConnection()
		{
			while (true)
			{
				yield return new WaitForSeconds(5);

				if (checkInternetConnection && MultiplayerHelper.GetHtmlFromUri(checkConnectionServer) != "" || !checkInternetConnection)
				{
					if (currentUIManager.menuUI.mainMenu.connectionStatus)
						currentUIManager.menuUI.mainMenu.connectionStatus.text = "Disconnected from Server";

					if (currentUIManager.menuUI.mainMenu.regionsDropdown)
						currentUIManager.menuUI.mainMenu.regionsDropdown.gameObject.SetActive(true);

					PhotonNetwork.ConnectUsingSettings();

					StopCoroutine(CheckInternetConnection());
					break;
				}
			}
		}


		private void ChangeRegion(int value)
		{
			// var newRegion = ;
			//
			// newRegion = newRegion.Trim();
			// if (newRegion.Equals(PhotonNetwork.CloudRegion, StringComparison.OrdinalIgnoreCase))
			// 	return;
			//
			// shouldChangeRegion = true;
			// nextRegionToConnect = newRegion;
			//
			// if(PhotonNetwork.IsConnected)
			// 	PhotonNetwork.Disconnect();
			
			PhotonNetwork.Disconnect();
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = MultiplayerHelper.ConvertRegionToCode(value);
			PhotonNetwork.ConnectUsingSettings();
		}

		private void StartMultiplayerGame()
		{
			var foundRoom = false;

			PhotonNetwork.NickName = gameAssets.valuesToSave.nickname;
			var carController = currentCarInMenu.GetComponent<VehicleController>();

			PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
			{
				{"ms", carController.carInfo.MaxSpeed}, {"ac", carController.carInfo.Acceleration},
				{"p", carController.carInfo.Power}, {"nt", carController.carInfo.nitroTime}, {"m", carController.carInfo.Mass},
				{"car", _currentCarInMenuIndex}, {"avatar", gameAssets.selectedAvatar}
			});

			if (allMultiplayerRooms.Count > 0)
			{
				foreach (var room in allMultiplayerRooms.Where(room => room.IsOpen && room.IsVisible))
				{
					foundRoom = true;
					PhotonNetwork.JoinRoom(room.Name);
					break;
				}

				if (!foundRoom)
					CreateGame();
			}
			else
			{
				CreateGame();
			}
		}

		private void CreateGame()
		{
			var index = Random.Range(1, levelsNames.Count + 1);

			var customValues = new Hashtable {{"map", index}, {"st", 5}};
			
			var roomOpt = new RoomOptions
			{
				MaxPlayers = 2,
				IsOpen = true, IsVisible = true,
				CustomRoomProperties = customValues
			};

			var value = new string[3];
			value[0] = "m";
			value[1] = "gm";
			value[2] = "e";
			roomOpt.CustomRoomPropertiesForLobby = value;


			PhotonNetwork.CreateRoom(MultiplayerHelper.GenerateRandomName(), roomOpt);
		}
#endif

		private void ManageCars()
		{
			gameAssets.carsList.Clear();
			gameAssets.carsList.AddRange(cars);
			
			gameAssets.opponentCarsList.Clear();
			gameAssets.opponentCarsList.AddRange(opponentCars);

			gameAssets.randomOpponents = randomOpponents;

			GameHelper.SaveCarParameters(gameAssets, currentCarInMenu.GetComponent<VehicleController>());
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);
		}

		private void ManageAvatars()
		{
			gameAssets.avatarsList.Clear();
			gameAssets.avatarsList.AddRange(defaultAvatars);
		}

		private void ManageValues()
		{
			gameAssets.scoreValues = scoreValues;
			gameAssets.moneyValues = moneyValues;

			gameAssets.levels.Clear();
			gameAssets.levels.AddRange(levels);
		}

		#region PhotonCallbacks

#if DR_MULTIPLAYER
		public override void OnConnectedToMaster()
		{
			PhotonNetwork.AutomaticallySyncScene = false;

			if (!PhotonNetwork.InLobby)
				PhotonNetwork.JoinLobby(TypedLobby.Default);
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			foreach (var room in roomList)
			{
				if (allMultiplayerRooms.Count > 0)
				{
					if (!allMultiplayerRooms.Exists(_room => _room.Name == room.Name))
					{
						if (room.PlayerCount > 0 && room.PlayerCount < 2)
							allMultiplayerRooms.Add(room);
					}
					else
					{
						if (room.PlayerCount <= 0)
						{
							allMultiplayerRooms.Remove(room);
						}
					}
				}
				else
				{
					if (room.PlayerCount > 0 && room.PlayerCount < 2)
						allMultiplayerRooms.Add(room);
				}
			}
		}

		public override void OnJoinedLobby()
		{
			if (currentUIManager.menuUI.mainMenu.connectionStatus)
				currentUIManager.menuUI.mainMenu.connectionStatus.text = "Connected | Ping - " + PhotonNetwork.GetPing() + " ms";

			if (currentUIManager.menuUI.mainMenu.regionsDropdown)
			{
				currentUIManager.menuUI.mainMenu.regionsDropdown.gameObject.SetActive(true);

				if (_firstTake)
				{
					currentUIManager.menuUI.mainMenu.regionsDropdown.value = MultiplayerHelper.ConvertCodeToRegion(PhotonNetwork.CloudRegion);
					currentUIManager.menuUI.mainMenu.regionsDropdown.onValueChanged.AddListener(ChangeRegion);
					_firstTake = false;
				}
			}

			_isConnected = true;
		}

		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			print("Failed create room: " + returnCode + "\n" + message);
		}

		public override void OnCreatedRoom()
		{

		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{

		}

		public override void OnJoinedRoom()
		{
			PhotonNetwork.LoadLevel(levelsNames[(int) PhotonNetwork.CurrentRoom.CustomProperties["map"] - 1]);
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			if (currentUIManager.menuUI.mainMenu.connectionStatus)
				currentUIManager.menuUI.mainMenu.connectionStatus.text = "Disconnected from Server";

			_isConnected = false;

			if (checkInternetConnection)
			{
				if (MultiplayerHelper.GetHtmlFromUri(checkConnectionServer) == "")
				{
					if (currentUIManager.menuUI.mainMenu.connectionStatus)
						currentUIManager.menuUI.mainMenu.connectionStatus.text = "No Internet Connection";

					if (currentUIManager.menuUI.mainMenu.regionsDropdown)
						currentUIManager.menuUI.mainMenu.regionsDropdown.gameObject.SetActive(false);

					StartCoroutine(CheckInternetConnection());
				}
			}

		}
#endif

		#endregion
	}
}
