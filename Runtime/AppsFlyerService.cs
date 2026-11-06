using com.ktgame.core;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if APPSFLYER
using AppsFlyerSDK;
#endif

namespace com.ktgame.services.appsflyer
{
	[Service(typeof(IAppsFlyerService))]
	public class AppsFlyerService : MonoBehaviour, IAppsFlyerService
#if APPSFLYER
		, IAppsFlyerConversionData
#endif
	{
		private const int AttWaitTimeoutSeconds = 60;

		public int Priority => 0;
		public bool Initialized { get; set; }

		private bool _connecting;
		private AppsflyerSettingSO _settings;

		public async UniTask OnInitialize(IArchitecture architecture)
		{
			await UniTask.WaitUntil(() => !_connecting);

			if (Initialized)
				return;

			_settings = AppsflyerSettingSO.Instance;
			_connecting = true;

#if APPSFLYER
			InitializeAppsFlyer();
#endif
			Initialized = true;
			_connecting = false;
		}

#if APPSFLYER
		private void InitializeAppsFlyer()
		{
			if (_settings == null)
			{
				Debug.LogError("[AppsFlyer] AppsflyerSettingSO not found. AppsFlyer initialization aborted.");
				return;
			}

			if (string.IsNullOrEmpty(_settings.DevKey))
			{
				Debug.LogError("[AppsFlyer] Missing Developer Key! Please set it in KTGame SDK Editor. AppsFlyer Initialization aborted.");
				return;
			}

#if UNITY_IOS
			if (string.IsNullOrEmpty(_settings.AppID))
			{
				Debug.LogWarning("[AppsFlyer] Missing App ID for iOS! Apple App ID is required for iOS builds. Initialization may fail.");
			}
#endif

			AppsFlyer.setIsDebug(_settings.IsDebug && Debug.isDebugBuild);

			AppsFlyer.initSDK(
				_settings.DevKey,
				_settings.AppID,
				_settings.IsGetConversionData ? this : null
			);
			
			AppsFlyer.enableTCFDataCollection(true);

#if UNITY_IOS && !UNITY_EDITOR
			AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(AttWaitTimeoutSeconds);
#endif

			AppsFlyer.startSDK();
		}

		public void onConversionDataSuccess(string conversionData)
		{
			AppsFlyer.AFLog("didReceiveConversionData", conversionData);

			var data = AppsFlyer.CallbackStringToDictionary(conversionData);
			if (data == null)
			{
				Debug.LogWarning("[AppsFlyer] Conversion data could not be parsed.");
				return;
			}

			var afStatus = data.TryGetValue("af_status", out var status) && status != null
				? status.ToString()
				: "unknown";

			VariableDefineService.IsOrganicUser = afStatus == "Organic";
			
			if (!VariableDefineService.IsVnCountry && data.TryGetValue("country", out var country) && country != null)
			{
				VariableDefineService.IsVnCountry = country.ToString() == "VN";
			}

			VariableDefineService.MarkConversionDataReceived();
		}

		public void onConversionDataFail(string error)
		{
			Debug.LogWarning($"[AppsFlyer] ConversionDataFail: {error}");
			VariableDefineService.IsOrganicUser = true;
			VariableDefineService.MarkConversionDataReceived();
		}

		public void onAppOpenAttribution(string attributionData)
		{
			Debug.Log($"[AppsFlyer] AppOpenAttribution: {attributionData}");
		}

		public void onAppOpenAttributionFailure(string error)
		{
			Debug.LogWarning($"[AppsFlyer] AppOpenAttributionFailure: {error}");
		}
#endif
	}
}
