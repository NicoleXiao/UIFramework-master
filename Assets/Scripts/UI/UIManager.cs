using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

namespace UIFramework
{
    //UI的加载信息
    public class LoadUIData : IEquatable<LoadUIData>
    {
        public UIConfig uiConfig;
        public int sortingOrder;
        public bool isLoading;  //是否正在加载
        public bool isClosed; //是已经被关闭了
        public object[] userDatas;    //向当前UI传入的参数

        public bool Equals(LoadUIData other)
        {
            return uiConfig.loadPath == other.uiConfig.loadPath &&
                   uiConfig.uiName == other.uiConfig.uiName &&
                   uiConfig.uiType == other.uiConfig.uiType &&
                   uiConfig.maskType == other.uiConfig.maskType;
        }
    }

    public class UIManager : MonoSingleton<UIManager>
    {
        /**父节点*/
        public Transform root;

        /**全局唯一的mask*/
        private MaskManager m_mask;

        /**当前需要mask的UI*/
        private UIBase m_curNeedMaskUI;

        /**UI配置集合*/
        public List<UIConfig> configList;

        private ObjectPool<GameObject> m_pool;

        /**将所有UI的配置信息用字典存储**/
        private Dictionary<UIName, UIConfig> m_dic = new Dictionary<UIName, UIConfig>();

        private Dictionary<UIType, UIListMgr> m_listMgr = new Dictionary<UIType, UIListMgr>();

        /**用于存放当前正在加载的UI信息*/
        private List<LoadUIData> m_loadDatas = new List<LoadUIData>();

        protected override void Init()
        {
            if (m_pool == null)
            {
                m_pool = PoolManager.GetInstance().GetPool<GameObject>(AssetType.Prefab);
            }
            InitDic();
            InitUIListMgr();
            m_curNeedMaskUI = null;
            StartCoroutine(LoadMask());
        }


        private void InitDic()
        {
            for (int i = 0; i < configList.Count; i++)
            {
                var config = configList[i];
                m_dic[config.uiName] = config;
            }
        }

        private void InitUIListMgr()
        {
            foreach (UIType type in System.Enum.GetValues(typeof(UIType)))
            {
                if (type != UIType.None)
                {
                    UIListMgr list = new UIListMgr(type);
                    m_listMgr[type] = list;
                }
            }
        }

        /// <summary>
        /// 游戏里面通用的消息提示弹窗  
        /// </summary>
        public void ShowMessage(string message, string messageTitle = "")
        {
            List<string> messageData = new List<string>();
            if (!string.IsNullOrEmpty(message))
            {
                messageData.Add(message);
            }
            if (!string.IsNullOrEmpty(messageTitle))
            {
                messageData.Add(messageTitle);
            }
            ShowUI(UIName.MessageUI, messageData.ToArray());
        }


        public void ShowUI(UIName uiName, object[] data = null)
        {
            UIConfig config;
            if (m_dic.TryGetValue(uiName, out config))
            {
                if (CanStartLoadUI(config, data))
                {
                    StartCoroutine(LoadUI(config, data));
                }
            }
            else
            {
                Debug.LogError($"Can not find config ：{uiName}");
            }
        }

        /// <summary>
        /// 是否可以加载UI
        /// </summary>
        /// <param name="uiInfo"></param>
        /// <param name="showEvent"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private bool CanStartLoadUI(UIConfig config, object[] userDatas = null)
        {
            if (m_loadDatas.Count <= 0)
            {
                return true;
            }

            for (int i = 0; i < m_loadDatas.Count; i++)
            {
                var data = m_loadDatas[i];
                if (data.uiConfig.Equals(config))
                {
                    if (data.isLoading)
                    {
                        Debug.Log($"{data.uiConfig.loadPath } is Loading！！！！");
                        data.userDatas = userDatas;
                        return false;
                    }
                }
            }
            return true;
        }

        private IEnumerator LoadUI(UIConfig config, object[] userData = null)
        {
            var data = PrePushToAllStack(config, userData);
            string path = config.loadPath;
            string assetName = path.GetAssetName();
            /**先从对象池里面取，取不出来才加载**/
            GameObject obj = m_pool.GetFromPool(path, assetName);
            AsyncOperationHandle<GameObject>? loader = null;
            if (!obj)
            {
                loader = AddressableResLoader.InstantiateAsync(path);
                yield return loader;
                if (loader.Value.Result)
                {
                    obj = loader.Value.Result;
                    /**加载完放入对象池里面**/
                    m_pool.AddItemToPool(path, assetName, obj, true, true);
                }
            }
            if (obj)
            {
                obj.transform.SetParent(root);
                UIBase ui = obj.GetOrAddComponent<UIBase>();
                ui.SetUIData(data.userDatas);
                InitUIBase(ui);
                yield return ui.ShowUI();
                RemoveLoadUIData(data);
                OpenUIManagerMask(ui);
            }
            else
            {
                Debug.LogError("Load " + config.loadPath + " fail!!! ");
            }
        }

        private void InitUIBase(UIBase ui)
        {
            ui.InitUI();
            if (ui.uiType != UIType.None)
            {
                m_listMgr[ui.uiType].AddUI(ui);
            }
            ui.closeSelfAction += CloseCurUI;
        }

