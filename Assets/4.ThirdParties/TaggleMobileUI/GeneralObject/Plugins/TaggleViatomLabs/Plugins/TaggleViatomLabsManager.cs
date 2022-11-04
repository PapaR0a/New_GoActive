using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class TaggleViatomLabsManager : MonoBehaviour
{
    static string TAG = "TaggleViatomLabsManager";

    public static Action<string> ScanDeviceEvent;
    public static Action StopScanDeviceEvent;
    public static Action<string> DeviceConnectedEvent;
    public static Action<string> SendDataEvent;
    public static Action<string> DisconnectDeviceEvent;

    private static TaggleViatomLabsManager _instance;
    public static TaggleViatomLabsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("TaggleViatomLabsManager");
                obj.transform.SetParent(HAGOSmartBluetoothManager.Api.transform);
                _instance = obj.AddComponent<TaggleViatomLabsManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            Destroy(transform.parent);
            return;
        }
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(transform.parent);
    }

    void Start()
    {
        CheckPermission();
    }

    public void Init()
    {
        ScanDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackScanDeviceEvent;
        StopScanDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackStopScanDeviceEvent;
        DeviceConnectedEvent += HAGOSmartBluetoothControl.Api.CallbackConnected;
        SendDataEvent += HAGOSmartBluetoothControl.Api.CallbackDataEvent;
        DisconnectDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackDisconnect;
    }

    private void CheckPermission()
    {
#if UNITY_ANDROID
        var permissions = new List<string>()
        {
            Permission.FineLocation, Permission.Microphone,
            Permission.CoarseLocation, Permission.ExternalStorageWrite
        };
        foreach (var p in permissions)
        {
            AskForPermissionIfNotGranted(p);
        }

#endif
    }

    void AskForPermissionIfNotGranted(string permission)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(permission))
        {
            Debug.Log("AskForPermissionIfNotGranted: " + permission);
            Permission.RequestUserPermission(permission);
        }
#endif
    }
    
    public void DeviceScanCallback(string result)
    {
        Debug.Log(string.Format("{0} Scanning: ", TAG, result));
        ScanDeviceEvent?.Invoke(result);
    }

    public void DeviceStopScanCallback(string result)
    {
        Debug.Log("{0} Stop scanning: " + result);
        StopScanDeviceEvent?.Invoke();
    }

    public void ConnectCallback(string result)
    {
        Debug.Log("Receive connect: " + result);
        DeviceConnectedEvent?.Invoke(result);
    }
    public void SendDataCallback(string result)
    {
         Debug.Log("Receive send data callback: " + result);
         SendDataEvent?.Invoke(result);
    }

    public void DisconnectCallback(string result)
    {
        Debug.Log("Receive send data callback: " + result);
        DisconnectDeviceEvent?.Invoke(result);
    }

}
