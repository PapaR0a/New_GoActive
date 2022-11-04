using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIFormSubmitDataDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_FORM_ID)]
	public long FormId;

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_DATA)]
	public Dictionary<string, string> Data; //<keyForm, jsonValue>

	public HAGOUIFormSubmitDataDTO(long formId, Dictionary<string, string> data)
	{
		FormId = formId;
		Data = data;
	}

	public HAGOUIFormSubmitDataDTO(JObject data)
	{
		FormId = data.Value<long>(HAGOServiceKey.PARAM_FORM_ID);
		Data = data.Value<Dictionary<string, string>>(HAGOServiceKey.PARAM_DATA);
	}

	public string GetRawData()
	{
		if(Data == null)
		{
			return string.Empty;
		}
		else
		{
			return JsonConvert.SerializeObject(Data);
		}
	}

	public JObject GetJObjectData()
	{
		if(Data == null)
		{
			return null;
		}
		else
		{
			return JObject.FromObject(Data);
		}
	}
}

public class HAGOUIFormDataResultDTO
{
	public bool IsSuccess { get; set; }
	public HAGOUIFormSubmitDataDTO Data { get; set; }

	public HAGOUIFormDataResultDTO(bool isSuccess, HAGOUIFormSubmitDataDTO data)
	{
		IsSuccess = isSuccess;
		Data = data;
	}
}