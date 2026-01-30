using UnityEditor;
using com.ktgame.core;
using com.ktgame.core.editor;
using Sirenix.OdinInspector.Editor;

namespace com.ktgame.services.appsflyer.editor
{
    [InitializeOnLoad]
    public class AppsflyerEditorModule : IEditorDirtyHandler,	IMenuTreeExtension
    {
        static AppsflyerEditorModule()
        {
            var module = new AppsflyerEditorModule();
            EditorDirtyRegistry.Register(module);
            MenuTreeExtensionRegistry.Register(module);
        }
        
        public void SetDirty()
        {
            var instance = AppsflyerSettingSO.Instance;
            if (instance != null)
            {
                EditorUtility.SetDirty(instance);
            }
        }
        public void BuildMenu(OdinMenuTree tree)
        {
            tree.Add("Appsflyer", new AppsflyerEditor(AppsflyerSettingSO.Instance), KTEditor.GetIconComponent("appsflyer"));
        }
    }
}
