using System;
using System.Collections;
using System.Collections.Generic;

#if DR_MULTIPLAYER
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
#endif

using UnityEngine;

namespace GercStudio.DragRacingFramework
{

	public class EventsManager : 
#if DR_MULTIPLAYER
		MonoBehaviourPun
#else
		MonoBehaviour
#endif
	{
		[HideInInspector] public GameManager gameManager;

#if DR_MULTIPLAYER
		private void OnEnable()
		{
			PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
		}

		private void OnDisable()
		{
			PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
		}

		void OnEvent(EventData photonEvent)
		{
			var eventCode = (MultiplayerHelper.PhotonEventCodes) photonEvent.Code;
			object[] data = photonEvent.CustomData as object[];

			if (data == null) return;

			switch (eventCode)
			{
				case MultiplayerHelper.PhotonEventCodes.SendTime:
					gameManager.intermediateOpponentTimes.Add((float) data[0]);
					SendEvent(MultiplayerHelper.PhotonEventCodes.SendAnswer);
					break;

				case MultiplayerHelper.PhotonEventCodes.SendAnswer:
					gameManager.getEventFromAnotherPlayer = true;
					break;

				case MultiplayerHelper.PhotonEventCodes.IsFinished:

					gameManager.hasOpponentFinished = true;
					gameManager.opponentController.Finish();
					gameManager.opponentFinishedEvent.Invoke();
					gameManager.opponentTime = (float) data[0];

					if (gameManager.hasPlayerFinished && gameManager.currentUIManager.inGameUI.gameOver.secondPlayerStats)
						gameManager.currentUIManager.inGameUI.gameOver.secondPlayerStats.text = gameManager.opponentName + " |  Time - " + gameManager.opponentTime.ToString("G");

					SendEvent(MultiplayerHelper.PhotonEventCodes.SendAnswer);
					break;
			}
		}

		public void SendEvent(MultiplayerHelper.PhotonEventCodes eventCode)
		{
			if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) return;

			RaiseEventOptions options = new RaiseEventOptions
			{
				CachingOption = EventCaching.DoNotCache,
				Receivers = ReceiverGroup.Others
			};

			object[] content = { };

			switch (eventCode)
			{
				case MultiplayerHelper.PhotonEventCodes.SendTime:
					content = new object[] {gameManager.playerTime};
					gameManager.getEventFromAnotherPlayer = false;
					break;
				case MultiplayerHelper.PhotonEventCodes.SendAnswer:
					content = new object[] { };
					PhotonNetwork.SendAllOutgoingCommands();
					break;
				case MultiplayerHelper.PhotonEventCodes.IsFinished:
					content = new object[] {gameManager.playerTime};
					gameManager.getEventFromAnotherPlayer = false;
					break;
			}

			PhotonNetwork.RaiseEvent((byte) eventCode, content, options, SendOptions.SendReliable);

		}
#endif
	}
}
