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

    private void Start()
    {
        GAMissionsControl.Api.onUnlockNewMission += OnUnlockNewMission;
        GAMissionsControl.Api.onChangeMap += OnChangeMap;

        foreach (var btn in m_missionBtns)
        {
            btn.interactable = false;
        }

        foreach (var btn in m_missionBtns)
        {
            btn.onClick.AddListener(() => GAMissionsControl.Api.SelectMission(btn.name));
        }

        GAMissionsControl.Api.RefreshMissions();
    }

    private void OnDestroy()
    {
        GAMissionsControl.Api.onUnlockNewMission -= OnUnlockNewMission;
        GAMissionsControl.Api.onChangeMap -= OnChangeMap;

        foreach (var btn in m_missionBtns)
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    private void OnUnlockNewMission()
    {
        if (GAMissionsModel.Api.unlockedMissionsCount >= m_missionBtns.Count)
            return;

        for (int i = 0; i < GAMissionsModel.Api.unlockedMissionsCount; i++)
        {
            m_missionBtns[i].interactable = true;
        }
    }

    private void OnChangeMap(int val)
    {
        m_mapName.sprite = m_mapNamesList[val];
        m_mapInfo.text = m_mapInfoList[val];
    }
}
