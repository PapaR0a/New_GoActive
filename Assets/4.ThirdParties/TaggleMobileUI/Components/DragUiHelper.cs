using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Drag UI in canvas
[RequireComponent(typeof(Image))]
public class DragUiHelper : MonoBehaviour
{
    public bool stickToWall;//stick to the wall
    private Action<Vector3> m_endDragAction;//notify end drag, param: vector3 position
    private RectTransform m_canvasRect;//canvas rect
    private RectTransform m_rect;//rect transform of self

    void Awake()
    {
        EventTrigger evt = GetComponent<EventTrigger>();//try get EventTrigger if exist
        if (evt == null)
            evt = gameObject.AddComponent<EventTrigger>();//add Envent Trigger if not exist
        //add begin drag handler
        EventTrigger.Entry beginDrag = new EventTrigger.Entry {eventID = EventTriggerType.BeginDrag};
        beginDrag.callback.AddListener(OnBeginDrag);
        evt.triggers.Add(beginDrag);
        //add drag handler
        EventTrigger.Entry drag = new EventTrigger.Entry {eventID = EventTriggerType.Drag};
        drag.callback.AddListener(OnDrag);
        evt.triggers.Add(drag);
        //add end drag handler
        EventTrigger.Entry endDrag = new EventTrigger.Entry {eventID = EventTriggerType.EndDrag};
        endDrag.callback.AddListener(OnEndDrag);
        evt.triggers.Add(endDrag);
        //find reference components
        var canvas = gameObject.FindInParents<Canvas>();
        if (canvas == null)
            return;
        m_canvasRect = canvas.transform as RectTransform;
        m_rect = transform as RectTransform;
    }

    void Start()
    {
        if (stickToWall)
            ProcessEndPosition();
    }
    
    private void OnEndDrag(BaseEventData data)
    {
        if(m_canvasRect == null)
            return;
        m_endDragAction?.Invoke(transform.localPosition);//notify end drag
        if (stickToWall)
            ProcessEndPosition();
    }

    private void OnDrag(BaseEventData data)
    {
        if(m_canvasRect == null)
            return;
        PointerEventData evt = (PointerEventData) data;
        SetDraggedPosition(evt);//set position for self
    }

    private void OnBeginDrag(BaseEventData data)
    {
        if(m_canvasRect == null)
            return;
        PointerEventData evt = (PointerEventData)data;
        SetDraggedPosition(evt);//set position for self
    }

    public void AddEndDragListener(Action<Vector3> listener)
    {
        m_endDragAction += listener;//register
    }

    public void RemoveEndDragListener(Action<Vector3> listener)
    {
        m_endDragAction -= listener;//unregister
    }

    //convert position from screen to canvas
    //set position for self
    private void SetDraggedPosition(PointerEventData data)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_canvasRect, data.position, data.pressEventCamera, out globalMousePos))//convert pos from screen to canvas
        {
            m_rect.position = globalMousePos;
            m_rect.rotation = m_canvasRect.rotation;
        }
    }

    //calculate end position if StickToWall enable
    private void ProcessEndPosition()
    {
        Vector3 pos = transform.localPosition;//end position
        float borderW = m_canvasRect.sizeDelta.x / 2f;//half width canvas
        float borderH = m_canvasRect.sizeDelta.y / 2f;//half height canvas
        float sizeX = m_rect.sizeDelta.x / 2f;//half width self
        float sizeY = m_rect.sizeDelta.y / 2f;//half height self
        float delX = borderW - Mathf.Abs(pos.x);//distance x to border canvas
        float delY = borderH - Mathf.Abs(pos.y);//distance y to boder canvas
        float x, y;//end position after calculate
        if (delX < delY)//keep y and change x
        {
            x = pos.x < 0 ? -borderW + sizeX : borderW - sizeX;
            y = pos.y < 0 ? Mathf.Max(pos.y, -borderH + sizeY) : Mathf.Min(pos.y, borderH - sizeY);
        }
        else//keep x and change y
        {
            x = pos.x < 0 ? Mathf.Max(pos.x, -borderW + sizeX) : Mathf.Min(pos.x, borderW - sizeX);
            y = pos.y < 0 ? -borderH + sizeY : borderH - sizeY;
        }
        transform.localPosition = new Vector3(x, y, 0);
    }
}
