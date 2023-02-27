using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if DR_CINEMACHINE
using Cinemachine;
#endif
#if DR_MULTIPLAYER
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif
using UnityEngine;
#if DR_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace GercStudio.DragRacingFramework
{
	[RequireComponent(typeof(EventsManager))]
	public class GameManager :
#if DR_MULTIPLAYER
		MonoBehaviourPunCallbacks 
#else
    MonoBehaviour
#endif
	{
		public EventsManager eventsManager;
		public VehicleController playerController;
		public VehicleController opponentController;

		public SpawnPoint playerSpawnPoint;
		public SpawnPoint opponentSpawnPoint;

		public Camera mainCamera;
#if DR_CINEMACHINE
		public CinemachineVirtualCamera virtualCamera;
		public NoiseSettings noiseProfile;
#endif

		public GameHelper.RoadType roadType;

		public GameAssets gameAssets;
		public UIManager currentUIManager;
		public InputSettings inputSettings;

		public List<GameObject> roadPrefabs;
		public GameObject lastRoadPart;
		public GameObject finishFlag;

		public Transform playerCar;
		public Transform opponentCar;
		
		public int networkCarIndex = -1;
		public int perfectShifts;
		public int currentEventsMenu; //inspector variable
		public int inspectorTab; //inspector variable

		public float distanceBetweenSpawnPoints;
		public float finishFlagDistance = 400f;
		public float startPlayerDistance;
		public float startOpponentDistance;
		public float distanceBetweenOpponentAndStart;
		public float networkSpeed = -1;
		public float roadLength = 27f;
		public float raceTrackLenght;
		public float countdownValue = 5.0f;

		public List<float> intermediatePositions = new List<float>();
		public List<float> intermediateOpponentTimes = new List<float>();
		public List<float> intermediatePlayerTimes = new List<float>();

		public bool hasOpponentFinished;
		public bool hasPlayerFinished;
		public bool gameStarted;
		public bool pause;
		public bool pressGas;
		public bool pressGasUI;
		public bool opponentPressGas;
		public bool playerPerfectStart;
		public bool opponentPerfectStart;
		public bool playerGoodStart;
		public bool getEventFromAnotherPlayer;
		public bool correctPosition;
		public bool isOptionsMenuOpened;

		public Texture roadImage;

		//game manager events
		public GameHelper.DREvent raceStartedEvent;
		public GameHelper.DREvent playerFinishedEvent;
		public GameHelper.DREvent opponentFinishedEvent;
		public GameHelper.DREvent victoryEvent;
		public GameHelper.DREvent lossEvent;

		//car controller events
		public GameHelper.DREvent gearUpEvent;
		public GameHelper.DREvent gearDownEvent;
		public GameHelper.DREvent startUsingNitroEvent;
		public GameHelper.DREvent endUsingNitroEvent;
		public GameHelper.DREvent perfectShiftEvent;
		public GameHelper.DREvent perfectStartEvent;

		private float findPlayersTimer;
		public float playerTime;
		public float opponentTime = -1;
		private float independentOpponentTime;
		private float currentTimerValue;
		
		private int currentSector;
		// private int startGameTimerValue = 5;
		private int findOpponentsTimerValue = 5;
		private int startTime;

		private bool firstTake;
		private bool isFlagSpawned;
		private bool opponentFound;
		private bool networkOpponentWasReplaced;
		private bool hasStartTime;

		public string opponentName = "AI";

		private Transform tempCarTransform;
		private Transform roadController;

		private Texture opponentAvatar;

		private VehicleController _firstVehicle;
		
#if DR_MULTIPLAYER

		
#endif

		private void Awake()
		{
			eventsManager = GetComponent<EventsManager>();
			eventsManager.gameManager = this;
			
			currentUIManager = !FindObjectOfType<UIManager>() ? Instantiate(Resources.Load("UI Manager", typeof(UIManager)) as UIManager) : FindObjectOfType<UIManager>();

			gameAssets = Resources.Load("GameAssets", typeof(GameAssets)) as GameAssets;
			inputSettings = Resources.Load("Input Manager", typeof(InputSettings)) as InputSettings;
			
			countdownValue = PlayerPrefs.GetInt("startGameTimer");
			findOpponentsTimerValue = PlayerPrefs.GetInt("findingOpponentsTimer");

			if (currentUIManager)
			{
				if (currentUIManager.inGameUI.gameUI.pauseButton)
					currentUIManager.inGameUI.gameUI.pauseButton.onClick.AddListener(Pause);

				if (currentUIManager.inGameUI.pauseMenu.resumeButton)
					currentUIManager.inGameUI.pauseMenu.resumeButton.onClick.AddListener(Pause);

				if (currentUIManager.inGameUI.pauseMenu.exitButton)
					currentUIManager.inGameUI.pauseMenu.exitButton.onClick.AddListener(Exit);

				if (currentUIManager.inGameUI.gameOver.restart)
					currentUIManager.inGameUI.gameOver.restart.onClick.AddListener(Restart);

				if (currentUIManager.inGameUI.gameOver.exit)
					currentUIManager.inGameUI.gameOver.exit.onClick.AddListener(Exit);

				if (currentUIManager.inGameUI.preGameTimer.exitButton)
					currentUIManager.inGameUI.preGameTimer.exitButton.onClick.AddListener(Exit);
				
				if (currentUIManager.inGameUI.pauseMenu.optionsButton)
					currentUIManager.inGameUI.pauseMenu.optionsButton.onClick.AddListener(OptionsMenu);
				
				if (currentUIManager.menuUI.settingsMenu.backButton)
					currentUIManager.menuUI.settingsMenu.backButton.onClick.AddListener(delegate { SwitchMenu("pause"); });

				if (currentUIManager.inGameUI.gameUI.roadPlaceholder && roadImage)
					currentUIManager.inGameUI.gameUI.roadPlaceholder.texture = roadImage;

				currentUIManager.HideAllMenuUI();
				currentUIManager.HideAllInGameUI();

				currentUIManager.inGameUI.preGameTimer.ActivateAll();

				currentUIManager.currentMenuPage = UIHelper.MenuPages.LoadingGameMenu;
				currentUIManager.previousMenuPage = UIHelper.MenuPages.Lobby;
			}
			
			InputHelper.InitializeKeyboardAndMouseButtons(inputSettings);
			InputHelper.InitializeGamepadButtons(inputSettings);
				
#if DR_INPUT_SYSTEM
			InputSystem.onDeviceChange += (device, change) =>
			{
				switch (change)
				{
					case InputDeviceChange.Added:
						InputHelper.InitializeGamepadButtons(inputSettings);
						break;
				}
			};
#endif
		}


		void Start()
		{
			distanceBetweenSpawnPoints = Vector3.Distance(opponentSpawnPoint.transform.position, playerSpawnPoint.transform.position);
			
			tempCarTransform = new GameObject("temp").transform;
			tempCarTransform.hideFlags = HideFlags.HideInHierarchy;
			
			roadController = new GameObject("Road Controller").transform;

			StartCoroutine(FindOpponentTimer());
		}

		void Update()
		{
#if DR_MULTIPLAYER

			if (!opponentController && PhotonNetwork.PlayerListOthers.Length > 0)
			{
				var opponent = PhotonNetwork.PlayerListOthers[0];
				var cars = FindObjectsOfType<VehicleController>().ToList();

				foreach (var car in cars.Where(car => !car.photonView.IsMine))
				{
					opponentCar = car.transform;
					opponentController = car;
					var name = opponentCar.name;
					if (name.Contains("(Clone)"))
					{
						var replace = name.Replace("(Clone)", "");
						name = replace;
					}
					networkCarIndex = gameAssets.carsList.IndexOf(gameAssets.carsList.Find(opponentCar => opponentCar.vehicleController.gameObject.name == name));
					
					var isRaycast = Physics.Raycast(opponentSpawnPoint.transform.position, Vector3.down, out var hitInfo);
					opponentCar.transform.position = isRaycast ? hitInfo.point + Vector3.up * 0.01f : opponentSpawnPoint.transform.position;
					
					opponentController.ai = true;
					opponentController.drivingDirection = opponentSpawnPoint.transform.forward;
					opponentController.gameManager = this;
					
					startOpponentDistance = CarHelper.Distance(GetCorrectTransform(opponentController), roadType == GameHelper.RoadType.AutoGeneration ? opponentSpawnPoint.transform.position + opponentSpawnPoint.transform.forward.normalized * finishFlagDistance : finishFlag.transform.position);

					if (currentUIManager.inGameUI.gameUI.opponentCarPlaceholder && opponentController.carImage)
						currentUIManager.inGameUI.gameUI.opponentCarPlaceholder.texture = opponentController.carImage;
					
					opponentController.multiplayerCar = true;
					
					var avatarIndex = (int) opponent.CustomProperties["avatar"];

					opponentName = opponent.NickName;
					opponentAvatar = gameAssets.avatarsList[avatarIndex];
					
					if (opponentController.avatarPlaceholder)
						opponentController.avatarPlaceholder.texture = opponentAvatar;

					if (opponentController.nicknamePlaceholder)
						opponentController.nicknamePlaceholder.text = opponentName;

					opponentController.carInfo.MaxSpeed = (int) opponent.CustomProperties["ms"];
					opponentController.carInfo.Acceleration = (int) opponent.CustomProperties["ac"];
					opponentController.carInfo.Power = (int) opponent.CustomProperties["p"];
					opponentController.carInfo.nitroTime = (int) opponent.CustomProperties["nt"];
					opponentController.carInfo.Mass = (int) opponent.CustomProperties["m"];
				}
			}

			if (opponentFound && PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 1 && !opponentController)
			{
				LoadOpponent(true);
			}
#endif
			
			if (!playerController || !opponentController) return;
			
#if DR_INPUT_SYSTEM


			if (!pressGasUI)
			{
				if (InputHelper.IsKeyboardButtonPressed(inputSettings.keyboardButtonsInUnityInputSystem[0]) || InputHelper.IsGamepadButtonPressed(inputSettings.gamepadButtonsInUnityInputSystem[0]))
				{
					PressGas("+");
				}
				else
				{
					PressGas("-");
				}
			}

			if (InputHelper.WasKeyboardButtonPressed(inputSettings.keyboardButtonsInUnityInputSystem[1]) || InputHelper.WasGamepadButtonPressed(inputSettings.gamepadButtonsInUnityInputSystem[1]))
			{
				SwitchGear("+");
			}
			
			if (InputHelper.WasKeyboardButtonPressed(inputSettings.keyboardButtonsInUnityInputSystem[2]) || InputHelper.WasGamepadButtonPressed(inputSettings.gamepadButtonsInUnityInputSystem[2]))
			{
				SwitchGear("-");
			}
			
			if (InputHelper.WasKeyboardButtonPressed(inputSettings.keyboardButtonsInUnityInputSystem[3]) || InputHelper.WasGamepadButtonPressed(inputSettings.gamepadButtonsInUnityInputSystem[3]))
			{
				EnableNitro();
			}
			
			if (InputHelper.WasKeyboardButtonPressed(inputSettings.keyboardButtonsInUnityInputSystem[4]) || InputHelper.WasGamepadButtonPressed(inputSettings.gamepadButtonsInUnityInputSystem[4]))
			{
				Pause();
			}
#endif
			
			
			if (gameStarted)
			{
				if (!hasPlayerFinished && playerController)
				{
					playerTime += Time.deltaTime;
				}

				if (!hasOpponentFinished && opponentController && !opponentController.multiplayerCar)
				{
					opponentTime += Time.deltaTime;
				}

				if (!hasOpponentFinished && opponentController)
				{
					independentOpponentTime += Time.deltaTime;
				}
			}

			var distanceBetweenPlayerAndFlag = CarHelper.Distance(GetCorrectTransform(playerController), roadType == GameHelper.RoadType.AutoGeneration ? playerSpawnPoint.transform.position + playerSpawnPoint.transform.forward.normalized * finishFlagDistance : finishFlag.transform.position);
			var distanceBetweenPlayerAndStart = CarHelper.Distance(GetCorrectTransform(playerController), playerSpawnPoint.transform.position);
			var distanceBetweenOpponentAndFlag = CarHelper.Distance(GetCorrectTransform(opponentController), roadType == GameHelper.RoadType.AutoGeneration ? opponentSpawnPoint.transform.position + opponentSpawnPoint.transform.forward.normalized * finishFlagDistance : finishFlag.transform.position);
			distanceBetweenOpponentAndStart = -CarHelper.Distance(GetCorrectTransform(opponentController), opponentSpawnPoint.transform.position);

			_firstVehicle = distanceBetweenPlayerAndFlag > distanceBetweenOpponentAndFlag ? opponentController : playerController;

#if DR_MULTIPLAYER
			if (gameStarted && PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length == 2)
			{
				if (intermediatePositions.Count >= currentSector + 1)
				{
					if (Math.Abs(-distanceBetweenPlayerAndStart - intermediatePositions[currentSector]) < 1) //&& !sendIntermediateTime)
					{
						intermediatePlayerTimes.Add(playerTime);
						currentSector++;
						correctPosition = true;

						eventsManager.SendEvent(MultiplayerHelper.PhotonEventCodes.SendTime);
					}
				}

				if (correctPosition && !hasOpponentFinished)
				{
					var distanceBetweenPlayers = CarHelper.Distance(GetCorrectTransform(playerController), opponentCar.position);

					if (intermediateOpponentTimes.Count >= currentSector)
					{
						if (intermediateOpponentTimes[currentSector - 1] < intermediatePlayerTimes[currentSector - 1])
						{
							var targetPos = playerCar.position + opponentController.drivingDirection * 5 - playerSpawnPoint.transform.right * distanceBetweenSpawnPoints;
							var targetValue = 10;//PhotonNetwork.GetPing() < 100 ? 0 : 10;
							
							CarHelper.CorrectCarPosition(opponentController, targetValue, targetPos, distanceBetweenPlayers, ref correctPosition);
						}
						else
						{
							var targetPos = playerCar.position - opponentController.drivingDirection * 5 - playerSpawnPoint.transform.right * distanceBetweenSpawnPoints;
							var targetValue = -10;//PhotonNetwork.GetPing() < 100 ? 0 : -10;
							
							CarHelper.CorrectCarPosition(opponentController, targetValue, targetPos, distanceBetweenPlayers, ref correctPosition);
						}
					}
					else
					{
						if (getEventFromAnotherPlayer)
						{
							var targetPos = playerCar.position - opponentController.drivingDirection * 5 - playerSpawnPoint.transform.right * distanceBetweenSpawnPoints;
							var targetValue = -10;//PhotonNetwork.GetPing() < 100 ? 0 : -10;
							
							CarHelper.CorrectCarPosition(opponentController, targetValue, targetPos, distanceBetweenPlayers, ref correctPosition);
						}
					}
				}
			}
#endif

			if (distanceBetweenPlayerAndFlag <= 1 && !hasPlayerFinished)
			{
				hasPlayerFinished = true;
				playerController.Finish();
				playerFinishedEvent.Invoke();
				StartCoroutine(ShowGameOverMenu());
			}

			if (!opponentController.multiplayerCar)
			{
				if (distanceBetweenOpponentAndFlag <= 1 && !hasOpponentFinished)
				{
					hasOpponentFinished = true;
					opponentController.Finish();
					opponentFinishedEvent.Invoke();

					if (hasPlayerFinished && currentUIManager.inGameUI.gameOver.secondPlayerStats)
						currentUIManager.inGameUI.gameOver.secondPlayerStats.text = opponentName + " |  Time - " + opponentTime.ToString("G");
				}
			}

			if (roadType == GameHelper.RoadType.AutoGeneration)
			{
				var distanceBetweenPlayerAndLastRoadPart = CarHelper.Distance(GetCorrectTransform(_firstVehicle),lastRoadPart.transform.position);

				if (distanceBetweenPlayerAndLastRoadPart < roadLength * 3)
				{
					SpawnRoad(4);
				}
			}

			if (!gameStarted)
			{
				
#if DR_MULTIPLAYER
				if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
#endif
				{
					currentTimerValue += Time.deltaTime;
					var leftTime = (int)countdownValue - currentTimerValue;
					
					if (leftTime <= 0)
					{
						currentUIManager.inGameUI.gameUI.countdownText.text = "GO!";
                
						if (leftTime <= -0.3f)
						{
							StartRace();
						}
					}
					else
					{
						var text = leftTime.ToString("00");
						currentUIManager.inGameUI.gameUI.countdownText.text = text != "00" ? text : "GO!"; 
					}
				}

				ChangeEnginePitchAtStart(playerController.engineAudioSource, playerController.defaultEngineVolume, pressGas);

				if (!opponentController.multiplayerCar)
					ChangeEnginePitchAtStart(opponentController.engineAudioSource, opponentController.defaultEngineVolume, opponentPressGas);
			}
			else
			{
				if (!playerController.startDriving)
				{
					var targetSpeed = 0f;

					if (playerPerfectStart) targetSpeed = 10f;
					else if (playerGoodStart) targetSpeed = 6f;
					else targetSpeed = 0f;

					playerController.currentSpeed = Mathf.Lerp(playerController.currentSpeed, targetSpeed, 10 * Time.deltaTime);

					if (Math.Abs(playerController.currentSpeed - targetSpeed) < 1)
					{
						playerController.startDriving = true;
					}
				}

				if (!opponentController.startDriving)
				{
					if (!opponentController.multiplayerCar)
					{
						var targetSpeed = opponentPerfectStart ? 10f : 0f;

						opponentController.currentSpeed = Mathf.Lerp(opponentController.currentSpeed, targetSpeed, 10 * Time.deltaTime);

						if (Math.Abs(opponentController.currentSpeed - targetSpeed) < 1)
						{
							opponentController.startDriving = true;
						}
					}
					else
					{
						opponentController.startDriving = true;
					}
				}
			}

			if (currentUIManager.inGameUI.gameUI.playerCarPlaceholder && currentUIManager.inGameUI.gameUI.playerCarPlaceholder.texture)
			{
				var playerCarUIPosition = CarHelper.CarUIPosition(currentUIManager.inGameUI.gameUI.roadPlaceholder, currentUIManager.inGameUI.gameUI.playerCarPlaceholder, distanceBetweenPlayerAndFlag, startPlayerDistance);
				currentUIManager.inGameUI.gameUI.playerCarPlaceholder.rectTransform.anchoredPosition = new Vector2(playerCarUIPosition, currentUIManager.inGameUI.gameUI.playerCarPlaceholder.rectTransform.anchoredPosition.y);
			}

			if (currentUIManager.inGameUI.gameUI.opponentCarPlaceholder && currentUIManager.inGameUI.gameUI.opponentCarPlaceholder.texture)
			{
				var opponentCarUIPosition = CarHelper.CarUIPosition(currentUIManager.inGameUI.gameUI.roadPlaceholder, currentUIManager.inGameUI.gameUI.opponentCarPlaceholder, distanceBetweenOpponentAndFlag, startOpponentDistance);
				currentUIManager.inGameUI.gameUI.opponentCarPlaceholder.rectTransform.anchoredPosition = new Vector2(opponentCarUIPosition, currentUIManager.inGameUI.gameUI.opponentCarPlaceholder.rectTransform.anchoredPosition.y);
			}
		}
		
		public void StartRace()
		{
			
			if (currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].gasButton)
				currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].gasButton.gameObject.SetActive(false);

			if (currentUIManager.inGameUI.gameUI.countdownText)
				currentUIManager.inGameUI.gameUI.countdownText.gameObject.SetActive(false);

			raceStartedEvent.Invoke();

			gameStarted = true;

			if (playerController.engineAudioSource && playerController.engineAudioClip)
			{
				playerController.targetEngineVolume = playerController.defaultEngineVolume;
				playerController.engineAudioSource.Play();
			}

			if (opponentController.engineAudioSource && opponentController.engineAudioClip && !opponentController.multiplayerCar)
			{
				opponentController.targetEngineVolume = opponentController.defaultEngineVolume;
			}

			if (!opponentController.multiplayerCar)
			{
				var perfectStartChance = Random.Range(0, 2);

				if (perfectStartChance == 1)
				{
					opponentController.startNitroTimeout = 8;
					opponentPerfectStart = true;
				}
			}

			if (GameHelper.IsBetween(playerController.currentTacho, currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].perfectStartRange.x, currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].perfectStartRange.y))
			{
				playerPerfectStart = true;

				playerController.currentSpeed = 0;

				perfectStartEvent.Invoke();

				if (currentUIManager.inGameUI.gameUI.gamePopUp)
				{
					if (currentUIManager.inGameUI.gameUI.gamePopUp)
					{
						UIHelper.EnableAllParents(currentUIManager.inGameUI.gameUI.gamePopUp.gameObject);
						currentUIManager.inGameUI.gameUI.gamePopUp.text = "Perfect Start!";
						StartCoroutine(DisableGamePopup());
					}
				}
			}
			else if (GameHelper.IsBetween(playerController.currentTacho, currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].perfectStartRange.x - 20, currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].perfectStartRange.y + 20))
			{
				playerGoodStart = true;

				playerController.currentSpeed = 0;

				if (currentUIManager.inGameUI.gameUI.gamePopUp)
				{
					UIHelper.EnableAllParents(currentUIManager.inGameUI.gameUI.gamePopUp.gameObject);
					currentUIManager.inGameUI.gameUI.gamePopUp.text = "Good Start";
					StartCoroutine(DisableGamePopup());
				}
			}
		}

		public void Restart()
		{

#if DR_MULTIPLAYER
			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
				PhotonNetwork.LeaveRoom();
#endif

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		void LoadGame()
		{
			if (currentUIManager.inGameUI.gameUI.gamePopUp)
				currentUIManager.inGameUI.gameUI.gamePopUp.gameObject.SetActive(false);
			
			if (playerSpawnPoint && opponentSpawnPoint)
			{
				LoadPlayer();
				LoadOpponent(false);
			}
			else
			{
				Debug.LogWarning("There are not spawn points in the scene. Set them in the [GameManager] script.");
				Debug.Break();
			}
			
			currentUIManager.inGameUI.preGameTimer.DisableAll();
			currentUIManager.inGameUI.gameUI.ActivateAll(currentUIManager.currentDashboard);

			if (currentUIManager)
			{
				if (currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].nextGearButton)
					currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].nextGearButton.onClick.AddListener(delegate { SwitchGear(("+")); });

				if (currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].previousGearButton)
					currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].previousGearButton.onClick.AddListener(delegate { SwitchGear("-"); });

				if (currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].useNitroButton)
					currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].useNitroButton.onClick.AddListener(EnableNitro);

				if (currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].gasButton)
					UIHelper.setButtonEvent(currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].gasButton.gameObject, this);
			}

