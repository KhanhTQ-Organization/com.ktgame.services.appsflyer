using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace com.ktgame.services.appsflyer.editor
{
	public class AppsflyerEditor
	{
		private AppsflyerSettingSO _appsflyerSo;

		public AppsflyerEditor(AppsflyerSettingSO appsflyerSo)
		{
			_appsflyerSo = appsflyerSo;
		}

		[Title("Appsflyer SDK Configuration", "Manage your Appsflyer analytics integration.", TitleAlignments.Centered)]
		[InfoBox("AppsFlyer allows you to track app installs, measure campaign performance, and monitor user activity. Ensure 'Debug Mode' is OFF for production builds.", InfoMessageType.Info)]
		
		[BoxGroup("Authentication", CenterLabel = true)]
		[PropertySpace(SpaceBefore = 20)]
		[LabelText("Developer Key")]
		[Tooltip("Your unique Developer Key from the Appsflyer dashboard.")]
		[ShowInInspector]
		[OnValueChanged("MarkDirty")]
		public string DevKey
		{
			get => _appsflyerSo.DevKey;
			set => _appsflyerSo.DevKey = value;
		}

		[BoxGroup("Authentication")]
		[PropertySpace(SpaceAfter = 10)]
		[LabelText("App ID (iOS)")]
		[Tooltip("The Apple App ID for iOS (e.g., id123456789). Leave blank if Android only.")]
		[ShowInInspector]
		[OnValueChanged("MarkDirty")]
		public string AppId
		{
			get => _appsflyerSo.AppID;
			set => _appsflyerSo.AppID = value;
		}

		[BoxGroup("General Settings", CenterLabel = true)]
		[PropertySpace(SpaceBefore = 10)]
		[LabelText("Enable Debug Mode")]
		[Tooltip("Enable verbose logging for debugging purposes.")]
		[ShowInInspector]
		[OnValueChanged("MarkDirty")]
		public bool IsDebug
		{
			get => _appsflyerSo.IsDebug;
			set => _appsflyerSo.IsDebug = value;
		}

		[BoxGroup("General Settings")]
		[PropertySpace(SpaceAfter = 10)]
		[LabelText("Get Conversion Data")]
		[Tooltip("Enable this if you need to access organic/non-organic conversion data on install.")]
		[ShowInInspector]
		[OnValueChanged("MarkDirty")]
		public bool IsGetConversionData
		{
			get => _appsflyerSo.IsGetConversionData;
			set => _appsflyerSo.IsGetConversionData = value;
		}

		[PropertySpace(20)]
		[Button("Open Appsflyer Dashboard", ButtonSizes.Medium)]
		private void OpenDashboard()
		{
			Application.OpenURL("https://www.appsflyer.com/");
		}

		private void MarkDirty()
		{
			if (_appsflyerSo != null)
			{
				EditorUtility.SetDirty(_appsflyerSo);
			}
		}
	}
}