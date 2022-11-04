using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class TaggleViatomLabs : MonoBehaviour
{
	private static TaggleViatomLabs _instance;

	public static TaggleViatomLabs Instance
	{
		get
		{
			if (_instance == null)
			{
				var obj = new GameObject("TaggleViatomLabs");
				obj.transform.SetParent(HAGOSmartBluetoothManager.Api.transform);
				_instance = obj.AddComponent<TaggleViatomLabs>();
			}

			return _instance;
		}
	}
	private void Awake()
	{
		if (_instance != null)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
	}

	public void ScanViatomDevice(string deviceType)
	{
#if UNITY_ANDROID
		TaggleViatomLabsAndroid.ScanDevices(deviceType);
#elif UNITY_IOS
		TaggleViatomLabsIOS.ScanDevices(deviceType);
#endif
	}

	public void StopScan()
	{
		Debug.Log("Click StopScan button");
#if UNITY_ANDROID
		TaggleViatomLabsAndroid.StopScanDevices();
#elif UNITY_IOS
        TaggleViatomLabsIOS.StopScanDevice();
#endif
    }

    public void ConnectDeviceViatomLabs(string mac, string deviceType)
    {
        Debug.Log("Click Connect button");
#if UNITY_ANDROID
        TaggleViatomLabsAndroid.ConnectDevice(mac, deviceType);
#elif UNITY_IOS
        TaggleViatomLabsIOS.ConnectDevices(mac, deviceType);
#endif
    }
    public void DisconnectDeviceViatomLabs(string mac, string deviceType)
	{
        Debug.Log("Click disconnect button");
#if UNITY_ANDROID
        TaggleViatomLabsAndroid.DisconnectDevice(mac, deviceType);
#elif UNITY_IOS
        TaggleViatomLabsIOS.DisconnectDevice(mac, deviceType);
#endif
    }
}
public class TaggleViatomLabsIOS
{
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void _scanDeviceTaggleViatomLabs(string deviceNameType);

	[DllImport("__Internal")]
	private static extern void _stopScanDeviceTaggleViatomLabs();

	[DllImport("__Internal")]
	private static extern void _connectDeviceTaggleViatomLabs(string deviceMac, string deviceName);

	[DllImport("__Internal")]
	private static extern void _disconnectDeviceTaggleViatomLabs(string deviceMac, string deviceName);

	//public static void ScanDevices()
	//   {
	//	_scanDeviceTaggleViatomLabs("");
	//}

	public static void ScanDevices(string deviceType)
	{
		_scanDeviceTaggleViatomLabs(deviceType);
	}

	public static void StopScanDevice()
	{
		_stopScanDeviceTaggleViatomLabs();
	}

	public static void ConnectDevices(string deviceMac, string deviceName)
	{
		_connectDeviceTaggleViatomLabs(deviceMac, deviceName);
	}

	public static void DisconnectDevice(string deviceMac, string deviceName)
	{
		_disconnectDeviceTaggleViatomLabs(deviceMac, deviceName);
	}

#endif
}

public class TaggleViatomLabsAndroid
{
	public static void Authentication()
	{
 		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.ihealthlabs.IHealthLabsBridge"))
        {
            utils.CallStatic("AuthenticationDevices");
        }
	}
	public static void GetDeviceTypes()
	{
		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.viatom.ViatomLabsBridge"))
		{
			utils.CallStatic("GetIHealthDeviceTypesList");
		}
	}
	
    public static void ScanDevices(string deviceType)
    {
        using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.viatom.ViatomLabsBridge"))
        {
            utils.CallStatic("ScanDevices", deviceType);
        }
    }
	
    public static void StopScanDevices()
    {
        using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.viatom.ViatomLabsBridge"))
        {
            utils.CallStatic("StopScanDevices");
        }
    }

    public static void ConnectDevice(string mac, string deviceType)
    {
		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.viatom.ViatomLabsBridge"))
		{
			utils.CallStatic("ConnectDevice", mac, deviceType);
        }
    }

    public static void DisconnectDevice(string mac, string deviceType)
	{
 		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.viatom.ViatomLabsBridge"))
        {
            utils.CallStatic("DisconnectDevice",mac,deviceType);
        }
	}
	
}