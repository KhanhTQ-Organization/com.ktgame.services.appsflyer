using UnityEditor;
using UnityEngine;

namespace com.ktgame.services.appsflyer.editor
{
    public class PackageInstaller
    {
        [MenuItem("Ktgame/Services/AppsFlyer")]
        private static void SelectionSettingsTrackingFirebase()
        {
            Selection.activeObject = AppsFlyerServiceSettings.Instance;
        }
    }
}
