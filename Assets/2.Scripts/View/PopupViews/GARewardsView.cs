using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GARewardsView : MonoBehaviour
{
    [SerializeField] private Button m_walkButton;
    [SerializeField] private Button m_missionButton;

    private void Start()
    {
        m_walkButton.onClick.AddListener(() => { GAMapControl.Api.ChooseReward(false); } );
        m_missionButton.onClick.AddListener(() => { GAMapControl.Api.ChooseReward(true); });
    }

    private void OnDestroy()
    {
        m_walkButton.onClick.RemoveAllListeners();
        m_missionButton.onClick.RemoveAllListeners();
    }
}