#if DR_MULTIPLAYER

			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
			{
				StartCoroutine(PreGameTimer());

				if(PhotonNetwork.IsMasterClient)
					PhotonNetwork.CurrentRoom.IsOpen = false;

				opponentFound = true;
			}
#endif
			
			// if (!playerController || !opponentController) return;
			
			if (roadType == GameHelper.RoadType.AutoGeneration)
			{
				SpawnRoad(2);
				SpawnFlag();
			}
		}

#if DR_MULTIPLAYER
		IEnumerator PreGameTimer()
		{
			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				startTime = PhotonNetwork.ServerTimestamp;
				hasStartTime = true;
				var customValues = new Hashtable {{"StartTimer", startTime}};
				PhotonNetwork.CurrentRoom.SetCustomProperties(customValues);
			}

			while (true)
			{
				if (currentUIManager.inGameUI.gameUI.countdownText)
				{
					if (!hasStartTime)
					{
						currentUIManager.inGameUI.gameUI.countdownText.text = "...";
					}
					else
					{
						if (!gameStarted)
						{
							var countdown = TimeRemaining();
							
							if (countdown <= 0)
							{
								currentUIManager.inGameUI.gameUI.countdownText.text = "GO!";

								if (countdown <= -0.3f)
								{
									StartRace();
									break;
								}
							}
							else
							{
								var text = countdown.ToString("00");
								currentUIManager.inGameUI.gameUI.countdownText.text = text != "00" ? text : "GO!";
							}
						}
					}
				}

				yield return 0;
			}
		}
		
		private float TimeRemaining()
		{
			
			var timer = PhotonNetwork.ServerTimestamp - startTime;
			var ret =  countdownValue - timer / 1000f;
			
			return ret;
		}
