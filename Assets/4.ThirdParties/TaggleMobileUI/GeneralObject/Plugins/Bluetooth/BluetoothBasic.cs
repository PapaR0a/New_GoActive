using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class BluetoothBasic : MonoBehaviour
{

    private static BluetoothBasic _instance;
    public static BluetoothBasic Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("BluetoothBasic");
                obj.transform.SetParent(HAGOSmartBluetoothManager.Api.transform);
                _instance = obj.AddComponent<BluetoothBasic>();
            }

            return _instance;
        }
    }

    #region DEF
    // UUID example
    public static string INFO_SERVICE_UUID_HEART_RATE = "0000180d-0000-1000-8000-00805f9b34fb";
    public static string INFO_CHAR_UUID_HEART_RATE = "00002a37-0000-1000-8000-00805f9b34fb";

    // UUID base
    static string INFO_SERVICE_UUID_BATTERY = "0000180f-0000-1000-8000-00805f9b34fb";
    static string INFO_CHAR_UUID_BATTERY = "00002a19-0000-1000-8000-00805f9b34fb";

    // Configs
    static string TAG = "[BluetoothBasic] ";
    static float TIME_OUT = 5;
    public static bool IsLog = false;

    // Cache
    string m_ServiceUUID, m_CharUUID;

    public enum BLE_DEVICE_SUPPORT
    {
        ANY_DEVICE,
        POLAR,
        MOVESENSE,
        NONE
    }

    public enum BLE_TypeValue
    {
        DECIMAL,
        FLOAT,
        DOUBLE,
        STRING,
        SINGLE,
        COUNT
    }

    public enum STATUS_BLE
    {
        RUNNING,
        IDLE
    }

    public static string ParseBleDeviceSupport(BLE_DEVICE_SUPPORT type)
    {
        switch (type)
        {
            case BLE_DEVICE_SUPPORT.ANY_DEVICE:
                return "any_device";
            case BLE_DEVICE_SUPPORT.POLAR:
                return "polar";
            case BLE_DEVICE_SUPPORT.MOVESENSE:
                return "movesense";
            case BLE_DEVICE_SUPPORT.NONE:
                return "";
            default:
                return "";
        }
    }

    class SensorInfo
    {
        public string Name;
        public string Address;
        public int Rssi;

        public SensorInfo(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
    #endregion

    #region REF
    public static Action<string> AuthenticationEvent;
    public static Action<string> ScanDeviceEvent;
    public static Action StopScanDeviceEvent;
    public static Action<string> DeviceConnectedEvent;
    public static Action<string> DataEvent;
    public static Action<string> DisconnectEvent;
    public static Action<int> BatteryEvent;
    public static Action<string> ReadEvent;
    #endregion

    #region CACHE
    STATUS_BLE m_BLEStatus;
    bool m_IsInitialized;
    List<SensorInfo> m_Sensors = new List<SensorInfo>();
    bool m_IsSequeneAutoConnect_Runing;
    bool m_IsCompleteDetector;
    bool m_IsCompleteConnected, m_IsCompleteFoundHrChar;
    bool m_IsListenning;
    SensorInfo m_FoundSensor;
    #endregion

    // Use this for initialization
    void Start()
    {

    }

    private void OnDestroy()
    {
        DeInitialize();
    }

    public void Init()
    {
        AuthenticationEvent += HAGOSmartBluetoothControl.Api.CallbackAuthenticationEvent;
        ScanDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackScanDeviceEvent;
        StopScanDeviceEvent += HAGOSmartBluetoothControl.Api.CallbackStopScanDeviceEvent;
        DeviceConnectedEvent += HAGOSmartBluetoothControl.Api.CallbackConnected;
        DataEvent += HAGOSmartBluetoothControl.Api.CallbackDataEvent;
        ReadEvent += HAGOSmartBluetoothControl.Api.CallbackReadEvent;
        DisconnectEvent += HAGOSmartBluetoothControl.Api.CallbackDisconnect;
        ResetValue();
        Initialize();
    }

    void ResetValue()
    {
        m_IsSequeneAutoConnect_Runing = false;
        m_IsCompleteDetector = false;
        m_IsCompleteConnected = false;
        m_IsCompleteFoundHrChar = false;
        m_IsListenning = false;
    }

    void Initialize()
    {
        if (!m_IsInitialized)
        {
            BluetoothLEHardwareInterface.Initialize(true, false, () =>
            {
                Debug.Log(string.Format("{0} {1}", TAG, "Setup complete"));
                m_IsInitialized = true;
                string resultSuccess = "{result_code:1}";
                AuthenticationEvent?.Invoke(resultSuccess);
            }, (error) =>
            {
                Debug.Log(string.Format("{0} Error: {1}", TAG, error));
                m_IsInitialized = false;
            });
        }
    }

    void DeInitialize()
    {
        BluetoothLEHardwareInterface.DeInitialize(() => { Debug.Log("BT-Deinitializing"); });
    }

    /// <summary>
    /// Input: TypeDevice, Mode
    /// </summary>
    public void AutoConnect(string serviceUUID, string charUUID, BLE_DEVICE_SUPPORT typeDevice = BLE_DEVICE_SUPPORT.ANY_DEVICE, BLE_DEVICE_SUPPORT exceptDevice = BLE_DEVICE_SUPPORT.NONE)
    {
        // Config UUID
        m_ServiceUUID = serviceUUID;
        m_CharUUID = charUUID;

        if (m_IsSequeneAutoConnect_Runing)
        {
            Debug.Log(string.Format("{0} AutoConnect already start", TAG));
            return;
        }

        if (!m_IsInitialized)
        {
            EventComplete(string.Format("{0} Setup not complete", TAG));
            return;
        }

        StartCoroutine(IEAutoConnect(typeDevice, exceptDevice));
    }

    IEnumerator IEAutoConnect(BLE_DEVICE_SUPPORT typeDevice, BLE_DEVICE_SUPPORT exceptDevice)
    {
        // Reset all sensors
        if (m_Sensors.Count > 0)
        {
            StopListenAndDisconnect();
            while (m_BLEStatus == STATUS_BLE.RUNNING)
            {
                Debug.Log(string.Format("{0} Stopping sensor", TAG));
                yield return new WaitForSeconds(1);
            }
        }
        ResetValue();
        m_Sensors.Clear();

        // Step 1 - Scan
        yield return new WaitForEndOfFrame();
        StartScan(m_ServiceUUID, m_CharUUID);
        yield return new WaitForSeconds(TIME_OUT);
        StopScan();

        // Step 2 - Filter
        m_FoundSensor = EventSmartFilter(typeDevice, exceptDevice);

        // Step 3 - Connect sensor
        if (m_FoundSensor == null)
        {
            EventComplete(string.Format("{0} Sensor not found", TAG));
            yield return true;
        }
        else
        {
            Debug.Log(string.Format("{0} Found:{1} - {2}", TAG, m_FoundSensor.Name, m_FoundSensor.Address));
            yield return new WaitForSeconds(1);
            ConnectSensor(m_FoundSensor.Address);
        }

        while (!EventCheckConnected() && !EventCheckFoundHRCharacteristic())
        {
            yield return new WaitForSeconds(1);
        }

        // Step 4 - Start listen
        string address = m_FoundSensor.Address + "";
        StartListener(address);
        yield return new WaitForSeconds(1);

        // Try Get Battery
        RequestBattery();

        // Step 6 - Complete
        EventComplete("", m_FoundSensor.Name);
        m_IsSequeneAutoConnect_Runing = false;
    }

    public void StartScan(string serviceUUID, string charUUID, string namefilter = "")
    {
        m_ServiceUUID = serviceUUID;
        m_CharUUID = charUUID;
        m_Sensors.Clear();

        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(new[] { m_ServiceUUID }, (address, name) =>
        {
            Debug.Log(string.Format("[StartScan] New device {0}, try found name: {1}", name, namefilter));
            if (!m_Sensors.Exists((s) => s.Address.Equals(address)))
            {
                if (!string.IsNullOrEmpty(namefilter))
                {
                    if (name.ToLower().Contains(namefilter))
                    {
                        Debug.Log("[StartScan] found " + namefilter);
                        m_Sensors.Add(new SensorInfo(name, address));
                    }
                }

                if (m_Sensors.Count > 0)
                {
                    string raw = "";
                    for (int i = 0; i < m_Sensors.Count; i++)
                    {
                        if (i == 0)
                        {
                            raw += string.Format("{0}-{1}", m_Sensors[i].Address, m_Sensors[i].Name);
                        }
                        else
                        {
                            raw += string.Format("|{0}-{1}", m_Sensors[i].Address, m_Sensors[i].Name);
                        }
                    }
                    ScanDeviceEvent?.Invoke(raw);
                }

            }
        });

    }

    public void StopScan()
    {
        BluetoothLEHardwareInterface.StopScan();
        StopScanDeviceEvent?.Invoke();
    }

    SensorInfo EventSmartFilter(BLE_DEVICE_SUPPORT typeDevice, BLE_DEVICE_SUPPORT exceptDevice)
    {
        string aimDeviceName = ParseBleDeviceSupport(typeDevice);
        string exceptDeviceName = ParseBleDeviceSupport(exceptDevice);

        // Target filter
        List<SensorInfo> aimFounds = new List<SensorInfo>();
        if (typeDevice == BLE_DEVICE_SUPPORT.ANY_DEVICE)
            aimFounds = m_Sensors.FindAll(s => !s.Name.ToLower().Equals(exceptDeviceName.ToLower()));
        else
            aimFounds = m_Sensors.FindAll(s => s.Name.ToLower().Equals(aimDeviceName.ToLower()));


        if (aimFounds == null)
            return null;
        if (aimFounds.Count == 0)
            return null;

        Debug.Log(string.Format("{0} Filter:{1} - Except:{2} - Devices Count:{3}", TAG, aimFounds.Count, exceptDevice, m_Sensors.Count));

        // Rssi filter
        aimFounds = aimFounds.OrderByDescending(s => s.Rssi).ToList();
        SensorInfo result = aimFounds[0];

        return result;
    }

    public void ConnectSensor(string mac)
    {
        Debug.Log(string.Format("{0} {1} Connecting", TAG, mac));
        m_IsCompleteFoundHrChar = false;
        if (m_FoundSensor == null)
        {
            m_FoundSensor = new SensorInfo("", mac);
        }
        else
        {
            m_FoundSensor.Address = mac;
        }
        Invoke("DelayConnectSensor", 1);
    }

    private void DelayConnectSensor()
    {
        Debug.Log(string.Format("{0} DelayConnectSensor. Connecting {1}", TAG, m_FoundSensor.Address));
        BluetoothLEHardwareInterface.ConnectToPeripheral(m_FoundSensor.Address, (address) =>
        {
            Debug.Log(string.Format("{0} {1} Connected", TAG, address));
            m_IsCompleteConnected = true;
        }, (address, serviceUUID) =>
        {
            //Debug.Log("[EventConnectSensor] - Service:" + address + "/" + serviceUUID);
        }, (address, serviceUUID, characteristic) =>
        {
            //Debug.Log("[EventConnectSensor] Characteristic:" + address + "/" + serviceUUID + "/" + characteristic);
            if (characteristic.ToLower().Contains(m_CharUUID.ToLower()))
            {
                m_IsCompleteFoundHrChar = true;
                Debug.Log(string.Format("{0} Found characteristic: {1}@{2}", TAG, address, m_CharUUID));
                DeviceConnectedEvent?.Invoke(address);
            }
        }, (address) =>
        {
            Debug.Log(string.Format("{0} {1} Disconnected", TAG, address));
            //m_IsCompleteConnected = false;
            //m_IsCompleteDetector = false;

            if (m_IsListenning)
            {
                // @ Restart listener
                Invoke("RestartListener", .5f);
            }
        });
    }

    public void RequestBattery()
    {
        if (!string.IsNullOrEmpty(m_FoundSensor.Address))
        {
            BluetoothLEHardwareInterface.ReadCharacteristic(m_FoundSensor.Address, INFO_SERVICE_UUID_BATTERY, INFO_CHAR_UUID_BATTERY, (characteristic, bytes) =>
            {
                if (bytes.Length > 0)
                {
                    BatteryEvent?.Invoke((int)Convert.ToDecimal(bytes[0]));
                }
            });
        }
    }

    public void ReadCharacteristic(string serviceUUID, string charUUID, BLE_TypeValue type)
    {
        if (!string.IsNullOrEmpty(m_FoundSensor.Address))
        {
            BluetoothLEHardwareInterface.ReadCharacteristic(m_FoundSensor.Address, serviceUUID, charUUID, (characteristic, bytes) =>
            {
                Debug.Log("Raw Data: " + bytes.ToString());
                Debug.Log("==============>received data: " + bytes.Length);

                if (bytes.Length > 0)
                {
                    var sb = new System.Text.StringBuilder("");
                    sb.Append(string.Format("({0})", bytes.Length));
                    foreach (var b in bytes)
                    {
                        sb.Append(b + ", ");
                    }

                    var finalValue = ParseBytes(bytes, type);
                    ReadEvent?.Invoke(finalValue);
                }
            });
        }
    }

    public void WriteCharacteristic(string serviceUUID, string charUUID, byte[] data)
    {
        Debug.Log("call WriteCharacteristic");
        if (!string.IsNullOrEmpty(m_FoundSensor.Address))
        {
            BluetoothLEHardwareInterface.WriteCharacteristic(m_FoundSensor.Address, serviceUUID, charUUID, data, data.Length, true, (result) =>
            {
                Debug.Log("Raw Data: " + result);
            });
        }
    }

    void ReconnectSensor()
    {
        ConnectSensor(m_FoundSensor.Address);
    }

    bool EventCheckConnected()
    {
        return m_IsCompleteConnected;
    }

    bool EventCheckFoundHRCharacteristic()
    {
        return m_IsCompleteFoundHrChar;
    }

    void RestartListener()
    {
        string mac = m_FoundSensor.Address;
        Debug.Log(string.Format("{0} RestartListener mac: {1}", TAG, mac));
        StartListener(mac);
    }

    public void StartListener(string mac)
    {
        if (m_FoundSensor == null)
        {
            m_FoundSensor = new SensorInfo("", mac);
        }
        else
        {
            m_FoundSensor.Address = mac;
        }
        Debug.Log(string.Format("{0} StartListener, Address: {1}", TAG, m_FoundSensor.Address));
        BluetoothLEHardwareInterface.SubscribeCharacteristic(m_FoundSensor.Address, m_ServiceUUID, m_CharUUID, RefeshDevice, OnReceiveData);
        m_IsListenning = true;
        Debug.Log("IEStartListenner success");
    }

    void RefeshDevice(string notification)
    {
        // Some error unknow
        if (notification.Equals("1"))
        {
            Debug.Log(string.Format("{0} Error unknow", TAG));
            //EventStopVisualAndDisconnect();
            //EventComplete("Sensor not found");
        }
    }

    void OnReceiveData(string characteristic, byte[] bytes)
    {
        m_IsCompleteDetector = true;
        string value = "";
        switch (HAGOSmartBluetoothControl.Api.FocusType)
        {
            case HAGOConnectType.BLUETOOTH_PROFILE:
                value = HAGOSmartBluetoothControl.Api.DecodeHeartRate(bytes).ToString();
                break;
            default:
                break;
        }

        //Debug.Log(string.Format("{0}OnReceiveData: {1} Value: {2}", TAG, characteristic, value));
        DataEvent?.Invoke(string.Format("{0}|{1}", characteristic, value));
    }

    public void StopListener()
    {
        if (!m_IsCompleteDetector)
            return;

        Debug.Log(string.Format("{0} Call stopListener", TAG));
        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(m_FoundSensor.Address, m_ServiceUUID, m_CharUUID, (noti) =>
        {
            m_IsCompleteDetector = false;
            Debug.Log(string.Format("{0} UnSubscribe noti: {1}", TAG, noti));
        });

        m_IsListenning = false;
    }

    public void DisconnectDevice(string focusAddress)
    {
        m_IsListenning = false;
        StopListener();
        BluetoothLEHardwareInterface.DisconnectPeripheral(focusAddress, (address) =>
        {
            Debug.Log(string.Format("{0} Disconnected address: {1}", TAG, address));
            DisconnectEvent?.Invoke(address);
            m_BLEStatus = STATUS_BLE.IDLE;
        });
    }

    public void DisconnectAll()
    {
        m_IsListenning = false;
        BluetoothLEHardwareInterface.DisconnectAll();
    }

    public void StopListenAndDisconnect()
    {
        if (m_BLEStatus == STATUS_BLE.IDLE)
        {
            return;
        }

        try
        {
            Invoke("EventDisconnect", 1);
            StopListener();
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("{0} StopVisualAndDisconnect error: {1}", TAG, e.Message));
            m_BLEStatus = STATUS_BLE.IDLE;
        }

    }

    void EventComplete(string error = "", string deviceName = "")
    {
        if (string.IsNullOrEmpty(error))
            m_BLEStatus = STATUS_BLE.RUNNING;
        else
            m_BLEStatus = STATUS_BLE.IDLE;

        Debug.Log(string.Format("{0} EventComplete: status {1}, error: {2}", TAG, m_BLEStatus, error));
        DeviceConnectedEvent?.Invoke(deviceName);
    }

    static string BytesToString(byte[] bytes)
    {
        string result = "";

        foreach (var b in bytes)
            result += b.ToString("X2");

        return result;
    }

    string ParseBytes(byte[] raw, BLE_TypeValue type)
    {
        string result = "";
        switch (type)
        {
            case BLE_TypeValue.DECIMAL:
                {
                    foreach (var item in raw)
                    {
                        var value = Convert.ToDecimal(item);
                        result += value.ToString() + " | ";
                    }
                }
                break;
            case BLE_TypeValue.FLOAT:
                {
                    if (raw.Length >= 4)
                    {
                        var value = BitConverter.ToSingle(raw, 0);
                        result += value.ToString() + " | ";
                    }
                }
                break;
            case BLE_TypeValue.DOUBLE:
                {
                    foreach (var item in raw)
                    {
                        var value = Convert.ToDouble(item);
                        result += value.ToString() + " | ";
                    }
                }
                break;
            case BLE_TypeValue.SINGLE:
                {
                    foreach (var item in raw)
                    {
                        var value = Convert.ToSingle(item);
                        result += value.ToString() + " | ";
                    }
                }
                break;
            case BLE_TypeValue.STRING:
                {
                    foreach (var item in raw)
                    {
                        var value = Convert.ToDouble(item);
                        result += value.ToString() + " | ";
                    }
                }
                break;
            default:
                break;
        }
        Debug.LogFormat("[ParseBytes - {0}] value: {1}", type.ToString(), result);
        return result;
    }

}
