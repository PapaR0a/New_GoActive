using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIToggleListDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
	public long ID { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_TITLE)]
	public string Title { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_KEY_FORM)]
	public string KeyForm { get; set; }
	
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_OPTIONS)]
	public List<HAGOUIToggleOptionDTO> Options { get; set; }

	public HAGOUIToggleListDTO(long id, string title, string keyForm, List<HAGOUIToggleOptionDTO> options)
	{
		ID = id; 
		Title = title;
		KeyForm = keyForm;
		Options = options;
	}

	public HAGOUIToggleListDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID); 
		string title = data.Value<string>(HAGOServiceKey.PARAM_TITLE);
		Title = HAGOUtils.IsLangKey(title) ? I18N.instance.getValue(title) : title;
		KeyForm = data.Value<string>(HAGOServiceKey.PARAM_KEY_FORM);
		//
		Options = new List<HAGOUIToggleOptionDTO>();
		JArray ja = data.Value<JArray>(HAGOServiceKey.PARAM_OPTIONS);
		foreach(JToken jt in ja)
		{
			HAGOUIToggleOptionDTO optDTO = new HAGOUIToggleOptionDTO((JObject)jt);
			Options.Add(optDTO);
		}
	}
}