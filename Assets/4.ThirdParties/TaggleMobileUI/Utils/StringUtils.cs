using System.Collections;
using System.Collections.Generic;
using Taggle.HealthApp.Others;
using UnityEngine;

public static class StringUtils {

    public static bool IsValidEmail(this string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch { return false; }
    }

    public static string GetKValueUnit(this float value)
    {
        if (value > 1000000000)
            return "B";
        if (value > 1000000)
            return "M";
        if (value > 1000)
            return "K";
        return "";
    }

    public static float GetKValueByUnit(this float value, string unit)
    {
        switch (unit.ToLower())
        {
            case "b":
                return Mathf.RoundToInt((value / 1000000000)*10)/10f;
            case "m":
                return Mathf.RoundToInt((value / 1000000) * 10) / 10f;
            case "k":
                return Mathf.RoundToInt((value / 1000) * 10) / 10f;
            default:
                return value;
        }
    }

    /// <summary>
	/// Returns the name translated of current month
	/// </summary>
    public static string GetMonthName(int index)
    {
        switch (index)
        {
            case 1:
                return Utils.GetValueTrans("^JANUARY");
            case 2:
                return Utils.GetValueTrans("^FEBRUARY");
            case 3:
                return Utils.GetValueTrans("^MARCH");
            case 4:
                return Utils.GetValueTrans("^APRIL");
            case 5:
                return Utils.GetValueTrans("^MAY");
            case 6:
                return Utils.GetValueTrans("^JUNE");
            case 7:
                return Utils.GetValueTrans("^JULY");
            case 8:
                return Utils.GetValueTrans("^AUGUST");
            case 9:
                return Utils.GetValueTrans("^SEPTEMBER");
            case 10:
                return Utils.GetValueTrans("^OCTOBER");
            case 11:
                return Utils.GetValueTrans("^NOVEMBER");
            case 12:
                return Utils.GetValueTrans("^DECEMBER");
            default:
                return "";
        }
    }
}
