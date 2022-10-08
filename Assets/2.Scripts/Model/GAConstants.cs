using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAConstants
{
    public const string DEFAULT_PASSWORD = "goactive123";

    public const string KEY_UNLOCKING_TYPE = "KEY_UNLOCKING_TYPE";
    public const string KEY_MISSIONS_UNLOCKED = "KEY_MISSIONS_UNLOCKED";
    public const string KEY_MINIMUM_DISTANCE = "KEY_MINIMUM_DISTANCE";
    public const string KEY_MINIMUM_STEPS = "KEY_MINIMUM_STEPS";

    public const string MAIN_SCENE = "GAGame";
    public const string PLAYER_TRAVELLED = "PLAYER_DISTANCE_TRAVELLED";
    public const string DISTANCE_TRAVELLED = "Distance Travelled: {0}km";
    public const string DISTANCE_REMAINING = "Distance Remaining: {0}km";
    public const string TOTAL_DISTANCE_TRAVELLED = "Total Travelled: {0}km";
    public const string AVATAR_TRIGGER = "HasMoved";
    public const string STATS_BOOL = "ShowStats";
    public const string TOGGLE_POPUP = "ShowPopup";
    public const string DROP_PINS_BOOL = "SetPins";
    public const string START_PINS_BOOL = "StartPins";
    public const string REWARDS_BOOL = "RewardsOptions";
    public const string SHOW = "Show";
    public const string HIDE = "Hide";
    public const string MISSION = "Mission";

    public const string SCHEMA_MAP_WALK = "ga_map_walk";
    public const string SCHEMA_MISSION_DATA = "ga_mission_data";

    public const float MARKER_DISTANCE_THRESHOLD = 0.005f; // Minimum distance to "get" a point on the map
    public const float MAX_TRAVEL_SPEED = 250f;

    public const int CIRCLE_SEGMENTS = 32;
    public const int MINIMUM_STEPS_REQUIRED = 1000; // 1m
}
