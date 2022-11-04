using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaggleTemplate.Comm;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class HAGOSmartBluetoothControl
{
    private static HAGOSmartBluetoothControl m_api;
    public static HAGOSmartBluetoothControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOSmartBluetoothControl();
            }
            return m_api;
        }
    }
    
    //debug
    private string TAG = "HAGOSmartBluetoothControl";

    //event
    public Action<string> AuthenticationEvent;
    public Action<HAGODataScan> ScanEvent;
    public Action StopScanEvent;
    public Action<HAGODeviceDTO> ConnectedEvent;
    public Action<Dictionary<string, string>> DataEvent;
    public Action<HAGODeviceDTO> DisconnectEvent;
    
    public HAGOConnectType FocusType 
    {
        get
        {
            return HAGOSmartBluetoothModel.Api.FocusType;
        }
        set
        {
            HAGOSmartBluetoothModel.Api.FocusType = value;
        }
    }
    
    public string DeviceType 
    {
        get
        {
            return HAGOSmartBluetoothModel.Api.DeviceType;
        }
        set
        {
            HAGOSmartBluetoothModel.Api.DeviceType = value;
        }
    }

    public void Init()
    {
        HAGOSmartBluetoothModel.Api.ResetValue();
        SetBluetoothEnabled();
        CoroutineHelper.Call(IERequestPermission());
    }
    
    public void Exit()
    {
        DisconnectAllDevices();
        UnregisterEvents();
        HAGOSmartBluetoothManager.Api.Destroy();
        Debug.Log("[HAGOSmartBluetoothManager] call Exit");
    }

    public HAGODeviceDTO GetDeviceConnected(string vsmStat)
    {
        return HAGOSmartBluetoothModel.Api.GetDeviceConnected(vsmStat);
    }

    public bool IsDeviceWithVSMStatConnected(string vsmStat)
    {
        return HAGOSmartBluetoothModel.Api.IsDeviceWithVSMStatConnected(vsmStat);
    }

    IEnumerator IERequestPermission()
    {
        // Check and request permission
        CheckPermissionSuccess();
        if(!HAGOSmartBluetoothModel.Api.GetPermissionSuccess())
            RequestPermission();

        while (!HAGOSmartBluetoothModel.Api.GetPermissionSuccess())
        {
            CheckPermissionSuccess();
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetBluetoothEnabled()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            try
            {
                using (var BluetoothManager = activity.Call<AndroidJavaObject>("getSystemService", "bluetooth"))
                {
                    using (var BluetoothAdapter = BluetoothManager.Call<AndroidJavaObject>("getAdapter"))
                    {
                        BluetoothAdapter.Call<bool>("enable");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("{0} Set enable bluetooth error: {1}", TAG, e.Message));
            }
        #endif
    }

    public void RequestPermission()
    {
        if (HAGOSmartBluetoothModel.Api.GetPermissionSuccess())
        {
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            //Call request
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
#elif UNITY_EDITOR
#endif
    }

    public void CheckPermissionSuccess()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            HAGOSmartBluetoothModel.Api.SetPermissionSuccess(true);
        }
#elif UNITY_EDITOR
        HAGOSmartBluetoothModel.Api.SetPermissionSuccess(true);
#endif
    }

    public void SelectTypeDevice(string connectType, string deviceType)
    {
        Debug.Log(string.Format("{0}SelectTypeDevice: {1}, DeviceType {2}", TAG, connectType, deviceType));

        DisconnectAllDevices();
        UnregisterEvents();

        FocusType = (HAGOConnectType)Enum.Parse(typeof(HAGOConnectType), connectType, true);
        DeviceType = deviceType;
        CoroutineHelper.Call(IERegisterEvents());
    }

    private IEnumerator IERegisterEvents()
    {
        yield return new WaitForEndOfFrame();

        // Register callback
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                {
                    if(TaggleIHealthLabsManager.Instance == null)
                    {
                        Debug.Log("TaggleIHealthLabsManager created");
                    }
                    TaggleIHealthLabsManager.Instance.Init();
                }
                break;
            case HAGOConnectType.VIATOM_SDK:
                {
                    if (TaggleViatomLabsManager.Instance == null)
                    {
                        Debug.Log("TaggleViatomLabsManager created");
                    }
                    TaggleViatomLabsManager.Instance.Init();
                }
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                {
                    Debug.Log("BluetoothBasic step 0");
                    if (BluetoothBasic.Instance == null)
                    {
                        Debug.Log("BluetoothBasic created");
                    }
                    BluetoothBasic.Instance.Init();
                    Debug.Log("BluetoothBasic step 1");
                }
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }

        Authentication();
    }

    private void UnregisterEvents()
    {
        Debug.Log("TaggleIHealthLabsManager call UnregisterEvents");

        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                {
                    TaggleIHealthLabsManager.AuthenticationEvent -= CallbackAuthenticationEvent;
                    TaggleIHealthLabsManager.ScanDeviceEvent -= CallbackScanDeviceEvent;
                    TaggleIHealthLabsManager.StopScanDeviceEvent -= CallbackStopScanDeviceEvent;
                    TaggleIHealthLabsManager.DeviceConnectedEvent -= CallbackConnected;
                    TaggleIHealthLabsManager.DataEvent -= CallbackDataEvent;
                    TaggleIHealthLabsManager.DisconnectEvent -= CallbackDisconnect;
                }
                break;
            case HAGOConnectType.VIATOM_SDK:
                {
                    TaggleViatomLabsManager.ScanDeviceEvent -= CallbackScanDeviceEvent;
                    TaggleViatomLabsManager.StopScanDeviceEvent -= CallbackStopScanDeviceEvent;
                    TaggleViatomLabsManager.DeviceConnectedEvent -= CallbackConnected;
                    TaggleViatomLabsManager.SendDataEvent -= CallbackDataEvent;
                    TaggleViatomLabsManager.DisconnectDeviceEvent -= CallbackDisconnect;
                }
                break;
            case HAGOConnectType.IMU:
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                {
                    BluetoothBasic.AuthenticationEvent -= CallbackAuthenticationEvent;
                    BluetoothBasic.ScanDeviceEvent -= CallbackScanDeviceEvent;
                    BluetoothBasic.StopScanDeviceEvent -= CallbackStopScanDeviceEvent;
                    BluetoothBasic.DeviceConnectedEvent -= CallbackConnected;
                    BluetoothBasic.DataEvent -= CallbackDataEvent;
                    BluetoothBasic.ReadEvent -= CallbackReadEvent;
                    BluetoothBasic.DisconnectEvent -= CallbackDisconnect;
                }
                break;
            default:
                break;
        }
    }

    private void DisconnectAllDevices()
    {
        List<HAGODeviceDTO> listDeviceConnected = HAGOSmartBluetoothModel.Api.GetListDeviceConnected();
        Debug.Log("[HAGOSmartBluetoothManager] Call DisconnectAllDevices. devices count: " + listDeviceConnected.Count);

        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                {
                    if (listDeviceConnected.Count > 0)
                    {
                        for (int i = 0; i < listDeviceConnected.Count; i++)
                        {
                            var item = listDeviceConnected[i];
                            Disconnect(new string[] { item.Id, item.Name });
                            Debug.Log("[HAGOSmartBluetoothManager] Call DisconnectAllDevices. disconnect: " + item.Id);
                        }
                    }
                }
                break;
            case HAGOConnectType.VIATOM_SDK:
                {
                    if (listDeviceConnected.Count > 0)
                    {
                        for (int i = 0; i < listDeviceConnected.Count; i++)
                        {
                            var item = listDeviceConnected[i];
                            Disconnect(new string[] { item.Id, item.Name });
                            Debug.Log("[HAGOSmartBluetoothManager] Call DisconnectAllDevices. disconnect: " + item.Id);
                        }
                    }
                }
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                {
                    BluetoothBasic.Instance.DisconnectAll();
                }
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }

        HAGOSmartBluetoothModel.Api.ClearListDeviceConnected();
    }

    public void Authentication()
    {
#if !UNITY_EDITOR
        if (FocusType == HAGOConnectType.IHEALTH_SDK)
        {
            TaggleIHealthLabs.Instance.AuthenticationIHealthLabs();
        }
        else // Others, no need to authentication
        {
            string resultSuccess = "{result_code:1}";
            AuthenticationEvent?.Invoke(resultSuccess);
        }
#else
        string resultSuccess = "{result_code:1}";
        HAGOSmartBluetoothControl.Api.AuthenticationEvent?.Invoke(resultSuccess);
#endif
    }

    public void ScanBluetooth(string deviceType)
    {
        Debug.Log(string.Format("{0}ScanBluetooth", TAG));
#if !UNITY_EDITOR
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                TaggleIHealthLabs.Instance.ScanIHealthLabs(deviceType);
                break;

            case HAGOConnectType.VIATOM_SDK:
                TaggleViatomLabs.Instance.ScanViatomDevice(deviceType);
                break;

            case HAGOConnectType.BLUETOOTH_PROFILE:
                BluetoothBasic.Instance.StartScan(BluetoothBasic.INFO_SERVICE_UUID_HEART_RATE, BluetoothBasic.INFO_CHAR_UUID_HEART_RATE, "polar");
                break;

            case HAGOConnectType.IMU:
                break;

            default:
                break;
        }
