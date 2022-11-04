using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAGOSubmitFormManager : MonoBehaviour
{
    private static HAGOSubmitFormManager m_api;
    public static HAGOSubmitFormManager Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = Instantiate(Resources.Load<GameObject>(HAGOConstant.PREFAB_SUBMIT_FORM)).GetComponent<HAGOSubmitFormManager>();
            }
            return m_api;
        }
    }

    private HAGOUIFormPopupView m_view;

    public void Init(HAGOUIFormPopupDTO data, Action<HAGOUIFormSubmitDataDTO> onCompleteEvent)
    {
        HAGOSubmitFormControl.Api.ResultCallbackEvent = onCompleteEvent;
        
        //init view
        m_view = transform.Find("Canvas").GetComponent<HAGOUIFormPopupView>();
        m_view?.Init(data, onCompleteEvent);
    }

    public void Destroy()
    {
        //unregister event
        m_view?.Destroy();

        Destroy(this.gameObject);
    }
}
