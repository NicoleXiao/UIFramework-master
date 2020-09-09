using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

namespace UIFramework
{

    public class UIBase : MonoBehaviour
    {
        [SerializeField]
        private Button m_backBtn;

        public UIName uiName;

        public UIType uiType;

        public UIMaskType maskType;

        public UILayer layer { get; private set; }


        protected object[] data;

        private bool m_isInit = false;


        private IEnumerator m_iterator;

        private UIStatus m_status;
        public bool IsShowing { get { return m_status == UIStatus.Showing; } }

        public Action<UIName> closeSelfAction;



        /// <summary>
        /// 基于3D模型  UI有两个摄像机
        /// 1.UICamera 层级在3D模型之上的。
        /// 2.UIBGCamera 层级在3D模型之下的(一般都是背景)
        /// </summary>
        public static string[] Camera = new string[]
         {
           "UICamera",
           "UIBGCamera",
         };
        [ValueDropdown("Camera")]
        public string m_Camera = Camera[0];

        private Canvas canvas;
        public Canvas UICanvas
        {
            get
            {
                if (canvas == null)
                {
                    canvas = this.GetComponent<Canvas>();
                }
                return canvas;
            }
        }

        private GraphicRaycaster m_raycaster;
        public GraphicRaycaster Raycaster
        {
            get
            {
                if (m_raycaster == null)
                {
                    m_raycaster = this.GetComponent<GraphicRaycaster>();
                }
                return m_raycaster;
            }
        }

        public void SetSortOrder(int order)
        {
            UICanvas.sortingOrder = order;
        }

        public void ChangeSortingOrder(int order)
        {
            if (UICanvas.sortingOrder == order) return;
            UICanvas.sortingOrder = order;
        }

        public void InitUI()
        {
            if (m_isInit) return;
            SetCamera();
            UICanvas.planeDistance = 0;
            if (m_backBtn != null)
            {
                m_backBtn.onClick.AddListener(() => OnClickCloseBtn());
            }
            m_isInit = true;
        }

        public void SetUIData(object[] data)
        {
            this.data = data;
        }

        private void SetCamera()
        {
            if (string.IsNullOrEmpty(m_Camera))
            {
                Debug.LogError("请设置Camera摄像机类型！！！！1");
                return;
            }
            UICanvas.worldCamera = GameObject.Find(m_Camera).GetComponent<Camera>();
        }

        public IEnumerator ShowUI()
        {
            yield return BeforeShow();
            yield return InnerShow();
            yield return AfterShow();
        }


        private IEnumerator InnerShow()
        {
            m_status = UIStatus.Showing;
            gameObject.CustomSetActive(true);
            Raycaster.enabled = true;
            Show();
            yield break;
        }

        /// <summary>
        /// 从隐藏状态重新显示UI
        /// </summary>
        public void ResumeUI()
        {
            m_status = UIStatus.Showing;
            gameObject.CustomSetActive(true);
            Raycaster.enabled = true;
            Resume();
        }


        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI()
        {
            m_status = UIStatus.Hide;
            Raycaster.enabled = false;
            gameObject.CustomSetActive(false);
            Hide();
        }

        /// <summary>
        /// 回收UI
        /// </summary>
        public void DeSpawnUI()
        {
            if (m_iterator != null)
            {
                StopCoroutine(m_iterator);
            }
            m_status = UIStatus.Despawn;
            closeSelfAction = null;
            Raycaster.enabled = false;
            DeSpawn();
            PoolManager.instance.DeSpawn<GameObject>(AssetType.Prefab, this.gameObject);
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void OnClickCloseBtn()
        {
            closeSelfAction?.TryInvoke(uiName);
        }

        #region 用于子类的继承

        protected virtual void Show()
        {

        }

        protected virtual IEnumerator BeforeShow()
        {
            yield break;
        }



        protected virtual IEnumerator AfterShow()
        {
            yield break;
        }


        /// <summary>
        /// 重新显示
        /// </summary>
        protected virtual void Resume()
        {

        }

        /// <summary>
        /// 隐藏
        /// </summary>
        protected virtual void Hide()
        {

        }

        /// <summary>
        /// 回收
        /// </summary>
        protected virtual void DeSpawn()
        {

        }

        /// <summary>
        /// 摧毁
        /// </summary>
        protected virtual void Destroy()
        {

        }
        #endregion

    }

}