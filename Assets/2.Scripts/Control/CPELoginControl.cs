using HutongGames.PlayMaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TaggleTemplate.Comm;
using TaggleTemplate.Comm.TaggleExtensions;
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

    public void Login(string username, string password, Action callback = null)
    {
        CoroutineHelper.Call(CPEAPIService.Api.LoginMobileAsync(username, password, BWConstant.FIREBASE_FAKE_KEY, (result) =>
        {
            if (result.Success)
                {
                    // load playground
                    CoroutineHelper.Call(CPEAPIService.Api.GetPlaygroundInfoByAppAsync(Application.identifier, (pgResult) =>
                    {
                        if (pgResult.Success)
                        {
                            CPEModel.Api.GameID = pgResult.Data.Game.ID;
                            CPEModel.Api.AppID = pgResult.Data.Game.AppId;
                            CPEModel.Api.GameUserID = pgResult.Data.GameUser.ID;

                            #region Player Data

                            CoroutineHelper.Call(CPEAPIService.Api.GetAppData((rs) =>
                            {
                                if (rs.Success)
                                {
                                    if (rs.Data != null && rs.Data.Count > 0)
                                    {
                                        JObject data = (JObject)rs.Data[0].Data;

                                        Debug.Log($"<color=yellow>LOGIN SUCCESS: {JsonConvert.SerializeObject(data)} </color>");

                                        var serverPlayerData = new GAPlayerDataDTO
                                        (
                                            lifePoints: data.Value<int>("lifePoints"),
                                            unlockedMissionsCount: data.Value<int>("unlockedMissionsCount"),
                                            painDiaryRecords: null,
                                            missionUnlocking: data.Value<int>("missionUnlocking"),
                                            minimumDistanceRequired: data.Value<int>("minimumDistanceRequired"),
                                            minimumStepsRequired: data.Value<int>("minimumStepsRequired"),
                                            settingsPassword: "goactive123",
                                            distanceRemaining: data.Value<float>("distanceRemaining"),
                                            distanceTraveled: data.Value<float>("distanceTraveled"),
                                            distanceTotalTraveled: data.Value<float>("distanceTotalTraveled"),
                                            stepsMade: data.Value<int>("stepsMade"),
                                            patientStory: data.Value<string>("patientStory")
                                        );

                                        GAMissionsModel.Api.UpdatePlayerData(serverPlayerData);
                                    }
                                }
                            }, CPEServiceKey.PARAM_SCHEMA_APP_DATA));

                            #endregion

                            #region Pain Diaries

                            CoroutineHelper.Call(CPEAPIService.Api.GetAppData((rs) =>
                            {
                                if (rs.Success)
                                {
                                    if (rs.Data != null && rs.Data.Count > 0)
                                    {
                                        JObject data = (JObject)rs.Data[0].Data;
                                        var oldData = GAMissionsModel.Api.GetMissionStatuses();

                                        foreach (var key in oldData.Keys)
                                        {
                                            List<MissionData> missionDataList = new List<MissionData>();
                                            string missionDataJson = data[key].ToString();
                                            JArray ja = JsonConvert.DeserializeObject<JArray>(missionDataJson);

                                            for (int i = 0; i < ja.Count; i++)
                                            {
                                                var missionData = new MissionData();

                                                JObject joData = (JObject)ja[i];
                                                missionData.key = joData.Value<string>("key");
                                                missionData.value = joData.Value<int>("value");

                                                missionDataList.Add(missionData);
                                            }
                                        }
                                    }
                                }
                            }, GAConstants.SCHEMA_PAIN_DIARY));

                            #endregion

                            #region Missions Status

                            CoroutineHelper.Call(CPEAPIService.Api.GetAppData((rs) =>
                            {
                                if (rs.Success)
                                {
                                    var serverData = new Dictionary<string, List<MissionData>>();

                                    if (rs.Data != null && rs.Data.Count > 0)
                                    {
                                        JObject data = (JObject)rs.Data[0].Data;
                                        var oldData = GAMissionsModel.Api.GetMissionStatuses();

                                        foreach (var key in oldData.Keys)
                                        {
                                            List<MissionData> missionDataList = new List<MissionData>();
                                            string missionDataJson = data[key].ToString();
                                            JArray ja = JsonConvert.DeserializeObject<JArray>(missionDataJson);

                                            for (int i = 0; i < ja.Count; i++)
                                            {
                                                var missionData = new MissionData();

                                                JObject joData = (JObject)ja[i];
                                                missionData.key = joData.Value<string>("key");
                                                missionData.value = joData.Value<int>("value");

                                                missionDataList.Add(missionData);
                                            }
                                            serverData.Add(key, missionDataList);
                                        }

                                        GAMissionsModel.Api.missionsStatuses = serverData;
                                    }
                                }

                                var currentStatuses = GAMissionsModel.Api.GetMissionStatuses();

                                foreach (var stat in currentStatuses)
                                {
                                    List<string> keyData = new List<string>();
                                    List<int> valueData = new List<int>();

                                    foreach(var data in stat.Value)
                                    {
                                        keyData.Add(data.key);
                                        valueData.Add(data.value);

                                        FsmVariables.GlobalVariables.FindFsmArray(stat.Key.Replace("_save", "_key")).stringValues = keyData.ToArray();
                                        FsmVariables.GlobalVariables.FindFsmArray(stat.Key.Replace("_save", "_value")).intValues = valueData.ToArray();
                                    }
                                }

                                Debug.Log($"<color=yellow> Updated status data: {JsonConvert.SerializeObject(GAMissionsModel.Api.GetMissionStatuses())} </color>");

                                StartSession(null);

                                InitializeGA();
                            }, GAConstants.SCHEMA_MISSION_STATUS));

                            #endregion
                        }
                        else
                        {
                            callback?.Invoke();
                            Debug.LogWarning("Error getting playground... Login again?");
                        }
                    }, 5));
                }
                else
                {
                // TODO: Please show error, can use MainError as errorkey
                // Example: INVALID_CREDENTIALS -> Wrong username password
                callback?.Invoke();
                Debug.Log("Error logging in with username password. MainError: " + result.MainError);
                }
        }));
    }

    public void InitializeGA()
    {
        SceneManager.UnloadSceneAsync($"GA_Title");
        //SceneManager.LoadSceneAsync($"GAGame", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync($"GA_Intro", LoadSceneMode.Additive);
    }

    public void SubmitAppData(JToken data, string schemaName = "", bool forceCreate = false)
    {
        schemaName = string.IsNullOrEmpty(schemaName) ? CPEServiceKey.PARAM_SCHEMA_APP_DATA : schemaName;

        CoroutineHelper.Call(CPEAPIService.Api.CreateOrUpdateAppData(schemaName, data, (result) =>
        {
            if (result.Success)
            {
                Debug.Log("<color=yellow> Submit Data Success </color>");
            }
        }, null, null, forceCreate, CPEModel.Api.AppID));
    }
}
