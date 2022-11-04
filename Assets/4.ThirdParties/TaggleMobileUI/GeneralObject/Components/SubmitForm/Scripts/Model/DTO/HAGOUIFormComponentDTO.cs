using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaggleTemplate.Comm;
using UnityEngine;

public class HAGOUIFormComponentDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
    public long ID { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_DATA)]
    public List<HAGOUIFormComponentItemDataDTO> Data { get; set; }

	[JsonIgnore]
	public List<string> VsmKeys { get {
		Data = Data ?? new List<HAGOUIFormComponentItemDataDTO>();
		return Data.Where(d=>d.IsVSM).Select(d=>d.KeyForm).ToList();
	}}

	public HAGOUIFormComponentDTO(long id, List<HAGOUIFormComponentItemDataDTO> data)
	{
		ID = id;
		Data = data;
	}

	public void OverrideDataValue(HAGOUIFormSubmitDataDTO data)
	{
		if(Data.Count != data.Data.Count)
		{
			Debug.LogError("Not same form style!");
			return;
		}

		foreach(HAGOUIFormComponentItemDataDTO itemDataDTO in Data)
		{
			itemDataDTO.Data = (object)data.Data[itemDataDTO.KeyForm];
		}
	}

	public HAGOUIFormComponentDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID);
		//
		Data = new List<HAGOUIFormComponentItemDataDTO>();
		JArray ja = data.Value<JArray>(HAGOServiceKey.PARAM_DATA);
		foreach(JToken jt in ja)
		{
			Data.Add(new HAGOUIFormComponentItemDataDTO((JObject)jt));
		}
	}

	public HAGOUIFormComponentDTO(VSMForm data)
	{
		if(data != null)
		{
			ID = data.Id;
			//
			Data = new List<HAGOUIFormComponentItemDataDTO>();
			JArray ja = data.Definition.Value<JArray>(HAGOServiceKey.PARAM_DATA);
			foreach(JToken jt in ja)
			{
				Data.Add(new HAGOUIFormComponentItemDataDTO((JObject)jt));
			}
		}
	}
}