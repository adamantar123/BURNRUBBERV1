using System;
using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.DragRacingFramework
{
	public static class GameHelper
	{

		public enum RoadType
		{
			AutoGeneration,
			SetManually
		}

		public enum UpgradesType
		{
			AddPercent,
			AddValue
		}

		[Serializable]
		public class LevelParameters
		{
			[MinMaxSlider(0, 10000)] public Vector2 limits;
		}

		[Serializable]
		public class DREvent : UnityEvent
		{
		}

		[Serializable]
		public class UpgradeParameter
		{
			public int price = 500;
			public int level = 1;

			public int addSpeedValue = 1;
			public int addAccelerationValue = 1;
			public int addPowerValue = 1;
			public int addNitroValue = 10;
			public int addMassValue = -1;

			public bool purchased;
		}

		[Serializable]
		public class AllUpgradeParameters
		{
			public List<UpgradeParameter> engineUpgrades;
			public List<UpgradeParameter> turboUpgrades;
			public List<UpgradeParameter> transmissionUpgrades;
			public List<UpgradeParameter> nitroUpgrades;
			public List<UpgradeParameter> weightUpgrades;
		}

		[Serializable]
		public class Car
		{
			public VehicleController vehicleController;
			public int price = 100;
			public int level = 1;
			public string name;
			public Texture avatar;
		}

		[Serializable]
		public class ScoreAndMoneyValues
		{
			public int winRace;
			public int loseRace;
			public int perfectStart;
			public int goodStart;
			public int perfectShift;
			public int distanceBonus;
		}

// 		[Serializable]
// 		public class Map
// 		{
// #if UNITY_EDITOR
// 			public SceneAsset scene;
// #endif
// 			public string name;
// 		}

#if UNITY_EDITOR
		[MenuItem("GameObject/Drag Racing Framework/Spawn Point", false, 1)]
		public static void CreateSpawnZone()
		{
			var zone = new GameObject("Spawn Point");
			zone.AddComponent<SpawnPoint>();

			if (SceneView.lastActiveSceneView)
			{
				var transform = SceneView.lastActiveSceneView.camera.transform;
				zone.transform.position = transform.position + transform.forward * 10;
			}

			EditorGUIUtility.PingObject(zone);
		}

		[MenuItem("GameObject/Drag Racing Framework/Game Manager", false, 1)]
		public static void GameManager()
		{
			var gameManager = new GameObject("Game Manager");
			var manager = gameManager.AddComponent<GameManager>();

			var eventManager = new GameObject("EventSystem");
			eventManager.AddComponent<EventSystem>();
			eventManager.AddComponent<StandaloneInputModule>();
			eventManager.transform.parent = gameManager.transform;
			eventManager.transform.localPosition = Vector3.zero;
			eventManager.transform.localEulerAngles = Vector3.zero;

			manager.mainCamera = new GameObject("Camera").AddComponent<Camera>();

			EditorGUIUtility.PingObject(gameManager);
		}

#if !DR_MULTIPLAYER
	[MenuItem("Tools/Drag Racing Framework/Integrations/[OFF] PUN Multiplayer", false, 1000)]
	public static void ActivateMultiplayer()
	{
		var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		newSymbols += ";DR_MULTIPLAYER";
		
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newSymbols);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, newSymbols);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newSymbols);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newSymbols);

	}
#else
		[MenuItem("Tools/Drag Racing Framework/Integrations/[ON] PUN Multiplayer", false, 1000)]
		public static void ActivateMultiplayer()
		{
			var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

			if (newSymbols.Contains("DR_MULTIPLAYER"))
			{
				var replace = newSymbols.Replace("DR_MULTIPLAYER", "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, replace);
			}
		}
#endif
		
		
#if !DRF_ES3_INTEGRATION
		[MenuItem("Tools/Drag Racing Framework/Integrations/[OFF] Easy Save", false, 1000)]
		public static void ActivateESIntegration()
		{
			var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			newSymbols += ";DRF_ES3_INTEGRATION";
		
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newSymbols);

		}
