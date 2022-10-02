using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAPlayerDataDTO
{
    public int lifePoints;
    public List<bool> missions;
    public List<bool> availableMissions;
    public List<List<GAPainRecordDTO>> painDiaryRecords;
    public bool nextMissionUnlocking;

    public GAPlayerDataDTO(int lifePoints = 0, List<bool> missions = null, List<bool> availableMissions = null, List<List<GAPainRecordDTO>> painDiaryRecords = null, bool nextMissionUnlocking = false)
    {
        this.lifePoints = lifePoints;
        this.missions = missions;
        this.availableMissions = availableMissions;
        this.painDiaryRecords = painDiaryRecords;
        this.nextMissionUnlocking = nextMissionUnlocking;
    }
}
