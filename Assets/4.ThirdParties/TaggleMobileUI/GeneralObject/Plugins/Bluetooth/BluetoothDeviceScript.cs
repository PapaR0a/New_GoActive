using System;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothDeviceScript : MonoBehaviour
{
    public List<string> DiscoveredDeviceList;

    private Action m_InitializedAction;

    public Action InitializedAction
    {
        get { return m_InitializedAction; }
        set { m_InitializedAction = value; }
    }
    private Action m_DeinitializedAction;

    public Action DeinitializedAction
    {
        get { return m_DeinitializedAction; }
        set { m_DeinitializedAction = value; }
    }
    private Action<string> m_ErrorAction;

    public Action<string> ErrorAction
    {
        get { return m_ErrorAction; }
        set { m_ErrorAction = value; }
    }
    private Action<string> m_ServiceAddedAction;

    public Action<string> ServiceAddedAction
    {
        get { return m_ServiceAddedAction; }
        set { m_ServiceAddedAction = value; }
    }
    private Action m_StartedAdvertisingAction;

    public Action StartedAdvertisingAction
    {
        get { return m_StartedAdvertisingAction; }
        set { m_StartedAdvertisingAction = value; }
    }
    private Action m_StoppedAdvertisingAction;

    public Action StoppedAdvertisingAction
    {
        get { return m_StoppedAdvertisingAction; }
        set { m_StoppedAdvertisingAction = value; }
    }
    private Action<string, string> m_DiscoveredPeripheralAction;

    public Action<string, string> DiscoveredPeripheralAction
    {
        get { return m_DiscoveredPeripheralAction; }
        set { m_DiscoveredPeripheralAction = value; }
    }
    private Action<string, string, int, byte[]> m_DiscoveredPeripheralWithAdvertisingInfoAction;

    public Action<string, string, int, byte[]> DiscoveredPeripheralWithAdvertisingInfoAction
    {
        get { return m_DiscoveredPeripheralWithAdvertisingInfoAction; }
        set { m_DiscoveredPeripheralWithAdvertisingInfoAction = value; }
    }
    private Action<string, string> m_RetrievedConnectedPeripheralAction;

    public Action<string, string> RetrievedConnectedPeripheralAction
    {
        get { return m_RetrievedConnectedPeripheralAction; }
        set { m_RetrievedConnectedPeripheralAction = value; }
    }
    private Action<string, byte[]> m_PeripheralReceivedWriteDataAction;

    public Action<string, byte[]> PeripheralReceivedWriteDataAction
    {
        get { return m_PeripheralReceivedWriteDataAction; }
        set { m_PeripheralReceivedWriteDataAction = value; }
    }
    private Action<string> m_ConnectedPeripheralAction;

    public Action<string> ConnectedPeripheralAction
    {
        get { return m_ConnectedPeripheralAction; }
        set { m_ConnectedPeripheralAction = value; }
    }
    private Action<string> m_ConnectedDisconnectPeripheralAction;

    public Action<string> ConnectedDisconnectPeripheralAction
    {
        get { return m_ConnectedDisconnectPeripheralAction; }
        set { m_ConnectedDisconnectPeripheralAction = value; }
    }
    private Action<string> m_DisconnectedPeripheralAction;

    public Action<string> DisconnectedPeripheralAction
    {
        get { return m_DisconnectedPeripheralAction; }
        set { m_DisconnectedPeripheralAction = value; }
    }
    private Action<string, string> m_DiscoveredServiceAction;

    public Action<string, string> DiscoveredServiceAction
    {
        get { return m_DiscoveredServiceAction; }
        set { m_DiscoveredServiceAction = value; }
    }
    private Action<string, string, string> m_DiscoveredCharacteristicAction;

    public Action<string, string, string> DiscoveredCharacteristicAction
    {
        get { return m_DiscoveredCharacteristicAction; }
        set { m_DiscoveredCharacteristicAction = value; }
    }
    private Action<string> m_DidWriteCharacteristicAction;

    public Action<string> DidWriteCharacteristicAction
    {
        get { return m_DidWriteCharacteristicAction; }
        set { m_DidWriteCharacteristicAction = value; }
    }
    private Dictionary<string, Dictionary<string, Action<string>>> m_DidUpdateNotificationStateForCharacteristicAction;

    public Dictionary<string, Dictionary<string, Action<string>>> DidUpdateNotificationStateForCharacteristicAction
    {
        get { return m_DidUpdateNotificationStateForCharacteristicAction; }
        set { m_DidUpdateNotificationStateForCharacteristicAction = value; }
    }

    private Dictionary<string, Dictionary<string, Action<string, string>>> m_DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction;

    public Dictionary<string, Dictionary<string, Action<string, string>>> DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction
    {
        get { return m_DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction; }
        set { m_DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction = value; }
    }

    private Dictionary<string, Dictionary<string, Action<string, byte[]>>> m_DidUpdateCharacteristicValueAction;

    public Dictionary<string, Dictionary<string, Action<string, byte[]>>> DidUpdateCharacteristicValueAction
    {
        get { return m_DidUpdateCharacteristicValueAction; }
        set { m_DidUpdateCharacteristicValueAction = value; }
    }

    private Dictionary<string, Dictionary<string, Action<string, string, byte[]>>> m_DidUpdateCharacteristicValueWithDeviceAddressAction;

    public Dictionary<string, Dictionary<string, Action<string, string, byte[]>>> DidUpdateCharacteristicValueWithDeviceAddressAction
    {
        get { return m_DidUpdateCharacteristicValueWithDeviceAddressAction; }
        set { m_DidUpdateCharacteristicValueWithDeviceAddressAction = value; }
    }

    // Use this for initialization
    private void Start()
    {
        DiscoveredDeviceList = new List<string>();
        DidUpdateNotificationStateForCharacteristicAction = new Dictionary<string, Dictionary<string, Action<string>>>();
        DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction = new Dictionary<string, Dictionary<string, Action<string, string>>>();
        DidUpdateCharacteristicValueAction = new Dictionary<string, Dictionary<string, Action<string, byte[]>>>();
        DidUpdateCharacteristicValueWithDeviceAddressAction = new Dictionary<string, Dictionary<string, Action<string, string, byte[]>>>();
    }

    private static string deviceInitializedString
    {
        get { return "Initialized"; }
    }

    private static string deviceDeInitializedString
    {
        get { return "DeInitialized"; }
    }

    private static string deviceErrorString
    {
        get { return "Error"; }
    }

    private static string deviceServiceAdded
    {
        get { return "ServiceAdded"; }
    }

    private static string deviceStartedAdvertising
    {
        get { return "StartedAdvertising"; }
    }

    private static string deviceStoppedAdvertising
    {
        get { return "StoppedAdvertising"; }
    }

    private static string deviceDiscoveredPeripheral
    {
        get { return "DiscoveredPeripheral"; }
    }

    private static string deviceRetrievedConnectedPeripheral
    {
        get { return "RetrievedConnectedPeripheral"; }
    }

    private static string devicePeripheralReceivedWriteData
    {
        get { return "PeripheralReceivedWriteData"; }
    }

    private static string deviceConnectedPeripheral
    {
        get { return "ConnectedPeripheral"; }
    }

    private static string deviceDisconnectedPeripheral
    {
        get { return "DisconnectedPeripheral"; }
    }

    private static string deviceDiscoveredService
    {
        get { return "DiscoveredService"; }
    }

    private static string deviceDiscoveredCharacteristic
    {
        get { return "DiscoveredCharacteristic"; }
    }

    private static string deviceDidWriteCharacteristic
    {
        get { return "DidWriteCharacteristic"; }
    }

    private static string deviceDidUpdateNotificationStateForCharacteristic
    {
        get { return "DidUpdateNotificationStateForCharacteristic"; }
    }

    private static string deviceDidUpdateValueForCharacteristic
    {
        get { return "DidUpdateValueForCharacteristic"; }
    }

    public void OnBluetoothMessage(string message)
    {
        if (message != null)
        {
            char[] delim = new char[]
            {
                '~'
            };
            string[] parts = message.Split(delim);

            for (int i = 0; i < parts.Length; ++i)
            {
                BluetoothLEHardwareInterface.Log(string.Format("Part: {0} - {1}", i, parts[i]));
            }

            if (message.Length >= deviceInitializedString.Length && message.Substring(0, deviceInitializedString.Length) == deviceInitializedString)
            {
                if (InitializedAction != null)
                {
                    InitializedAction();
                }
            }
            else if (message.Length >= deviceDeInitializedString.Length && message.Substring(0, deviceDeInitializedString.Length) == deviceDeInitializedString)
            {
                BluetoothLEHardwareInterface.FinishDeInitialize();

                if (DeinitializedAction != null)
                {
                    DeinitializedAction();
                }
            }
            else if (message.Length >= deviceErrorString.Length && message.Substring(0, deviceErrorString.Length) == deviceErrorString)
            {
                string error = "";

                if (parts.Length >= 2)
                {
                    error = parts[1];
                }

                if (ErrorAction != null)
                {
                    ErrorAction(error);
                }
            }
            else if (message.Length >= deviceServiceAdded.Length && message.Substring(0, deviceServiceAdded.Length) == deviceServiceAdded)
            {
                if (parts.Length >= 2)
                {
                    if (ServiceAddedAction != null)
                    {
                        ServiceAddedAction(parts[1]);
                    }
                }
            }
            else if (message.Length >= deviceStartedAdvertising.Length && message.Substring(0, deviceStartedAdvertising.Length) == deviceStartedAdvertising)
            {
                BluetoothLEHardwareInterface.Log("Started Advertising");

                if (StartedAdvertisingAction != null)
                {
                    StartedAdvertisingAction();
                }
            }
            else if (message.Length >= deviceStoppedAdvertising.Length && message.Substring(0, deviceStoppedAdvertising.Length) == deviceStoppedAdvertising)
            {
                BluetoothLEHardwareInterface.Log("Stopped Advertising");

                if (StoppedAdvertisingAction != null)
                {
                    StoppedAdvertisingAction();
                }
            }
            else if (message.Length >= deviceDiscoveredPeripheral.Length && message.Substring(0, deviceDiscoveredPeripheral.Length) == deviceDiscoveredPeripheral)
            {
                if (parts.Length >= 3)
                {
                    // the first callback will only get called the first time this device is seen
                    // this is because it gets added to the a list in the DiscoveredDeviceList
                    // after that only the second callback will get called and only if there is
                    // advertising data available
                    if (!DiscoveredDeviceList.Contains(parts[1]))
                    {
                        DiscoveredDeviceList.Add(parts[1]);

                        if (DiscoveredPeripheralAction != null)
                        {
                            DiscoveredPeripheralAction(parts[1], parts[2]);
                        }
                    }

                    if (parts.Length >= 5 && DiscoveredPeripheralWithAdvertisingInfoAction != null)
                    {
                        // get the rssi from the 4th value
                        int rssi = 0;
                        if (!int.TryParse(parts[3], out rssi))
                        {
                            rssi = 0;
                        }

                        // parse the base 64 encoded data that is the 5th value
                        byte[] bytes = Convert.FromBase64String(parts[4]);

                        DiscoveredPeripheralWithAdvertisingInfoAction(parts[1], parts[2], rssi, bytes);
                    }
                }
            }
            else if (message.Length >= deviceRetrievedConnectedPeripheral.Length && message.Substring(0, deviceRetrievedConnectedPeripheral.Length) == deviceRetrievedConnectedPeripheral)
            {
                if (parts.Length >= 3)
                {
                    DiscoveredDeviceList.Add(parts[1]);

                    if (RetrievedConnectedPeripheralAction != null)
                    {
                        RetrievedConnectedPeripheralAction(parts[1], parts[2]);
                    }
                }
            }
            else if (message.Length >= devicePeripheralReceivedWriteData.Length && message.Substring(0, devicePeripheralReceivedWriteData.Length) == devicePeripheralReceivedWriteData)
            {
                if (parts.Length >= 3)
                {
                    OnPeripheralData(parts[1], parts[2]);
                }
            }
            else if (message.Length >= deviceConnectedPeripheral.Length && message.Substring(0, deviceConnectedPeripheral.Length) == deviceConnectedPeripheral)
            {
                if (parts.Length >= 2 && ConnectedPeripheralAction != null)
                {
                    ConnectedPeripheralAction(parts[1]);
                }
            }
            else if (message.Length >= deviceDisconnectedPeripheral.Length && message.Substring(0, deviceDisconnectedPeripheral.Length) == deviceDisconnectedPeripheral)
            {
                if (parts.Length >= 2)
                {
                    if (ConnectedDisconnectPeripheralAction != null)
                    {
                        ConnectedDisconnectPeripheralAction(parts[1]);
                    }

                    if (DisconnectedPeripheralAction != null)
                    {
                        DisconnectedPeripheralAction(parts[1]);
                    }
                }
            }
            else if (message.Length >= deviceDiscoveredService.Length && message.Substring(0, deviceDiscoveredService.Length) == deviceDiscoveredService)
            {
                if (parts.Length >= 3 && DiscoveredServiceAction != null)
                {
                    DiscoveredServiceAction(parts[1], parts[2]);
                }
            }
            else if (message.Length >= deviceDiscoveredCharacteristic.Length && message.Substring(0, deviceDiscoveredCharacteristic.Length) == deviceDiscoveredCharacteristic)
            {
                if (parts.Length >= 4 && DiscoveredCharacteristicAction != null)
                {
                    DiscoveredCharacteristicAction(parts[1], parts[2], parts[3]);
                }
            }
            else if (message.Length >= deviceDidWriteCharacteristic.Length && message.Substring(0, deviceDidWriteCharacteristic.Length) == deviceDidWriteCharacteristic)
            {
                if (parts.Length >= 2 && DidWriteCharacteristicAction != null)
                {
                    DidWriteCharacteristicAction(parts[1]);
                }
            }
            else if (message.Length >= deviceDidUpdateNotificationStateForCharacteristic.Length && message.Substring(0, deviceDidUpdateNotificationStateForCharacteristic.Length) == deviceDidUpdateNotificationStateForCharacteristic)
            {
                if (parts.Length >= 3)
                {
                    if (DidUpdateNotificationStateForCharacteristicAction != null && DidUpdateNotificationStateForCharacteristicAction.ContainsKey(parts[1]))
                    {
                        Dictionary<string, Action<string>> characteristicAction = DidUpdateNotificationStateForCharacteristicAction[parts[1]];
                        if (characteristicAction != null && characteristicAction.ContainsKey(parts[2]))
                        {
                            Action<string> action = characteristicAction[parts[2]];
                            if (action != null)
                            {
                                action(parts[2]);
                            }
                        }
                    }

                    if (DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction != null && DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction.ContainsKey(parts[1]))
                    {
                        Dictionary<string, Action<string, string>> characteristicAction = DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[parts[1]];
                        if (characteristicAction != null && characteristicAction.ContainsKey(parts[2]))
                        {
                            Action<string, string> action = characteristicAction[parts[2]];
                            if (action != null)
                            {
                                action(parts[1], parts[2]);
                            }
                        }
                    }
                }
            }
            else if (message.Length >= deviceDidUpdateValueForCharacteristic.Length && message.Substring(0, deviceDidUpdateValueForCharacteristic.Length) == deviceDidUpdateValueForCharacteristic)
            {
                if (parts.Length >= 4)
                {
                    OnBluetoothData(parts[1], parts[2], parts[3]);
                }
            }
        }
    }

    public void OnBluetoothData(string base64Data)
    {
        OnBluetoothData("", "", base64Data);
    }

    public void OnBluetoothData(string deviceAddress, string characteristic, string base64Data)
    {
        if (base64Data != null)
        {
            byte[] bytes = Convert.FromBase64String(base64Data);
            if (bytes.Length > 0)
            {
                deviceAddress = deviceAddress.ToUpper();
                characteristic = characteristic.ToUpper();

                BluetoothLEHardwareInterface.Log("Device: " + deviceAddress + " Characteristic Received: " + characteristic);

                string byteString = "";
                foreach (byte b in bytes)
                {
                    byteString += string.Format("{0:X2}", b);
                }

                BluetoothLEHardwareInterface.Log(byteString);

                if (DidUpdateCharacteristicValueAction != null && DidUpdateCharacteristicValueAction.ContainsKey(deviceAddress))
                {
                    Dictionary<string, Action<string, byte[]>> characteristicAction = DidUpdateCharacteristicValueAction[deviceAddress];
#if UNITY_ANDROID
					characteristic = characteristic.ToLower ();
#endif
                    if (characteristicAction != null && characteristicAction.ContainsKey(characteristic))
                    {
                        Action<string, byte[]> action = characteristicAction[characteristic];
                        if (action != null)
                        {
                            action(characteristic, bytes);
                        }
                    }
                }

                if (DidUpdateCharacteristicValueWithDeviceAddressAction != null && DidUpdateCharacteristicValueWithDeviceAddressAction.ContainsKey(deviceAddress))
                {
                    Dictionary<string, Action<string, string, byte[]>> characteristicAction = DidUpdateCharacteristicValueWithDeviceAddressAction[deviceAddress];
#if UNITY_ANDROID
					characteristic = characteristic.ToLower ();
#endif
                    if (characteristicAction != null && characteristicAction.ContainsKey(characteristic))
                    {
                        Action<string, string, byte[]> action = characteristicAction[characteristic];
                        if (action != null)
                        {
                            action(deviceAddress, characteristic, bytes);
                        }
                    }
                }
            }
        }
    }

    public void OnPeripheralData(string characteristic, string base64Data)
    {
        if (base64Data != null)
        {
            byte[] bytes = Convert.FromBase64String(base64Data);
            if (bytes.Length > 0)
            {
                BluetoothLEHardwareInterface.Log("Peripheral Received: " + characteristic);

                string byteString = "";
                foreach (byte b in bytes)
                {
                    byteString += string.Format("{0:X2}", b);
                }

                BluetoothLEHardwareInterface.Log(byteString);

                if (PeripheralReceivedWriteDataAction != null)
                {
                    PeripheralReceivedWriteDataAction(characteristic, bytes);
                }
            }
        }
    }
}