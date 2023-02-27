using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace GercStudio.DragRacingFramework
{
	public class GameAssets : ScriptableObject
	{
		public List<GameHelper.Car> carsList;
		public List<GameHelper.Car> opponentCarsList;
		public List<Texture> avatarsList;

		public GameHelper.ScoreAndMoneyValues scoreValues;
		public GameHelper.ScoreAndMoneyValues moneyValues;

		public List<GameHelper.LevelParameters> levels;
		
		public int selectedAvatar;
		public bool randomOpponents;

		[SerializeField] public ValuesToSave valuesToSave;

		[Serializable]
		public class ValuesToSave
		{
			public int currentScore;
			public int currentMoney;
			public int currentLevel;
			public int selectedCar = -1;
			public int currentOpponent;
			
			public string nickname = "Nickname" ;

			public List<string> purchasedCars = new List<string>();
			public UpgradesDictionary installedUpgrades = new UpgradesDictionary();

			public void ResetAllData()
			{
				currentScore = 0;
				currentMoney = 1000;
				currentLevel = 0;

				nickname = "Nickname";

				currentOpponent = 0;

				selectedCar = -1;
			
				purchasedCars.Clear();
				installedUpgrades.Clear();
			}
		}

		[Serializable]
		public class UpgradesDictionary: Dictionary<string, int>, ISerializationCallbackReceiver
		{
			[HideInInspector][SerializeField] private List<string> _keys = new List<string>();
			[HideInInspector][SerializeField] private List<int> _values = new List<int>();

			public void OnBeforeSerialize()
			{
				_keys.Clear();
				_values.Clear();
    
				foreach (var kvp in this)
				{
					_keys.Add(kvp.Key);
					_values.Add(kvp.Value);
				}
			}
    
			public void OnAfterDeserialize()
			{
				Clear();
    
				for (var i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
				{
					Add(_keys[i], _values[i]);
				}
			}
		}

		public static void SaveDataToFile(ValuesToSave valuesToSave)
		{

			var path = Application.persistentDataPath + "/GameData.json";
			var json = JsonUtility.ToJson(valuesToSave);
			
			System.IO.File.WriteAllText(path, json);
			
#if DRF_ES3_INTEGRATION
			ES3.Save("gameData", valuesToSave);
#endif
		}

		public static void LoadDataFromFile(ref ValuesToSave valuesToSave)
		{
			
#if !DRF_ES3_INTEGRATION
			var path = Application.persistentDataPath + "/GameData.json";
			
			if(!System.IO.File.Exists(path)) return;
			
			var fileContents = System.IO.File.ReadAllText(path);

			valuesToSave = JsonUtility.FromJson<ValuesToSave>(fileContents);
#else
			valuesToSave = ES3.Load("gameData", new ValuesToSave());
#endif
			
		}
	}
}
