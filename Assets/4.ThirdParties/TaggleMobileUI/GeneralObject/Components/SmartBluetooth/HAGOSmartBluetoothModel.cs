using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HAGOSmartBluetoothModel
{
    private static HAGOSmartBluetoothModel m_api;
    public static HAGOSmartBluetoothModel Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOSmartBluetoothModel();
            }
            return m_api;
        }
    }

    public HAGOConnectType FocusType; //focus connect type
    public string DeviceType;
    //
    bool m_PermissionSuccess;
    List<HAGODeviceDTO> m_ListDeviceConnected = new List<HAGODeviceDTO>();

    public void ResetValue()
    {
        m_PermissionSuccess = false;
    }

    public bool GetPermissionSuccess()
    {
        return m_PermissionSuccess;
    }

    public void SetPermissionSuccess(bool b)
    {
        m_PermissionSuccess = b;
    }

    public HAGODeviceDTO GetDeviceConnected(string vsmStat)
    {
        if(string.IsNullOrEmpty(vsmStat))
        {
            return m_ListDeviceConnected.FirstOrDefault();
        }
        else
        {
            foreach(HAGODeviceDTO deviceDTO in m_ListDeviceConnected)
            {
                if(deviceDTO.Type != null && deviceDTO.Type.VSMStats.Contains(vsmStat))
                {
                    return deviceDTO;
                }
            }
            
            return null;
        }
    }

    //get status device conneted measuring specify vsmStat
    public bool IsDeviceWithVSMStatConnected(string vsmStat)
    {
        foreach(HAGODeviceDTO deviceDTO in m_ListDeviceConnected)
        {
            if(deviceDTO.Type != null && deviceDTO.Type.VSMStats.Contains(vsmStat))
            {
                return true;
            }
        }

        return false;
    }

    public List<HAGODeviceDTO> GetListDeviceConnected()
    {
        return m_ListDeviceConnected;
    }

    public void AddDeviceConnected(HAGODeviceDTO device)
    {
        m_ListDeviceConnected.Add(device);
    }

    public void RemoveDeviceConnected(HAGODeviceDTO device)
    {
        if(m_ListDeviceConnected.Contains(device))
        {
            m_ListDeviceConnected.Remove(device);
        }
    }

    public void ClearListDeviceConnected()
    {
        m_ListDeviceConnected.Clear();
    }
}

public class HAGODataScan
{
    public int code;
    public List<HAGODeviceDTO> data;

    public HAGODataScan(int code, List<HAGODeviceDTO> data)
    {
        this.code = code;
        this.data = data;
    }
}