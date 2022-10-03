using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAPlayerDataDTO
{
    public int lifePoints;
    public int unlockedMissionsCount;
    public List<List<GAPainRecordDTO>> painDiaryRecords;
    public int missionUnlocking; // 0 - default , 1 - unlock next mission when current mission is viewed, 2 - unlock all missions
    public float minimumDistanceRequired;
    public int minimumStepsRequired;
    public string settingsPassword;

    public float distanceRemaining;
    public float distanceTraveled;
    public float distanceTotalTraveled;

    public int stepsMade;

    public GAPlayerDataDTO(int lifePoints = 0, int unlockedMissionsCount = 0, List<List<GAPainRecordDTO>> painDiaryRecords = null, int missionUnlocking = 0, float minimumDistanceRequired = 1000, int minimumStepsRequired = 1000, string settingsPassword = "goactive123", float distanceRemaining = 0, float distanceTraveled = 0, float distanceTotalTraveled = 0, int stepsMade = 0)
    {
        this.lifePoints = lifePoints;
        this.unlockedMissionsCount = unlockedMissionsCount;
        this.painDiaryRecords = painDiaryRecords;
        this.missionUnlocking = missionUnlocking;

        this.minimumDistanceRequired = minimumDistanceRequired;
        this.minimumStepsRequired = minimumStepsRequired;
        this.settingsPassword = settingsPassword;

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
