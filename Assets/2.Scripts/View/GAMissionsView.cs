using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAMissionsView : MonoBehaviour
{
    [SerializeField] private List<Button> m_missionBtns = null;
    [SerializeField] private List<Sprite> m_mapNamesList = null;
    [SerializeField] private List<string> m_mapInfoList = null;

    [SerializeField] private Image m_mapName = null;
    [SerializeField] private Text m_mapInfo = null;

    [SerializeField] private GameObject m_mainCamera = null;

    private void Start()
    {
        GAMissionsControl.Api.onUnlockNewMission += OnUnlockNewMission;
        GAMissionsControl.Api.onChangeMap += OnChangeMap;
        GAMissionsControl.Api.onToggleMainCamera += ToggleMainCamera;

        foreach (var btn in m_missionBtns)
        {
            btn.interactable = false;
        }

        //foreach (var btn in m_missionBtns)
        //{
            //btn.onClick.AddListener(() => GAMissionsControl.Api.SelectMission(btn.name));
        //}

        GAMissionsControl.Api.RefreshMissions();
    }

    private void OnDestroy()
    {
        GAMissionsControl.Api.onUnlockNewMission -= OnUnlockNewMission;
        GAMissionsControl.Api.onChangeMap -= OnChangeMap;
        GAMissionsControl.Api.onToggleMainCamera -= ToggleMainCamera;

        //foreach (var btn in m_missionBtns)
        //{
        //    btn.onClick.RemoveAllListeners();
        //}
    }

    private void ToggleMainCamera(bool val)
    {
        m_mainCamera.SetActive(val);
    }

    private void OnUnlockNewMission()
    {
        for (int i = 0; i < GAMissionsModel.Api.unlockedMissionsCount; i++)
        {
            if (i < m_missionBtns.Count)
                m_missionBtns[i].interactable = true;
        }
    }

    private void OnChangeMap(int val)
    {
        m_mapName.sprite = m_mapNamesList[val];
        m_mapInfo.text = m_mapInfoList[val];
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GAMissionsModel.Api.missionsStatuses = null;

            CPELoginControl.Api.SubmitAppData((JObject)JToken.FromObject(GAMissionsModel.Api.GetMissionStatuses()), GAConstants.SCHEMA_MISSION_STATUS);

            Debug.Log($"<color=red> DEBUG: SEND DEFAULT Mission Status: {JsonConvert.SerializeObject(GAMissionsModel.Api.missionsStatuses)} </color>");
        }
#endif
    }
}
