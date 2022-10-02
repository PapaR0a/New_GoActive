using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAPlayerDataDTO
{
    public int lifePoints;
    public int unlockedMissionsCount;
    public List<bool> availableMissions;
    public List<List<GAPainRecordDTO>> painDiaryRecords;
    public int missionUnlocking; // 0 - default , 1 - unlock next mission when current mission is viewed, 2 - unlock all missions

    public GAPlayerDataDTO(int lifePoints = 0, int unlockedMissionsCount = 0, List<bool> availableMissions = null, List<List<GAPainRecordDTO>> painDiaryRecords = null, int missionUnlocking = 0)
    {
        this.lifePoints = lifePoints;
        this.unlockedMissionsCount = unlockedMissionsCount;
        this.availableMissions = availableMissions;
        this.painDiaryRecords = painDiaryRecords;
        this.missionUnlocking = missionUnlocking;
    }
}
