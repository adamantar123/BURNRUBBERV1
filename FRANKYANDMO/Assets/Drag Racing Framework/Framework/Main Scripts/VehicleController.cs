using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if DR_MULTIPLAYER
using Photon.Pun;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.DragRacingFramework
{
#if DR_MULTIPLAYER
	// [RequireComponent(typeof(PhotonView))]
#endif
	// [RequireComponent(typeof(BoxCollider))]
	public class VehicleController : 
#if DR_MULTIPLAYER
			MonoBehaviourPun, IPunObservable
#else
			MonoBehaviour
#endif
	{
		public CarHelper.CarInfo carInfo = new CarHelper.CarInfo();

		public GameManager gameManager;

		public Transform rotationPart;
		public List<Transform> wheels = new List<Transform>();

		public CarHelper.BodyRotationAxis bodyRotationAxis;
		public CarHelper.BodyRotationAxis wheelsRotationAxis;

		public List<CarHelper.Gear> gears = new List<CarHelper.Gear>();

		public UIManager uiManagerAsset;

		[Range(0.1f, 2)] public float bodyRotationSpeed = 0.5f;
		[Range(-3, 3)] public float drivingRotation = -1.5f;
		[Range(-3, 3)] public float brakingRotation = 1.5f;
		[Range(0, 180)] public float rotationOffset;
		public float defaultEngineVolume;
		public float targetEngineVolume;
		public float maxTransferSpeed;
		public float currentNitro;
		public float currentTacho;
		public float currentSpeed;
		public float startNitroTimeout = 1f;

		public int selectedDashboard;
		public int currentTransmission = 0;
		public int predictedTransmission;
		public int currentInspectorTab; //inspector variable

		public bool isNitro;
		public bool isFinished;
		public bool ai;
		public bool multiplayerCar;
		public bool startDriving;
		public bool firstInstance;

		public string carId;

		public Vector3 desiredRotation;
		public Vector3 defaultBodyRotation;
		public Vector3 drivingDirection;

		public Texture carImage;

		public RawImage avatarPlaceholder;
		public Text nicknamePlaceholder;

		public AudioSource switchGearAudioSource;
		public AudioSource engineAudioSource;
		public AudioSource nitroAudioSource;

		public AudioClip switchGearAudioClip;
		public AudioClip engineAudioClip;
		public AudioClip nitroAudioClip;

		private float elapsed;
		private float speedInLastFrame;
		private float estimatedSpeed;
		private float currentAcceleration;
		private float switchGearBodyRotation;
		private float currentAngle;
		private float nitroCounterTime;
		private float lowerEngineSoundTimer;
		private float correctPositionTimeout;

		private float networkSpeed;
		private float networkBodyRotation;

		private int randomNitroChance = 90;

		private bool isMenu;
		private bool lowerEngineSound;
		

		private void Start()
		{
			if(GetComponent<Rigidbody>())
				Destroy(GetComponent<Rigidbody>());
			
			if(GetComponent<BoxCollider>())
				Destroy(GetComponent<BoxCollider>());
			
			if (FindObjectOfType<MenuManager>())
			{
				isMenu = true;
				return;
			}

			currentNitro = carInfo.nitroTime;

			currentTransmission = 0;

			CarHelper.CalculateTransferValues(gears[currentTransmission], ref maxTransferSpeed);

			if (switchGearAudioSource && switchGearAudioClip)
				switchGearAudioSource.clip = switchGearAudioClip;

			if (engineAudioSource)
			{
				if (engineAudioClip)
					engineAudioSource.clip = engineAudioClip;

				engineAudioSource.loop = true;
				

				var volume = engineAudioSource.volume;
				defaultEngineVolume = volume;

				volume /= 2;
				engineAudioSource.volume = volume;

				engineAudioSource.Play();

				if (!ai)
					engineAudioSource.priority += 50;
			}

			if (nitroAudioSource && nitroAudioClip)
				nitroAudioSource.clip = nitroAudioClip;

			defaultBodyRotation = rotationPart.localEulerAngles;
		}

		void Update()
		{
			
			if (isMenu)
				return;

			if (ai && multiplayerCar)
			{
				if (!isFinished)
				{
					currentSpeed = networkSpeed;
					
					var rotationPartLocalEulerAngles = rotationPart.localEulerAngles;
				
					switch (bodyRotationAxis)
					{
						case CarHelper.BodyRotationAxis.X:
							rotationPartLocalEulerAngles.x = networkBodyRotation;
							break;
						case CarHelper.BodyRotationAxis.Y:
							rotationPartLocalEulerAngles.y = networkBodyRotation;
							break;
						case CarHelper.BodyRotationAxis.Z:
							rotationPartLocalEulerAngles.z = networkBodyRotation;
							break;
					}

					rotationPart.localEulerAngles = rotationPartLocalEulerAngles;
				}
				else StopDriving();
			}
		}

		private void FixedUpdate()
		{
			if (isMenu)
				return;

			elapsed += Time.deltaTime * bodyRotationSpeed;
			var rotatingPartLocalEulerAngles = rotationPart.localEulerAngles;

			if (ai)
			{
				if (avatarPlaceholder)
				{
#if DR_CINEMACHINE
					avatarPlaceholder.transform.LookAt(gameManager.virtualCamera.transform);
#else
				avatarPlaceholder.transform.LookAt(gameManager.mainCamera.transform);
#endif
					avatarPlaceholder.transform.Rotate(Vector3.up, 180);
				}

				if (nicknamePlaceholder)
				{
#if DR_CINEMACHINE
					nicknamePlaceholder.transform.LookAt(gameManager.virtualCamera.transform);
#else
				nicknamePlaceholder.transform.LookAt(gameManager.mainCamera.transform);
#endif
					nicknamePlaceholder.transform.Rotate(Vector3.up, 180);
				}
			}

			if (lowerEngineSound)
			{
				lowerEngineSoundTimer += Time.deltaTime;

				if (lowerEngineSoundTimer > 0.7f)
					lowerEngineSound = false;
			}
			
			switch (bodyRotationAxis)
			{
				case CarHelper.BodyRotationAxis.X:
					rotatingPartLocalEulerAngles.x = Mathf.LerpAngle(rotationPart.localEulerAngles.x, desiredRotation.x, elapsed);
					break;
				case CarHelper.BodyRotationAxis.Y:
					rotatingPartLocalEulerAngles.y = Mathf.LerpAngle(rotationPart.localEulerAngles.y, desiredRotation.y, elapsed);
					break;
				case CarHelper.BodyRotationAxis.Z:
					rotatingPartLocalEulerAngles.z = Mathf.LerpAngle(rotationPart.localEulerAngles.z, desiredRotation.z, elapsed);
					break;
			}

			if (!multiplayerCar)
				rotationPart.localEulerAngles = rotatingPartLocalEulerAngles;

			currentAcceleration = (currentSpeed - speedInLastFrame) / Time.deltaTime;
			speedInLastFrame = currentSpeed;

			if (gameManager && (!gameManager.pause && gameManager.gameStarted && (startDriving || !startDriving && (!ai && (gameManager.playerPerfectStart || gameManager.playerGoodStart) || ai && !multiplayerCar && gameManager.opponentPerfectStart) || ai && multiplayerCar)))
			{
				var rotationAxis = Vector3.zero;
				
				switch (wheelsRotationAxis)
				{
					case CarHelper.BodyRotationAxis.X:
						rotationAxis = transform.right;
						break;
					case CarHelper.BodyRotationAxis.Y:
						rotationAxis = transform.forward;
						break;
					case CarHelper.BodyRotationAxis.Z:
						rotationAxis = transform.up;
						break;
				}
				
				foreach (var wheel in wheels)
				{
					wheel.Rotate(rotationAxis * currentSpeed * 0.3f);
				}
			}

			if (!multiplayerCar)
				CalculateTachometerArrowAngle(maxTransferSpeed, currentSpeed);

			if (currentSpeed < 0) currentSpeed = 0;
			else if (currentSpeed > carInfo.MaxSpeed) currentSpeed = carInfo.MaxSpeed;

			if (!ai)
			{
				ShowNitroArrowAngle();

				if (gameManager.gameStarted)
				{
					if (startDriving || !startDriving && (gameManager.playerPerfectStart || gameManager.playerGoodStart))
					{
						transform.position += 0.25f * currentSpeed * Time.deltaTime * drivingDirection;
					}

					if (startDriving && !gameManager.pause)
					{
						ShowSpeedometerArrowAngle(currentSpeed);

						Driving();
					}
				}
				else //if player presses on the acceleration pedal at start
				{
					if (gameManager.pressGas)
					{
						currentTransmission = 0;
						currentSpeed += Time.deltaTime * 25;

						if (currentSpeed > maxTransferSpeed)
							currentSpeed = maxTransferSpeed;

						RotateCarBody(drivingRotation);
					}
					else
					{
						currentSpeed -= Time.deltaTime * 10;

						RotateCarBody(0);

						if (currentSpeed < 0)
							currentSpeed = 0;
					}
				}
			}
			else //Car AI Controller
			{
				if (gameManager.gameStarted && !multiplayerCar && !gameManager.pause || multiplayerCar)
				{
					if ((startDriving || !startDriving && gameManager.opponentPerfectStart))
					{
						transform.position += 0.25f * currentSpeed * Time.deltaTime * drivingDirection;
					}

					if (startDriving)
					{
						if(!multiplayerCar)
							Driving();
					}
				}
			}
		}

		public void RotateCarBody(float rotationAngle)
		{
			if (Math.Abs(currentAngle - rotationAngle) > 0.1f)
				elapsed = 0f;

			var xAngle = bodyRotationAxis == CarHelper.BodyRotationAxis.X ? rotationAngle : 0;
			var yAngle = bodyRotationAxis == CarHelper.BodyRotationAxis.Y ? rotationAngle : 0;
			var zAngle = bodyRotationAxis == CarHelper.BodyRotationAxis.Z ? rotationAngle : 0;

			desiredRotation = new Vector3(defaultBodyRotation.x + xAngle, defaultBodyRotation.y + yAngle, defaultBodyRotation.z + zAngle);
			currentAngle = rotationAngle;
		}

		public void ShowNitroArrowAngle()
		{
			if (gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].nitrometerArrow.transform != null)
			{
				var ang = Mathf.Lerp(gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].nitrometerLimits.y, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].nitrometerLimits.x, Mathf.InverseLerp(0, carInfo.nitroTime, currentNitro));
				gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].nitrometerArrow.transform.eulerAngles = new Vector3(0, 0, ang);
			}
		}

		public void CalculateNitro()
		{
			currentNitro -= Time.deltaTime;
		}

		public void ShowSpeedometerArrowAngle(float currentSpeed)
		{
			if (gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].speedometerArrow.transform)
			{
				var ang = Mathf.Lerp(gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].speedometerLimits.y, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].speedometerLimits.x, Mathf.InverseLerp(gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].minSpeed, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].maxSpeed, currentSpeed));
				gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].speedometerArrow.transform.eulerAngles = (currentSpeed < carInfo.MaxSpeed - 5 && currentSpeed < maxTransferSpeed) ? new Vector3(0, 0, ang) : new Vector3(0, 0, ang - Random.Range(-1f, 1f));
			}
		}

		public void NextTransmission()
		{
			if (currentTransmission >= gears.Count - 1 || currentSpeed > carInfo.MaxSpeed - 5)
				return;

			if (!ai && switchGearAudioSource)
				switchGearAudioSource.Play();

			currentTransmission++;
			lowerEngineSound = true;
			lowerEngineSoundTimer = 0;

			if (!ai)
				gameManager.gearUpEvent.Invoke();

			CarHelper.CalculateTransferValues(gears[currentTransmission], ref maxTransferSpeed);

			if (engineAudioSource && gears[currentTransmission].specificEngineSound)
			{
					engineAudioSource.clip = gears[currentTransmission].specificEngineSound;
			}

			if (!ai && gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].currentGearText)
			{
				gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].currentGearText.text = (currentTransmission + 1).ToString();

				if (GameHelper.IsBetween(currentTacho, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].perfectShiftRange.x, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].perfectShiftRange.y))
				{
					gameManager.perfectShifts++;

					gameManager.perfectShiftEvent.Invoke();
					
					targetEngineVolume = defaultEngineVolume * 2;

					StopAllCoroutines();
					StartCoroutine(AddSpeedAfterPerfectShift());

					if (gameManager.currentUIManager.inGameUI.gameUI.gamePopUp)
					{
						if (gameManager.currentUIManager.inGameUI.gameUI.gamePopUp.gameObject.activeSelf) StopCoroutine(gameManager.DisableGamePopup());
						else UIHelper.EnableAllParents(gameManager.currentUIManager.inGameUI.gameUI.gamePopUp.gameObject);

						gameManager.currentUIManager.inGameUI.gameUI.gamePopUp.text = "Perfect Shift";
						StartCoroutine(gameManager.DisableGamePopup());
					}
				}
			}
		}

		public void PreviousTransmission()
		{
			if (currentTransmission > 0)
			{
				currentTransmission--;

				if (!ai && switchGearAudioSource)
					switchGearAudioSource.Play();

				if (!ai)
					gameManager.gearDownEvent.Invoke();
			}

			CarHelper.CalculateTransferValues(gears[currentTransmission], ref maxTransferSpeed);
			
			if (engineAudioSource && gears[currentTransmission].specificEngineSound)
			{
				engineAudioSource.clip = gears[currentTransmission].specificEngineSound;
			}

			if (!ai && gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].currentGearText)
				gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].currentGearText.text = (currentTransmission + 1).ToString();
		}

		public void CalculateTachometerArrowAngle(float maxTransferSpeed, float currentSpeed)
		{
			var maxEngineSpeed = false;

			if (currentSpeed < carInfo.MaxSpeed - 5)
				estimatedSpeed = currentSpeed;
			else
				estimatedSpeed += 10 * Time.deltaTime;
			

			var ang = Mathf.Lerp(gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerLimits.y, gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerLimits.x, Mathf.InverseLerp(0, maxTransferSpeed, estimatedSpeed));

			if (gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerArrow.transform && !ai)
			{
				gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerArrow.transform.eulerAngles = estimatedSpeed < maxTransferSpeed ? new Vector3(0, 0, ang) : new Vector3(0, 0, ang - Random.Range(-1f, 2f));
			}

			// check slippage 
			ang = ang > 180 ? ang - 360 : ang;

			if (Math.Abs(ang - gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerLimits.x) < 20)
			{
				maxEngineSpeed = true;
			}

			currentTacho = ang;

			if (gameManager.gameStarted && !isFinished)
			{
				CarHelper.CalculatePredictedTransmission(gears, this.currentSpeed, ref predictedTransmission);
				 
				if (engineAudioSource)
				{
					if(!lowerEngineSound)
						engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, CarHelper.CalculatePitch(this, maxEngineSpeed), 1 * Time.deltaTime);
					else engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, engineAudioSource.pitch / 1.1f, 2 * Time.deltaTime);
					
					engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, targetEngineVolume, 10 * Time.deltaTime);
				}
			}
		}

		public void Finish()
		{
			isFinished = true;

			StopAllCoroutines();

			if (engineAudioSource && !multiplayerCar)
			{
				StartCoroutine(ReduceEngineSound());
			}

#if DR_MULTIPLAYER
			if (!multiplayerCar)
			{
				gameManager.eventsManager.SendEvent(MultiplayerHelper.PhotonEventCodes.IsFinished);
			}
#endif

		}

		public void EnableNitro()
		{
			if (!isFinished)
			{
				isNitro = true;

				if (!ai) gameManager.startUsingNitroEvent.Invoke();

				if (nitroAudioSource)
					nitroAudioSource.Play();
			}
		}

		private void CheckNextGear()
		{
			if (currentTransmission >= gears.Count - 1 || currentSpeed > carInfo.MaxSpeed - 5)
				return;

			var currentTacho = CarHelper.GetArrowAngle(maxTransferSpeed, currentSpeed, gameManager);

			if (Math.Abs(currentTacho - gameManager.currentUIManager.inGameUI.gameUI.dashboards[gameManager.currentUIManager.currentDashboard].tachometerLimits.x) < 20)
			{
				var perfectSwitchChance = Random.Range(0, 2);

				if (perfectSwitchChance == 1)
				{
					StopAllCoroutines();
					targetEngineVolume = defaultEngineVolume * 2;
					StartCoroutine(AddSpeedAfterPerfectShift());
				}

				NextTransmission();
			}
		}

		private void DrivingRotation()
		{
			if (currentSpeed < maxTransferSpeed - 5f)
			{
				var additionalRotation = Mathf.Abs(currentAcceleration) / (!isNitro ? 7 : 4);

				if (additionalRotation > 1)
					additionalRotation = 1;
				else if (additionalRotation < 0.5f)
					additionalRotation = 0;

				if (drivingRotation >= 0)
					additionalRotation *= -1;

				RotateCarBody(drivingRotation - additionalRotation - switchGearBodyRotation);
			}
			else if (currentSpeed >= maxTransferSpeed - 5f)
			{
				RotateCarBody(drivingRotation / 2);
			}
		}

		private void RandomNitroChance()
		{
			var random = Random.Range(0, 100);

			if (random > randomNitroChance)
			{
				isNitro = true;

				if (nitroAudioSource)
					nitroAudioSource.Play();
			}
		}

		private void Driving()
		{
			if (currentNitro > 0f && isNitro)
			{
				CalculateNitro();
			}
			else
			{
				if (isNitro)
				{
					isNitro = false;
					gameManager.endUsingNitroEvent.Invoke();

					if (nitroAudioSource)
						nitroAudioSource.Stop();
				}
			}

			if (!isFinished)
			{
				if (isNitro)
				{
					if (currentSpeed <= maxTransferSpeed)
					{
						var speed = (carInfo.Power / carInfo.Mass * 10f + carInfo.Acceleration / 200f) / ((currentTransmission + 1) * 1.5f);
						speed *= 1.5f; //CarHelper.CalculateSpeedAdditionalValue(predictedTransmission);
						currentSpeed += speed;
					}
					else
					{
						var speed = (carInfo.Power / carInfo.Mass * 10f + carInfo.Acceleration / 200f) / ((currentTransmission + 1) * 1.5f);
						speed *= 1.5f; //CarHelper.CalculateSpeedAdditionalValue(predictedTransmission) / 2f;
						currentSpeed -= speed;
					}
				}
				else
				{
					if (currentSpeed <= maxTransferSpeed)
					{
						var speed = (carInfo.Power / carInfo.Mass * 10f + carInfo.Acceleration / 200f) / ((currentTransmission + 1) * 1.5f);
						currentSpeed += speed;
					}
					else
					{
						currentSpeed -= 0.035f * (21f - (currentTransmission + 1) * 3.5f);
					}
				}

				DrivingRotation();

				if (ai && !multiplayerCar)
				{
					CheckNextGear();

					nitroCounterTime += Time.deltaTime;

					if (nitroCounterTime >= startNitroTimeout)
					{
						nitroCounterTime = 0f;
						RandomNitroChance();
					}
				}
			}
			else
			{
				StopDriving();
			}
		}

		private void StopDriving()
		{
			if (nitroAudioSource)
				nitroAudioSource.Stop();

			if (currentSpeed > 0)
			{
				currentSpeed -= 60 * Time.deltaTime;
				RotateCarBody(brakingRotation);
			}
			else if (Math.Abs(currentSpeed) <= 0)
			{
				RotateCarBody(0);
			}
		}

		IEnumerator AddSpeedAfterPerfectShift()
		{
			var targetSpeed = currentSpeed * (!ai ? 1.1f : 1.12f);

			switchGearBodyRotation = -drivingRotation / 2;
			StartCoroutine(AddRotationAfterPerfectShift());

			while (true)
			{
				currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, (!ai ? 3 : 4) * Time.deltaTime);

				if (Math.Abs(currentSpeed - targetSpeed) < 0.5f)
				{
					StopCoroutine(AddSpeedAfterPerfectShift());
					break;
				}

				yield return 0;
			}
		}

		IEnumerator AddRotationAfterPerfectShift()
		{
			yield return new WaitForSeconds(1);
			switchGearBodyRotation = 0;
			targetEngineVolume = defaultEngineVolume;
			StopCoroutine(AddRotationAfterPerfectShift());
		}

		IEnumerator ReduceEngineSound()
		{
			var targetVolume = engineAudioSource.volume / 2;

			while (true)
			{
				engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, 1, 1 * Time.deltaTime);
				engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, targetVolume, 1 * Time.deltaTime);

				if (Math.Abs(engineAudioSource.pitch - 1) < 0.1f && Math.Abs(engineAudioSource.volume - targetVolume) < 0.1f)
				{
					engineAudioSource.pitch = 1;
					engineAudioSource.volume = targetVolume;
					StopCoroutine(ReduceEngineSound());
					break;
				}

				yield return 0;
			}
		}

