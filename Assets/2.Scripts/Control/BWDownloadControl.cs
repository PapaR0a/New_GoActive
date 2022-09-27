using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BWDownloadControl
{
    private static BWDownloadControl api;

    public static BWDownloadControl Api
    {
        get { return api; }
        set { api = value; }
    }

    private Action<float> m_processEvent; // notify process

    public Action<float> ProcessEvent
    {
        get { return m_processEvent; }
        set { m_processEvent = value; }
    }

    public void ShowLoader()
    {
        SceneManager.LoadSceneAsync(BWConstant.SCENE_LOADER,LoadSceneMode.Additive);
    }

    public void Init()
    {
        DownloadResource();
    }

    //download all pet resource and animation
    private void DownloadResource()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)//finish if no internet
        {
            DownloadResourceFinish();
            return;
        }

        //TODO: handle download model & animation later
        // Dictionary<string, int> dicUrls = new Dictionary<string, int>(BWModel.api.GetAllPathModelDownload());
        // foreach (KeyValuePair<string, int> child in BWModel.api.GetAllPathAnimationDownload())
        // {
        //     dicUrls.Add(child.Key, child.Value);
        // }
        // if (dicUrls.Count < 1)//finish if no item download
        // {
            DownloadResourceFinish();
        //     return;
        // }
        // List<string> urls = dicUrls.Keys.ToList();
        // List<string> savePaths = new List<string>();
        // for (int i = 0; i < urls.Count; i++)
        // {
        //     savePaths.Add(BWModel.api.GetPath(urls[i]));
        // }
        // DownloadHelper.DownloadFile(urls, savePaths, 8, (data) => DownloadResourceComplete(dicUrls, data), DownloadResourceFinish, process => {NotifyProcess(0.2f + process*0.8f);});
    }

    ////handle download single pet resource complete
    // private void DownloadResourceComplete(Dictionary<string, int> urls, string url)
    // {
    //     //save version to cache
    //     PlayerPrefs.SetInt(url, urls[url]);
    // }

    //handle download resource complete, login now
    private void DownloadResourceFinish()
    {
        Debug.Log("Download resource complete");
        NotifyProcess(1f);
        
        OnDownLoadPetResourcesComplete("");
    }

    public void NotifyProcess(float process)
    {
        // Debug.Log("Download process complete: " + process);
        ProcessEvent?.Invoke(process);
    }

    private void OnDownLoadPetResourcesComplete(string url)
    {
        Debug.Log("Load info");
        CoroutineHelper.Call(LoadInfo());
    }

    private IEnumerator LoadInfo()
    {
        // BWModel.api.isLoadProcess = true;

        // //TODO: call api to load info if needed
		// //...

		BWModel.Api.isLoadProcess = false;

        yield return new WaitUntil(()=> !BWModel.Api.isLoadProcess);

        CoroutineHelper.Call(OnDownloadFinish());
    }

    private IEnumerator OnDownloadFinish()
    {
        yield return new WaitForSeconds(1f);

        //load scene home
        SceneManager.LoadScene(BWConstant.SCENE_HOME, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(BWConstant.SCENE_LOADER);
        
        BWControl.Api.ShowLoading(false);
    }
}