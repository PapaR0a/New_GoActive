using System.Collections.Generic;
using UnityEngine;

public class HAGOSmartBluetoothManager : MonoBehaviour
{
    static string TAG = "[HAGOSmartBluetoothManager] ";
    // API
    private static HAGOSmartBluetoothManager m_api;
    public static HAGOSmartBluetoothManager Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new GameObject("HAGOSmartBluetoothManager").AddComponent<HAGOSmartBluetoothManager>();
            }
            return m_api;
        }
    }

    public void Init()
    {
        HAGOSmartBluetoothControl.Api.Init();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
