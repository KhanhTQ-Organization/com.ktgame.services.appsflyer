using System;
using System.Collections;
using System.IO;
using System.Net;
using com.ktgame.core.editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace com.ktgame.services.appsflyer.editor
{
	public class AppsflyerEditor
	{
		private AppsflyerSettingSO _appsflyerSo;
		private KTSettingSO _setting;

		private bool IsInstalledAppsflyer => DefineSymbolsEditor.HasDefineSymbol(DefineSymbolName.DS_APPSFLYER_INSTALLED);

		private static bool HasAppsflyerFiles =>
			Directory.Exists(Path.Combine(Application.dataPath, "AppsFlyer")) ||
			Directory.Exists(Path.Combine(Application.dataPath, "Appsflyer"));

		public AppsflyerEditor(AppsflyerSettingSO appsflyerSo) : this(appsflyerSo, KTWindow.Setting)
		{
		}

		public AppsflyerEditor(AppsflyerSettingSO appsflyerSo, KTSettingSO setting)
		{
			_appsflyerSo = appsflyerSo;
			_setting = setting ?? KTWindow.Setting;
		}

		[Title("Appsflyer SDK Configuration", "Manage your Appsflyer analytics integration.", TitleAlignments.Centered)]
		[InfoBox("AppsFlyer allows you to track app installs, measure campaign performance, and monitor user activity. Ensure 'Debug Mode' is OFF for production builds.", InfoMessageType.Info)]
		[InfoBox("Appsflyer is not installed. Please install Appsflyer SDK to use this service.", InfoMessageType.Warning, VisibleIf = "@!IsInstalledAppsflyer")]
#pragma warning disable CS0414
		[PropertyOrder(-10)]
		[ShowInInspector, HideLabel, DisplayAsString(false)]
		private string _dummyInfo = "";
#pragma warning restore CS0414

		[ShowIf("@!IsInstalledAppsflyer")]
		[BoxGroup("Installation", CenterLabel = true)]
		[PropertyOrder(-5)]
		[ShowInInspector]
		public string AppsflyerVersion
		{
			get => !string.IsNullOrEmpty(_appsflyerSo?.AppsflyerVersion) ? _appsflyerSo.AppsflyerVersion : AppsflyerSettingSO.DefaultAppsflyerVersion;
			set
			{
				if (_appsflyerSo != null) _appsflyerSo.AppsflyerVersion = value;
			}
		}

		[ShowIf("@!IsInstalledAppsflyer")]
		[BoxGroup("Installation")]
		[PropertyOrder(-5)]
		[Button("Install / Refresh", ButtonSizes.Medium), GUIColor(0.2f, 0.6f, 1f)]
		private void HandleInstallation()
		{
			if (!HasAppsflyerFiles)
			{
				string edmVer = _setting != null ? _setting.EdmVersion : "1.2.185";
				if (!PackageDependenceEditor.IsPackageInstalled(VariableEditor.ExternalDependencyManagerName))
				{
					PackageDependenceEditor.InstallPackage(VariableEditor.ExternalDependencyManagerName, edmVer);
				}

				string version = !string.IsNullOrEmpty(AppsflyerVersion) ? AppsflyerVersion.Trim() : AppsflyerSettingSO.DefaultAppsflyerVersion;
				EditorCoroutine.Start(DownloadAndInstallAppsflyer(version));
			}
			else
			{
				PackageDependenceEditor.RefreshPackage();
				DefineSymbolsEditor.AddDefineSymbol(DefineSymbolName.DS_APPSFLYER_INSTALLED);
			}
		}

		private static IEnumerator DownloadAndInstallAppsflyer(string version)
		{
			string cleanVer = version.Trim().TrimStart('v', 'V');
			string tag = "v" + cleanVer;
			string primaryUrl = $"https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/releases/download/{tag}/appsflyer-unity-plugin-{cleanVer}.unitypackage";
			string fallbackUrl = $"https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/releases/download/{cleanVer}/appsflyer-unity-plugin-{cleanVer}.unitypackage";

			string tempDir = Path.Combine(Application.dataPath, "../Temp");
			if (!Directory.Exists(tempDir))
			{
				Directory.CreateDirectory(tempDir);
			}
			string packagePath = Path.Combine(tempDir, $"appsflyer-unity-plugin-{cleanVer}.unitypackage");

			EditorUtility.DisplayProgressBar("Installing AppsFlyer", $"Downloading AppsFlyer v{cleanVer} from GitHub...", 0.2f);
			Debug.Log($"[AppsFlyer] Downloading package from {primaryUrl}...");

			bool downloadSuccess = false;
			using (var client = new WebClient())
			{
				client.Headers.Add("User-Agent", "Unity-Appsflyer-Installer");

				var task = client.DownloadFileTaskAsync(new Uri(primaryUrl), packagePath);
				while (!task.IsCompleted)
				{
					yield return null;
				}

				if (task.IsFaulted)
				{
					Debug.LogWarning($"[AppsFlyer] Primary URL failed ({task.Exception?.GetBaseException()?.Message}), trying fallback URL: {fallbackUrl}");
					var fallbackTask = client.DownloadFileTaskAsync(new Uri(fallbackUrl), packagePath);
					while (!fallbackTask.IsCompleted)
					{
						yield return null;
					}

					downloadSuccess = !fallbackTask.IsFaulted;
					if (!downloadSuccess)
					{
						Debug.LogError($"[AppsFlyer] Failed to download AppsFlyer package: {fallbackTask.Exception?.GetBaseException()?.Message}");
					}
				}
				else
				{
					downloadSuccess = true;
				}
			}

			EditorUtility.ClearProgressBar();

			if (downloadSuccess && File.Exists(packagePath))
			{
				Debug.Log($"[AppsFlyer] Download complete ({new FileInfo(packagePath).Length} bytes). Importing package into Unity...");
				EditorUtility.DisplayProgressBar("Installing AppsFlyer", "Importing package into Unity...", 0.8f);

				AssetDatabase.ImportPackage(packagePath, false);
				AssetDatabase.Refresh();

				EditorUtility.ClearProgressBar();
				
				DefineSymbolsEditor.AddDefineSymbol(DefineSymbolName.DS_APPSFLYER_INSTALLED);
				Debug.Log("[AppsFlyer] Package imported and APPSFLYER define symbol applied. Installation complete!");
			}
			else
			{
				EditorUtility.DisplayDialog("AppsFlyer Download Error",
					$"Could not download AppsFlyer {cleanVer} from GitHub releases.\nPlease verify that version '{cleanVer}' exists on GitHub or check your internet connection.",
					"OK");
			}
		}

		[ShowIf("@IsInstalledAppsflyer")]
		[BoxGroup("Installation", CenterLabel = true)]
		[PropertyOrder(-5)]
		[ShowInInspector]
		public string AppsflyerVersionInstalled
		{
			get => !string.IsNullOrEmpty(_appsflyerSo?.AppsflyerVersion) ? _appsflyerSo.AppsflyerVersion : AppsflyerSettingSO.DefaultAppsflyerVersion;
			set
			{
				if (_appsflyerSo != null) _appsflyerSo.AppsflyerVersion = value;
			}
		}

		[ShowIf("@IsInstalledAppsflyer")]
		[BoxGroup("Installation")]
		[PropertyOrder(-5)]
		[HorizontalGroup("Installation/Actions")]
		[Button("Reinstall", ButtonSizes.Medium), GUIColor(0.2f, 0.6f, 1f)]
		private void HandleReinstall()
		{
			if (!EditorUtility.DisplayDialog("Reinstall AppsFlyer",
				$"This will delete Assets/AppsFlyer and re-download version {AppsflyerVersionInstalled}.\nContinue?",
				"Yes", "Cancel"))
				return;

			RemoveAppsflyerFiles();
			DefineSymbolsEditor.RemoveDefineSymbol(DefineSymbolName.DS_APPSFLYER_INSTALLED);

			string version = !string.IsNullOrEmpty(AppsflyerVersionInstalled) ? AppsflyerVersionInstalled.Trim() : AppsflyerSettingSO.DefaultAppsflyerVersion;
			EditorCoroutine.Start(DownloadAndInstallAppsflyer(version));
		}

		[ShowIf("@IsInstalledAppsflyer")]
		[HorizontalGroup("Installation/Actions")]
		[Button("Remove SDK", ButtonSizes.Medium), GUIColor(0.8f, 0.3f, 0.3f)]
		private void HandleRemove()
		{
			if (!EditorUtility.DisplayDialog("Remove AppsFlyer SDK",
				"This will remove the APPSFLYER define symbol and delete Assets/AppsFlyer from your project.\nContinue?",
				"Yes", "Cancel"))
				return;

			DefineSymbolsEditor.RemoveDefineSymbol(DefineSymbolName.DS_APPSFLYER_INSTALLED);
			RemoveAppsflyerFiles();
			Debug.Log("[AppsFlyer] SDK removed. You can now install a different version.");
		}

		private static void RemoveAppsflyerFiles()
		{
			string[] folders = { "AppsFlyer", "Appsflyer" };
			foreach (var folder in folders)
			{
				string path = Path.Combine(Application.dataPath, folder);
				if (Directory.Exists(path))
				{
					FileUtil.DeleteFileOrDirectory(path);
					FileUtil.DeleteFileOrDirectory(path + ".meta");
					Debug.Log($"[AppsFlyer] Removed {path}");
				}
			}
			AssetDatabase.Refresh();
		}

		[OnInspectorGUI]
		[PropertyOrder(-1)]
		private void MarkDirtyOnGuiChange()
		{
			if (GUI.changed)
			{
				if (_appsflyerSo != null) EditorUtility.SetDirty(_appsflyerSo);
				if (_setting != null) EditorUtility.SetDirty(_setting);
				AssetDatabase.SaveAssets();
			}
		}

		[BoxGroup("Authentication", CenterLabel = true)]
		[PropertyOrder(0)]
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
		[PropertyOrder(0)]
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
		[PropertyOrder(1)]
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
		[PropertyOrder(1)]
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

		[PropertyOrder(2)]
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
