using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaggleTemplate.Comm;
using UnityEngine;

public class BWControl
{
	//singleton
	private static BWControl m_api;

    public static BWControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new BWControl();
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
		BWDownloadControl.Api = new BWDownloadControl();
        GAMapControl.Api = new GAMapControl();

		//ShowLoading(true);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }
        else
        {	
            AppBridge.Instance.CallOnPlaygroundLoaded(OnPlaygroundLoaded);

            SceneHelper.LoadSceneAdditiveAsync(GAConstants.MAIN_SCENE);
        }
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
		
	//playground of the user for current app will auto be loaded.
	private void OnPlaygroundLoaded(Playground pg, APIUnity api)
    {
        // cache it locally
        BWAPIService.Api = api;

        //TODO: using for load simulate configs from file. Replace it after integrate with api
        SimulateLoadConfigData(()=>{
			OnFinishLoadConfigData();
		});


        //load configs by api
        // CoroutineHelper.Call(BWAPIService.Api.GetAppOwnInfo((result)=>{
        //     JObject data = result.Data.MobileConfig;
        // 	BWModel.api.Init(data);
        //OnFinishLoadConfigData();
        // }));
    }

	private void OnFinishLoadConfigData()
    {
        //show scene loader to download resource
        BWDownloadControl.Api.ShowLoader();
    }

	private void SimulateLoadConfigData(Action callback)
	{
		//simulate load data from mockup
        // string json = ResourceObject.GetResource<TextAsset>(BWConstant.PATH_MOCKUP_CONFIG_DATA).text;
		// JObject data = JsonConvert.DeserializeObject<JObject>(json);

		// BWModel.Api.Init(data);

		callback?.Invoke();
	}
}