#endif

		public Transform GetCorrectTransform(VehicleController controller)
		{
			tempCarTransform.position = controller.transform.position;
			tempCarTransform.rotation = controller.transform.rotation * Quaternion.Euler(0, controller.rotationOffset, 0);

			return tempCarTransform;
		}

		private void LoadPlayer()
		{
			var isRaycast = Physics.Raycast(playerSpawnPoint.transform.position, Vector3.down, out var hitInfo);
			var carPrefab = gameAssets.carsList[gameAssets.valuesToSave.selectedCar].vehicleController;
			GameObject car = null;
#if DR_MULTIPLAYER
			if(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CountOfPlayers == 2)
				car = PhotonNetwork.Instantiate(carPrefab.gameObject.name, isRaycast ? hitInfo.point + Vector3.up * 0.05f : playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation * Quaternion.Euler(0, -carPrefab.rotationOffset, 0));
			else 
				car = Instantiate(carPrefab.gameObject, isRaycast ? hitInfo.point + Vector3.up * 0.01f : playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation * Quaternion.Euler(0, -carPrefab.rotationOffset, 0));
#else
			car = Instantiate(carPrefab.gameObject, isRaycast ? hitInfo.point + Vector3.up * 0.01f : playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation * Quaternion.Euler(0, -carPrefab.rotationOffset, 0));
#endif

			if (car == null) return;
			
			var carController = car.GetComponent<VehicleController>();

			carController.drivingDirection = opponentSpawnPoint.transform.forward;
			carController.gameManager = this;

			if (carController.avatarPlaceholder)
				carController.avatarPlaceholder.gameObject.SetActive(false);

			if (carController.nicknamePlaceholder)
				carController.nicknamePlaceholder.gameObject.SetActive(false);

			GameHelper.LoadCarParameters(carController);

			if (carController.selectedDashboard > currentUIManager.inGameUI.gameUI.dashboards.Count - 1)
				carController.selectedDashboard = 0;
			
			currentUIManager.currentDashboard = carController.selectedDashboard;
			
			if (currentUIManager.inGameUI.gameUI.playerCarPlaceholder && carController.carImage)
				currentUIManager.inGameUI.gameUI.playerCarPlaceholder.texture = carController.carImage;

			playerController = carController;
			
			raceTrackLenght = roadType == GameHelper.RoadType.AutoGeneration ? finishFlagDistance : CarHelper.Distance(GetCorrectTransform(playerController), finishFlag.transform.position);

			var sectorsCount = (int)raceTrackLenght / 50;
			sectorsCount -= 1;

			for (int i = 1; i < sectorsCount; i++)
			{
				intermediatePositions.Add(raceTrackLenght * i / sectorsCount);
			}
			
			startPlayerDistance = CarHelper.Distance(GetCorrectTransform(playerController), roadType == GameHelper.RoadType.AutoGeneration ? playerSpawnPoint.transform.position + playerSpawnPoint.transform.forward.normalized * finishFlagDistance : finishFlag.transform.position);
			
#if !DR_CINEMACHINE
		if (mainCamera)
  		mainCamera.transform.parent = car.transform;

#else
			if (virtualCamera)
			{
				virtualCamera.Follow = car.transform;
				virtualCamera.LookAt = car.transform;

				var noiseStage = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

				if (noiseProfile != null)
				{
					noiseStage.m_NoiseProfile = noiseProfile;
				}
			}

#endif
			playerCar = car.transform;
		}

		private void LoadOpponent(bool replaceNetworkPlayer)
		{
			var isRaycast = Physics.Raycast(opponentSpawnPoint.transform.position, Vector3.down, out var hitInfo);

			GameObject car = null;

			var savedOpponentIndex = gameAssets.valuesToSave.currentOpponent;
			var index = gameAssets.randomOpponents || savedOpponentIndex > gameAssets.opponentCarsList.Count - 1 ? Random.Range(0, gameAssets.opponentCarsList.Count) : savedOpponentIndex;
			
			if (!replaceNetworkPlayer)
			{
#if !DR_MULTIPLAYER
				var carPrefab = gameAssets.opponentCarsList[index].vehicleController;
				car = Instantiate(carPrefab.gameObject, isRaycast ? hitInfo.point + Vector3.up * 0.01f : opponentSpawnPoint.transform.position, opponentSpawnPoint.transform.rotation * Quaternion.Euler(0, -carPrefab.rotationOffset, 0));
			}
#else
				if (PhotonNetwork.PlayerListOthers.Length == 0)
				{
					var carPrefab = gameAssets.opponentCarsList[index].vehicleController;
					car = Instantiate(carPrefab.gameObject, isRaycast ? hitInfo.point + Vector3.up * 0.01f : opponentSpawnPoint.transform.position, opponentSpawnPoint.transform.rotation * Quaternion.Euler(0, -carPrefab.rotationOffset, 0));
				}
			}
			else
			{
				networkOpponentWasReplaced = true;
				var racingPosition = opponentSpawnPoint.transform.position + playerController.drivingDirection * distanceBetweenOpponentAndStart;
				racingPosition.y = playerCar.position.y;
				
				var startPosition = isRaycast ? hitInfo.point + Vector3.up * 0.01f : opponentSpawnPoint.transform.position;
				var carPrefab = gameAssets.carsList[networkCarIndex != -1 ? networkCarIndex : Random.Range(0, gameAssets.carsList.Count)].vehicleController;
				car = Instantiate(carPrefab.gameObject, gameStarted ? racingPosition : startPosition, opponentSpawnPoint.transform.rotation * Quaternion.Euler(0, -carPrefab.rotationOffset, 0));
			}
