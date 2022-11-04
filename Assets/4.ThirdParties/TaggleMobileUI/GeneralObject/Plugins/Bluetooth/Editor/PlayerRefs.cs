using UnityEditor;
using UnityEngine;

public class PlayerRefs
{
    [MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}