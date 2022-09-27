using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaggleTemplate.Comm;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CPEControl
{
	//singleton
	private static CPEControl m_api;

    public static CPEControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new CPEControl();
            }
            return m_api;
        }
    }

    private Action<bool, BWLoadingType> m_showLoadingEvent; //notify show hide mood rate popup
    public Action<bool, BWLoadingType> ShowLoadingEvent
    {
        get { return m_showLoadingEvent; }
        set { m_showLoadingEvent = value; }
    }

    private Action<BWPopupIconType, string, bool, Action> m_showPopupEvent; //notify show popup message
    public Action<BWPopupIconType, string, bool, Action> ShowPopupEvent
    {
        get { return m_showPopupEvent; }
        set { m_showPopupEvent = value; }
    }

	public void Init()
	{
		//init other controls here
		CPELoginControl.Api = new CPELoginControl();

        SceneManager.LoadSceneAsync(CPEConstant.SCENE_LOGIN, LoadSceneMode.Additive);

	}

	//show hide loading
    //isShow: true = show, false = hide
    public void ShowLoading(bool isShow, BWLoadingType type = BWLoadingType.Default)
    {
        ShowLoadingEvent?.Invoke(isShow,type);//call action
    }

	 //show popup message
    public void ShowPopup(BWPopupIconType iconType, string content, bool isAutoTurnOff, Action callback)
    {
        ShowPopupEvent?.Invoke(iconType,content,isAutoTurnOff,callback);
    }
		
}