#else
        Debug.Log(string.Format("{0}Scan Bluetooth", TAG));
#endif
    }

    public void StopScanBluetooth()
    {
#if !UNITY_EDITOR
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                TaggleIHealthLabs.Instance.StopScanIHealthLabs();
                break;
             case HAGOConnectType.VIATOM_SDK:
                TaggleViatomLabs.Instance.StopScan();
                break;

            case HAGOConnectType.BLUETOOTH_PROFILE:
                BluetoothBasic.Instance.StopScan();
                break;

            case HAGOConnectType.IMU:
                break;

            default:
                break;
        }
#else
        Debug.Log(string.Format("{0}Scan Bluetooth", TAG));
#endif
    }

    public void ConnectBluetooth(string[] input)
    {
        StopScanBluetooth();
        Debug.Log(string.Format("{0}ConnectBluetooth: {1}", TAG, FocusType.ToString()));

        string mac = "";
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                mac = input[0];
                string detailType = input[1];
                Debug.Log("ConnectBluetooth IHEALTH_SDK - device type: " + DeviceType);
                TaggleIHealthLabs.Instance.ConnectDeviceIHealthLabs(mac, DeviceType);
                break;
            case HAGOConnectType.VIATOM_SDK:
                mac = input[0];
                Debug.Log("ConnectBluetooth VIATOM_SDK - device type: " + DeviceType);
                TaggleViatomLabs.Instance.ConnectDeviceViatomLabs(mac, DeviceType);
                break;

            case HAGOConnectType.BLUETOOTH_PROFILE:
                mac = input[0];
                BluetoothBasic.Instance.ConnectSensor(mac);
                break;

            default:
                // Device not define
                break;
        }
    }

    public void Disconnect(string[] input)
    {
        string mac = input[0];
        string detailType = input[1];
        Debug.Log(string.Format("{0}Disconnect: {1}", TAG, FocusType.ToString()));
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                {
                    TaggleIHealthLabs.Instance.DisconnectDeviceIHealthLabs(mac, detailType.Split('-')[0]);
                }
                break; 
            case HAGOConnectType.VIATOM_SDK:
                {
                    TaggleViatomLabs.Instance.DisconnectDeviceViatomLabs(mac, detailType.Split('-')[0]);
                }
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                {
                    BluetoothBasic.Instance.DisconnectDevice(mac);
                }
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }
        
    }

    private void StartListener(string mac)
    {
        Debug.Log(string.Format("{0} Call StartListener mac: {1}", TAG, mac));
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                // Auto start listener
                break;  
            case HAGOConnectType.VIATOM_SDK:
                // Auto start listener
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                {
                    BluetoothBasic.Instance.StartListener(mac);
                }
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }
    }

    public void ReadValue(string serviceUUID, string characteristicUUID, BluetoothBasic.BLE_TypeValue type)
    {
        Debug.LogFormat("{0} Call ReadValue service: {1} Characteristic: {2}", TAG, serviceUUID, characteristicUUID);
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                // Auto start listener
                break;
            case HAGOConnectType.VIATOM_SDK:
                // Auto start listener
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                {
                    BluetoothBasic.Instance.ReadCharacteristic(serviceUUID, characteristicUUID, type);
                }
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }
    }

    private void RefeshListConnectedDevice(HAGODeviceDTO device)
    {
        Debug.Log(string.Format("{0} RefeshListConnectedDevice. Raw Device: {1}@{2}", TAG, device.Id, device.Status));
        if (device.Status == HAGODeviceStatusType.Connected)
        {
            Debug.Log(string.Format("{0} RefeshListConnectedDevice. Add device: {1}@{2}", TAG, device.Id, device.Status));
            HAGOSmartBluetoothModel.Api.AddDeviceConnected(device);
        }
        else
        {
            HAGODeviceDTO found = HAGOSmartBluetoothModel.Api.GetListDeviceConnected().Find((d) => d.Id.Equals(device.Id));
            if (found != null)
            {
                HAGOSmartBluetoothModel.Api.RemoveDeviceConnected(found);
                Debug.Log(string.Format("{0} RefeshListConnectedDevice. Remove device: {1}@{2}", TAG, device.Id, device.Status));
            }
        }
        Debug.Log(string.Format("{0} RefeshListConnectedDevice. Device Count: {1}", TAG, HAGOSmartBluetoothModel.Api.GetListDeviceConnected().Count));
    }
    

