using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIDropdownDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
	public long ID { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_TITLE)]
	public string Title { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_KEY_FORM)]
	public string KeyForm { get; set; }
	
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_OPTIONS)]
	public List<HAGOUIDropdownOptionDTO> Options { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_DEFAULT_VALUE)]
	public int DefaultValueIndex { get; set; }

	public HAGOUIDropdownDTO(long id, string title, string keyForm, List<HAGOUIDropdownOptionDTO> options, int defaultValue = 0)
	{
		ID = id; 
		Title = title;
		KeyForm = keyForm;
		Options = options;
		DefaultValueIndex = defaultValue;
	}

	public HAGOUIDropdownDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID); 
		string title = data.Value<string>(HAGOServiceKey.PARAM_TITLE);
		Title = HAGOUtils.IsLangKey(title) ? I18N.instance.getValue(title) : title;
		KeyForm = data.Value<string>(HAGOServiceKey.PARAM_KEY_FORM); 
		DefaultValueIndex = data.Value<int>(HAGOServiceKey.PARAM_DEFAULT_VALUE);
		//
		Options = new List<HAGOUIDropdownOptionDTO>();
		JArray ja = data.Value<JArray>(HAGOServiceKey.PARAM_OPTIONS); 
		foreach(JToken jt in ja)
		{
			HAGOUIDropdownOptionDTO optDTO = new HAGOUIDropdownOptionDTO((JObject)jt);
			Options.Add(optDTO);
		}
	}
}