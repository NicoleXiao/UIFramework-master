using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UIFramework
{
    public enum UIStatus
    {
        Showing,
        Hide,
        Despawn
    }


    public enum UILayer
    {
        None = 0,
        CommonLayer = 1,
        TipsLayer = 2,
        SystemLayer = 3,
    }

    public enum UIType
    {
        None = 0,
        FullUI = 1,         //普通全屏界面
        Popup = 2,       //普通弹窗
        Tips = 3,          //提示信息UI
        SystemPopup = 4,  //最高弹窗UI
    }

    public enum UIMaskType
    {
        None = 0,
        OnlyMask = 1,               //有颜色，不可点击
        MaskClickClose = 2,         //有颜色且可点击关闭当前UI
        TransparentMask = 3,       //透明，不可点击
        TransparentClickMask = 4,  //透明且可点击关闭当前UI
    }



    [Serializable]
    public class UIConfig
    {
        public string loadPath;
        public UIName uiName;
        public UIType uiType;
        public UIMaskType maskType;
    }

    public enum UIName
    {
        None,
        MaskUI,
        LoginUI,
        MessageUI,
        HomeUI,
        EquipUI,
    }
}
