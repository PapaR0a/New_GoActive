using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IHLSTATE
{
    DEVICE_STATE_SCAN_FINISHED,
    DEVICE_STATE_CONNECTION_FAIL,
    DEVICE_STATE_CONNECTED,
    DEVICE_STATE_DISCONNECTED
}
public class IHLManagerValues : IHLSingleton<IHLManagerValues>
{
    private string INDEX_KEY = "WORKER_INDEX";
    private string MAC_KEY = "MAC_KEY";
    private string DEVICE_TYPE_KEY = "DEVICE_TYPE_KEY";
    private string HINT_KEY = "HINT_KEY";

    private string CONNECT_KEY = "CONNECT_KEY";

    public int IndexClick
    {
        get { return PlayerPrefs.GetInt(INDEX_KEY, 0); }
        set { PlayerPrefs.SetInt(INDEX_KEY, value); }
    }
    public string Mac
    {
        get { return PlayerPrefs.GetString(MAC_KEY, ""); }
        set { PlayerPrefs.SetString(MAC_KEY, value); }
    }
    public string DeviceType
    {
        get { return PlayerPrefs.GetString(DEVICE_TYPE_KEY, ""); }
        set { PlayerPrefs.SetString(DEVICE_TYPE_KEY, value); }
    }
    public int ShowHint
    {
        get { return PlayerPrefs.GetInt(HINT_KEY, 1); }
        set { PlayerPrefs.SetInt(HINT_KEY, value); }
    }

    public int Connected 
    {
         get { return PlayerPrefs.GetInt(CONNECT_KEY, 0); }
        set { PlayerPrefs.SetInt(CONNECT_KEY, value); }
    }
}
