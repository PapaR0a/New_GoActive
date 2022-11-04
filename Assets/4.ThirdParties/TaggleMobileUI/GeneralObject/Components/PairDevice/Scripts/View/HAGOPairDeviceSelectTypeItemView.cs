using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGOPairDeviceSelectTypeItemView : MonoBehaviour
{
	private Button m_btnItem;
	private RawImage m_rimgIcon;
	private Text m_txtName;
	private Text m_txtDesc;

	//param
	private HAGODeviceTypeDTO m_data;

    private void OnDestroy()
    {
        Destroy();
    }

    public void Destroy()
    {
        //unregister event
        HAGOSmartBluetoothControl.Api.AuthenticationEvent -= OnAuthenticationEvent;
    }
    
    public void Init(HAGODeviceTypeDTO data)
	{
		m_data = data;

		//find reference
		m_btnItem = GetComponent<Button>();
		m_rimgIcon = transform.Find("Icon/RimgIcon").GetComponent<RawImage>();
		m_txtName = transform.Find("Info/TxtName").GetComponent<Text>();
		m_txtDesc = transform.Find("Info/TxtDesc").GetComponent<Text>();

		//handle view
        if(!string.IsNullOrEmpty(m_data.Image))
        {
		    m_rimgIcon.LoadTexture(m_data.Image);
            m_rimgIcon.color = Color.white;
        }
		m_txtName.text = m_data.DisplayName;
		m_txtDesc.text = string.Empty;  //TODO: handle paired status later

		//add listener
		m_btnItem.onClick.AddListener(OnClickSelectType);

        //register event
        HAGOSmartBluetoothControl.Api.AuthenticationEvent += OnAuthenticationEvent;
	}


    private void OnClickSelectType()
    {
        HAGOSmartBluetoothControl.Api.SelectTypeDevice(m_data.ConnectType, m_data.Name);
    }

    private void OnAuthenticationEvent(string result)
    {
        //TODO: cheat to get FocusType to prevent calling with unmatch type
        if(!m_data.ConnectType.Equals(HAGOSmartBluetoothControl.Api.FocusType.ToString().ToLower()) || !m_data.Name.Equals(HAGOSmartBluetoothControl.Api.DeviceType.ToString()))
        {
            return;
        }

        Debug.Log(string.Format("[OnAuthenticationEvent] m_data.Id {0} - FocusType {1}", m_data.Name, HAGOSmartBluetoothControl.Api.FocusType.ToString()));
        JObject itemData = JObject.Parse(result);
        if (itemData != null)
        {
            Debug.Log("OnAuthenticationEvent result: " + result);
            string resultCode = itemData.Value<string>("result_code");
            if (resultCode == "1")
            {
                Debug.Log("Authentication Successful");
                if (m_data != null)
                    HAGOPairDeviceControl.Api.SelectDeviceType(m_data.Name);
            }
            else
            {
                Debug.Log("Authentication Error");
            }
        }
    }

}