using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIDropdownOptionDTO
{
    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
    public long ID { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_NAME)]
    public string Name { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_ICON)]
    public string Icon { get; set; }
 
	public HAGOUIDropdownOptionDTO(long id, string name, string icon)
	{
		ID = id;
		Name = name;
		Icon = icon;
	}

    public HAGOUIDropdownOptionDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID);
		string name = data.Value<string>(HAGOServiceKey.PARAM_NAME);
		Name = HAGOUtils.IsLangKey(name) ? I18N.instance.getValue(name) : name;
		Icon = data.Value<string>(HAGOServiceKey.PARAM_ICON);
	}
}