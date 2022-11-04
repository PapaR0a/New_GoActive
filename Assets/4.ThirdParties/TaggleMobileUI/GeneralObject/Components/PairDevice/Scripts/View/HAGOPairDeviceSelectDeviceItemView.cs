using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGOPairDeviceSelectDeviceItemView : MonoBehaviour
{
	private Button m_btnItem;
	private Button m_btnDisconnect;
	private Transform m_tfDisconnectLoader;
	private GameObject m_iconDisconnect;
	private RawImage m_rimgIcon;
	private Text m_txtName;
	private GameObject m_objStatusConnected;
	private GameObject m_objStatusDisconnected;
	private GameObject m_objStatusPaired;

	//param
	private HAGODeviceDTO m_data;

	public void OnDestroy()
	{
		Destroy();
	}

	public void Destroy()
	{
        //unregister event
        HAGOSmartBluetoothControl.Api.ConnectedEvent -= OnDeviceStatusChangeHandler;
        HAGOSmartBluetoothControl.Api.DisconnectEvent -= OnDeviceStatusChangeHandler;
    }

	public void Init(HAGODeviceDTO data)
	{
		//register event
		HAGOSmartBluetoothControl.Api.ConnectedEvent += OnDeviceStatusChangeHandler;
		HAGOSmartBluetoothControl.Api.DisconnectEvent += OnDeviceStatusChangeHandler;

		Debug.Log("Device item init");
		m_data = data;

		//find reference
		m_btnItem = GetComponent<Button>();
		m_btnDisconnect = transform.Find("BtnDisconnect").GetComponent<Button>();
		m_iconDisconnect = transform.Find("BtnDisconnect/Icon").gameObject;
		m_tfDisconnectLoader = transform.Find("BtnDisconnect/Loader");
		m_rimgIcon = transform.Find("IconDevice/RimgIcon").GetComponent<RawImage>();
		m_txtName = transform.Find("Info/TxtName").GetComponent<Text>();
		m_objStatusConnected = transform.Find("Info/TxtConnected").gameObject;
		m_objStatusDisconnected = transform.Find("Info/TxtDisconnected").gameObject;
		m_objStatusPaired = transform.Find("Info/TxtPaired").gameObject;

		m_txtName.text = m_data.Name;
		ChangeStatus();
		//handle view
		if (m_data.Type != null && !string.IsNullOrEmpty(m_data.Type.Image))
        {
			m_rimgIcon.LoadTexture(m_data.Type.Image);
			m_rimgIcon.color = Color.white;
		}			
		
		//add listener
		m_btnItem.onClick.AddListener(ItemOnClick);
		m_btnDisconnect.onClick.AddListener(DisconnectOnClick);

		
    }

	public bool IsConnected()
	{
		return m_data.Status == HAGODeviceStatusType.Connected;
	}

	public string GetId()
	{
		return m_data.Id;
	}

	public HAGODeviceDTO GetDeviceInfo()
    {
		return m_data;
    }		
	private void DisconnectOnClick()
    {
		Debug.Log("DisconnectOnClick");
		ShowDisconnectLoader(true);
        HAGOPairDeviceControl.Api.DisconnectDevice(m_data);
    }

    private void ItemOnClick()
    {
		Debug.Log("Device Item click");
		ShowDisconnectLoader(true);
#if !UNITY_EDITOR
		HAGOPairDeviceControl.Api.ConnectDevice(m_data);
#else
		EditorConnectDevice();
#endif
	}

	private void EditorConnectDevice()
    {
		StartCoroutine(IEDelayConnect());
    }		

	IEnumerator IEDelayConnect()
    {
		yield return new WaitForSeconds(5f);
		string result = "{\"result_code\": 1,\"value\": {\"deviceMac\": \"DEVICE_STATE_CONNECTED\"}}";
        HAGOSmartBluetoothControl.Api.CallbackConnected(result);
	}

	private void OnDeviceStatusChangeHandler(HAGODeviceDTO result)
	{
		Debug.Log("[HAGOPairDeviceSelectDeviceItemView] OnDeviceStatusChangeHandler - deviceID: " + result.Id + " + result.Status = " + result.Status);
        if (result.Id.Equals(m_data.Id))
        {
            m_data.Status = result.Status;
            ChangeStatus();
            ShowDisconnectLoader(false);
        }
	}

    private void ShowDisconnectLoader(bool isShow)
	{
		m_tfDisconnectLoader.gameObject.SetActive(isShow);
	}

	private void ChangeStatus()
	{
		m_btnItem.interactable = m_data.Status != HAGODeviceStatusType.Connected;
		m_iconDisconnect.SetActive(m_data.Status == HAGODeviceStatusType.Connected);
		//
		m_objStatusConnected.SetActive(m_data.Status == HAGODeviceStatusType.Connected);
		m_objStatusDisconnected.SetActive(m_data.Status == HAGODeviceStatusType.Disconnected);
		m_objStatusPaired.SetActive(m_data.Status == HAGODeviceStatusType.Paired);
	}
}