#if DR_MULTIPLAYER
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				if (!isFinished)
				{
					stream.SendNext(currentSpeed);

					switch (bodyRotationAxis)
					{
						case CarHelper.BodyRotationAxis.X:
							stream.SendNext(rotationPart.localEulerAngles.x);
							break;
						case CarHelper.BodyRotationAxis.Y:
							stream.SendNext(rotationPart.localEulerAngles.y);
							break;
						case CarHelper.BodyRotationAxis.Z:
							stream.SendNext(rotationPart.localEulerAngles.z);
							break;
					}

					stream.SendNext(engineAudioSource.pitch);
					stream.SendNext(engineAudioSource.volume);
					stream.SendNext(currentTransmission);
				}
			}
			else
			{
				if (stream.Count > 0)
				{
					networkSpeed = (float) stream.ReceiveNext();

					if (gameManager) gameManager.networkSpeed = networkSpeed;

					networkBodyRotation = (float) stream.ReceiveNext();

					engineAudioSource.pitch = (float) stream.ReceiveNext();
					engineAudioSource.volume = (float) stream.ReceiveNext();
					currentTransmission = (int) stream.ReceiveNext();
				}
			}
		}
#endif

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (gameManager && Selection.gameObjects.Contains(gameManager.gameObject) || Selection.gameObjects.Contains(gameObject))
			{
				var offset = transform.rotation * Quaternion.Euler(0, rotationOffset, 0);
					
				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(0, 255, 0, 255);
				Handles.ArrowHandleCap(0, transform.position, offset, 4, EventType.Repaint);
				Handles.SphereHandleCap(0, transform.position, Quaternion.identity, 0.5f, EventType.Repaint);


				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(0, 255, 0, 100);
				Handles.ArrowHandleCap(0, transform.position, offset, 4, EventType.Repaint);
				Handles.SphereHandleCap(0, transform.position, Quaternion.identity, 0.5f, EventType.Repaint);
			}
		}
#endif
	}
}
