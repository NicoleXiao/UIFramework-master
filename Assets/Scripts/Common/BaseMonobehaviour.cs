using UnityEngine;

public class BaseMonobehaviour : MonoBehaviour
{
    protected bool m_IsApplicationQuit = false;

    private void OnApplicationQuit()
    {
        m_IsApplicationQuit = true;
    }
}
