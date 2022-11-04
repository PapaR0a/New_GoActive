using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGOTimePickerTextOptionItemView : MonoBehaviour
{
    private Text m_txtItem;
    
    //param
    private bool m_isInitComplete = false;
    private float m_halfHeight;
    private float m_centerPointY;

    //const 
    private const float MAX_DELTA_DISTANCE = 1f;
    private const float MIN_DELTA_DISTANCE = 0.2f;

    public void Init(float centerPoint)
    {
        //find reference
        m_txtItem = GetComponent<Text>();

        //update value
        m_centerPointY = centerPoint;
        m_isInitComplete = true;
    }

    private IEnumerator IEUpdateAlpha()
    {
        m_halfHeight = GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        while(true)
        {
            UpdateAlpha();
            yield return new WaitForEndOfFrame();
        }
    }

    private void UpdateAlpha()
    {
        if(m_txtItem != null)
        {
            //get new halft height item
            float deltaDistance = GetDeltaDistance();
            m_txtItem.color = new Color(66f/255f, 66f/255f, 66f/255f, deltaDistance);
        }
    }

    private float GetDeltaDistance()
    {
        float distance = Vector2.Distance(transform.position, new Vector2(transform.position.x, m_centerPointY));
        return Mathf.Clamp(1 - (distance / m_halfHeight), MIN_DELTA_DISTANCE, MAX_DELTA_DISTANCE);
    }

    private void OnEnable()
    {
        if(!m_isInitComplete)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(IEUpdateAlpha());
    }

    private void OnDisable()
    {
        if(!m_isInitComplete)
        {
            return;
        }
        
        StopAllCoroutines();
    }
}
