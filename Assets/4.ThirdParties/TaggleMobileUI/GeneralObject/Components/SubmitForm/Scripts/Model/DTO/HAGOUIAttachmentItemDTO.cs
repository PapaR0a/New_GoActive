using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIAttachmentItemDTO
{
	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
	public long ID { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_TITLE)]
	private int TypeInt { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_RESOURCE)]
	public object Resource { get; set; }

	[JsonProperty(PropertyName = HAGOServiceKey.PARAM_URL)]
	public string Url { get; set; }
	
	[JsonIgnore]
	public HAGOUIAttachmentType Type { get { return (HAGOUIAttachmentType)TypeInt; } }

	//use for upload data to server
	public HAGOUIAttachmentItemDTO(long id, HAGOUIAttachmentType type, object resource)
	{
		ID = id;
		TypeInt = (int)type;
		Resource = resource;
		Url = string.Empty;
	}

	//user for loading data from server
	public HAGOUIAttachmentItemDTO(long id, HAGOUIAttachmentType type, string url)
	{
		ID = id;
		TypeInt = (int)type;
		Resource = null;
		Url = url;
	}

	public HAGOUIAttachmentItemDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID);
		TypeInt = data.Value<int>(HAGOServiceKey.PARAM_TYPE);
		Resource = data[HAGOServiceKey.PARAM_RESOURCE] == null ? null : data.Value<object>(HAGOServiceKey.PARAM_RESOURCE);
		Url = data.Value<string>(HAGOServiceKey.PARAM_URL);
	}
}