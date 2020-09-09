using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

public class UIHome : UIBase
{
    public AddressableImage m_image;

    protected override void Show()
    {
        if (data != null)
        {
            Debug.LogError(data[0].ToString());
        }
        string path = $"atlas/common.spriteatlas[ui_comprop_close]";
        m_image.SetSprite(path);
    }



    protected override IEnumerator AfterShow()
    {
        UIManager.GetInstance().CloseAllButCur(UIName.HomeUI);
        return base.AfterShow();
    }

    public void OnClickEquipBtn()
    {
        UIManager.GetInstance().ShowUI(UIName.EquipUI);
    }

    public void OnClickSignBtn()
    {
        UIManager.GetInstance().ShowMessage("签到成功！！");
    }

    public void OnClickMatchBtn()
    {

    }
}
