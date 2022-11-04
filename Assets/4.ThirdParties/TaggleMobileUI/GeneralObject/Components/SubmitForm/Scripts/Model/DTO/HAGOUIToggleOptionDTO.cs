using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIToggleOptionDTO
{
    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
    public long ID { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_TITLE)]
    public string Title { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_KEY_FORM)]
    public string KeyForm { get; set; }
    
    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_DEFAULT_VALUE)]
    public bool DefaulValue { get; set; }

	public HAGOUIToggleOptionDTO(bool isOn)
	{
		DefaulValue = isOn;
	}
 
	public HAGOUIToggleOptionDTO(long id, string title, string keyForm, bool defaultValue)
	{
		ID = id;
		Title = title;
		KeyForm = keyForm;
        DefaulValue = defaultValue;
	}

    public HAGOUIToggleOptionDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID);
		string title = data.Value<string>(HAGOServiceKey.PARAM_TITLE);
		Title = HAGOUtils.IsLangKey(title) ? I18N.instance.getValue(title) : title;
		KeyForm = data.Value<string>(HAGOServiceKey.PARAM_KEY_FORM);
        DefaulValue = data.Value<bool>(HAGOServiceKey.PARAM_DEFAULT_VALUE);
	}
}