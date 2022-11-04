using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class IHLWorkersManager : MonoBehaviour
{
    public Transform contentsTransform;
    public GameObject workerPref;
    public GameObject hintPanel;
    public List<Button> hintBtns;
    public Toggle toggleShow;
    public GameObject splashScreen;
    void Start()
    {
        splashScreen.SetActive(true);
        workerPref.SetActive(false);
        LoadWorkerFromResource();
        ShowHintUI();

    }

    void ShowHintUI()
    {
        toggleShow.onValueChanged.AddListener((isClick) =>
           {
               if (isClick)
               {
                   IHLManagerValues.Instance.ShowHint = 1;
               }
               else
               {
                   IHLManagerValues.Instance.ShowHint = 0;
               }
           });

        if (IHLManagerValues.Instance.ShowHint == 1)
        {
            hintPanel.SetActive(true);
            int indexClick = 0;
            foreach (var item in hintBtns)
            {
                item.onClick.AddListener(() =>
                {
                    item.gameObject.SetActive(false);
                    indexClick++;
                   
                    if (indexClick >= hintBtns.Count)
                    {
                        indexClick = hintBtns.Count - 1;
                        hintPanel.SetActive(false);
                    }
                    hintBtns[indexClick].gameObject.SetActive(true);
                });
                item.gameObject.SetActive(false);
            }
            hintBtns[0].gameObject.SetActive(true);


        }
        else
        {
            hintPanel.SetActive(false);
        }
    }

    void LoadWorkerFromResource()
    {
        TextAsset text = Resources.Load<TextAsset>("workers");

        JObject data = JObject.Parse(text.text);
        JArray workers = data.Value<JArray>("workers");
        int index = 0;
        foreach (var worker in workers)
        {
            GameObject obj = Instantiate(workerPref, contentsTransform);
            obj.SetActive(true);
            WorkerItem workerItem = obj.GetComponent<WorkerItem>();
            workerItem.Init(worker.ToString(), index, "", "");
            index++;
        }
        

    }

    public void DisconnectDeviceIHealthLabs()
    {
        Debug.Log("End IHealth splash screen - Disconnect Device IHealthLabs");
        IHLManagerValues.Instance.Connected = 0;
        TaggleIHealthLabs.Instance.DisconnectDeviceIHealthLabs(IHLManagerValues.Instance.Mac, IHLManagerValues.Instance.DeviceType);
    }
    
}
