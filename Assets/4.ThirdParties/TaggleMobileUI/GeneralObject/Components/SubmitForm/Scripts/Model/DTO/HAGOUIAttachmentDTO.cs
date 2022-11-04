using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIAttachmentDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
	public long ID { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_TITLE)]
	public string Title { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_KEY_FORM)]
	public string KeyForm { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_DATA)]
	public List<HAGOUIAttachmentItemDTO> Data { get; set; }

	public HAGOUIAttachmentDTO(long id, string title, string keyForm, List<HAGOUIAttachmentItemDTO> data)
	{
		ID = id;
		Title = title;
		KeyForm = keyForm;
 		Data = data;
	}

	public HAGOUIAttachmentDTO(JObject data)
	{
		// Debug.Log("HAGOUIAttachmentDTO " + (data[HAGOServiceKey.PARAM_DATA] == null).ToString());

		ID = data.Value<long>(HAGOServiceKey.PARAM_ID);
		string title = data.Value<string>(HAGOServiceKey.PARAM_TITLE);
		Title = HAGOUtils.IsLangKey(title) ? I18N.instance.getValue(title) : title;
		KeyForm = data.Value<string>(HAGOServiceKey.PARAM_KEY_FORM);
 		//
		Data = new List<HAGOUIAttachmentItemDTO>();
		JArray ja = data.Value<JArray>(HAGOServiceKey.PARAM_DATA);
		if(ja != null)
		{
			foreach(JToken jt in ja)
			{
				HAGOUIAttachmentItemDTO dto = new HAGOUIAttachmentItemDTO((JObject)jt);
				Data.Add(dto);
			}
		}
	}
}