using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HAGOPairDeviceView : MonoBehaviour
{
	private CanvasGroup m_canvas;
	private Transform m_content;
	private Button m_btnClose;
	//
	private HAGOPairDeviceSelectTypeView m_selectTypeView;
	private HAGOPairDeviceScanView m_scanView;
	private HAGOPairDeviceSelectDeviceView m_selectDeviceView;

	//param
	private bool m_isInitComplete = false;
	private bool m_isSkipSelectType = false;

	private void OnDestroy()
	{
		Destroy();
	}

	private void Destroy()
	{
		//unregister event
		m_selectTypeView.Destroy();
		m_scanView.Destroy();
		m_selectDeviceView.Destroy();
		//
		HAGOPairDeviceControl.Api.ShowSelectTypeEvent -= ShowSelectTypeHandler;
		HAGOPairDeviceControl.Api.ShowScanEvent -= ShowScanHandler;
		HAGOPairDeviceControl.Api.BackToSelectTypeEvent -= BackToSelectTypeHandler;
		HAGOPairDeviceControl.Api.ShowSelectDeviceEvent -= ShowSelectDeviceHandler;
		HAGOPairDeviceControl.Api.ExitEvent -= ClosePopup;
	}

	public void Init(List<HAGODeviceTypeDTO> lstAllDeviceType, List<string> lstDeviceTypeIdAutoFill)
	{
		//find reference
		m_canvas = GetComponent<CanvasGroup>();
		m_content = transform.Find("Content");
		m_btnClose = transform.Find("Content/TopBar/BtnClose").GetComponent<Button>();
		//
		m_selectTypeView = transform.Find("Content/Body/SelectType").GetComponent<HAGOPairDeviceSelectTypeView>();
		m_scanView = transform.Find("Content/Body/Scan").GetComponent<HAGOPairDeviceScanView>();
		m_selectDeviceView = transform.Find("Content/Body/SelectDevice").GetComponent<HAGOPairDeviceSelectDeviceView>();

		//handle value
		m_isSkipSelectType = lstDeviceTypeIdAutoFill != null && lstDeviceTypeIdAutoFill.Count == 1;

		//filter list deviceType
		if(lstDeviceTypeIdAutoFill != null && lstDeviceTypeIdAutoFill.Count > 0)
		{
			lstAllDeviceType = lstAllDeviceType.Where(x => lstDeviceTypeIdAutoFill.Contains(x.Name)).ToList();
		}

		//init child view
		m_selectTypeView.Init(lstAllDeviceType);
		m_scanView.Init();
		m_selectDeviceView.Init();

		//add listener
		m_btnClose.onClick.AddListener(CloseOnClick);

		//register event
		HAGOPairDeviceControl.Api.ShowSelectTypeEvent += ShowSelectTypeHandler;
		HAGOPairDeviceControl.Api.ShowScanEvent += ShowScanHandler;
		HAGOPairDeviceControl.Api.BackToSelectTypeEvent += BackToSelectTypeHandler;
		HAGOPairDeviceControl.Api.ShowSelectDeviceEvent += ShowSelectDeviceHandler;
		HAGOPairDeviceControl.Api.ExitEvent += ClosePopup;

		//handle view
		if(m_isSkipSelectType)
		{
			//skip select device type
			HAGOSmartBluetoothControl.Api.SelectTypeDevice(lstAllDeviceType.FirstOrDefault().ConnectType, lstAllDeviceType.FirstOrDefault().Name);
		}
		else
		{
			//show select device type
			ShowSelectTypeHandler();
		}

		ShowPopup();
	}

	private void CloseOnClick()
	{
		HAGOPairDeviceControl.Api.Exit();
	}

	public void ShowPopup()
	{
		HAGOTweenUtils.ShowPopup(m_canvas, m_content);
	}

	public void ClosePopup(Action callback)
	{
		HAGOTweenUtils.HidePopup(m_canvas, m_content, callback);
	}

	public void ShowScanHandler()
	{
		HideAllViewHandler(() => m_scanView.ShowView());
	}

	public void BackToSelectTypeHandler()
	{
		if(m_isSkipSelectType)
		{
			HAGOPairDeviceControl.Api.Exit();
		}
		else
		{
			ShowSelectTypeHandler();
		}
	}

	public void ShowSelectDeviceHandler(List<HAGODeviceDTO> lstDevice)
	{
		HideAllViewHandler(() => m_selectDeviceView.ShowView(lstDevice));
	}

	public void ShowSelectTypeHandler()
	{
		HideAllViewHandler(() => m_selectTypeView.ShowView());
	}

	public void HideAllViewHandler(Action callback)
	{
		StartCoroutine(IEHideAllView(callback));
	}

	public IEnumerator IEHideAllView(Action callback)
	{
		if(m_selectTypeView.gameObject.activeSelf)
			m_selectTypeView.HideView();

		if(m_scanView.gameObject.activeSelf)
			m_scanView.HideView();

		if(m_selectDeviceView.gameObject.activeSelf)
			m_selectDeviceView.HideView();

		yield return new WaitForSeconds(0.3f);
		callback.Invoke();
	}
}
