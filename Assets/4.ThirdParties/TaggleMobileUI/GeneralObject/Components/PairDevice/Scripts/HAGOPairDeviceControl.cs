using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using DeviceType = TaggleTemplate.Comm.DeviceType;

public enum HAGOConnectType
{
    // IHEALTH
    IHEALTH_SDK,
  
    // BASIC
    BLUETOOTH_PROFILE,

    //VIATOM_SDK
    VIATOM_SDK,

    // OTHER
    IMU
}

public class HAGOPairDeviceControl
{
    private static HAGOPairDeviceControl m_api;
    public static HAGOPairDeviceControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOPairDeviceControl();
            }
            return m_api;
        }
    }
    
    //event
    public Action ShowSelectTypeEvent;
    public Action ShowScanEvent; //<list device type id>
    public Action BackToSelectTypeEvent;
    public Action<List<HAGODeviceDTO>> ShowSelectDeviceEvent; //<list device>
    //public Action<string, HAGODeviceStatusType> DeviceStatusChangeEvent;//<device id, status type>
    public Action<Action> ExitEvent;
    //
    public Action<List<HAGODeviceDTO>> ResultCallbackEvent;

    public void ShowSelectType()
    {
        ShowSelectTypeEvent?.Invoke();
    }

    public List<HAGODeviceTypeDTO> GetAllDeviceType()
    {
        return HAGOPairDeviceModel.Api.GetAllDeviceTypeDTO();
    }

    public void SelectDeviceType(string typeId)
    {
        ShowScan(new List<string>{ typeId });
    }

    public void SelectType(List<string> listDeviceTypeId)
    {
        ShowScan(listDeviceTypeId);
    }

    public void ShowScan(List<string> listDeviceTypeId)
    {
        ShowScanEvent?.Invoke();
        HAGOPairDeviceModel.Api.CoroutineObjScanning = CoroutineHelper.Call(IEStartScan(listDeviceTypeId))[0];
    }

    public void UpdateScanStatus(bool isScanning)
    {
        HAGOPairDeviceModel.Api.IsScanning = isScanning;
    }
    public IEnumerator IEStartScan(List<string> listDeviceTypeId)
    {
        Debug.Log("call IEStartScan");

        HAGOPairDeviceModel.Api.IsScanning = true;
        
        HAGOSmartBluetoothControl.Api.ScanBluetooth(listDeviceTypeId[0]);
        yield return new WaitUntil(() => !HAGOPairDeviceModel.Api.IsScanning);
    }

    public void StopScan()
    {
        Debug.Log("[HAGOPairDeviceControl] StopScan");
        HAGOSmartBluetoothControl.Api.StopScanBluetooth();
        HAGOPairDeviceModel.Api.IsScanning = false;
        if (HAGOPairDeviceModel.Api.CoroutineObjScanning != null)
        {
            HAGOPairDeviceModel.Api.CoroutineObjScanning.GetComponent<CoroutineHelper>().StopAllCoroutines();
        }
        UnityEngine.GameObject.Destroy(HAGOPairDeviceModel.Api.CoroutineObjScanning);
        
        BackToSelectTypeEvent();
    }

    public void BackToSelectType()
    {
        BackToSelectTypeEvent?.Invoke();
    }

    public void ShowSelectDevice(List<HAGODeviceDTO> lstDevice)
    {
        ShowSelectDeviceEvent?.Invoke(lstDevice);
    }

    public void ConnectDevice(HAGODeviceDTO deviceData)
    {
        Debug.Log(string.Format("{0} ConnectDevice: {1} @ {2}", "[HAGOPairDeviceControl]", deviceData.Id, deviceData.Name));
        HAGOSmartBluetoothControl.Api.ConnectBluetooth(new string[] { deviceData.Id, deviceData.Name });
    }

    public void DisconnectDevice(HAGODeviceDTO deviceData)
    {
        Debug.Log(string.Format("{0} DisconnectDevice", "[HAGOPairDeviceControl]"));
        // CHECK DEVICE WITH CONNECT TYPES
        if (deviceData.Type.ConnectType == HAGOConnectType.IHEALTH_SDK.ToString().ToLower())
        {
            HAGOSmartBluetoothControl.Api.Disconnect(new string[] { deviceData.Id, deviceData.Name });
        }
        else if (deviceData.Type.ConnectType == HAGOConnectType.BLUETOOTH_PROFILE.ToString().ToLower())
        {
            HAGOSmartBluetoothControl.Api.Disconnect(new string[] { deviceData.Id, deviceData.Name });
        }
    }

    public void CompletePairDevice(List<HAGODeviceDTO> lstDeviceConnected)
    {
        ResultCallbackEvent?.Invoke(lstDeviceConnected);
        Exit();
    }

    public void Exit()
    {
        ExitEvent?.Invoke(() => HAGOPairDeviceManager.Api.Destroy());
    }
}