#region CALLBACKS
    public void CallbackAuthenticationEvent(string result)
    {
        HAGOSmartBluetoothControl.Api.AuthenticationEvent?.Invoke(result);
    }

    public void CallbackScanDeviceEvent(string result)
    {
        HAGODataScan finalResult = null;
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                finalResult = ParseScanDevice(result, HAGOConnectType.IHEALTH_SDK);
                break;
            case HAGOConnectType.VIATOM_SDK:
                finalResult = ParseScanDevice(result, HAGOConnectType.VIATOM_SDK);
                break;

            case HAGOConnectType.BLUETOOTH_PROFILE:
                finalResult = ParseScanPolar(result);
                break;

            case HAGOConnectType.IMU:
                break;

            default:
                break;
        }

        HAGOSmartBluetoothControl.Api.ScanEvent?.Invoke(finalResult);
    }

    public void CallbackStopScanDeviceEvent()
    {
        Debug.Log(string.Format("{0} CallbackStopScanDeviceEvent", TAG));
        HAGOSmartBluetoothControl.Api.StopScanEvent?.Invoke();
    }

    public void CallbackConnected(string result)
    {
        Debug.Log(string.Format("{0} CallbackConnected Result: {1}", TAG, result));
        Debug.Log(string.Format("{0} CallbackConnected FocusType: {1} - DeviceType {2}", TAG, FocusType.ToString(), DeviceType));
        HAGODeviceDTO finalResult = null;
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                finalResult = ParseConnectStatus(result, HAGOConnectType.IHEALTH_SDK);
                break;
            case HAGOConnectType.VIATOM_SDK:
                finalResult = ParseConnectStatus(result, HAGOConnectType.VIATOM_SDK);
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                finalResult = ParseConnectStatusPolar(result, true);
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }

        // Cache list connected
        RefeshListConnectedDevice(finalResult);

        HAGOSmartBluetoothControl.Api.ConnectedEvent?.Invoke(finalResult);
        string mac = finalResult.Id;
        StartListener(mac);
    }
    public void CallbackDataEvent(string result)
    {
        string deviceType = DeviceType;
        Debug.Log(string.Format("{0} CallbackDataEvent. result: {1}", TAG, result));
        Debug.Log("CallbackDataEvent device type: " + deviceType);
        Dictionary<string, string> finalResult = new Dictionary<string, string>();
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                if(deviceType.Equals("PO3"))
                {
                    finalResult = ParseDataPO3MOrOxyViatom(result);
                }
                else if(deviceType.Equals("KN550BT") || deviceType.Equals("KN-550BT"))
                {
                    finalResult = ParseDataKN550BT(result);
                }                    
                break;
            case HAGOConnectType.VIATOM_SDK:
                finalResult = ParseDataPO3MOrOxyViatom(result);
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                finalResult = ParseDataPolarHR(result);
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }

        HAGOSmartBluetoothControl.Api.DataEvent?.Invoke(finalResult);
    }
    public void CallbackReadEvent(string result)
    {
        string deviceType = DeviceType;
        Debug.Log(string.Format("{0} CallbackDataEvent. result: {1}", TAG, result));
        Debug.Log("CallbackDataEvent device type: " + deviceType);
       
        //HAGOSmartBluetoothControl.Api.DataEvent?.Invoke(finalResult);
    }

    public void CallbackDisconnect(string result)
    {
        Debug.Log(string.Format("{0} CallbackDisconnect: {1}", TAG, result));
        HAGODeviceDTO finalResult = null;
        switch (FocusType)
        {
            case HAGOConnectType.IHEALTH_SDK:
                finalResult = ParseConnectStatus(result, HAGOConnectType.IHEALTH_SDK);
                break;
            case HAGOConnectType.VIATOM_SDK:
                finalResult = ParseConnectStatus(result, HAGOConnectType.VIATOM_SDK);
                break;
            case HAGOConnectType.BLUETOOTH_PROFILE:
                finalResult = ParseConnectStatusPolar(result, false);
                break;
            case HAGOConnectType.IMU:
                break;
            default:
                break;
        }

        // Cache list connected
        RefeshListConnectedDevice(finalResult);

        HAGOSmartBluetoothControl.Api.DisconnectEvent?.Invoke(finalResult);
    }
