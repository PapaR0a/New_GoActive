using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GASetDistanceView : MonoBehaviour
{
    [SerializeField] private Dropdown m_kmDropdown = null;
    [SerializeField] private Dropdown m_mDropdown = null;
    [SerializeField] private Text m_distanceRequired = null;

    private void OnEnable()
    {
        m_distanceRequired.text = $"{GAMapModel.Api.minimumDistanceToTravel}m";
    }

    public void SetWalkDistance()
    {
        float km = float.Parse(m_kmDropdown.options[m_kmDropdown.value].text);
        float m = float.Parse(m_mDropdown.options[m_mDropdown.value].text) / 100f;
        GAMapControl.Api.SetWalkDistance( km + m );
    }
}