        #region Mask的功能
        /// <summary>
        /// Mask是特殊的加载,不会放在UIStack里面,也不会放在对象池里面,只有显示和隐藏
        /// 所以游戏开始的时候就要开始加载
        /// </summary>
        /// <param name="sortingOrder"></param>
        /// <param name="isClick"></param>
        public IEnumerator LoadMask()
        {
            UIConfig config;
            if (m_dic.TryGetValue(UIName.MaskUI, out config))
            {
                var handle = AddressableResLoader.InstantiateAsync(config.loadPath);
                yield return handle;
                if (handle.Result != null)
                {
                    GameObject obj = handle.Result;
                    obj.transform.SetParent(root);
                    m_mask = obj.GetOrAddComponent<MaskManager>();
                    m_mask.InitUI();
                    m_mask.clickCloseMaskEvent += CloseCurUI;
                    m_mask.HideUI();
                }
                else
                {
                    Debug.LogError("load file failur:" + config.loadPath);
                }
            }
        }

        private UIConfig GetConfig(UIName uiName)
        {
            UIConfig config;
            m_dic.TryGetValue(uiName, out config);
            return config;
        }


        private UIListMgr GetUIList(UIName uiName)
        {
            var config = GetConfig(uiName);
            if (config == null) return null;
            return m_listMgr[config.uiType];
        }


        /// <summary>
        /// 开启一个新UI的时候管理Mask显示
        /// </summary>
        /// <param name="ui"></param>

        private void OpenUIManagerMask(UIBase ui)
        {
            if (m_mask == null) return;
            if (ui.maskType != UIMaskType.None)
            {
                if (m_curNeedMaskUI != null && ui.uiType < m_curNeedMaskUI.uiType)
                {
                    return;
                }
                m_curNeedMaskUI = ui;
                m_mask.ManagerMaskShow(ui);
            }
        }

        /// <summary>
        /// 当有关闭UI操作的时候，管理Mask显示
        /// </summary>
        private void ChangeShowMask()
        {
            UIBase ui = null;
            foreach (UIListMgr list in m_listMgr.Values)
            {
                if (list.curNeedMaskUI != null)
                {
                    if ((ui != null && ui.maskType < list.curNeedMaskUI.maskType) || ui == null)
                    {
                        ui = list.curNeedMaskUI;
                    }
                }
            }
            if (ui)
            {
                m_mask.ManagerMaskShow(ui);
            }
            else
            {
                HideMask();
            }
            m_curNeedMaskUI = ui;
        }

        private void HideMask()
        {
            m_mask.HideUI();
        }

        #endregion

        /// <summary>
        /// 先把加载信息放入全局的堆栈里面
        /// </summary>
        private LoadUIData PrePushToAllStack(UIConfig config, params object[] userDatas)
        {
            LoadUIData data = new LoadUIData();
            data.uiConfig = config;
            data.isLoading = true;
            data.isClosed = false;
            data.userDatas = userDatas;
            m_loadDatas.Add(data);
            return data;
        }

        /// <summary>
        /// 加载ui成功之后，移除加载信息。
        /// </summary>
        /// <param name="data"></param>
        private void RemoveLoadUIData(LoadUIData data)
        {
            if (m_loadDatas.Count <= 0)
            {
                return;
            }
            for (int i = m_loadDatas.Count - 1; i >= 0; i--)
            {
                if (m_loadDatas[i].Equals(data))
                {
                    m_loadDatas.RemoveAt(i);
                    break;
                }
            }
        }


        /// <summary>
        /// 关闭当前UI
        /// </summary>
        /// <param name="uiName"></param>
        public void CloseCurUI(UIName uiName)
        {
            var list = GetUIList(uiName);
            if (list != null)
            {
                list.CloseCurUI(uiName);
                ChangeShowMask();
            }
        }


        private bool IsExitUI(UIName uiName)
        {
            var list = GetUIList(uiName);
            if (list != null)
            {
                return list.IsExitUI(uiName);
            }
            return false;
        }


        /// <summary>
        /// 关闭除了当前UI的所有UI界面
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="isCurType">是否只是关闭同类型的</param>
        public void CloseAllButCur(UIName uiName, bool isCurType = true)
        {
            var list = GetUIList(uiName);
            if (list != null)
            {
                list.CloseAllButCurUI(uiName);
                if (!isCurType)
                {
                    foreach (var uiList in m_listMgr.Values)
                    {
                        if (uiList != list)
                        {
                            list.CloseAll();
                        }
                    }
                }
                ChangeShowMask();
            }
        }

        /// <summary>
        /// 关闭指定UIType类型的所有UI
        /// </summary>
        /// <param name="uiType"></param>
        public void CloseAllByUIType(UIType uiType)
        {
            if (uiType == UIType.None) return;
            m_listMgr[uiType].CloseAll();
            ChangeShowMask();
        }

        /// <summary>
        /// 关闭所有的ui
        /// </summary>
        public void CloseAll()
        {
            foreach (var list in m_listMgr.Values)
            {
                list.CloseAll();
            }
            ChangeShowMask();
        }
    }
}
