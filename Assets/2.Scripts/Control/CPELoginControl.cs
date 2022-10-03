﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TaggleTemplate.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CPELoginControl
{
    #region API
    private static CPELoginControl api;

    public static CPELoginControl Api
    {
        get { return api; }
        set { api = value; }
    }
    #endregion


    #region Session Flow: StartSession -> [SubmitSessionData,...,SubmitSessionData] -> CloseSession
    public void StartSession(JObject gameConfig)
    {
        long prescriptionConfID = -1;
        //  start session
        CoroutineHelper.Call(CPEAPIService.Api.StartGameSessionAsync(CPEModel.Api.GameUserID, CPEModel.Api.GameID, (gssResult) =>
        {
            if (gssResult.Success)
            {
                Debug.Log("Start session successful");
                CPEModel.Api.SessionID = gssResult.Data.ID;
            }
            else
            {
                Debug.LogError("Start session Fail");
            }

        }, gameConfig: gameConfig, prescriptionConfID: prescriptionConfID));
    }


    public void SubmitSessionData(JObject data, bool isCloseSession = false)
    {

        Utils.DebugLog("SubmitSessionData: "+ isCloseSession);
        Utils.DebugLog("SubmitSessionData with data: "+ data.ToString());

        CoroutineHelper.Call(CPEAPIService.Api.PushGameSessionDataAsync(CPEModel.Api.SessionID, JsonConvert.SerializeObject(data), (ssDataResult) =>
        {
            Debug.Log("Submitted successful? " + ssDataResult.Success);
            if (isCloseSession)
            {
                CloseSession();
            }
        }, null));
    }

    public void CloseSession()
    {
        Debug.Log("Call CloseSession");
        CoroutineHelper.Call(CPEAPIService.Api.CloseGameSessionAsync(CPEModel.Api.SessionID, 0, (ssResult) =>
        {
            Debug.Log("Submitted successful? " + ssResult.Success);
            if (ssResult.Success)
            {

            }
        }));

    }
    #endregion

    public void Login(string username, string password)
    {
        CoroutineHelper.Call(CPEAPIService.Api.LoginAsync(username, password, (result) =>
            {
                // load playground
                CoroutineHelper.Call(CPEAPIService.Api.GetPlaygroundInfoByAppAsync("com.taggle.goactive", (pgResult) =>
                {
                    if (pgResult.Success)
                    {
                        Debug.LogError($"LOGIN SUCCESS: {JsonConvert.SerializeObject(pgResult.Data)}");

                        CPEModel.Api.GameID = pgResult.Data.Game.ID;
                        CPEModel.Api.AppID = pgResult.Data.Game.AppId;
                        CPEModel.Api.GameUserID = pgResult.Data.GameUser.ID;

                        StartSession(null);

                        InitializeGA();
                    }
                    else
                    {
                        Debug.Log("LoginAsync - Login Error");
                    }
                }));

                // load prescription today
                //GetPrescriptionToday();
            }));
    }

    public void InitializeGA()
    {
        SceneManager.UnloadSceneAsync($"GA_Title");
        //SceneManager.LoadSceneAsync($"GAGame", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync($"GA_Intro", LoadSceneMode.Additive);
    }

    public void GetAppData(Action<bool> onCallback)
    {
        CoroutineHelper.Call(CPEAPIService.Api.GetAppData((result) =>
        {
            if (result.Success)
            {
                if (result.Data != null && result.Data.Count > 0)
                {
                    //TODO:
                    //
                }
            }
            onCallback?.Invoke(result.Success && result.Data != null && result.Data.Count > 0);
        }, CPEServiceKey.PARAM_SCHEMA_APP_DATA, CPEModel.Api.AppID));



    }

    public void SubmitAppData(JToken data)
    {
        CoroutineHelper.Call(CPEAPIService.Api.CreateOrUpdateAppData(CPEServiceKey.PARAM_SCHEMA_APP_DATA, data, (result) =>
        {
            if (result.Success)
            {
                Debug.Log("<color=yellow> Submit Player Data Success </color>");
            }
        }, null, null, false, CPEModel.Api.AppID));
    }
}