#endif

			if (car == null) return;
			
			var carController = car.GetComponent<VehicleController>();
			carController.ai = true;
			carController.drivingDirection = opponentSpawnPoint.transform.forward;
			carController.gameManager = this;

			if (replaceNetworkPlayer)
			{
				if (gameStarted)
				{
					carController.currentSpeed = networkSpeed != -1 ? networkSpeed : playerController.currentSpeed;
					
					if(!hasOpponentFinished)
						carController.startDriving = true;
					
					carController.isFinished = hasOpponentFinished;
				}
			}

			if(!gameAssets.randomOpponents)
				GameHelper.LoadOpponentCarParameters(carController, playerController);

			if (currentUIManager.inGameUI.gameUI.opponentCarPlaceholder && carController.carImage)
				currentUIManager.inGameUI.gameUI.opponentCarPlaceholder.texture = carController.carImage;

			opponentTime = 0;
			opponentCar = car.transform;
			opponentController = carController;
			
			startOpponentDistance = CarHelper.Distance(!replaceNetworkPlayer ? GetCorrectTransform(opponentController) : opponentSpawnPoint.transform, roadType == GameHelper.RoadType.AutoGeneration ? opponentSpawnPoint.transform.position + opponentSpawnPoint.transform.forward.normalized * finishFlagDistance : finishFlag.transform.position);

			if (carController.avatarPlaceholder)
			{
				if (!replaceNetworkPlayer)
				{
					if (gameAssets.randomOpponents)
					{
						if (gameAssets.avatarsList.Count > 0)
						{
							var avatar = gameAssets.avatarsList[Random.Range(0, gameAssets.avatarsList.Count)];
							carController.avatarPlaceholder.texture = avatar;
							opponentAvatar = avatar;
						}
					}
					else
					{
						carController.avatarPlaceholder.texture = gameAssets.opponentCarsList[index].avatar;
						opponentAvatar = gameAssets.opponentCarsList[index].avatar;
					}
				}
				else
				{
					if(hasOpponentFinished)
						carController.avatarPlaceholder.gameObject.SetActive(false);
					
					carController.avatarPlaceholder.texture = opponentAvatar;
				}
			}

			if (carController.nicknamePlaceholder)
			{
				if (!replaceNetworkPlayer)
				{
					carController.nicknamePlaceholder.text = gameAssets.randomOpponents ? "AI" : gameAssets.opponentCarsList[index].name;
					opponentName = carController.nicknamePlaceholder.text;
				}
				else
				{
					if(hasOpponentFinished)
						carController.nicknamePlaceholder.gameObject.SetActive(false);
					
					carController.nicknamePlaceholder.text = opponentName + " (AI)";
				}
			}

