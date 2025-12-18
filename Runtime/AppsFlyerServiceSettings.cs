using com.ktgame.core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.services.appsflyer
{
	public class AppsFlyerServiceSettings : ServiceSettingsSingleton<AppsFlyerServiceSettings>
	{
		public override string PackageName => GetType().Namespace;
		[VerticalGroup("AppsFlyer")] [SerializeField] private string _keyAppsflyerDev;
		[VerticalGroup("AppsFlyer")] [SerializeField] private string _appsflyerAppID;
		[VerticalGroup("AppsFlyer")] [SerializeField] private bool _isDebug;
		[VerticalGroup("AppsFlyer")] [SerializeField] private bool _isGetConversionData;
		
		public string DevKey => _keyAppsflyerDev;
		public string AppID => _appsflyerAppID;
		public bool IsDebug => _isDebug;
		public bool IsGetConversionData => _isGetConversionData;
	}
}
