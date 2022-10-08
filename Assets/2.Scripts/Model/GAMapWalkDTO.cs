using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMapWalkDTO
{
    public string activityName;
    public string typeSelected;

    public string dateTime;
    public string dateStarted;
    public string dateEnded;

    public List<string> destinations;
    public string currentDestination;

    public float distanceRemaining;
    public float distanceTraveled;
    public float distanceTotalTraveled;

    public int stepsMade;

    public GAMapWalkDTO(string activityName = "", string typeSelected = "", string dateTime = "", string dateStartedWalking = "", string dateEndedWalking = "", List<string> destinations = null, string currentDestination = "", float distanceRemaining = 0, float distanceTraveled = 0, float distanceTotalTraveled = 0, int stepsMade = 0)
    {
        this.activityName = activityName;
        this.typeSelected = typeSelected;
        this.dateTime = dateTime;
        this.dateStarted = dateStartedWalking;
        this.dateEnded = dateEndedWalking;
        this.destinations = destinations;
        this.currentDestination = currentDestination;
        this.distanceRemaining = RoundFloatValue(distanceRemaining);
        this.distanceTraveled = RoundFloatValue(distanceTraveled);
        this.distanceTotalTraveled = RoundFloatValue(distanceTotalTraveled);

        this.stepsMade = stepsMade;
    }

    private float RoundFloatValue(float value)
    {
        return (float)Math.Round(value * 100f) / 100f;
    }
}
