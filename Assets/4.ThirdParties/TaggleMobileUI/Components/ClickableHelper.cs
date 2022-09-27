using System;
using System.Collections;
using System.Collections.Generic;
using Taggle.HealthApp.Others;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickableHelper : MonoBehaviour {
    private Action m_onMouseDownEvent;

    public void Init(Action action)
    {
        m_onMouseDownEvent = action;
    }
    void OnMouseDown()
    {
        if(UIHoverListener.CheckPointerUI())
            return;
        m_onMouseDownEvent?.Invoke();
    }
}
