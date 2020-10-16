using UnityEngine;

public class DontDestroyOnload : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (transform.parent == null || !transform.parent.GetComponent<DontDestroyOnload>())
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
