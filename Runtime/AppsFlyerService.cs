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
        public int Priority => 0;
        public bool Initialized { get; set; }

        private bool _connecting;
		private AppsFlyerServiceSettings _settings;

        public async UniTask OnInitialize(IArchitecture architecture)
        {
            await UniTask.WaitUntil(() => !_connecting);

            if (Initialized)
                return;

			_settings = AppsFlyerServiceSettings.Instance;
            _connecting = true;

#if APPSFLYER
			InitializeAppsFlyer();
			Initialized = true;
#endif

            _connecting = false;
        }

#if APPSFLYER
		private void InitializeAppsFlyer()
		{
			AppsFlyer.setIsDebug(_settings.IsDebug);

			AppsFlyer.initSDK(
				_settings.DevKey,
				_settings.AppID,
				_settings.IsGetConversionData ? this : null
			);

			AppsFlyer.startSDK();
		}

		public void onConversionDataSuccess(string conversionData)
		{
			AppsFlyer.AFLog("didReceiveConversionData", conversionData);

			var data = AppsFlyer.CallbackStringToDictionary(conversionData);

			var afStatus = data.TryGetValue("af_status", out var status)
				? status.ToString()
				: "unknown";

			VariableDefineService.IsOrganicUser = afStatus == "Organic";
			
			if (VariableDefineService.IsVnCountry)
			{
				return;
			}

			if (data.TryGetValue("country", out var country))
			{
				VariableDefineService.IsVnCountry = country.ToString() == "VN";
			}
		}

		public void onConversionDataFail(string error)
		{
			Debug.LogWarning($"[AppsFlyer] ConversionDataFail: {error}");
			VariableDefineService.IsOrganicUser = true;
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
