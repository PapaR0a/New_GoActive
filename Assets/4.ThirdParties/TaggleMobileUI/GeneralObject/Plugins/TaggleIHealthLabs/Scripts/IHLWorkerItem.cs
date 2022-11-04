using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorkerItem : MonoBehaviour
{
    public TMP_Text WorkerText;
    public string WorkerName {get; set; }
    public int Index { get; set; }
    public string BloodOxygen {get; set; }
    public string HeartRate {get; set; }

    private bool isShowDataErrorMessage;
    public void Init(string name, int index, string bloodOxygen, string heartRate)
    {
       isShowDataErrorMessage = false;
        WorkerName = name;
        Index = index;
        BloodOxygen = bloodOxygen;
        HeartRate = heartRate;
        
        WorkerText = gameObject.GetComponentInChildren<TMP_Text>();
        WorkerText.text = WorkerName;

        gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                // TaggleIHealthLabs.Instance.DisconnectDeviceIHealthLabs(ManagerValues.Instance.Mac, ManagerValues.Instance.DeviceType);
                IHLManagerValues.Instance.IndexClick = index;
                if (IHLManagerValues.Instance.Connected != 1)
                {
                    // not connected need to scan
                    SceneManager.LoadScene("2_ScanScene", LoadSceneMode.Additive);
                }
                else
                {
                    // connected just update ui
                    //Debug.Log("Update data for UI");
                }
            });
            
        TaggleIHealthLabsManager.DataEvent += OnSendDataEvent;
    }

    private void OnDestroy()
    {
        TaggleIHealthLabsManager.DataEvent -= OnSendDataEvent;
    }
    private void OnSendDataEvent(string result)
    {
        // {"bloodoxygen":99,"heartrate":73,"pulsestrength":0,"pi":4.300000190734863,"pulseWave":[0,0,0],"dataID":"4C2E15BE0378A2048686596914F3708F"}
        Debug.Log("Worker Item OnSendDataEvent - Data received: " + result);
        if (IHLManagerValues.Instance.IndexClick == Index)
        {
            bool isNull = false;
#if UNITY_ANDROID
            JObject itemData = JObject.Parse(result);
            isNull = itemData == null;
            Debug.Log("Worker Item OnSendDataEvent - token: " + itemData);
#elif UNITY_IOS
            JToken itemData = JToken.Parse(result);
            isNull = itemData == null;
            Debug.Log("Worker Item OnSendDataEvent - token: " + itemData);
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
                    string bloodOxygen = value.Value<string>("bloodoxygen");
                    string heartRate = value.Value<string>("heartrate");
                    UpdateHeartText(bloodOxygen, heartRate);
                }
                else
                {
                    //if (!isShowDataErrorMessage)
                    {
                        isShowDataErrorMessage = true;
                        IHLPopupMessage.Instance.ShowErrorDelay();
                    }
                }
            }
        }
    }

    private void UpdateHeartText(string bloodOxygen, string heartRate)
    {
        WorkerText.text = string.Format("{0}\nBlood Oxygen:{1}\nHeart Rate:{2}", WorkerName, bloodOxygen, heartRate);
    }

}
