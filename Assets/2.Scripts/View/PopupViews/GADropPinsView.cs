using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GADropPinsView : MonoBehaviour
{
    [SerializeField] private Text m_currentTotalDistanceText = null;
    [SerializeField] private Text m_minimumDistanceText = null;
    [SerializeField] private Button m_setDistanceStartButton = null;

    private void Start()
    {
        GAMapControl.Api.onSetNewGoal += OnSetNewGoal;
        GAMapControl.Api.onSetPlaces += OnSetPlaces;
        GAMapControl.Api.onRemoveMarkers += OnRemoveMarkers;

        m_setDistanceStartButton.onClick.AddListener( GAMapControl.Api.ConfirmPlaces );
    }

    private void OnEnable()
    {
        m_minimumDistanceText.text = $"{GAMapModel.Api.minimumDistanceToTravel}m";
        m_currentTotalDistanceText.text = string.Format("{0}km", 0);
    }

    private void OnDestroy()
    {
        GAMapControl.Api.onSetNewGoal -= OnSetNewGoal;
        GAMapControl.Api.onSetPlaces -= OnSetPlaces;
        GAMapControl.Api.onRemoveMarkers -= OnRemoveMarkers;
    }

    private void OnSetNewGoal(bool val)
    {
        m_currentTotalDistanceText.text = string.Format("{0}m", (GAMapModel.Api.minimumDistanceToTravel - (GAMapModel.Api.pointsTotalDistance * 1000)).ToString("0.00"));

        if (val)
            m_setDistanceStartButton.interactable = true;
    }

    private void OnSetPlaces()
    {
        m_setDistanceStartButton.interactable = false;
    }

    private void OnRemoveMarkers()
    {
        m_currentTotalDistanceText.text = string.Format("{0}km", GAMapModel.Api.pointsTotalDistance.ToString("0.00"));
    }
}
