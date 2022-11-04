using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class TaggleIHealthLabs : MonoBehaviour
{
	private static TaggleIHealthLabs _instance;

	public static TaggleIHealthLabs Instance
	{
		get
		{
			if (_instance == null)
			{
				var obj = new GameObject("TaggleIHealthLabs");
				_instance = obj.AddComponent<TaggleIHealthLabs>();
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
		//DontDestroyOnLoad(gameObject);
	}

	private void Start()
    {

	}

	public void AuthenticationIHealthLabs()
	{
		//CheckPermission();

		Debug.Log("Click Authentication button");
#if UNITY_ANDROID
        TaggleIHealthLabsAndroid.Authentication();
#elif UNITY_IOS 
		TaggleIHealthLabsIOS.Authentication();
#endif
	}

	public void ScanIHealthLabs(string deviceType)
	{
#if UNITY_ANDROID
        TaggleIHealthLabsAndroid.GetDeviceTypes();
        TaggleIHealthLabsAndroid.ScanDevices(deviceType);
#elif UNITY_IOS
		TaggleIHealthLabsIOS.ScanDevices(deviceType);
#endif
	}

	public void StopScanIHealthLabs()
    {
#if UNITY_ANDROID
        //TaggleIHealthLabsAndroid.GetDeviceTypes();
        TaggleIHealthLabsAndroid.StopScanDevices();
#elif UNITY_IOS
		TaggleIHealthLabsIOS.StopScanDevices();
#endif
    }

    public void ConnectDeviceIHealthLabs(string mac, string deviceType)
	{
		Debug.Log("Click Connect button");
#if UNITY_ANDROID
        TaggleIHealthLabsAndroid.ConnectDevice(mac, deviceType);
#elif UNITY_IOS
		TaggleIHealthLabsIOS.ConnectDevices();
#endif
	}

	public void DisconnectDeviceIHealthLabs(string mac, string deviceType)
	{
		Debug.Log("Click Disconnect button");
#if UNITY_ANDROID
        TaggleIHealthLabsAndroid.DisconnectDevice(mac, deviceType);
#elif UNITY_IOS
		TaggleIHealthLabsIOS.DisconnectDevice();
#endif
	}

}

public class TaggleIHealthLabsIOS
{
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void _authenticationIHealthLabs();

	[DllImport("__Internal")]
	private static extern void _scanDeviceIHealthLabs(string deviceType);

    [DllImport("__Internal")]
    private static extern void _stopScanDeviceIHealthLabs();

    [DllImport("__Internal")]
	private static extern void _connectDeviceIHealthLabs();

	[DllImport("__Internal")]
	private static extern void _disconnectDeviceIHealthLabs();

	public static void Authentication()
	{
		_authenticationIHealthLabs();
	}

	public static void ScanDevices(string deviceType)
	{
		_scanDeviceIHealthLabs(deviceType);
	}

    public static void StopScanDevices()
    {
        _stopScanDeviceIHealthLabs();
    }

    public static void ConnectDevices()
	{
		_connectDeviceIHealthLabs();
	}
	public static void DisconnectDevice()
	{
		_disconnectDeviceIHealthLabs();
	}
#endif
}

public class TaggleIHealthLabsAndroid
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
		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.ihealthlabs.IHealthLabsBridge"))
		{
			utils.CallStatic("GetIHealthDeviceTypesList");
		}
	}

	public static void ScanDevices(string deviceType)
	{
		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.ihealthlabs.IHealthLabsBridge"))
		{
			utils.CallStatic("ScanDevices", deviceType);
		}
	}

    public static void StopScanDevices()
    {
        using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.ihealthlabs.IHealthLabsBridge"))
        {
            utils.CallStatic("StopScanDevices");
        }
    }

    public static void ConnectDevice(string mac, string deviceType)
	{
		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.ihealthlabs.IHealthLabsBridge"))
		{
			utils.CallStatic("ConnectDevice", mac, deviceType);
		}
	}
	public static void DisconnectDevice(string mac, string deviceType)
	{
		using (AndroidJavaClass utils = new AndroidJavaClass("com.taggle.ihealthlabs.IHealthLabsBridge"))
		{
			utils.CallStatic("DisconnectDevice", mac, deviceType);
		}
	}

}