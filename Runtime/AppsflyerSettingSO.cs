using com.ktgame.core;
using UnityEngine;

namespace com.ktgame.services.appsflyer
{
	public class AppsflyerSettingSO : ServiceSettingsSingleton<AppsflyerSettingSO>
	{
		public const string DefaultAppsflyerVersion = "6.18.1";

		public override string PackageName => GetType().Namespace;
		
		[HideInInspector] public string DevKey;
		[HideInInspector] public string AppID;
		[HideInInspector] public bool IsDebug;
		[HideInInspector] public bool IsGetConversionData;

		[HideInInspector] public string AppsflyerVersion = DefaultAppsflyerVersion;
	}
}
