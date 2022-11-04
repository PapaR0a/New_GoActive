using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIFormComponentItemDataDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_KEY_FORM)]
    public string KeyForm { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_TYPE)]
    public string Type { get; set; }
	
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_IS_VSM)]
    public bool IsVSM { get; set; } = false;

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_DATA)]
    public object Data { get; set; }

	public HAGOUIFormComponentItemDataDTO(string keyForm, string type, object data)
	{
		KeyForm = keyForm;
		Type = type;
		Data = data;
	}

	public HAGOUIFormComponentItemDataDTO(JObject data)
	{
		KeyForm = data.Value<string>(HAGOServiceKey.PARAM_KEY_FORM);
		Type = data.Value<string>(HAGOServiceKey.PARAM_TYPE);
		Data = data[HAGOServiceKey.PARAM_DATA] == null ? null : ParseDataToObject(data.Value<JObject>(HAGOServiceKey.PARAM_DATA));
		IsVSM =  data.Value<bool>(HAGOServiceKey.PARAM_IS_VSM);
	}

	public object ParseDataToObject(JObject json)
	{
		var type = HAGOUtils.GetComponentType(Type);
		if(type == null)
		{
			return null;
		}
		
		// Debug.Log("/////ParseDataFromToObject " + type.Name);
		ConstructorInfo ctor = type.GetConstructor(new[] { typeof(JObject) });
		return ctor.Invoke(new object[]{json});
	}
}