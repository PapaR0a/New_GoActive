using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GAMapModel
{
    public static GAMapModel m_api;
    public static GAMapModel Api
    {
        get
        {
            if (m_api == null)
                m_api = new GAMapModel();
            return m_api;
        }
    }

    public enum MapState
    {
        SetGametype,
        SetDistance,
        SetPlaces,
        SetSteps,
        Rewards,
        StopWalking,
        IsPlaying
    }

    public enum GoalType
    {
        Distance,
        Marker,
        Steps
    }

    public MapState menuState = MapState.SetGametype;
    public GoalType currentGoalType = GoalType.Marker;

    public Vector2 previousPlayerCoordinates = Vector2.zero;
    public float distanceTraveled = 0f;
    public float totalDistanceTraveled = 0f;
    public float distanceRemaining = 0;
    public float rotation = 0;
    public float speed = 0;
    public float maxSpeed = 0;
    public float distanceToTravel = 0f;
    public float pointsTotalDistance = 0f;

    public float minimumDistanceToTravel = PlayerPrefs.GetInt(GAConstants.KEY_MINIMUM_DISTANCE, 1000);
    public int minimumStepsRequired = PlayerPrefs.GetInt(GAConstants.KEY_MINIMUM_STEPS, 1000);

    public int currentStepsCount = 0;

    public bool isWaitingForResponse = false;
    public bool isInitialUpdate = true;
    public bool hasGoal = false;
    public bool isMoving = false;
    public bool isPlaying = true;
    public bool isAutoPilot = false;

    public double lng, lat;
    public double targetLng, targetLat;

    public OnlineMapsMarker3D currentGoalMarker;
    public OnlineMapsMarker3D playerMarker;

    public GAMapModel()
    {
        maxSpeed = GAConstants.MAX_TRAVEL_SPEED;
        speed = GAConstants.MAX_TRAVEL_SPEED;
    }
}
