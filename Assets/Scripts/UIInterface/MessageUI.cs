using System.Collections;
using UIFramework;
using UnityEngine.UI;

public class MessageUI : UIBase
{
    public Text m_title;
    public Text m_messageInfo;

    protected override IEnumerator BeforeShow()
    {
        if (data != null && data.Length > 0)
        {
            m_messageInfo.text = data[0].ToString();
            if (data.Length >= 2)
            {
                var title = data[1].ToString();
                if (!string.IsNullOrEmpty(title))
                {
                    m_title.text = title;
                }
            }
        }
        return base.BeforeShow();
    }

    protected override void DeSpawn()
    {
        m_messageInfo.text = "";
    }
}

