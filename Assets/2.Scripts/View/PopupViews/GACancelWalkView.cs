using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GACancelWalkView : MonoBehaviour
{
    [SerializeField] private Button m_stopWalkButton;
    [SerializeField] private Button m_continueWalkButton;

    private void Start()
    {
        m_stopWalkButton.onClick.AddListener(() => { GAMapControl.Api.CancelWalk(true); });
        m_continueWalkButton.onClick.AddListener(() => { GAMapControl.Api.CancelWalk(false); });
    }

    private void OnDestroy()
    {
        m_stopWalkButton.onClick.RemoveAllListeners();
        m_continueWalkButton.onClick.RemoveAllListeners();
    }
}
