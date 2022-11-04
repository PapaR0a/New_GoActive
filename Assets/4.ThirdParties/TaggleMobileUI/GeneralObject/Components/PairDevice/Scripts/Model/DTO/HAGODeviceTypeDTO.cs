using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

[Serializable]
public class HAGODeviceTypeDTO
{
	public string Name;
	public string DisplayName;
	public string Image;
	public string ImageThumb;
	public string ConnectType;
	public List<string> VSMStats;
	public string ServiceUUID;
	public string CharacteristicUUID;

	public HAGODeviceTypeDTO(HAGODeviceTypeConfig config)
	{
		Name = config.Name;
		DisplayName = config.DisplayName;
		Image = config.Image;
		ImageThumb = config.ImageThumb;
		ConnectType = config.ConnectType;
		ServiceUUID = config.ServiceUUID;
		CharacteristicUUID = config.CharacteristicUUID;
		VSMStats = new List<string>(config.VSMStats);
	}
}