#endregion
    
    // @ Parser
    public int DecodeHeartRate(byte[] data)
    {
        int bpm = data[1];
        if ((data[0] & 0x01) != 0)
        {
            bpm = (ushort)(((bpm >> 8) & 0xFF) | ((bpm << 8) & 0xFF00));
        }
        return bpm;
    }

    public string BytesToString(byte[] bytes)
    {
        string result = "";

        foreach (var b in bytes)
            result += b.ToString("X2");

        return result;
    }

    public HAGODataScan ParseScanPolar(string input)
    {
        string[] raw = input.Split('|');
        if(raw.Length > 0)
        {
            var listDevices = new List<HAGODeviceDTO>();
            foreach (string item in raw)
            {
                string[] rawcontent = item.Split('-');
                string mac = rawcontent[0];
                string deviceName = rawcontent[1];
                //TODO: should pass DeviceType to deviceNameId (currently it not static)
                listDevices.Add(new HAGODeviceDTO(mac, deviceName + "-" + mac, HAGOConnectType.BLUETOOTH_PROFILE.ToString().ToLower(), DeviceType, HAGODeviceStatusType.Paired));
            }
            //// Set flag finish scan
            //listDevices.Add(new HAGODeviceDTO("DEVICE_STATE_SCAN_FINISHED", "Unknow", HAGOConnectType.POLAR_HR.ToString(), HAGODeviceStatusType.Paired));
            return new HAGODataScan(1, listDevices);
        }
        return new HAGODataScan(1, new List<HAGODeviceDTO>());
    }

    public HAGODeviceDTO ParseConnectStatusPolar(string input, bool isConnected)
    {
        string mac = input;
        
        //TODO: should pass DeviceType to deviceNameId (currently it not static)
        return new HAGODeviceDTO(mac, "", HAGOConnectType.BLUETOOTH_PROFILE.ToString().ToLower(), DeviceType, isConnected ? HAGODeviceStatusType.Connected : HAGODeviceStatusType.Disconnected);
    }

    private Dictionary<string, string> ParseDataPolarHR(string input)
    {
        string[] raw = input.Split('|');
        string charUUID = raw[0];
        string heartRate = raw[1];
        return new Dictionary<string, string>(){ { VSMTypes.HEART_RATE, heartRate } };
    }
    
    public HAGODataScan ParseScanDevice(string result, HAGOConnectType connectType)
    {
        //Debug.Log("ParseScanPO3M: " + result);
        bool isNull = false;
        string resultCode = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
        JObject itemData = JObject.Parse(result);
        isNull = itemData == null;
        resultCode = itemData.Value<string>("result_code");
#elif UNITY_IOS && !UNITY_EDITOR
        JToken itemData = JToken.Parse(result);
        isNull = itemData == null;
        resultCode = itemData.Value<string>("result_code");
#endif
        //Debug.Log("ParseScanDevice token: " + itemData.ToString());

        if (isNull == false)
        {
            //Debug.Log("ParseScanDevice resultCode: " + resultCode);
            List<HAGODeviceDTO> lstDevices = new List<HAGODeviceDTO>();
            if (resultCode == "1")
            {
                string deviceMac = string.Empty;
                string mac = string.Empty;
                string deviceName = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
                Debug.Log("ParseScanIHealth itemData: " + itemData.Count + " - " + itemData);
                JObject value = JObject.Parse(itemData.Value<string>("value"));
                deviceMac = value.Value<string>("deviceMac");
                mac = value.Value<string>("deviceMac");
                deviceName = value.Value<string>("deviceName");
#elif UNITY_IOS && !UNITY_EDITOR
                JObject value = itemData.Value<JObject>("value");
                deviceMac = value.Value<string>("deviceMac");
                mac = value.Value<string>("deviceMac");
                deviceName = value.Value<string>("deviceName");
#endif
                if (deviceMac != IHLSTATE.DEVICE_STATE_SCAN_FINISHED.ToString())
                {
                    lstDevices.Add(new HAGODeviceDTO(mac, deviceName + "-" + mac, connectType.ToString().ToLower(), DeviceType, HAGODeviceStatusType.Paired));
                }
            }
#if UNITY_EDITOR
            resultCode = "1"; // add for test at Unity Editor
            lstDevices.Add(new HAGODeviceDTO("00002a37-0000-1000-8000-00805f9b34fb", "PO3" + "-" + "00002a37-0000-1000-8000-00805f9b34fb", connectType.ToString().ToLower(), DeviceType, HAGODeviceStatusType.Paired));
#endif
            return new HAGODataScan(int.Parse(resultCode), lstDevices);
        }
        else
        {
            Debug.Log("Can't parse device data");
            return new HAGODataScan(-1, null);
        }
    }

    public HAGODeviceDTO ParseConnectStatus(string input, HAGOConnectType connectType)
    {
        JObject itemData = JObject.Parse(input);
        if (itemData != null)
        {
            string resultCode = itemData.Value<string>("result_code");
            if (resultCode == "1")
            {
                JObject value = null;
#if UNITY_ANDROID
                Debug.Log("ParseConnectStatus itemData: " + itemData.Count + " - " + itemData);
                value = JObject.Parse(itemData.Value<string>("value"));
#elif UNITY_IOS
                value = itemData.Value<JObject>("value");
#endif
                string deviceMac = value.Value<string>("deviceMac");
                string status = value.Value<string>("status");

                Debug.Log(string.Format("{0} ParseConnectStatus status: {1}", TAG, status));
                if (status.Equals(IHLSTATE.DEVICE_STATE_CONNECTED.ToString()))
                {
                    return new HAGODeviceDTO(deviceMac, "", connectType.ToString().ToLower(), DeviceType, HAGODeviceStatusType.Connected);
                }
                else
                {
                    return new HAGODeviceDTO(deviceMac, "", connectType.ToString().ToLower(), DeviceType, HAGODeviceStatusType.Disconnected);
                }
            }
            else
            {
                return new HAGODeviceDTO("unknow", "", "", "", HAGODeviceStatusType.Disconnected);
            }
            
        }
        else
        {
            Debug.Log("Can't parse device data");
            return new HAGODeviceDTO("", "", "", "", HAGODeviceStatusType.Disconnected);
        }
    }
      

    private Dictionary<string, string> ParseDataPO3MOrOxyViatom(string result)
    {
        //Android raw data from device
        //{"bloodoxygen":99,"heartrate":73,"pulsestrength":0,"pi":4.300000190734863,"pulseWave":[0,0,0],"dataID":"4C2E15BE0378A2048686596914F3708F"}
        //ios raw data from device
        //{ PI = "10.06786";  bpm = 66; dataID = 60a03a753dab99a12864022895bb9143;height = 0;spo2 = 98;  wave = (0,0,429); }
        ////update new data: {"oxygen_saturation":99,"heart_rate":73,"pulse_strength":0,"pi":4.300000190734863,"pulse_wave":[0,0,0],"data_id":"4C2E15BE0378A2048686596914F3708F","udid":"5CF821DECFCA"}
        Debug.Log(string.Format("{0} ParseDataPO3M - OnSendDataEvent - Data received: {1}", TAG, result));

        string resultCode = "";
        bool isNull = false;

#if UNITY_ANDROID
        JObject itemData = JObject.Parse(result);
        isNull = itemData == null;
#elif UNITY_IOS
        JToken itemData = JToken.Parse(result);
        isNull = itemData == null;
#endif

        if (isNull == false)
        {

#if UNITY_ANDROID || UNITY_IOS
        resultCode = itemData.Value<string>("result_code");
#endif
            //Debug.Log(string.Format("{0}OnScanDeviceEvent resultCode: {1}", TAG, resultCode));
            if (resultCode == "1")
            {
                JObject value = null;
#if UNITY_ANDROID
                value = JObject.Parse(itemData.Value<string>("value"));
#elif UNITY_IOS
                value = itemData.Value<JObject>("value");
#endif
                if (value != null)
                {
                    string oxygenSaturation = value.Value<string>("oxygen_saturation"); // value.Value<string>("bloodoxygen");
                    string heartRate = value.Value<string>("heart_rate"); // value.Value<string>("heartrate");
                    Debug.Log(string.Format("{0}Blood Oxygen:{1} - Heart Rate:{2}", TAG, oxygenSaturation, heartRate));
                    return new Dictionary<string, string> { { VSMTypes.OXYGEN_SATURATION, oxygenSaturation }, { VSMTypes.HEART_RATE, heartRate } };
                }
                else
                {
                    return new Dictionary<string, string> { { VSMTypes.OXYGEN_SATURATION, string.Empty }, { VSMTypes.HEART_RATE, string.Empty } };
                }
            }
            else
            {
                Debug.Log(string.Format("{0}Blood Oxygen - Heart Rate: Error", TAG));
                return new Dictionary<string, string> { { VSMTypes.OXYGEN_SATURATION, string.Empty }, { VSMTypes.HEART_RATE, string.Empty } };
            }
        }
        else
        {
            return new Dictionary<string, string> {};
        }

    }
    private Dictionary<string, string> ParseDataKN550BT(string result)
    {
        //Android raw data from device
        //{"date": "2014-02-21 21:15:00","blood_pressure_s": 131,"blood_pressure_d": 78,"pulse_wave": 67,"ahr": false,"udid":"9C1D5817EC99"}

        Debug.Log(string.Format("{0} ParseDataKN550BT - OnSendDataEvent - Data received: {1}", TAG, result));

        string resultCode = "";
        bool isNull = false;

#if UNITY_ANDROID
        JObject itemData = JObject.Parse(result);
        isNull = itemData == null;
#elif UNITY_IOS
        JToken itemData = JToken.Parse(result);
        isNull = itemData == null;
#endif

        if (isNull == false)
        {
#if UNITY_ANDROID || UNITY_IOS
        resultCode = itemData.Value<string>("result_code");
#endif
            //Debug.Log(string.Format("{0}OnScanDeviceEvent resultCode: {1}", TAG, resultCode));
            if (resultCode == "1")
            {
                JObject value = null;
#if UNITY_ANDROID
                value = JObject.Parse(itemData.Value<string>("value"));
#elif UNITY_IOS
                value = itemData.Value<JObject>("value");
#endif
                Debug.Log("ParseDataKN550BT value.ToString(): " + value.ToString());
                if (value != null && !value.ToString().Equals("message: null"))
                {
                    string blood_pressure_s = value.Value<string>("blood_pressure_s");
                    string blood_pressure_d = value.Value<string>("blood_pressure_d"); 
                    Debug.Log(string.Format("{0} Blood Pressure systolic: {1} - Blood Pressure diastolic: {2}", TAG, blood_pressure_s, blood_pressure_d));
                    return new Dictionary<string, string> { { VSMTypes.BLOOD_PRESSURE_SYS, blood_pressure_s }, { VSMTypes.BLOOD_PRESSURE_DIA, blood_pressure_d } };
                }
                else
                {
                    return new Dictionary<string, string> { { VSMTypes.BLOOD_PRESSURE_SYS, string.Empty }, { VSMTypes.BLOOD_PRESSURE_DIA, string.Empty } };
                }
            }
            else
            {
                Debug.Log(string.Format("{0} Blood Pressure systolic - Blood Pressure diastolic:: Error", TAG));
                return new Dictionary<string, string> { { VSMTypes.BLOOD_PRESSURE_SYS, string.Empty }, { VSMTypes.BLOOD_PRESSURE_DIA, string.Empty } };
            }
        }
        else
        {
            return new Dictionary<string, string> { };
        }

    }
}