using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TaggleTemplate.Comm;
using UnityEngine;
using UnityEngine.UI;

public class MVVIconBluetoothStatusView : MonoBehaviour
{
    private Image m_imgIconBluetooth;
    private Image m_imgIconConnectedBluetooth;
    private GameObject m_effectIconBluetoothObj;
    private Transform m_pnlResult;
    private GameObject m_panelStatusDisconnectObj;
    private Text m_txtStatus;
    private Button m_btnIconBluetooth;
    private Text m_txtCurrentHR;
    private Text m_txtCurrentSpo2;
    private Button m_btnReconnectBluetooth;

    [HideInInspector]
    public bool isConnected;
    private int m_intCurrentSpo2;
    private int m_intCurrentHR;

    void OnDestroy()    
    {
        // unregistry event 
        Destroy();
        //
    }

    public void Destroy()
    {
        HAGOSmartBluetoothControl.Api.ConnectedEvent -= OnDeviceStatusChangeHandler;
        HAGOSmartBluetoothControl.Api.DisconnectEvent -= OnDeviceStatusChangeHandler;
        HAGOSmartBluetoothControl.Api.DataEvent -= OnReceiveDataHandler;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // Find reference
        m_imgIconConnectedBluetooth = transform.Find("BTStatus_Panel/IconBT/Btn/IconEffect").GetComponent<Image>();
        m_imgIconBluetooth = transform.Find("BTStatus_Panel/IconBT/Btn/Icon").GetComponent<Image>();
        m_effectIconBluetoothObj = transform.Find("BTStatus_Panel/IconBT/ConnectedEffect").gameObject;
        m_pnlResult = transform.Find("Image");
        m_panelStatusDisconnectObj = transform.Find("BTStatus_Panel/Panel Status").gameObject;
        m_txtStatus = transform.Find("BTStatus_Panel/Panel Status/Panel/StatusTxt").GetComponent<Text>();
        m_btnIconBluetooth = transform.Find("BTStatus_Panel/IconBT/Btn").GetComponent<Button>();
        m_txtCurrentHR = transform.Find("Image/HrPanel/Txt").GetComponent<Text>();
        m_txtCurrentSpo2 = transform.Find("Image/Spo2Panel/Txt").GetComponent<Text>();
        m_btnReconnectBluetooth = transform.Find("BTStatus_Panel/Panel Status/Panel/Cancel_Bt").GetComponent<Button>();

        //add listener
        m_btnIconBluetooth.onClick.AddListener(ShowConnectPairDevicePopup);
        m_btnReconnectBluetooth.onClick.AddListener(OnReconnectBluetoothBtn);

        // registry event
        HAGOSmartBluetoothControl.Api.ConnectedEvent += OnDeviceStatusChangeHandler;
        HAGOSmartBluetoothControl.Api.DisconnectEvent += OnDeviceStatusChangeHandler;
        HAGOSmartBluetoothControl.Api.DataEvent += OnReceiveDataHandler;

        //default value
        DefaultValue();
        
    }

    private void OnReconnectBluetoothBtn()
    {
        DisablePnlDisconnect();
        ShowConnectPairDevicePopup();
    }

    private void DefaultValue()
    {
        //TODO: get cache ConnectedHR from model if it is needed
        //isConnected = ?
        //

        if (isConnected)
        {
            m_imgIconConnectedBluetooth.gameObject.SetActive(true);
            m_imgIconBluetooth.gameObject.SetActive(false);

            if (m_effectIconBluetoothObj.activeSelf)
                m_effectIconBluetoothObj.SetActive(false);
        }
        else
        {
            m_imgIconConnectedBluetooth.gameObject.SetActive(false);
            m_imgIconBluetooth.gameObject.SetActive(true);
            if (!m_effectIconBluetoothObj.activeSelf)
                m_effectIconBluetoothObj.SetActive(true);
        }
        m_pnlResult.gameObject.SetActive(isConnected);
        m_panelStatusDisconnectObj.SetActive(false);

        m_txtCurrentHR.text = "bpm";
        m_txtCurrentSpo2.text = "%";
    }

    public void ShowConnectPairDevicePopup()
    {
        if (!isConnected)
        {
            CPEControl.Api.ConnectHAGOBluetooth();
        }
    }

    private void OnDeviceStatusChangeHandler(HAGODeviceDTO obj)
    {
        Debug.Log("OnDeviceStatusChangeHandler:"+ obj.Status.ToString());
        isConnected = obj.Status == HAGODeviceStatusType.Connected ? true : false;
        if (isConnected)
        {
            m_imgIconConnectedBluetooth.gameObject.SetActive(true);
            m_imgIconBluetooth.gameObject.SetActive(false);

            if (m_effectIconBluetoothObj.activeSelf)
                m_effectIconBluetoothObj.SetActive(false);
            DisablePnlDisconnect();
        }
        else
        {
            m_imgIconConnectedBluetooth.gameObject.SetActive(false);
            m_imgIconBluetooth.gameObject.SetActive(true);
            if (!m_effectIconBluetoothObj.activeSelf)
                m_effectIconBluetoothObj.SetActive(true);
            ShowPnlStatusConnecting();
        }
        m_pnlResult.gameObject.SetActive(isConnected);

    }

    public void ShowPnlStatusConnecting()
    {
        if (isConnected)
            return;
        //TODO: get language text
        m_txtStatus.text = "No device connected";
        //

        m_panelStatusDisconnectObj.GetComponent<CanvasGroup>().alpha = 0f;
        m_panelStatusDisconnectObj.SetActive(true);
        m_panelStatusDisconnectObj.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }

    public void DisablePnlDisconnect()
    {
        m_panelStatusDisconnectObj.GetComponent<CanvasGroup>().alpha = 1f;
        m_panelStatusDisconnectObj.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => {
            m_panelStatusDisconnectObj.SetActive(false);
        });
    }


    private void OnReceiveDataHandler(Dictionary<string, string> obj)
    {
        if (obj.ContainsKey(VSMTypes.HEART_RATE))
        {
            m_txtCurrentHR.text = obj[VSMTypes.HEART_RATE] == "0"?"--": obj[VSMTypes.HEART_RATE] + "bpm";
        }

        if (obj.ContainsKey(VSMTypes.OXYGEN_SATURATION))
        {
            m_txtCurrentSpo2.text = obj[VSMTypes.OXYGEN_SATURATION] == "0" ? "--" : obj[VSMTypes.OXYGEN_SATURATION] + "%";
        }

        int.TryParse(obj[VSMTypes.HEART_RATE], out m_intCurrentHR);
        int.TryParse(obj[VSMTypes.OXYGEN_SATURATION], out m_intCurrentSpo2);
        Debug.Log("m_txtCurrentHR :" + m_txtCurrentHR.text);
        Debug.Log("m_txtCurrentSpo2 :" + m_txtCurrentSpo2.text);

    }

}