// #endif
		}

		public void SwitchGear(string type)
		{
			if (!gameStarted) return;
			
			if (type == "+") playerController.NextTransmission();
			else playerController.PreviousTransmission();
		}

		public void EnableNitro()
		{
			if (!gameStarted) return;

			playerController.EnableNitro();
		}

		void ChangeEnginePitchAtStart(AudioSource audioSource, float defaultEngineVolume, bool isPressed)
		{
			if (audioSource)
			{
				if (isPressed)
				{
					if (audioSource.pitch < 1.7f)
						audioSource.pitch += 1 * Time.deltaTime;

					if (audioSource.volume < defaultEngineVolume * 1.1f)
						audioSource.volume += 1 * Time.deltaTime;
				}
				else
				{
					if (audioSource.pitch > 1)
						audioSource.pitch -= 2 * Time.deltaTime;

					if (audioSource.volume > defaultEngineVolume / 2)
						audioSource.volume -= 2 * Time.deltaTime;
				}
			}
		}

		public void PressGasUIButton (string type)
		{
			pressGasUI = type == "+";
			pressGas = pressGasUI;
		}

		public void PressGas(string type)
		{
			pressGas = type == "+";
		}

		void Pause()
		{
			if (hasPlayerFinished) return;

			if (!pause)
			{
				pause = true;
				SwitchMenu("pause");

				if (!opponentController.multiplayerCar)
				{
					Time.timeScale = 0;
					AudioListener.pause = true;
				}
			}
			else
			{
				currentUIManager.inGameUI.gameUI.ActivateAll(currentUIManager.currentDashboard);

				if (currentUIManager.inGameUI.gameUI.countdownText && gameStarted)
					currentUIManager.inGameUI.gameUI.countdownText.gameObject.SetActive(false);

				if (currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].gasButton && gameStarted)
					currentUIManager.inGameUI.gameUI.dashboards[currentUIManager.currentDashboard].gasButton.gameObject.SetActive(false);

				if (currentUIManager.inGameUI.gameUI.gamePopUp)
					currentUIManager.inGameUI.gameUI.gamePopUp.gameObject.SetActive(false);

				currentUIManager.inGameUI.pauseMenu.DisableAll();
				pause = false;
				isOptionsMenuOpened = false;

				Time.timeScale = 1;
				AudioListener.pause = false;
			}
		}
		
		private void OptionsMenu()
		{
			isOptionsMenuOpened = !isOptionsMenuOpened;
			SwitchMenu(isOptionsMenuOpened ? "options" : "pause");
		}
		
		void SwitchMenu(string type)
		{
			currentUIManager.HideAllInGameUI();
			currentUIManager.HideAllMenuUI();
            
			switch (type)
			{
				case "pause":
					isOptionsMenuOpened = false;
					currentUIManager.inGameUI.pauseMenu.ActivateAll();
					break;
                
				case "options":
					currentUIManager.menuUI.settingsMenu.ActivateAll();
					break;
			}
		}

		private IEnumerator ShowGameOverMenu()
		{
			currentUIManager.HideAllInGameUI();
			
			while (true)
			{
#if DR_MULTIPLAYER
				if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 1)
				{
					ShowMenu(!hasOpponentFinished);

					StopCoroutine(ShowGameOverMenu());
					break;
				}
				else if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
				{
					ShowMenu(!hasOpponentFinished);

					StopCoroutine(ShowGameOverMenu());
					break;
				}
				else if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2)
				{
#endif
					if (opponentTime != -1)
					{
						if(opponentController.multiplayerCar) ShowMenu(opponentTime > playerTime);
						else ShowMenu(!hasOpponentFinished);
						
						StopCoroutine(ShowGameOverMenu());
						break;
					}
					else if (opponentTime == -1 && getEventFromAnotherPlayer)
					{
						ShowMenu(true);
						StopCoroutine(ShowGameOverMenu());
						break;
					}
#if DR_MULTIPLAYER
				}
