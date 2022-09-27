using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ScrollViewUtil : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect m_scrollRect = null;
    private Vector2 m_startPos = Vector2.zero;
    private Vector2 m_endPos = Vector2.zero;
    private int currentIndex = 0;


    private void Start()
    {
        m_scrollRect = gameObject.GetComponent<ScrollRect>();
        m_scrollRect.content.DOLocalMove(m_scrollRect.GetSnapToPositionToBringChildIntoView((RectTransform)m_scrollRect.content.GetChild(currentIndex)), 0);
    }

    private void SnapToPosition()
    {
        if (m_startPos.x > m_endPos.x)
        {
            if (currentIndex < m_scrollRect.content.childCount-1)
                currentIndex++;
        }
        else
        {
            if (currentIndex > 0)
                currentIndex--;
        }

        m_scrollRect.content.DOLocalMove(m_scrollRect.GetSnapToPositionToBringChildIntoView((RectTransform)m_scrollRect.content.GetChild(currentIndex)), 1);

        GAMissionsControl.Api.ChangeMap(currentIndex);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        m_startPos = m_scrollRect.content.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_endPos = m_scrollRect.content.position;

        SnapToPosition();
    }
}
