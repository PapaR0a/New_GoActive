using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAGOConstant
{
	//path
	public const string PATH_LANGUAGE = "Language/HAGOLanguage";

	//mockup config
	public const string CONFIG_PAIR_DEVICE_TYPE = "Config/hago_pairdevice_device_type";

	//prefab
	public const string PREFAB_PAIR_DEVICE = "GOPairDeviceComp";
	public const string PREFAB_SHOW_REWARD = "GOShowRewardComp";
	public const string PREFAB_DATETIME_PICKER = "GODateTimePickerComp";
    public const string PREFAB_EMOJI_PICKER = "GOEmojiPickerComp";
	//
	public const string PREFAB_SUBMIT_FORM = "GOUISubmitFormComp";
	public static string PREFAB_COMPONENT_DATETIME_ITEM = "Components/GOUIDateTimeComp";
    public static string PREFAB_COMPONENT_DATE_ITEM = "Components/GOUIDateComp";
    public static string PREFAB_COMPONENT_TIME_ITEM = "Components/GOUITimeComp";
    public static string PREFAB_COMPONENT_DROPDOWN_ITEM = "Components/GOUIDropdownListComp";
    public static string PREFAB_COMPONENT_CHECK_LIST_ITEM = "Components/GOUICheckListComp";
    public static string PREFAB_COMPONENT_TOGGLE_LIST_ITEM = "Components/GOUIToggleListComp";
    public static string PREFAB_COMPONENT_TOGGLE_ITEM = "Components/GOUIToggleComp";
    public static string PREFAB_COMPONENT_INPUTFIELD_ITEM = "Components/GOUIInputfieldComp";
    public static string PREFAB_COMPONENT_INPUTFIELD_MULTILINE_ITEM = "Components/GOUIInputfieldMultilineComp";
    public static string PREFAB_COMPONENT_ATTACHMENT_ITEM = "Components/GOUIAttachmentComp";
    public static string PREFAB_COMPONENT_FORM_ITEM = "Components/GOUIFormComp";

	//color
	public static string COLOR_TEXT_DARK_DEFAULT //The default text dark color
    {
        get { return "#424242"; }
    }

    public static string COLOR_ERROR //The error red color
    {
        get { return "#F44336"; }
    }
    
    public static string COLOR_HIGHLIGHT //The highlight cyan color
    {
        get { return "#23B188"; }
    }
    
    public static string COLOR_TAB_ICON_NORMAL
    {
        get { return "#727272"; }
    }

    //format
    public static string FORMAT_DATE //format date
    {
        get { return "dd MMM yyyy"; }
    }

    public static string FORMAT_TIME_12_HOURS //format 12h time
    {
        get { return "hh:mm tt"; }
    }
}
