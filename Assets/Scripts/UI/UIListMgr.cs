using System.Collections.Generic;

namespace UIFramework
{
    public class UIListMgr
    {
        public UIType uiType { get; private set; }
        /**UI的sortingOrder的取值差值*/
        private int m_layerSpace = 10;
        /**开始层级初始值*/
        private int m_startLayer = 100;
        /**存放加载的ui信息*/
        private List<UIBase> m_list = new List<UIBase>();
        /**当前需要Mask的UI*/
        public UIBase curNeedMaskUI { private set; get; }


        /// <summary>
        /// 层级的初始值根据UIType类型来
        /// </summary>
        /// <param name="type"></param>
        public UIListMgr(UIType type)
        {
            m_startLayer = ((int)type - 1) * 1000 + 100;
            this.uiType = type;
        }

        /// <summary>
        /// 根据当前在List的下标位置获取相应显示层级
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetSortingOrder(int index)
        {
            return m_startLayer + index * m_layerSpace;
        }

        /// <summary>
        /// 获取当前最高层级数
        /// </summary>
        /// <returns></returns>
        public UIBase GetTopUI()
        {
            if (m_list.Count <= 0) return null;
            return m_list[m_list.Count - 1];
        }


        public bool IsExitUI(UIName uiName)
        {
            return m_list.Exists((UIBase ui) => ui.uiName == uiName);
        }


        /// <summary>
        /// FullUI类型的ui打开之后，需要隐藏之前打开的所有全屏UI
        /// </summary>
        /// <param name="ui"></param>

        public void AddUI(UIBase ui)
        {
            ui.SetSortOrder(GetSortingOrder(m_list.Count));
            if (uiType == UIType.FullUI && m_list.Count > 0)
            {
                for (int i = m_list.Count - 1; i >= 0; i--)
                {
                    if (m_list[i].IsShowing)
                    {
                        m_list[i].HideUI();
                    }
                }
            }
            m_list.Add(ui);
            SetNeedMaskUI();
        }

        /// <summary>
        ///设置当前需要Mask的类型，优先考虑最高层级的UI。
        /// </summary>
        private void SetNeedMaskUI()
        {
            curNeedMaskUI = null;
            for (int i = m_list.Count - 1; i >= 0; i--)
            {
                var ui = m_list[i];
                if (ui.maskType != UIMaskType.None)
                {
                    curNeedMaskUI = ui;
                    break;
                }
            }
        }


        /// <summary>
        /// FullUI 在关闭的时候需要特殊处理,关闭了UI之后要把在列表最上层的ui显示出来
        /// </summary>
        public void CloseCurUI(UIName uiName)
        {
            if (m_list.Count == 0) return;
            int removeIndex = m_list.FindIndex(0, (UIBase ui) => ui.uiName == uiName);
            m_list[removeIndex].DeSpawnUI();
            m_list.RemoveAt(removeIndex);
            int count = m_list.Count;
            for (int i = removeIndex + 1; i < count; i++)
            {
                //刷新sortingOrder
                m_list[i].ChangeSortingOrder(GetSortingOrder(i));
            }
            //关闭UI以后，把最上面的ui显示出来。
            if (uiType == UIType.FullUI && !m_list[count - 1].IsShowing)
            {
                m_list[count - 1].ResumeUI();
            }
            SetNeedMaskUI();
        }


        public void CloseAllButCurUI(UIName uiName)
        {
            if (m_list.Count == 0) return;
            for (int i = 0; i < m_list.Count; i++)
            {
                var ui = m_list[i];
                if (ui.uiName != uiName)
                {
                    ui.DeSpawnUI();
                    m_list.RemoveAt(i);
                }
            }
            if (m_list.Count > 0)
            {
                m_list[0].ChangeSortingOrder(GetSortingOrder(0));
            }
            SetNeedMaskUI();
        }

        public void CloseAll()
        {
            if (m_list.Count == 0) return;
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].DeSpawnUI();
            }
            m_list.Clear();
            curNeedMaskUI = null;
        }
    }
}