#else
		[MenuItem("Tools/Drag Racing Framework/Integrations/[ON] Easy Save", false, 1000)]
		public static void ActivateESIntegration()
		{
			var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

			if (newSymbols.Contains("DRF_ES3_INTEGRATION"))
			{
				var replace = newSymbols.Replace("DRF_ES3_INTEGRATION", "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, replace);
			}
		}
#endif	

		
#if !DR_CINEMACHINE
	[MenuItem("Tools/Drag Racing Framework/Integrations/[OFF] Cinemachine", false, 1000)]
	public static void ActivateCinemachine()
	{
		var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		newSymbols += ";DR_CINEMACHINE";
		
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newSymbols);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, newSymbols);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newSymbols);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newSymbols);
	}
#else
		[MenuItem("Tools/Drag Racing Framework/Integrations/[ON] Cinemachine", false, 1000)]
		public static void ActivateCinemachine()
		{
			var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			
			if (newSymbols.Contains("DR_CINEMACHINE"))
			{
				var replace = newSymbols.Replace("DR_CINEMACHINE", "");

				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, replace);
			}
		}
#endif
		
#if !DR_INPUT_SYSTEM
		[MenuItem("Tools/Drag Racing Framework/Integrations/[OFF] Input System", false, 1000)]
		public static void ActivateInputSystem()
		{
			var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			newSymbols += ";DR_INPUT_SYSTEM";
		
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newSymbols);
		}
#else
		[MenuItem("Tools/Drag Racing Framework/Integrations/[ON] Input System", false, 1000)]
		public static void ActivateInputSystem()
		{
			var newSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			
			if (newSymbols.Contains("DR_INPUT_SYSTEM"))
			{
				var replace = newSymbols.Replace("DR_INPUT_SYSTEM", "");

				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, replace);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, replace);
			}
		}
#endif

		[MenuItem("Tools/Drag Racing Framework/Project Settings", false, 1000)]
		public static void empty()
		{

		}
		
		[MenuItem("Tools/Drag Racing Framework/Integrations/To [Enable] and [Disable] modules, click on the corresponding line.", true)]
		private static bool epty2Validate() {
			return false;
		}
		
		[MenuItem("Tools/Drag Racing Framework/Integrations/To [Enable] and [Disable] modules, click on the corresponding line.", false, -1000)]
		public static void empty2()
		{
			
		}

		[MenuItem("Tools/Drag Racing Framework/Project Settings/UI Manager #u", false, 0)]
		public static void UI()
		{
			if(Application.isPlaying) return;
			
			var ui = Resources.Load("UI Manager", typeof(UIManager)) as UIManager;

			if (ui)
			{
				ui.gameObject.SetActive(false);
				EditorGUIUtility.PingObject(ui);
				AssetDatabase.OpenAsset(ui);
				UnityEditor.SceneView.RepaintAll();
				

			}
		}
		
		[MenuItem("Tools/Drag Racing Framework/Project Settings/Input Manager #i", false, 2)]
		public static void InputManager()
		{
			if(Application.isPlaying) return;
            
			var inputs = Resources.Load("Input Manager", typeof(InputSettings)) as InputSettings;
			Selection.activeObject = inputs;
			EditorGUIUtility.PingObject(inputs);
		}
