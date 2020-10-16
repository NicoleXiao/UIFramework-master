using System.Collections;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : UIBase
{
    public AddressableImage m_image;
    public Text m_copyText;

    public Text m_pasteText;

    AndroidJavaObject jo = null;

    protected override void Show()
    {
        if (data != null)
        {
            Debug.Log($"HomeUI data Test : {data[0].ToString()}");
        }
        string path = $"atlas/common.spriteatlas[ui_comprop_close]";
        m_image.SetSprite(path);

#if UNITY_ANDROID

        //先拿到jar中的某个类。
        AndroidJavaClass jc = new AndroidJavaClass("com.NicoleXiao.UIFramework.MyPluginClass"); //和java代码包名统一
        //获取到类的实例
        jo = jc.CallStatic<AndroidJavaObject>("GetInstance", gameObject.name);
        Debug.Log("AndroidJavaObject : " + jo);
#endif
    }



    protected override IEnumerator AfterShow()
    {
        UIManager.GetInstance().CloseAllButCur(UIName.HomeUI);
        return base.AfterShow();
    }

    public void OnClickCopy()
    {
#if UNITY_ANDROID
        if (jo != null)
        {
            Debug.Log("Copy :" + m_copyText.text);
            jo.Call("OnClickCopy", m_copyText.text);
        }
#elif UNITY_EDITOR
#endif
    }

    public void OnClickPaste()
    {
#if UNITY_ANDROID
        if (jo!=null)
        {
            string text= jo.Call<string>("OnClickPaste");
            Debug.Log("Paste :  " + text);
            m_pasteText.text = text;
        }
#elif UNITY_EDITOR

#endif
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
