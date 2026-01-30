using Sirenix.OdinInspector;
using UnityEditor;

namespace com.ktgame.services.appsflyer.editor
{
	public class AppsflyerEditor
	{
		private AppsflyerSettingSO _appsflyerSo;

		public AppsflyerEditor(AppsflyerSettingSO appsflyerSo)
		{
			_appsflyerSo = appsflyerSo;
		}

		[ShowInInspector]
		[LabelText("Developer Key")]
		public string DevKey
		{
			get => _appsflyerSo.DevKey;
			set
			{
				_appsflyerSo.DevKey = value;
				AssetDatabase.SaveAssets();
			}
		}

		[ShowInInspector]
		[LabelText("AppId iOS")]
		public string AppId
		{
			get => _appsflyerSo.AppID;
			set
			{
				_appsflyerSo.AppID = value;
				AssetDatabase.SaveAssets();
			}
		}

		[ShowInInspector]
		[LabelText("Is Debug")]
		public bool IsDebug
		{
			get => _appsflyerSo.IsDebug;
			set
			{
				_appsflyerSo.IsDebug = value;
				AssetDatabase.SaveAssets();
			}
		}

		[ShowInInspector]
		[LabelText("Is Get Conversion Data")]
		public bool IsGetConversionData
		{
			get => _appsflyerSo.IsGetConversionData;
			set
			{
				_appsflyerSo.IsGetConversionData = value;
				AssetDatabase.SaveAssets();
			}
		}
	}
}