using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GANavigationView : MonoBehaviour
{
    [SerializeField] private Transform m_mainPanel;
    [SerializeField] private List<Image> m_btnsBG;

    private void Start()
    {
        GAMapControl.Api.onChooseReward += ()=> OnChangeScreen(0);

        UpdateNavButtons();
    }

    private void OnDestroy()
    {
        GAMapControl.Api.onChooseReward -= () => OnChangeScreen(0);
    }

    public void OnChangeScreen( int val )
    {
        UpdateNavButtons(val);
        m_mainPanel.DOLocalMoveX(-760 * val, 1, true);
    }

    private void UpdateNavButtons( int val = 0 )
    {
        foreach (var btnBG in m_btnsBG)
        {
            btnBG.color = new Color(0, 0, 0, 0);
        }

        m_btnsBG[val].color = new Color32(0, 0, 0, 100);
    }
}
