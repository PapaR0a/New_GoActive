using Newtonsoft.Json;
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
                //TODO: get sessionID and cache in view
                // gssResult.Data.ID
            }
            else
            {
                Debug.LogError("Start session Fail");
            }

        }, gameConfig: gameConfig, prescriptionConfID: prescriptionConfID));
    }


    private void SubmitSessionData(long sessionID, JObject data, bool isCloseSession = false)
    {

        Utils.DebugLog("SubmitSessionData: "+ isCloseSession);
        Utils.DebugLog("SubmitSessionData with data: "+ data.ToString());

        CoroutineHelper.Call(CPEAPIService.Api.PushGameSessionDataAsync(sessionID, JsonConvert.SerializeObject(data), (ssDataResult) =>
        {
            Debug.Log("Submitted successful? " + ssDataResult.Success);
            if (isCloseSession)
            {
                CloseSession(sessionID);
            }
        }, null));
    }

    public void CloseSession(long sessionID)
    {
        Debug.Log("Call CloseSession");
        CoroutineHelper.Call(CPEAPIService.Api.CloseGameSessionAsync(sessionID, 0, (ssResult) =>
        {
            Debug.Log("Submitted successful? " + ssResult.Success);
            if (ssResult.Success)
            {
                 //GetPrescriptionToday();
            }
        }));

    }
    #endregion

    public void Login(string username, string password)
    {
        CoroutineHelper.Call(CPEAPIService.Api.LoginAsync(username, password, (result) =>
            {
                // load playground
                CoroutineHelper.Call(CPEAPIService.Api.GetPlaygroundInfoByAppAsync("com.taggle.combatpd", (pgResult) =>
                {
                    if (pgResult.Success)
                    {
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
}
