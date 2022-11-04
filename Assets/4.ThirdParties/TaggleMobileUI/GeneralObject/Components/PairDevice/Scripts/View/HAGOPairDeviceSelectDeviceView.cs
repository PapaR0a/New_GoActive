using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGOPairDeviceSelectDeviceView : MonoBehaviour
{
	private CanvasGroup m_canvas;
	private Transform m_content;
	private Button m_btnConfirm;
	private Button m_btnBack;
	//
	private GameObject m_prefItem;


	//param
	private List<HAGODeviceDTO> m_data;

	public void Init()
	{
		//find reference
		m_canvas = GetComponent<CanvasGroup>();
		m_content = transform.Find("Devices/Viewport/Content");
		m_btnConfirm = transform.Find("Buttons/BtnConfirm").GetComponent<Button>();
		m_btnBack = transform.Find("Buttons/BtnBack").GetComponent<Button>();
		//
		m_prefItem = transform.Find("Devices/Viewport/Content/Item").gameObject;
		m_prefItem.SetActive(false);

		//add listener
		m_btnConfirm.onClick.AddListener(ConfirmOnClick);
		m_btnBack.onClick.AddListener(BackOnClick);
	}

	private void OnDestroy()
	{
		Destroy();
	}

	public void Destroy()
	{
		//unregister event
		foreach(HAGOPairDeviceSelectDeviceItemView itemView in m_content.GetComponentsInChildren<HAGOPairDeviceSelectDeviceItemView>())
		{
			itemView.Destroy();
		}
	}

    private void ConfirmOnClick()
    {
		List<HAGODeviceDTO> lstDeviceConnected = new List<HAGODeviceDTO>();
		
        foreach(Transform tf in m_content)
		{
			if(tf.gameObject != m_prefItem)
			{
				HAGOPairDeviceSelectDeviceItemView itemView = tf.gameObject.GetComponent<HAGOPairDeviceSelectDeviceItemView>();
				if(itemView.IsConnected())
				{
					lstDeviceConnected.Add(itemView.GetDeviceInfo());
				}
			}
		}
		Debug.Log("Click confirm btn");
		HAGOPairDeviceControl.Api.CompletePairDevice(lstDeviceConnected);
    }

    private void BackOnClick()
    {
        HAGOPairDeviceControl.Api.BackToSelectType();
    }

    public void CreateItem(List<HAGODeviceDTO> data)
	{
		Debug.Log("Create item selected device ");
		m_data = data;

		//clear old item
		foreach(Transform tf in m_content)
		{
			if(tf.gameObject != m_prefItem)
			{
				Destroy(tf.gameObject);
			}
		}

		foreach(HAGODeviceDTO device in m_data)
		{
			GameObject go = Instantiate(m_prefItem, m_content);
			go.SetActive(true);
			go.GetComponent<HAGOPairDeviceSelectDeviceItemView>().Init(device);
		}
	}

	public void ShowView(List<HAGODeviceDTO> data)
	{
		HAGOTweenUtils.ShowPopup(m_canvas, transform, () => CreateItem(data));
	}

	public void HideView()
	{
		HAGOTweenUtils.HidePopup(m_canvas, transform, null, false);
	}
}