using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GAMapView : MonoBehaviour
{
    [SerializeField] private Text m_distanceTraveledText = null;
    [SerializeField] private Text m_distanceRemainingText = null;
    [SerializeField] private Text m_distanceTotalTraveledText = null;
    [SerializeField] private Text m_stepsRequirementInput = null;

    [SerializeField] private Animator m_statsAnimator = null;
    [SerializeField] private Animator m_popupAnimator = null;

    [SerializeField] private List<GameObject> m_popupList = null;
    [SerializeField] private GameObject m_mainCanvas = null;

    [SerializeField] private Button m_cancelButton = null;
    [SerializeField] private Button m_demoButton = null;
    [SerializeField] private Button m_setStepsStartButton = null;

    private OnlineMapsLocationService m_locationService => OnlineMapsLocationService.instance;
    private OnlineMapsControlBase m_onlineMapsControlBase => OnlineMapsControlBase.instance;
    private OnlineMapsMarker3DManager m_markerManager => OnlineMapsMarker3DManager.instance;

    private void Update()
    {
        if (GAMapModel.Api.isAutoPilot && GAMapModel.Api.currentGoalType == GAMapModel.GoalType.Marker && GAMapModel.Api.menuState == GAMapModel.MapState.IsPlaying)
        {
            GAMapControl.Api.MoveToTarget();
        }
    }

    private void UpdatePlayerData()
    {
        GAMissionsModel.Api.distanceRemaining = GAMapModel.Api.distanceRemaining;
        GAMissionsModel.Api.distanceTraveled = GAMapModel.Api.distanceTraveled;
        GAMissionsModel.Api.distanceTotalTraveled = GAMapModel.Api.totalDistanceTraveled;

        GAMissionsModel.Api.stepsMade = GAMapModel.Api.currentStepsCount;
    }

    private void Start()
    {
        BindListeners();

        FsmVariables.GlobalVariables.GetFsmInt("GA_Lifepoints").Value = GAMissionsModel.Api.lifePoints;
        GAMapModel.Api.totalDistanceTraveled = GAMissionsModel.Api.distanceTotalTraveled;

        GAMissionsControl.Api.onUpdatePlayerData += UpdatePlayerData;
    }

    private void OnDestroy()
    {
        UnBindListeners();

        GAMissionsControl.Api.onUpdatePlayerData -= UpdatePlayerData;
    }

    private void BindListeners()
    {
        m_locationService.OnLocationChanged += UpdateStats;
        m_onlineMapsControlBase.OnMapClick += GAMapControl.Api.MapClicked;
        m_markerManager.OnCreateItem += GAMapControl.Api.SetNewGoal;

        m_cancelButton.onClick.AddListener(GAMapControl.Api.ShowCancelWalk);
        m_demoButton.onClick.AddListener(GAMapControl.Api.ToggleAutoPilot);

        GAMissionsControl.Api.onToggleMission += (x) => { StartCoroutine(onLoadMission(x)); };
        GAMapControl.Api.onChooseGameType += () => { StartCoroutine(OnShowNextPopup()); };
        GAMapControl.Api.onStartWalking += OnStartWalking;
        GAMapControl.Api.onToggleAnimation += OnToggleAnimation;
        GAMapControl.Api.onUpdateStats += OnUpdateStat;
        GAMapControl.Api.onShowRewardsPopup += OnShowRewardsPopup;
        GAMapControl.Api.onShowNextPopup += () => { StartCoroutine(OnShowNextPopup()); };
        GAMapControl.Api.onSetStepsRequirement += OnSetStepsRequirement;
        GAMapControl.Api.onSetSteps += OnSetSteps;
        GAMapControl.Api.onShowCancelWalk += OnShowCancelWalk;
        GAMapControl.Api.onCancelWalk += OnCancelWalk;
        GAMapControl.Api.onResetStats += OnResetStats;
    }

    private void UnBindListeners()
    {
        m_locationService.OnLocationChanged -= UpdateStats;
        m_onlineMapsControlBase.OnMapClick -= GAMapControl.Api.MapClicked;
        m_markerManager.OnCreateItem -= GAMapControl.Api.SetNewGoal;

        m_cancelButton.onClick.RemoveAllListeners();
        m_demoButton.onClick.RemoveAllListeners();

        GAMissionsControl.Api.onToggleMission -= (x) => { StartCoroutine( onLoadMission(x) ); };
        GAMapControl.Api.onChooseGameType -= () => { StartCoroutine(OnShowNextPopup()); };
        GAMapControl.Api.onStartWalking -= OnStartWalking;
        GAMapControl.Api.onToggleAnimation -= OnToggleAnimation;
        GAMapControl.Api.onUpdateStats -= OnUpdateStat;
        GAMapControl.Api.onShowRewardsPopup -= OnShowRewardsPopup;
        GAMapControl.Api.onShowNextPopup -= () => { StartCoroutine(OnShowNextPopup()); };
        GAMapControl.Api.onSetPlaces -= OnSetPlaces;
        GAMapControl.Api.onSetStepsRequirement -= OnSetStepsRequirement;
        GAMapControl.Api.onSetSteps -= OnSetSteps;
        GAMapControl.Api.onShowCancelWalk -= OnShowCancelWalk;
        GAMapControl.Api.onCancelWalk -= OnCancelWalk;
        GAMapControl.Api.onResetStats -= OnResetStats;
    }

    private IEnumerator onLoadMission(bool val)
    {
        float delay = val ? 0 : 1f;
        yield return new WaitForSeconds(delay);
        m_mainCanvas.SetActive(val);
    }

    private void UpdateStats(Vector2 newPos)
    {
        m_locationService.UpdatePosition();
        GAMapControl.Api.UpdateStats(newPos);
    }

    private void OnStartWalking()
    {
        ToggleWalkButtons(true, GAMapModel.Api.currentGoalType == GAMapModel.GoalType.Marker);

        m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, false);
        m_statsAnimator.gameObject.SetActive(true);
        m_statsAnimator.SetBool(GAConstants.STATS_BOOL, true);
    }

    private void OnUpdateStat()
    {
        m_distanceRemainingText.text = string.Format(GAConstants.DISTANCE_REMAINING, GAMapModel.Api.distanceRemaining.ToString("0.00"));
        m_distanceTraveledText.text = string.Format(GAConstants.DISTANCE_TRAVELLED, GAMapModel.Api.distanceTraveled.ToString("0.00"));
        m_distanceTotalTraveledText.text = string.Format(GAConstants.TOTAL_DISTANCE_TRAVELLED, GAMapModel.Api.totalDistanceTraveled.ToString("0.00"));
    }

    private void OnSetPlaces()
    {
        m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, false);
        m_statsAnimator.gameObject.SetActive(true);
        m_statsAnimator.SetBool(GAConstants.STATS_BOOL, true);
    }

    public void OnSetStepsRequirement()
    {
        int.TryParse(m_stepsRequirementInput.text, out int inputSteps);

        if (inputSteps >= GAMapModel.Api.minimumStepsRequired)
            m_setStepsStartButton.interactable = true;
        else
            m_setStepsStartButton.interactable = false;
    }

    public void OnSetSteps()
    {
        m_setStepsStartButton.interactable = false;

        m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, false);

        m_statsAnimator.gameObject.SetActive(true);
        m_statsAnimator.SetBool(GAConstants.STATS_BOOL, true);
    }

    public void OnShowCancelWalk()
    {
        ToggleWalkButtons(false);

        m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, true);
    }

    public void OnCancelWalk(bool val)
    {
        ToggleWalkButtons(!val);

        if (val)
        {
            m_distanceRemainingText.text = string.Format(GAConstants.DISTANCE_REMAINING, GAMapModel.Api.distanceRemaining.ToString("0.00"));
            m_statsAnimator.SetBool(GAConstants.STATS_BOOL, false);
        }
        else
            m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, false);
    }

    private void OnShowRewardsPopup()
    {
        m_statsAnimator.SetBool(GAConstants.STATS_BOOL, false);
    }

    private IEnumerator OnShowNextPopup()
    {
        ToggleWalkButtons(false);

        m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, false);
        yield return new WaitForSeconds(.01f);
        foreach (var popup in m_popupList)
        {
            popup.SetActive(false);
        }

        m_popupList[(int)GAMapModel.Api.menuState].SetActive(true);
        m_popupAnimator.SetBool(GAConstants.TOGGLE_POPUP, true);
    }

    private void OnResetStats()
    {
        m_distanceRemainingText.text = string.Format(GAConstants.DISTANCE_REMAINING, GAMapModel.Api.distanceRemaining.ToString("0.00"));
        m_distanceTraveledText.text = string.Format(GAConstants.DISTANCE_TRAVELLED, GAMapModel.Api.distanceTraveled.ToString("0.00"));
    }

    private void OnToggleAnimation()
    {
        GAMapModel.Api.playerMarker.transform.gameObject.GetComponent<Animator>().SetTrigger(GAConstants.AVATAR_TRIGGER);
    }

    private void ToggleWalkButtons(bool cancelWalk, bool autoPilot = false)
    {
        m_cancelButton.gameObject.SetActive(cancelWalk);
        m_demoButton.gameObject.SetActive(autoPilot);
    }
}
