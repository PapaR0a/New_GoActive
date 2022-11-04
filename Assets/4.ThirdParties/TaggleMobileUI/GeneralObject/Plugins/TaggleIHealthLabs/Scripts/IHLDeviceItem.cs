using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class IHLDeviceItem : MonoBehaviour
{
    //public TaggleIHealthLabsDeviceData data;
    public string DeviceType;
    public string Mac;
    public TMP_Text text;

    Button btnConnect;

    void Start()
    {
        TaggleIHealthLabsManager.DeviceConnectedEvent += OnDeviceConnectedEvent;
    }
    private void OnDestroy()
    {
        TaggleIHealthLabsManager.DeviceConnectedEvent -= OnDeviceConnectedEvent;
    }
    private void OnDeviceConnectedEvent(string result)
    {
        JObject itemData = JObject.Parse(result);
        if (itemData != null)
        {
            string resultCode = itemData.Value<string>("result_code");
            if (resultCode == "1")
            {
                IHLIndicator.Instance.HideIndicator();
                Debug.Log("UnLoad ScanScene scene");
                IHLManagerValues.Instance.Connected = 1;
                IHLPopupMessage.Instance.ShowMessage("Connected");
                SceneManager.UnloadSceneAsync("2_ScanScene");
            }
            else
            {
                IHLIndicator.Instance.HideIndicator();
                Debug.Log("Connect failed.....");
                IHLManagerValues.Instance.Connected = 0;
                IHLPopupMessage.Instance.ShowErrorMessage("Connect error");
            }
        }
        else
        {
            IHLIndicator.Instance.HideIndicator();
            IHLManagerValues.Instance.Connected = 0;
            IHLPopupMessage.Instance.ShowErrorMessage("Can't parse device data");
            Debug.Log("Can't parse device data");
        }
    }

    public void Init( string mac, string deviceType)
    {
        DeviceType = deviceType;
        Mac = mac;
        text.text = DeviceType + " - " + Mac;

        gameObject.name = text.text;

        IHLManagerValues.Instance.Mac = mac;
        IHLManagerValues.Instance.DeviceType = deviceType;
        
        Debug.Log("Mac: " + mac);
        Debug.Log("Device Type: " + DeviceType);
        
        btnConnect = gameObject.GetComponent<Button>();
        btnConnect.onClick.AddListener(Connect);
    }

    public void Connect()
    {
        StartCoroutine(StartConnectDevice());
    }

    IEnumerator StartConnectDevice()
    {
        IHLIndicator.Instance.ShowIndicator();
        yield return new WaitForSeconds(0.15f);
        IHLPopupMessage.Instance.ShowMessage("Connecting...");
#if !UNITY_EDITOR
        TaggleIHealthLabs.Instance.ConnectDeviceIHealthLabs(Mac, DeviceType);
#else
        yield return new WaitForSeconds(1);
        int random = UnityEngine.Random.Range(0, 2);
        Debug.Log("random connected: " + random);
        bool isConnected = random == 1 ? true : false;
        EditorConnectDeviceIHealthLabs(isConnected);
        IHLPopupMessage.Instance.ShowErrorDelay();
        IHLIndicator.Instance.HideIndicator();
        IHLManagerValues.Instance.Connected = 0;
#endif

    }

    private IEnumerator EditorConnectDeviceIHealthLabs(bool isConnected)
    {
        if (isConnected)
        {
            IHLIndicator.Instance.HideIndicator();
            IHLManagerValues.Instance.Connected = 1;
            IHLPopupMessage.Instance.ShowMessage("Connected");
            SceneManager.UnloadSceneAsync("2_ScanScene");
        }
        else
        {
            IHLIndicator.Instance.HideIndicator();
            IHLManagerValues.Instance.Connected = 0;
            IHLPopupMessage.Instance.ShowErrorMessage("Connect error");
        }
        yield return new WaitForSeconds(1);
    }
}
