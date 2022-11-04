using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAGOPairDeviceSelectTypeView : MonoBehaviour
{
	private CanvasGroup m_canvas;
	private Transform m_content;
	//
	private GameObject m_prefItem;

	//param
	private List<HAGODeviceTypeDTO> m_data;

	public void Init(List<HAGODeviceTypeDTO> data)
	{
		m_data = data;

		//find reference
		m_canvas = GetComponent<CanvasGroup>();
		m_content = transform.Find("Devices/Viewport/Content");
		//
		m_prefItem = transform.Find("Devices/Viewport/Content/Item").gameObject;
		m_prefItem.SetActive(false);

		//handle view
		CreateItem();
	}

	private void OnDestroy()
	{
		Destroy();
	}

	public void Destroy()
	{
		//unregister event
		foreach(HAGOPairDeviceSelectTypeItemView itemView in m_content.GetComponentsInChildren<HAGOPairDeviceSelectTypeItemView>())
		{
			itemView.Destroy();
		}
	}

	private void CreateItem()
	{
		foreach(HAGODeviceTypeDTO type in m_data)
		{
			GameObject go = Instantiate(m_prefItem, m_content);
			go.SetActive(true);
			go.GetComponent<HAGOPairDeviceSelectTypeItemView>().Init(type);
		}
	}

	public void ShowView()
	{
		HAGOTweenUtils.ShowPopup(m_canvas, transform);
	}

	public void HideView()
	{
		HAGOTweenUtils.HidePopup(m_canvas, transform, null, false);
	}
}