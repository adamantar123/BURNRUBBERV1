using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.DragRacingFramework
{
	public static class CarHelper
	{

		[Serializable]
		public class CarInfo
		{
			public int MaxSpeed = 250;
			public int Acceleration = 100;
			public int Power = 100;
			public int nitroTime = 3;
			public int Mass = 1100;
		}

		[Serializable]
		public class Gear
		{
			public int maxTransferSpeed;
			public float enginePitch = 1;
			public AudioClip specificEngineSound;
		}

		public enum BodyRotationAxis
		{
			X,
			Y,
			Z
		}

		public static float Distance(Transform car, Vector3 targetPoint)
		{
			var relativePos = car.InverseTransformPoint(targetPoint);

			var cosAngle = Mathf.Atan2(relativePos.z, relativePos.x);

			var horizontalOffset = Mathf.Sin(cosAngle) * relativePos.magnitude;

			return horizontalOffset;
		}

		public static float CarUIPosition(RawImage roadPlaceholder, RawImage carPlaceholder, float distanceBetweenCarAndFlag, float startDistanceBetweenCarAndFlag)
		{
			var distancePercent = (startDistanceBetweenCarAndFlag - distanceBetweenCarAndFlag) * 100 / startDistanceBetweenCarAndFlag;

			var leftBorder = roadPlaceholder.rectTransform.rect.xMin + roadPlaceholder.rectTransform.anchoredPosition.x;
			var rightBorder = roadPlaceholder.rectTransform.rect.xMax + roadPlaceholder.rectTransform.anchoredPosition.x;

			var carUIPosition = leftBorder + roadPlaceholder.rectTransform.rect.width * distancePercent / 100 + carPlaceholder.rectTransform.rect.width / 2 + 5;

			if (carUIPosition > rightBorder - carPlaceholder.rectTransform.sizeDelta.x / 2)
				carUIPosition = rightBorder - carPlaceholder.rectTransform.sizeDelta.x / 2;
			// else if (carUIPosition < leftBorder + carPlaceholder.rectTransform.rect.width / 2)
			// 	carUIPosition = leftBorder + carPlaceholder.rectTransform.rect.width / 2;

			return carUIPosition;
		}

		public static AudioSource CreateAudioSourse(Transform parent, string name)
		{
			var audioSource = new GameObject(name).AddComponent<AudioSource>();

			audioSource.transform.parent = parent;
			audioSource.transform.localPosition = Vector3.zero;

			return audioSource;
		}

		public static float CalculatePitch(VehicleController vehicleController, bool maxEngineSpeed)
		{
			var pitch = 0f;

			var multiplier = Mathf.Abs(vehicleController.currentTransmission - vehicleController.predictedTransmission);

			if (multiplier >= 2)
			{
				ChangePitch(vehicleController.gears[vehicleController.predictedTransmission], ref pitch);
				pitch /= 1.2f;
			}
			else
			{
				ChangePitch(vehicleController.gears[vehicleController.currentTransmission], ref pitch);

				if (maxEngineSpeed)
				{
					pitch /= 1.2f;
				}
			}

			return pitch;
		}

		static void ChangePitch(Gear gear, ref float pitch)
		{
			pitch = gear.enginePitch;
			
			// switch (transmission)
			// {
			// 	case 1:
			// 		pitch = 1.1f;
			// 		break;
			// 	case 2:
			// 		pitch = 1.2f;
			// 		break;
			// 	case 3:
			// 		pitch = 1.4f;
			// 		break;
			// 	case 4:
			// 		pitch = 1.6f;
			// 		break;
			// 	case 5:
			// 		pitch = 1.8f;
			// 		break;
			// 	case 6:
			// 		pitch = 2f;
			// 		break;
			// }
		}

		// public static void ChangeEnginePitch(int gear, AudioSource audioSource, bool maxEngineSpeed, float currentSpeed, float maxTransferSpeed, bool ai)
		// {
		// 	switch (gear)
		// 	{
		// 		case 1:
		// 			audioSource.pitch = 1.1f;
		// 			break;
		// 		case 2:
		// 			audioSource.pitch = 1.2f;
		// 			break;
		// 		case 3:
		// 			audioSource.pitch = 1.4f;
		// 			break;
		// 		case 4:
		// 			audioSource.pitch = 1.6f;
		// 			break;
		// 		case 5:
		// 			audioSource.pitch = 1.8f;
		// 			break;
		// 		case 6:
		// 			audioSource.pitch = 2f;
		// 			break;
		// 	}
		//
		// 	var multiplier = (int)(maxTransferSpeed / currentSpeed);
		// 	
		// 	if (maxEngineSpeed)
		// 	{
		// 		audioSource.pitch /= 1.2f;
		// 	}
		// 	else if (multiplier > 2)
		// 	{
		// 		audioSource.pitch /= 1.5f + multiplier / 5;
		// 	}
		// }

		public static void CalculatePredictedTransmission(List<Gear> gears, float currentSpeed, ref int predictedTransmission)
		{
			for (var i = 0; i < gears.Count; i++)
			{
				if (i == 0)
				{
					if (currentSpeed < gears[i].maxTransferSpeed)
						predictedTransmission = 0;
				}
				else if (i < gears.Count - 1)
				{
					if (currentSpeed > gears[i].maxTransferSpeed && currentSpeed < gears[i + 1].maxTransferSpeed)
						predictedTransmission = i;
				}
				else
				{
					if (currentSpeed > gears[i].maxTransferSpeed)
						predictedTransmission = gears.Count - 1;
				}
			}

			// if (currentSpeed < 30)
			// 	predictedTransmission = 1;
			// else if (currentSpeed > 30 && currentSpeed < 60)
			// 	predictedTransmission = 2;
			// else if (currentSpeed > 60 && currentSpeed < 90)
			// 	predictedTransmission = 3;
			// else if (currentSpeed > 90 && currentSpeed < 120)
			// 	predictedTransmission = 4;
			// else if (currentSpeed > 120 && currentSpeed < 160)
			// 	predictedTransmission = 5;
			// else if (currentSpeed > 160)
			// 	predictedTransmission = 6;
		}

		public static void CalculateTransferValues(Gear currentGear, ref float maxTransferSpeed)
		{

			maxTransferSpeed = currentGear.maxTransferSpeed;
			// switch (currentGear)
			// {
			// 	case 1:
			// 		maxTransferSpeed = 30f;
			// 		break;
			// 	case 2:
			// 		maxTransferSpeed = 60f;
			// 		break;
			// 	case 3:
			// 		maxTransferSpeed = 90f;
			// 		break;
			// 	case 4:
			// 		maxTransferSpeed = 120f;
			// 		break;
			// 	case 5:
			// 		maxTransferSpeed = 160;
			// 		break;
			// 	case 6:
			// 		maxTransferSpeed = 200;
			// 		break;
			// }
		}

		public static void CorrectCarPosition(VehicleController vehicleController, float targetValue, Vector3 targetPos, float distanceBetweenPlayers, ref bool sendIntermediateTime)
		{
			if (targetValue > 0 && distanceBetweenPlayers < targetValue || targetValue < 0 && distanceBetweenPlayers > targetValue)
			{
				vehicleController.transform.position = Vector3.MoveTowards(vehicleController.transform.position, targetPos, 3 * Time.deltaTime);
				
				if(targetValue > 0)
					vehicleController.RotateCarBody(vehicleController.drivingRotation);
				else vehicleController.RotateCarBody(vehicleController.drivingRotation / 2);
				
				var dist = Distance(vehicleController.gameManager.GetCorrectTransform(vehicleController), targetPos);
				
				if (Mathf.Abs(dist) < 1)
				{
					sendIntermediateTime = false;
				}
			}
			else
			{
				sendIntermediateTime = false;
			}
		}

		public static float CalculateSpeedAdditionalValue(int predictedTransmission)
		{
			var additionalValue = 0f;

			switch (predictedTransmission)
			{
				case 1:
					additionalValue = 1.1f;
					break;

				case 2:
					additionalValue = 1.9f;
					break;

				case 3:
					additionalValue = 3f;
					break;

				case 4:
					additionalValue = 2.5f;
					break;

				case 5:
					additionalValue = 3.2f;
					break;

				case 6:
					additionalValue = 3.8f;
					break;
			}

			return additionalValue;
		}

		public static float GetArrowAngle(float maxTransferSpeed, float currentSpeed, GameManager gameManager)
		{
			var ang = Mathf.Lerp(gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerLimits.y, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerLimits.x, Mathf.InverseLerp(0, maxTransferSpeed, currentSpeed));
			return (ang > 180 ? ang - 360 : ang);
		}
	}
}
