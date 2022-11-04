using Newtonsoft.Json.Linq;

public class HAGODeviceDTO
{

	public string Id;
	public string Name;
	public HAGODeviceTypeDTO Type;
	public HAGODeviceStatusType Status;

	public HAGODeviceDTO(JObject data, string typeID)
	{
		Id = data.Value<string>(HAGOServiceKey.PARAM_ID);
		Name = data.Value<string>(HAGOServiceKey.PARAM_NAME);
		if (Name.Contains("-"))
		{
			Type = HAGOPairDeviceModel.Api.GetDeviceTypeDTOByName(Name.Substring(0, Name.LastIndexOf('-')), typeID);
		}
		else
		{
			Type = HAGOPairDeviceModel.Api.GetDeviceTypeDTOByName(Name, typeID);
		}
		Status = (HAGODeviceStatusType)data.Value<int>(HAGOServiceKey.PARAM_STATUS);
	}

	public HAGODeviceDTO(string id, string name, string connecType, string deviceNameId, HAGODeviceStatusType status)
	{
		Id = id;
		Name = name;
		Type = HAGOPairDeviceModel.Api.GetDeviceTypeDTOByName(deviceNameId, connecType);
		Status = status;
	}
}

