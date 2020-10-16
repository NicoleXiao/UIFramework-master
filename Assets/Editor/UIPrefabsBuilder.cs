using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UIFramework
{
    public class UIPrefabsBuilder : OdinValueDrawer<List<UIConfig>>
    {
        const string UI_PREFABS_PATH = "Assets/BundleResource/Prefabs/UI";

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);
            var UIConfigList = ValueEntry.SmartValue;
            if (GUILayout.Button("自动生成UIConfigLsit"))
            {
                UIConfigList.Clear();
                if (UIManager.HasInstance())
                {
                    UIManager.GetInstance().configList.Clear();
                }
                var builder = new UIConfigBulider(UIConfigList);
                builder.AddConfig(UI_PREFABS_PATH);
                ValueEntry.SmartValue = UIConfigList;
            }
            CallNextDrawer(label);
        }
    }

    public struct UIConfigBulider
    {
        List<UIConfig> _configs;

        public List<UIConfig> Configs { get { return _configs; } }

        public UIConfigBulider(List<UIConfig> configs)
        {
            _configs = configs;
        }

        public void AddConfig(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError($"{path} is not Exist");
                return;
            }
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(".meta"))
                {
                    continue;
                }
                var asset = (GameObject)AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
                if (asset != null)
                {
                    var uibase = asset.GetComponent<UIBase>();
                    if (uibase != null && uibase.uiName != UIName.None)
                    {
                        _configs.Add(new UIConfig()
                        {
                            loadPath = GameUtils.GetSimpleAddresName(file.Replace("\\", "/")),
                            uiName = uibase.uiName,
                            uiType = uibase.uiType,
                            maskType = uibase.maskType
                        });
                    }
                }
            }
        }
    }
}
