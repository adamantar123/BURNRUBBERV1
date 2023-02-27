using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Random = UnityEngine.Random;

namespace GercStudio.DragRacingFramework
{
	public static class MultiplayerHelper
	{

		public static string GenerateRandomName()
		{
			const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

			var pickUpId = "";

			for (int i = 0; i < 20; i++)
			{
				pickUpId += glyphs[Random.Range(0, glyphs.Length)];
			}

			return pickUpId;
		}

		public enum PhotonEventCodes
		{
			// SendAudioVolume = 0,
			// SendAudioPitch = 1,
			// PressAccelerationPedal = 2,
			SendTime = 0,
			// SendCarBodyRotation = 4,
			IsFinished = 1, 
			SendAnswer = 2,
		}


		public static string GetHtmlFromUri(string resource)
		{
			var html = string.Empty;
			var req = (HttpWebRequest) WebRequest.Create(resource);
			try
			{
				using (var resp = (HttpWebResponse) req.GetResponse())
				{
					var isSuccess = (int) resp.StatusCode < 299 && (int) resp.StatusCode >= 200;
					if (isSuccess)
					{
						using (var reader = new StreamReader(resp.GetResponseStream()))
						{
							var cs = new char[80];
							reader.Read(cs, 0, cs.Length);
							foreach (var ch in cs)
							{
								html += ch;
							}
						}
					}
				}
			}
			catch
			{
				return "";
			}

			return html;
		}

		public static List<string> PhotonRegions = new List<string>()
		{
			"Asia",
			"Australia",
			"Canada, East",
			"Europe",
			"India",
			"Japan",
			"Russia",
			"Russia, East",
			"South America",
			"South Korea",
			"USA, East",
			"USA, West"
		};

		public static int ConvertCodeToRegion(string value)
		{
			switch (value)
			{
				case "asia":
					return 0;

				case "au":
					return 1;

				case "cae":
					return 2;

				case "eu":
					return 3;

				case "in":
					return 4;

				case "jp":
					return 5;

				case "ru":
					return 6;

				case "rue":
					return 7;

				case "sa":
					return 8;

				case "kr":
					return 9;

				case "us":
					return 10;

				case "usw":
					return 11;
			}

			return 0;
		}

		public static string ConvertRegionToCode(int value)
		{
			switch (value)
			{
				case 0:
					return "asia";

				case 1:
					return "au";

				case 2:
					return "cae";

				case 3:
					return "eu";

				case 4:
					return "in";

				case 5:
					return "jp";

				case 6:
					return "ru";

				case 7:
					return "rue";

				case 8:
					return "sa";

				case 9:
					return "kr";

				case 10:
					return "us";

				case 11:
					return "usw";
			}

			return "";
		}
	}
}
