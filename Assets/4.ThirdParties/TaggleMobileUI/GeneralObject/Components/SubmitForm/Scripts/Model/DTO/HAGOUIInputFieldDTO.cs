using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIInputFieldDTO
{
    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_ID)]
    public long ID { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_TITLE)]
    public string Title { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_CONTENT)]
    public string Content { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_PLACEHOLDER)]
    public string Placeholder { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_KEY_FORM)]
	public string KeyForm { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_CONTENT_TYPE)]
	private string ContentTypeStr { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_UNIT)]
	public string Unit { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_MIN_VALUE)]
	public float MinValue { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_MAX_VALUE)]
	public float MaxValue { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_MIN_LENGTH)]
	public int MinLength { get; set; }

    [JsonProperty(PropertyName = HAGOServiceKey.PARAM_MAX_LENGTH)]
	public int MaxLength { get; set; }


    [JsonIgnore]
    public HAGOUIInputFieldContentType ContentType { get { return GetContentTypeFromString(ContentTypeStr); } }
    
	public HAGOUIInputFieldDTO(
        long id, string title, string placeholder, string content, string unit, HAGOUIInputFieldContentType contentType, string keyForm,
        float minValue = 0, float maxValue = 0, int minLength = 0, int maxLength = 0
    )
	{
		ID = id;
		Title = title;
        Placeholder = placeholder;
        Content = content;
        Unit = unit;
		KeyForm = keyForm;
        ContentTypeStr = GetStringContentType(contentType);
        MinValue = minValue;
        MaxValue = maxValue;
        MinLength = minLength;
        MaxLength = maxLength;
	}

    public HAGOUIInputFieldDTO(JObject data)
	{
		ID = data.Value<long>(HAGOServiceKey.PARAM_ID);
		string title = data.Value<string>(HAGOServiceKey.PARAM_TITLE);
		Title = HAGOUtils.IsLangKey(title) ? I18N.instance.getValue(title) : title;
        string placeHolder = data.Value<string>(HAGOServiceKey.PARAM_PLACEHOLDER);
		Placeholder = HAGOUtils.IsLangKey(placeHolder) ? I18N.instance.getValue(placeHolder) : placeHolder;
        Content = data.Value<string>(HAGOServiceKey.PARAM_CONTENT);
        string unit = data.Value<string>(HAGOServiceKey.PARAM_UNIT);
        Unit = HAGOUtils.IsLangKey(unit) ?
        I18N.instance.getValue(unit) :
        unit;
        ContentTypeStr = data.Value<string>(HAGOServiceKey.PARAM_CONTENT_TYPE);
		KeyForm = data.Value<string>(HAGOServiceKey.PARAM_KEY_FORM);
        MinValue = data.Value<float>(HAGOServiceKey.PARAM_MIN_VALUE);
        MaxValue = data.Value<float>(HAGOServiceKey.PARAM_MAX_VALUE);
        MinLength = data.Value<int>(HAGOServiceKey.PARAM_MIN_LENGTH);
        MaxLength = data.Value<int>(HAGOServiceKey.PARAM_MAX_LENGTH);
	}

    public HAGOUIInputFieldContentType GetContentTypeFromString(string contentType)
    {
        switch(contentType)
        {
            case HAGOServiceKey.PARAM_INTEGER:
                return HAGOUIInputFieldContentType.Integer;

            case HAGOServiceKey.PARAM_DECIMAL:
                return HAGOUIInputFieldContentType.Decimal;

            case HAGOServiceKey.PARAM_STANDARD:
            default:
                return HAGOUIInputFieldContentType.Standard;
        }
    }

    public string GetStringContentType(HAGOUIInputFieldContentType contentType)
    {
        switch(contentType)
        {
            case HAGOUIInputFieldContentType.Integer:
                return HAGOServiceKey.PARAM_INTEGER;

            case HAGOUIInputFieldContentType.Decimal:
                return HAGOServiceKey.PARAM_DECIMAL;

            case HAGOUIInputFieldContentType.Standard:
            default:
                return HAGOServiceKey.PARAM_STANDARD;
        }
    }
}