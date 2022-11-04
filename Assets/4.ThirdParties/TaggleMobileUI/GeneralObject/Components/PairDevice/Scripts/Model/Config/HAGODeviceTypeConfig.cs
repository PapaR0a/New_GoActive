using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using DeviceType = TaggleTemplate.Comm.DeviceType;

public class HAGODeviceTypeConfig
{
	public readonly string Id;
	public readonly string Name;
	public readonly string DisplayName;
	public readonly string Image;
	public readonly string ImageThumb;
	public readonly string ConnectType;
	public readonly List<string> VSMStats;
	public readonly string ServiceUUID;
	public readonly string CharacteristicUUID;

	public HAGODeviceTypeConfig(DeviceType data)
	{
		Id = data.Id.ToString();
		Name = data.Name;
		DisplayName = data.DisplayName;
		Image = data.Image;
		ImageThumb = data.ImageThumb;
		ConnectType = HAGOUtils.ParseConnectType(data.ConnectType).ToString().ToLower();
		ServiceUUID = data.ServiceUUID;
		CharacteristicUUID = data.CharacteristicUUID;
		//
		VSMStats = new List<string>(data.VSMTypes);
	}
}