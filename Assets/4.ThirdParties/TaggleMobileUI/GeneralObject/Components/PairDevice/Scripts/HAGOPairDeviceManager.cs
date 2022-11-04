using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeviceType = TaggleTemplate.Comm.DeviceType;
public class HAGOPairDeviceManager : MonoBehaviour
{
    private static HAGOPairDeviceManager m_api;
    public static HAGOPairDeviceManager Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = Instantiate(Resources.Load<GameObject>(HAGOConstant.PREFAB_PAIR_DEVICE)).GetComponent<HAGOPairDeviceManager>();
            }
            return m_api;
        }
    }
    public void Init(List<DeviceType> deviceTypes, List<string> lstDeviceTypeAutoFill, Action<List<HAGODeviceDTO>> callback)
    {
        HAGOPairDeviceModel.Api.ParseDeviceTypeConfig(deviceTypes);
        Init(lstDeviceTypeAutoFill, callback);
    }
    public void Init(List<string> lstDeviceTypeAutoFill, Action<List<HAGODeviceDTO>> callback)
    {
        if (!HAGOPairDeviceModel.Api.IsDeviceTypeLoaded())
        {
            Debug.Log("<color=red>Need to call api to get device types first</color>");
            return;
        }
        HAGOPairDeviceControl.Api.ResultCallbackEvent = callback;
        //init view
        HAGOPairDeviceView view = transform.Find("Canvas").GetComponent<HAGOPairDeviceView>();
        view.Init(HAGOPairDeviceControl.Api.GetAllDeviceType(), lstDeviceTypeAutoFill);
        //Init smart bluetooth
        HAGOSmartBluetoothManager.Api.Init();
    }
    public void Destroy()
    {
        Debug.Log("HAGOPairDeviceManager call destroy");
        // HAGOSmartBluetoothManager.Api.Exit();
        GameObject.Destroy(this.gameObject);
    }
}