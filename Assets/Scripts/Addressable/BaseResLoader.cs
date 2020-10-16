using UnityEngine;

namespace UIFramework
{
    public class ResLoaderWrapper : MonoSingleton<ResLoaderWrapper>
    {

    }

    public class BaseResLoader
    {
        private static MonoBehaviour m_Monobehaviour;

        public static MonoBehaviour GetBehaviour()
        {
            if (!m_Monobehaviour)
            {
                m_Monobehaviour = ResLoaderWrapper.GetInstance();
            }

            return m_Monobehaviour;
        }
    }
}
