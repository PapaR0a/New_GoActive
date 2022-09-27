using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GASetStepsView : MonoBehaviour
{
    [SerializeField] private Text m_requiredStepsText;
    [SerializeField] private InputField m_stepsInput;
    [SerializeField] private Button m_startWalkingButton;

    private void Start()
    {
        m_stepsInput.onValueChanged.AddListener(OnSetSteps);
        m_startWalkingButton.onClick.AddListener(GAMapControl.Api.ConfirmSteps);
    }

    private void OnEnable()
    {
        m_requiredStepsText.text = GAMapModel.Api.minimumStepsRequired.ToString();
    }

    private void OnDestroy()
    {
        m_startWalkingButton.onClick.RemoveAllListeners();
    }

    private void OnSetSteps(string val)
    {
        int stepsCount = 0;
        int.TryParse(val, out stepsCount);

        m_startWalkingButton.interactable = stepsCount >= GAConstants.MINIMUM_STEPS_REQUIRED;
    }
}
