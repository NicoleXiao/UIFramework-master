using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace UIFramework
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    public class GameInit : MonoBehaviour
    {
        public List<string> preLoad = new List<string>();
        private const string loadPath = "Prefabs/GameManager/";

        private void Start()
        {
            StartCoroutine(BeforeInit());
        }


        public IEnumerator BeforeInit()
        {
            CSpriteAtlasManager.GetInstance().Initialize();
            var op = Addressables.InitializeAsync();
            op.Completed += (s) =>
            {
                AddressableMgr.GetInstance().Initialize();
            };
            yield return op;
            yield return CSpriteAtlasManager.instance.LoadAllSpriteAtlas(AddressableMgr.instance.spriteAtlasPrimaryKeys);
            yield return StartCoroutine(PreLoad());
            SceneManager.LoadScene("UI");
        }

        public IEnumerator PreLoad()
        {
            for (int i = 0; i < preLoad.Count; i++)
            {
                string path = $"{loadPath}{preLoad[i]}.prefab";
                var handle = AddressableResLoader.InstantiateAsync(path);
                yield return handle;
                if (handle.Result != null)
                {
                    handle.Result.AddComponent<DontDestroyOnload>();
                }
            }
            
        }

    }
}
