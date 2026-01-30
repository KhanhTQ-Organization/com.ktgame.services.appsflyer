using com.ktgame.core;
using UnityEngine;

namespace com.ktgame.services.appsflyer
{
	public class AppsflyerSettingSO : ServiceSettingsSingleton<AppsflyerSettingSO>
	{
		public override string PackageName => GetType().Namespace;
		
		[HideInInspector] public string DevKey;
		[HideInInspector] public string AppID;
		[HideInInspector] public bool IsDebug;
		[HideInInspector] public bool IsGetConversionData;
	}
}
