using System;

namespace UIFramework
{
    public enum UIStatus
    {
        Showing,          //正在显示
        Hide,             //隐藏
        Despawn           //回收
    }

    public enum UIType
    {
        None = 0,
        FullUI = 1,         //普通全屏界面，层级最低
        Popup = 2,          //普通弹窗
        Tips = 3,           //提示信息UI
        SystemPopup = 4,    //最高弹窗UI，层级最高
    }

    public enum UIMaskType
    {
        None = 0,
        OnlyMask = 1,               //有颜色，不可点击
        MaskClickClose = 2,         //有颜色且可点击关闭当前UI
        TransparentMask = 3,        //透明，不可点击
        TransparentClickMask = 4,   //透明且可点击关闭当前UI
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
