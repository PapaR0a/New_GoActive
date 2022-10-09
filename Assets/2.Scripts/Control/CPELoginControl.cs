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
                            Debug.Log($"<color=yellow>LOGIN SUCCESS: {JsonConvert.SerializeObject(pgResult.Data)} </color>");

                            CPEModel.Api.GameID = pgResult.Data.Game.ID;
                            CPEModel.Api.AppID = pgResult.Data.Game.AppId;
                            CPEModel.Api.GameUserID = pgResult.Data.GameUser.ID;

                            CoroutineHelper.Call(CPEAPIService.Api.GetAppData((rs) =>
                            {
                                if (rs.Success)
                                {
                                    if (rs.Data != null && rs.Data.Count > 0)
                                    {
                                        JObject data = (JObject)rs.Data[0].Data;
                                        //JObject data = ja.Value<JObject>(CPEServiceKey.PARAM_SCHEMA_APP_DATA);

                                        GAMissionsModel.Api.stepsMade = data.Value<int>("stepsMade");
                                        GAMissionsModel.Api.lifePoints = data.Value<int>("lifePoints");
                                        GAMissionsModel.Api.patientStory = data.Value<string>("patientStory");
                                        GAMissionsModel.Api.distanceTraveled = data.Value<float>("distanceTraveled");
                                        GAMissionsModel.Api.missionUnlocking = data.Value<int>("missionUnlocking");
                                        GAMissionsModel.Api.stepsMade = data.Value<int>("stepsMade");
                                        GAMissionsModel.Api.distanceRemaining = data.Value<float>("distanceRemaining");
                                        GAMissionsModel.Api.minimumStepsRequired = data.Value<int>("minimumStepsRequired");
                                        GAMissionsModel.Api.distanceTotalTraveled = data.Value<float>("distanceTotalTraveled");
                                        GAMissionsModel.Api.unlockedMissionsCount = data.Value<int>("unlockedMissionsCount");
                                        GAMissionsModel.Api.minimumDistanceToTravel = data.Value<int>("minimumDistanceRequired");
                                    }
                                }
                            }, CPEServiceKey.PARAM_SCHEMA_APP_DATA));

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

                                Debug.Log($"<color=yellow> Updated status data: {JsonConvert.SerializeObject(GAMissionsModel.Api.GetMissionStatuses())} </color>");

                                //foreach(var key in GAMissionsModel.Api.missionsStatuses.Keys)
                                //{
                                //    string[] keys = FsmVariables.GlobalVariables.setfm($"{gameObject.name.Replace("_save", "_key")}").stringValues;
                                //    bool[] values = FsmVariables.GlobalVariables.GetFsmArray($"{gameObject.name.Replace("_save", "_value")}").boolValues;
                                //}

                                StartSession(null);

                                InitializeGA();
                            }, GAConstants.SCHEMA_MISSION_STATUS));
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
