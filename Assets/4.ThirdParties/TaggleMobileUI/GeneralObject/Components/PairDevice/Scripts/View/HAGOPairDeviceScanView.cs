using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGOPairDeviceScanView : MonoBehaviour
{
	private CanvasGroup m_canvas;
	private Button m_btnCancel;

	public void Init()
	{
		//find reference
		m_canvas = GetComponent<CanvasGroup>();
		m_btnCancel = transform.Find("Buttons/BtnCancel").GetComponent<Button>();

		//add listener
		m_btnCancel.onClick.AddListener(CancelOnClick);

        //register event
		HAGOSmartBluetoothControl.Api.ScanEvent += OnScanDeviceEvent;
    }

    void EditorScan()
    {
        StartCoroutine(IEDelayScanDevice());
    }
    IEnumerator IEDelayScanDevice()
    {
        Debug.Log("Delay scane device");
        yield return new WaitForSeconds(5f);
        string result = "{\"result_code\": 1,\"value\": {\"deviceMac\": \"5CF821DECFCA\",\"deviceName\": \"PO3\"}}";
        OnScanDeviceEvent(HAGOSmartBluetoothControl.Api.ParseScanDevice(result, HAGOConnectType.IHEALTH_SDK));
    }        

	private void OnDestroy()
	{
        Destroy();
	}

    public void Destroy()
    {
        //unregister event
        HAGOSmartBluetoothControl.Api.ScanEvent -= OnScanDeviceEvent;
    }

    private void CancelOnClick()
    {
        HAGOPairDeviceControl.Api.StopScan();
    }

    public void ShowView()
    {
        HAGOTweenUtils.ShowPopup(m_canvas, transform, () => StartCoroutine(IECDScanTimeout()));
#if UNITY_EDITOR
        EditorScan();
#endif
    }

    public void HideView()
    {
        StopAllCoroutines();
        HAGOTweenUtils.HidePopup(m_canvas, transform, null, false);
    }

    private void OnScanDeviceEvent(HAGODataScan result)
    {
        HAGOPairDeviceControl.Api.UpdateScanStatus(false);

        Debug.Log("OnScanDeviceEvent resultCode: " + result.code);
        if (result.code == 1)
        {
            if (result.data != null && result.data.Count > 0)
            {
                Debug.Log("[OnScanDeviceEvent] device count: " + result.data.Count);
                for (int i = 0; i < result.data.Count; i++)
                {
                    if (result.data[0].Id != IHLSTATE.DEVICE_STATE_SCAN_FINISHED.ToString())
                    {
                        StopAllCoroutines();
                        HAGOPairDeviceControl.Api.ShowSelectDevice(result.data);
                    }
                    else
                    {
                        Debug.Log("[OnScanDeviceEvent] Scan finished.....");
                    }
                }
            }
        }
        else
        {
            Debug.Log("[OnScanDeviceEvent] Scan Error");
        }
    }

    private IEnumerator IECDScanTimeout()
    {
        Debug.Log("Start ScanTimeout");
        yield return new WaitForSeconds(30f);
        CancelOnClick();
        Debug.Log("End ScanTimeout");
        yield return new WaitForEndOfFrame();
    }
}