#endif

				yield return 0;
			}
		}

		void ShowMenu(bool isLocalPlayerWinner)
		{
			currentUIManager.inGameUI.gameOver.ActivateAll();
			
			var generalPoints = 0;
			var generalMoney = 0;
			
			if (!isLocalPlayerWinner)
			{
				currentUIManager.inGameUI.gameOver.result.text = "Defeat";
				lossEvent.Invoke();
			}
			else
			{
				currentUIManager.inGameUI.gameOver.result.text = "Victory";
				victoryEvent.Invoke();
			}

			if (opponentController.avatarPlaceholder)
				opponentController.avatarPlaceholder.gameObject.SetActive(false);

			if (opponentController.nicknamePlaceholder)
				opponentController.nicknamePlaceholder.gameObject.SetActive(false);

			var time = networkOpponentWasReplaced ? independentOpponentTime : opponentTime;
			
			if (currentUIManager.inGameUI.gameOver.secondPlayerStats && currentUIManager.inGameUI.gameOver.firstPlayerStats)
			{
				if (!isLocalPlayerWinner)
				{
					if (currentUIManager.inGameUI.gameOver.secondPlayerBackground)
						currentUIManager.inGameUI.gameOver.secondPlayerBackground.color = currentUIManager.inGameUI.gameOver.currentPlayerHighlight;

					GameHelper.SetAvatars(currentUIManager, gameAssets, opponentAvatar, gameAssets.avatarsList, false);

					currentUIManager.inGameUI.gameOver.secondPlayerStats.text = gameAssets.valuesToSave.nickname + " |  Time - " + playerTime.ToString("G");

					currentUIManager.inGameUI.gameOver.firstPlayerStats.text = opponentName + " |  Time - " + time.ToString("G");
				}
				else
				{
					if (currentUIManager.inGameUI.gameOver.firstPlayerBackground)
						currentUIManager.inGameUI.gameOver.firstPlayerBackground.color = currentUIManager.inGameUI.gameOver.currentPlayerHighlight;

					var curOpponent = gameAssets.valuesToSave.currentOpponent;
					
					if (!opponentController.multiplayerCar && !gameAssets.randomOpponents && curOpponent <= gameAssets.opponentCarsList.Count - 1)
					{
						curOpponent++;
						gameAssets.valuesToSave.currentOpponent++; 
					}

					GameHelper.SetAvatars(currentUIManager, gameAssets, opponentAvatar, gameAssets.avatarsList, true);

					currentUIManager.inGameUI.gameOver.firstPlayerStats.text = gameAssets.valuesToSave.nickname + " |  Time - " + playerTime.ToString("G");

					if(opponentController.multiplayerCar)
						currentUIManager.inGameUI.gameOver.secondPlayerStats.text = opponentName + "  |  Time - " + (time == -1 ? "not finished" : time.ToString("G"));
					else 
						currentUIManager.inGameUI.gameOver.secondPlayerStats.text = opponentName + "  |  Time - " + (!hasOpponentFinished ? "not finished" : time.ToString("G"));
				}
			}

			if (currentUIManager.inGameUI.gameOver.raceProfit)
			{
				if (isLocalPlayerWinner) currentUIManager.inGameUI.gameOver.raceProfit.text = "Race Profit: " + "<color=#00FF21FF>" + gameAssets.scoreValues.winRace + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + gameAssets.moneyValues.winRace + "</color>";
				else currentUIManager.inGameUI.gameOver.raceProfit.text = "Race Profit: " + "<color=#00FF21FF>" + gameAssets.scoreValues.loseRace + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + gameAssets.moneyValues.loseRace + "</color>";
			}

			if (currentUIManager.inGameUI.gameOver.perfectStart)
			{
				if (playerPerfectStart) currentUIManager.inGameUI.gameOver.perfectStart.text = "Perfect Start: " + "<color=#00FF21FF>" + gameAssets.scoreValues.perfectStart + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + gameAssets.moneyValues.perfectStart + "</color>";
				else if (playerGoodStart) currentUIManager.inGameUI.gameOver.perfectStart.text = "Good Start: " + "<color=#00FF21FF>" + gameAssets.scoreValues.goodStart + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + gameAssets.moneyValues.goodStart + "</color>";
				else currentUIManager.inGameUI.gameOver.perfectStart.text = "Perfect Start: 0 points | $0";
			}

			if (currentUIManager.inGameUI.gameOver.perfectShifts)
			{
				if (perfectShifts > 0) currentUIManager.inGameUI.gameOver.perfectShifts.text = "Perfect Shifts: <color=#00FF21FF>x" + perfectShifts + "</color><color=#FFFFFFFF>" + " - " + "</color><color=#00FF21FF>" + gameAssets.scoreValues.perfectShift * perfectShifts + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + gameAssets.moneyValues.perfectShift * perfectShifts + "</color>";
				else currentUIManager.inGameUI.gameOver.perfectShifts.text = "Perfect Shifts: 0 points | $0";

			}

			if (currentUIManager.inGameUI.gameOver.distanceBonus)
			{
				if (isLocalPlayerWinner)
				{
					var distance = Vector3.Distance(playerCar.transform.position, opponentCar.transform.position);
					currentUIManager.inGameUI.gameOver.distanceBonus.text = "Distance Bonus: " + "<color=#00FF21FF>" + (gameAssets.scoreValues.distanceBonus * distance).ToString("00") + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + (gameAssets.moneyValues.distanceBonus * distance).ToString("00") + "</color>";
				}
				else
				{
					currentUIManager.inGameUI.gameOver.distanceBonus.text = "Distance Bonus: 0 points | $0";
				}
			}

			if (playerPerfectStart)
			{
				generalPoints += gameAssets.scoreValues.perfectStart;
				generalMoney += gameAssets.moneyValues.perfectStart;
			}
			else if (playerGoodStart)
			{
				generalPoints += gameAssets.scoreValues.goodStart;
				generalMoney += gameAssets.moneyValues.goodStart;
			}

			if (perfectShifts > 0)
			{
				generalPoints += gameAssets.scoreValues.perfectShift * perfectShifts;
				generalMoney += gameAssets.moneyValues.perfectShift * perfectShifts;
			}

			if (isLocalPlayerWinner)
			{
				generalPoints += gameAssets.scoreValues.winRace;
				generalMoney += gameAssets.moneyValues.winRace;

				var distance = Vector3.Distance(playerCar.transform.position, opponentCar.transform.position);

				generalPoints += (int) (gameAssets.scoreValues.distanceBonus * distance);
				generalMoney += (int) (gameAssets.moneyValues.distanceBonus * distance);

			}
			else
			{
				generalPoints += gameAssets.scoreValues.loseRace;
				generalMoney += gameAssets.moneyValues.loseRace;
			}

			gameAssets.valuesToSave.currentScore += generalPoints;
			gameAssets.valuesToSave.currentMoney += generalMoney;

			if (currentUIManager.inGameUI.gameOver.generalRaceProfit)
				currentUIManager.inGameUI.gameOver.generalRaceProfit.text = "<color=#00FF21FF>+ " + generalPoints + " points" + "</color><color=#FFFFFFFF>" + " | " + "</color><color=#00FF21FF>" + "$" + generalMoney + "</color>";

			if (gameAssets.valuesToSave.currentScore > gameAssets.levels[gameAssets.valuesToSave.currentLevel].limits.y && gameAssets.valuesToSave.currentLevel + 1 <= gameAssets.levels.Count - 1)
			{
				GameHelper.CalculateLevel(gameAssets);

				if (currentUIManager.inGameUI.gameOver.newLevelPopup)
				{
					UIHelper.EnableAllParents(currentUIManager.inGameUI.gameOver.newLevelPopup.gameObject);
					currentUIManager.inGameUI.gameOver.newLevelPopup.text = "New Level - " + (gameAssets.valuesToSave.currentLevel + 1 + "!");
				}
			}
			else
			{
				if (currentUIManager.inGameUI.gameOver.newLevelPopup)
					currentUIManager.inGameUI.gameOver.newLevelPopup.gameObject.SetActive(false);
			}
			
			GameAssets.SaveDataToFile(gameAssets.valuesToSave);
		}

		void Exit()
		{
			StopAllCoroutines();

			Time.timeScale = 1;

#if DR_MULTIPLAYER
			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
			{
				PhotonNetwork.LeaveRoom();
				SceneManager.LoadScene(0);
			}
			else
			{
				SceneManager.LoadScene(0);
			}
#else
		SceneManager.LoadScene(0);
#endif
		}

		private void SpawnRoad(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var newRoad = Instantiate(roadPrefabs[Random.Range(0, roadPrefabs.Count)], roadController, true);
				newRoad.transform.position = lastRoadPart.transform.position + playerSpawnPoint.transform.forward * roadLength;
				newRoad.transform.rotation = lastRoadPart.transform.rotation;
				lastRoadPart = newRoad;
				
			}
		}

		private void SpawnFlag()
		{
			var flag  = finishFlag ? Instantiate(finishFlag) : new GameObject("Finish Line");
			flag.transform.position = playerSpawnPoint.transform.position + playerSpawnPoint.transform.forward * finishFlagDistance;
			flag.transform.rotation = Quaternion.identity;
		}

		public IEnumerator DisableGamePopup()
		{
			yield return new WaitForSeconds(2);

			if (currentUIManager.inGameUI.gameUI.gamePopUp)
				currentUIManager.inGameUI.gameUI.gamePopUp.gameObject.SetActive(false);

			StopCoroutine(DisableGamePopup());
		}

		IEnumerator FindOpponentTimer()
		{
			var startTimer = 0f;
			var statusText = "";

#if DR_CINEMACHINE
			if (virtualCamera)
			{
				var noiseStage = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

				if (noiseStage.m_NoiseProfile != null)
				{
					noiseProfile = noiseStage.m_NoiseProfile;
					noiseStage.m_NoiseProfile = null;
				}
			}
#endif

			while (true)
			{
				findPlayersTimer += Time.deltaTime;

#if DR_MULTIPLAYER
				if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount < 2 && findPlayersTimer <= findOpponentsTimerValue)
				{
					statusText = "Finding an opponent...";
				}
				else if (!PhotonNetwork.IsConnected)
				{
					statusText = "No Internet Connection" + "\n" + "Starting a game with AI...";

					startTimer += Time.deltaTime;

					if (startTimer > 2)
					{
						LoadGame(); 
						break;
					}
				}
				else if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
				{
					statusText = "No Connection to the Server" + "\n" + "Starting a game with AI...";

					if (findPlayersTimer > 2)
					{
						LoadGame();
						break;
					}
				}
				else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
				{
					statusText = "Opponent has been founded!" + "\n" + "Starting a game...";

					startTimer += Time.deltaTime;

					if (startTimer > 2)
					{
						LoadGame();
						break;
					}
				}
				else if (findPlayersTimer > findOpponentsTimerValue)
				{
					statusText = "Opponent has not been founded! " + "\n" + "Starting a game with AI...";

					if (findPlayersTimer > findOpponentsTimerValue + 2)
					{
						LoadGame();
						break;
					}
				}
#else
			statusText = "Starting a game with AI...";

				if (findPlayersTimer > 2)
				{
					LoadGame();
					break;
				}
#endif
				if (currentUIManager.inGameUI.preGameTimer.status)
					currentUIManager.inGameUI.preGameTimer.status.text = statusText;

				if (currentUIManager.inGameUI.preGameTimer.timer)
					currentUIManager.inGameUI.preGameTimer.timer.text = findPlayersTimer.ToString("00");

				yield return 0;
			}
		}

#if DR_MULTIPLAYER
		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				if (propertiesThatChanged.ContainsKey("StartTimer"))
				{
					startTime = (int) PhotonNetwork.CurrentRoom.CustomProperties["StartTimer"];
					hasStartTime = true;
				}
			}
		}
#endif
	}
}
