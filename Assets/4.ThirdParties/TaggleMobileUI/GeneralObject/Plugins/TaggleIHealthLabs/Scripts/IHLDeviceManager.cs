using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class IHLDeviceManager : MonoBehaviour
{
    public Transform contentTransform;
    public GameObject deviceItemPref;
    public List<IHLDeviceItem> deviceItems;
    void Start()
    {
        ClearDevice();
        deviceItemPref.SetActive(false);
        deviceItems = new List<IHLDeviceItem>();
        TaggleIHealthLabsManager.ScanDeviceEvent += OnScanDeviceEvent;
        TaggleIHealthLabsManager.AuthenticationEvent += OnAuthenticationEvent;
    }

    void ClearDevice()
    {
        foreach (Transform child in contentTransform)
        {
            if (child.name != deviceItemPref.name)
            {
                Debug.Log("Delete device: name:" + child.name);
                Destroy(child.gameObject);
            }
        }
    }

    public void ScanDevices()
    {
        ClearDevice();
        deviceItems = new List<IHLDeviceItem>();
        IHLIndicator.Instance.ShowIndicator();
#if !UNITY_EDITOR
        TaggleIHealthLabs.Instance.AuthenticationIHealthLabs();
#else
        StartCoroutine(EditorAddDevice());
#endif
    }

    IEnumerator EditorAddDevice()
    {
        yield return new WaitForSeconds(2f);
        GameObject deviceItemObj = Instantiate(deviceItemPref, contentTransform);
        deviceItemObj.SetActive(true);
        IHLDeviceItem deviceItem = deviceItemObj.GetComponent<IHLDeviceItem>();
        int random = UnityEngine.Random.Range(1, 1000);
        deviceItem.Init("5CF821DECFCA", "PO3 " + random);
        deviceItems.Add(deviceItem);
        IHLIndicator.Instance.HideIndicator();
        IHLPopupMessage.Instance.ShowMessage("Scan finished");
    }

    private void OnDestroy()
    {
        TaggleIHealthLabsManager.ScanDeviceEvent -= OnScanDeviceEvent;
        TaggleIHealthLabsManager.AuthenticationEvent -= OnAuthenticationEvent;
    }

    private void OnAuthenticationEvent(string result)
    {
        Debug.Log("OnAuthenticationEvent" + result);
        JObject itemData = JObject.Parse(result);
        if (itemData != null)
        {
            string resultCode = itemData.Value<string>("result_code");
            if (resultCode == "1")
            {
                Debug.Log("Authentication Successful - LaunchIHealthLabs");
            }
            else
            {
                IHLIndicator.Instance.HideIndicator();
                IHLPopupMessage.Instance.ShowErrorMessage("Authentication Error");
                Debug.Log("Authentication Error");
            }
        }
    }

    void OnScanDeviceEvent(string result)
    {
        Debug.Log("OnScanDeviceEvent: " + result);

        bool isNull = false;
#if UNITY_ANDROID
        JObject itemData = JObject.Parse(result);
        isNull = itemData == null;
        Debug.Log("OnScanDeviceEvent token: " + itemData);
#elif UNITY_IOS
        JToken itemData = JToken.Parse(result);
        isNull = itemData == null;
        Debug.Log("OnScanDeviceEvent token: " + itemData);
#endif


        if (isNull == false)
        {
            string resultCode = string.Empty;

#if UNITY_ANDROID || UNITY_IOS
                resultCode = itemData.Value<string>("result_code");
#endif


            Debug.Log("OnScanDeviceEvent resultCode: " + resultCode);
            if (resultCode == "1")
            {
                JObject value = null;
#if UNITY_ANDROID
                value = JObject.Parse(itemData.Value<string>("value"));
#elif UNITY_IOS
                value = itemData.Value<JObject>("value");
#endif
                if (value.Value<string>("deviceMac") != IHLSTATE.DEVICE_STATE_SCAN_FINISHED.ToString())
                {
                    string mac = value.Value<string>("deviceMac");
                    string deviceName = value.Value<string>("deviceName");
                    if (deviceItems.Count > 0)
                    {
                        foreach (var device in deviceItems)
                        {
                            string newMacDevice = value.Value<string>("deviceMac");
                            if (device.Mac != newMacDevice)
                            {
                                AddDeviceItem(mac, deviceName);
                            }
                        }
                    }
                    else
                    {
                        AddDeviceItem(mac, deviceName);
                    }
                    IHLIndicator.Instance.HideIndicator();
                    IHLPopupMessage.Instance.ShowMessage("Scan finished");
                }
                else
                {
                    Debug.Log("Scan finished.....");
                    IHLIndicator.Instance.HideIndicator();
                    IHLPopupMessage.Instance.ShowMessage("Scan finished");
                }
            }
            else
            {
                Debug.Log("Scan Error");
                IHLIndicator.Instance.HideIndicator();
                IHLPopupMessage.Instance.ShowErrorMessage("Scan Error");
            }
        }
        else
        {
            Debug.Log("Can't parse device data");
            IHLIndicator.Instance.HideIndicator();
            IHLPopupMessage.Instance.ShowErrorMessage("Can't parse device data");
        }

    }

    private void AddDeviceItem(string mac, string deviceName)
    {
        GameObject deviceItemObj = Instantiate(deviceItemPref, contentTransform);
        deviceItemObj.SetActive(true);
        IHLDeviceItem deviceItem = deviceItemObj.GetComponent<IHLDeviceItem>();
        deviceItem.Init(mac, deviceName);
        deviceItems.Add(deviceItem);
        deviceItems = deviceItems.Distinct().ToList();
    }
    public void UnloadScanScene()
    {
        Debug.Log("UnLoad ScanScene scene");
        SceneManager.UnloadSceneAsync("2_ScanScene");
    }
}
