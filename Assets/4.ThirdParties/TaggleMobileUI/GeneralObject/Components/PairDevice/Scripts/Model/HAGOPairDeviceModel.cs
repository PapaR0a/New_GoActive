using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using DeviceType = TaggleTemplate.Comm.DeviceType;

public enum HAGODeviceStatusType
{
    Default,
    Paired,
    Connected,
    Disconnected
}

public class HAGOPairDeviceModel
{
    private static HAGOPairDeviceModel m_api;
    public static HAGOPairDeviceModel Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOPairDeviceModel();
            }
            return m_api;
        }
    }

    //config
    //private Dictionary<string, HAGODeviceTypeConfig> m_configDeviceType;
    private List<HAGODeviceTypeConfig> m_configDeviceType;
    
    //param
    public bool IsScanning = false;
    public GameObject CoroutineObjScanning;

    public bool IsDeviceTypeLoaded()
    {
        return m_configDeviceType != null;
    }

    public void ParseDeviceTypeConfig(List<DeviceType> data)
    {
        m_configDeviceType = new List<HAGODeviceTypeConfig>();

        foreach (DeviceType dt in data)
        {
            HAGODeviceTypeConfig config = new HAGODeviceTypeConfig(dt);
            m_configDeviceType.Add(config);
        }
    }

    public List<HAGODeviceTypeDTO> GetAllDeviceTypeDTO()
    {
        List<HAGODeviceTypeDTO> rs = new List<HAGODeviceTypeDTO>();

        foreach (HAGODeviceTypeConfig config in m_configDeviceType)
        {
            HAGODeviceTypeDTO dto = new HAGODeviceTypeDTO(config);
            rs.Add(dto);
        }

        return rs;
    }

    public HAGODeviceTypeDTO GetDeviceTypeDTOByName(string name, string type)
    {
        Debug.Log("Get device type DTO type: " + type + " - name: " + name);
        foreach (HAGODeviceTypeConfig config in m_configDeviceType)
        {
            if (type.ToLower().Equals(config.ConnectType) && name.Equals(config.Name))
            {
                return new HAGODeviceTypeDTO(config);
            }
        }
        return null;
    }
}
