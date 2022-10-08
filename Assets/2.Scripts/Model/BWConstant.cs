using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWConstant
{
	//mockup file
	// public const string PATH_MOCKUP_CONFIG_DATA = "BWConfig";

	// scene name
	public static string SCENE_LOADFIRST // name scene load first
    {
        get { return "BWLoadFirst"; }
    }

	public static string SCENE_HOME // name scene home
    {
        get { return "BWHome"; }
    }

	public static string SCENE_LOADER // name scene downloader
    {
        get { return "BWDownloader"; }
    }

	// prefab
	public static string CONFIG_PREFAB_LOADING // name prefab loading
    {
        get { return "Loading"; }
    }

	public static string CONFIG_PREFAB_LOADING_BG // name prefab loading with background
    {
        get { return "LoadingBG"; }
    }

	public static string CONFIG_PREFAB_POPUP_MESSAGE // name prefab popup message
    {
        get { return "PopupMessage"; }
    }

	// format
	public static string FORMAT_DATETIME_12_HOURS
    {
        get { return "hh:mm tt"; }
    }

	public static string FORMAT_TIMESPAN_IN_DAY // format timespan 24h for timespan
    {
        get { return @"hh\:mm"; }
    }

	public static string FORMAT_TIMESPAN_DURATION_TO_HOUR // format timespan duration with total hours > 0
    {
        get { return @"hh\:mm\:ss"; }
    }

	public static string FORMAT_TIMESPAN_DURATION_TO_MINUTE // format timespan duration with total hours <= 0
    {
        get { return @"mm\:ss"; }
    }

    public static string FIREBASE_FAKE_KEY // format timespan duration with total hours <= 0
    {
        get { return @"g0active-xxxx-yyyy-zzzz"; }
    }
}