#endif

		[Serializable]
		public class CameraPosition
		{
			public Vector3 position;
			public Quaternion rotation;
		}

		public static bool IsBetween(float value, float bound1, float bound2)
		{
			return value >= Math.Min(bound1, bound2) && value <= Math.Max(bound1, bound2);
		}

		public static void LoadCarParameters(VehicleController vehicleController)
		{
			vehicleController.carInfo.MaxSpeed = vehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[vehicleController.carId + "speed"];
			vehicleController.carInfo.Acceleration = vehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[vehicleController.carId + "acceleration"];
			vehicleController.carInfo.Power = vehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[vehicleController.carId + "power"];
			vehicleController.carInfo.nitroTime = vehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[vehicleController.carId + "nitro"];
			vehicleController.carInfo.Mass = vehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[vehicleController.carId + "mass"];
		}

		public static void LoadOpponentCarParameters(VehicleController opponentVehicleController, VehicleController playerVehicleController)
		{
			opponentVehicleController.carInfo.MaxSpeed = playerVehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[playerVehicleController.carId + "speed"];
			opponentVehicleController.carInfo.Acceleration = playerVehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[playerVehicleController.carId + "acceleration"];
			opponentVehicleController.carInfo.Power = playerVehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[playerVehicleController.carId + "power"];
			opponentVehicleController.carInfo.nitroTime = playerVehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[playerVehicleController.carId + "nitro"];
			opponentVehicleController.carInfo.Mass = playerVehicleController.gameManager.gameAssets.valuesToSave.installedUpgrades[playerVehicleController.carId + "mass"];
		}

		public static void LoadCarParameters(VehicleController vehicleController, GameAssets gameData, AllUpgradeParameters upgradesParameters, UpgradesType upgradesType)
		{
			AddParameters("engineStage", vehicleController, gameData, upgradesParameters.engineUpgrades, upgradesType);
			AddParameters("turboStage", vehicleController, gameData, upgradesParameters.turboUpgrades, upgradesType);
			AddParameters("transmissionStage", vehicleController, gameData, upgradesParameters.transmissionUpgrades, upgradesType);
			AddParameters("nitroStage", vehicleController, gameData, upgradesParameters.nitroUpgrades, upgradesType);
			AddParameters("weightStage", vehicleController, gameData, upgradesParameters.weightUpgrades, upgradesType);
		}

		static void AddParameters(string type, VehicleController vehicleController, GameAssets gameData, List<UpgradeParameter> upgradesParameters, UpgradesType upgradeType)
		{
			var stage = gameData.valuesToSave.installedUpgrades[vehicleController.carId + type];

			if (stage < 0) return;

			for (var i = 0; i <= stage; i++)
			{
				if (upgradeType == UpgradesType.AddValue)
				{
					vehicleController.carInfo.MaxSpeed += upgradesParameters[stage].addSpeedValue;
					vehicleController.carInfo.Acceleration += upgradesParameters[stage].addAccelerationValue;
					vehicleController.carInfo.Power += upgradesParameters[stage].addPowerValue;
					vehicleController.carInfo.nitroTime += upgradesParameters[stage].addNitroValue;
					vehicleController.carInfo.Mass += upgradesParameters[stage].addMassValue;
				}
				else
				{
					AddPercent(ref vehicleController.carInfo.MaxSpeed, upgradesParameters[stage].addSpeedValue);
					AddPercent(ref vehicleController.carInfo.Acceleration, upgradesParameters[stage].addAccelerationValue);
					AddPercent(ref vehicleController.carInfo.Power, upgradesParameters[stage].addPowerValue);
					AddPercent(ref vehicleController.carInfo.nitroTime, upgradesParameters[stage].addNitroValue);
					AddPercent(ref vehicleController.carInfo.Mass, upgradesParameters[stage].addMassValue);
				}
			}
		}

		public static void AddPercent(ref int currentValue, int upgradeValue)
		{
			var addedValue = 1 + (float) upgradeValue / 100;
			float curValue = currentValue;
			curValue *= addedValue;
			currentValue = (int) curValue;
		}

		public static void SaveCarParameters(GameAssets gameData, VehicleController vehicleController)
		{
			gameData.valuesToSave.installedUpgrades[vehicleController.carId + "speed"] = vehicleController.carInfo.MaxSpeed;
			gameData.valuesToSave.installedUpgrades[vehicleController.carId + "acceleration"] = vehicleController.carInfo.Acceleration;
			gameData.valuesToSave.installedUpgrades[vehicleController.carId + "power"] = vehicleController.carInfo.Power;
			gameData.valuesToSave.installedUpgrades[vehicleController.carId + "nitro"] = vehicleController.carInfo.nitroTime;
			gameData.valuesToSave.installedUpgrades[vehicleController.carId + "mass"] = vehicleController.carInfo.Mass;
		}
		
		public static string CorrectName(string currentName)
		{
			var name = currentName;
					
			if (name.Contains("(Clone)"))
			{
				var replace = name.Replace("(Clone)", "");
				name = replace;
			}

			return name;
		}

		public static void CheckCarParameters(VehicleController vehicleController)
		{
			if (vehicleController.carInfo.MaxSpeed < 1)
				vehicleController.carInfo.MaxSpeed = 1;

			if (vehicleController.carInfo.Acceleration < 1)
				vehicleController.carInfo.Acceleration = 1;

			if (vehicleController.carInfo.Power < 1)
				vehicleController.carInfo.Power = 1;

			if (vehicleController.carInfo.nitroTime < 1)
				vehicleController.carInfo.nitroTime = 1;

			if (vehicleController.carInfo.Mass < 1)
				vehicleController.carInfo.Mass = 1;
		}

		public static void CalculateLevel(GameAssets gameAssets)
		{
			for (var i = gameAssets.valuesToSave.currentLevel; i <= gameAssets.levels.Count - 1; i++)
			{
				var level = gameAssets.levels[i];

				if (gameAssets.valuesToSave.currentScore < level.limits.y || gameAssets.valuesToSave.currentScore > level.limits.y && i == gameAssets.levels.Count - 1)
				{
					gameAssets.valuesToSave.currentLevel = i;
					
					GameAssets.SaveDataToFile(gameAssets.valuesToSave);
					
					break;
				}
			}
		}

		public static void SetAvatars(UIManager currentUIManager, GameAssets gameData, Texture opponentAvatar, List<Texture> avatarsList, bool isPlayerWon)
		{
			if (!isPlayerWon)
			{
				if (currentUIManager.inGameUI.gameOver.firstPlayerAvatarPlaceholder)
				{
					if (opponentAvatar)
						currentUIManager.inGameUI.gameOver.firstPlayerAvatarPlaceholder.texture = opponentAvatar;
					else
					{
						if (avatarsList.Count > 0)
							currentUIManager.inGameUI.gameOver.firstPlayerAvatarPlaceholder.texture = avatarsList[Random.Range(0, avatarsList.Count)];
					}
				}

				if (currentUIManager.inGameUI.gameOver.secondPlayerAvatarPlaceholder)
				{
					currentUIManager.inGameUI.gameOver.secondPlayerAvatarPlaceholder.texture = avatarsList[gameData.selectedAvatar];
				}
			}
			else
			{
				if (currentUIManager.inGameUI.gameOver.secondPlayerAvatarPlaceholder)
				{
					if (opponentAvatar)
						currentUIManager.inGameUI.gameOver.secondPlayerAvatarPlaceholder.texture = opponentAvatar;
					else
					{
						if (avatarsList.Count > 0)
							currentUIManager.inGameUI.gameOver.secondPlayerAvatarPlaceholder.texture = avatarsList[Random.Range(0, avatarsList.Count)];
					}
				}

				if (currentUIManager.inGameUI.gameOver.firstPlayerAvatarPlaceholder)
				{
					currentUIManager.inGameUI.gameOver.firstPlayerAvatarPlaceholder.texture = avatarsList[gameData.selectedAvatar];
				}
			}
		}
		
		public static bool ReachedPosition(Vector3 position1, Vector3 position2)
		{
			return Math.Abs(position1.x - position2.x) < 1 && Math.Abs(position1.y - position2.y) < 1 && Math.Abs(position1.z - position2.z) < 1;
		}
	}
}
