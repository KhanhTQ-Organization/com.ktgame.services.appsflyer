using System;
using UnityEngine;

namespace com.ktgame.services.appsflyer
{
	public static class VariableDefineService
	{
		private const string PlayerPrefPlayerVn = "playerpref_player_vn";
		private const string PlayerPrefPlayerOrganic = "playerpref_player_organic";
		private const string PlayerPrefConversionDataReceived = "playerpref_appsflyer_conversion_received";
		
		public static event Action OnConversionDataReceived;

		public static bool IsVnCountry
		{
			get => PlayerPrefs.GetInt(PlayerPrefPlayerVn, 0) == 1;
			set => PlayerPrefs.SetInt(PlayerPrefPlayerVn, value ? 1 : 0);
		}
		
		public static bool IsOrganicUser
		{
			get => PlayerPrefs.GetInt(PlayerPrefPlayerOrganic, 0) == 1;
			set => PlayerPrefs.SetInt(PlayerPrefPlayerOrganic, value ? 1 : 0);
		}
		
		public static bool HasConversionData => PlayerPrefs.GetInt(PlayerPrefConversionDataReceived, 0) == 1;

		internal static void MarkConversionDataReceived()
		{
			PlayerPrefs.SetInt(PlayerPrefConversionDataReceived, 1);
			PlayerPrefs.Save();

			try
			{
				OnConversionDataReceived?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
