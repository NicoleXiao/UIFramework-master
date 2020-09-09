using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace UIFramework
{
    public class MaskManager : UIBase
    {
        public Image maskImage;
        public Action<UIName> clickCloseMaskEvent;
        private UIName m_clickCloseUI;
        public UIType curMaskUIType { get; private set; }

        protected override void Show()
        {
            base.Show();
            maskImage.GetComponent<Button>().onClick.AddListener(()=>OnClickMask());
        }

        public void ManagerMaskShow(UIBase ui)
        {
            switch (ui.maskType)
            {
                case UIMaskType.OnlyMask:
                    SetAlpha();
                    m_clickCloseUI = UIName.None;
                    break;
                case UIMaskType.MaskClickClose:
                    SetAlpha();
                    m_clickCloseUI = ui.uiName;
                    break;
                case UIMaskType.TransparentMask:
                    SetTransparent();
                    m_clickCloseUI = UIName.None;
                    break;
                case UIMaskType.TransparentClickMask:
                    SetTransparent();
                    m_clickCloseUI = ui.uiName;
                    break;
            }
            curMaskUIType = ui.uiType;
            SetSortOrder(ui.UICanvas.sortingOrder - 1);
            ResumeUI();
        }

       

        public void SetTransparent()
        {
            maskImage.color = new Color(0, 0, 0, 0);
        }

        public void SetAlpha()
        {
            maskImage.color = new Color(0, 0, 0, 0.5f);
        }

        public void OnClickMask()
        {
            if (m_clickCloseUI != UIName.None)
            {
                clickCloseMaskEvent?.TryInvoke(m_clickCloseUI);
            }

        }

        protected override void Hide()
        {
            ResetMaskStatus();
        }

        private void ResetMaskStatus()
        {
            m_clickCloseUI = UIName.None;
            curMaskUIType = UIType.None;
        }


        protected override void DeSpawn()
        {
            ResetMaskStatus();
            clickCloseMaskEvent = null;
            maskImage.GetComponent<Button>().onClick.RemoveListener(() => OnClickMask());
        }

    }
}