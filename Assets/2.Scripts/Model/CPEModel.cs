using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

//public enum BWLoadingType
//{
//	Default,
//    WithBackground
//}

//public enum BWPopupIconType
//{
//	Normal,
//    Success,
//    Error
//}

public class CPEModel
{
	public static CPEModel m_api;
	public static CPEModel Api
    {
        get
        {
            if(m_api == null)
                m_api = new CPEModel();
            return m_api;
        }
    }

	//configs
    // private Dictionary<long,string> m_appDataConfig; // app config

    // public Dictionary<long,string> AppDataConfig
    // {
    //     get { return m_appDataConfig; }
    //     set { m_appDataConfig = value; }
    // }

	//param
	public bool isLoadProcess;//flag for downloading process
    public long GameID { get; set; }
    public string AppID { get; set; }
    public long GameUserID { get; set; }

    //init configs
    public void Init(JObject obj)
	{
        // parse app configs
        // ParseAppConfig(obj.SafeValue<JArray>(BWServiceKey.PARAM_APP_CONFIG_DATA));
        // ParseOtherAppConfig(obj.Value<string>(BWServiceKey.PARAM_OTHER_APP_CONFIG_DATA) ?? "");
	}

	// parse app config
    // private void ParseAppConfig(JArray data)
    // {
    //     AppDataConfig = new Dictionary<string, BWAppDataConfig>();
    //     foreach (var jToken in data)
    //     {
    //         JObject child = (JObject)jToken;
    //         BWAppDataConfig pr = new BWAppDataConfig(child);
    //         AppDataConfig.Add(pr.id, pr);
    //     }
    // }

	// get appDataDTO by indentify key
    // public BWAppDataDTO GetAppDataDTO(string id)
    // {
    //     if(!AppDataConfig.ContainsKey(id))
    //         return null;

    //     return new BWAppDataDTO(AppDataConfig[id]);
    // }
}