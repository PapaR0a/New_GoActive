using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class TaggleIHealthLabsManager : MonoBehaviour
{
    static string TAG = "TaggleIHealthLabsManager";

    
    public static Action<string> AuthenticationEvent;
    public static Action<string> ScanDeviceEvent;
    public static Action StopScanDeviceEvent;
    public static Action<string> DeviceConnectedEvent;
    public static Action<string> DataEvent;
    public static Action<string> DisconnectEvent;

    private static TaggleIHealthLabsManager _instance;
    public static TaggleIHealthLabsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("TaggleIHealthLabsManager");
                obj.transform.SetParent(HAGOSmartBluetoothManager.Api.transform);
                _instance = obj.AddComponent<TaggleIHealthLabsManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        CheckPermission();
    }

    public void Init()
    {
        AuthenticationEvent += HAGOSmartBluetoothControl.Api.CallbackAuthenticationEvent;
        ScanDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackScanDeviceEvent;
        StopScanDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackStopScanDeviceEvent;
        DeviceConnectedEvent += HAGOSmartBluetoothControl.Api.CallbackConnected;
        DataEvent += HAGOSmartBluetoothControl.Api.CallbackDataEvent;
        DisconnectEvent += HAGOSmartBluetoothControl.Api.CallbackDisconnect;
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
    /// <summary>
    /// receive callback from getting device types list
    /// </summary>
    /// <param name="result"></param>

    public void AuthenticationCallback(string result)
    {
        AuthenticationEvent?.Invoke(result);
    }
    public void DeviceTypeCallback(string result)
    {
        Debug.Log(string.Format("{0} Receive device type list: {1}", TAG, result));
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
         //Debug.Log("Receive send data callback: " + result);
         DataEvent?.Invoke(result);
    }

    public void DisconnectCallback(string result)
    {
        Debug.Log("Receive disconnect data callback: " + result);
        DisconnectEvent?.Invoke(result);
    }

}
