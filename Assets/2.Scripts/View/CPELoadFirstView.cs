using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPELoadFirstView : MonoBehaviour {
	void Start ()
	{
		Application.runInBackground = true;//run in background
	    Screen.sleepTimeout = SleepTimeout.NeverSleep;//never sleep screen
		Application.targetFrameRate = 30;
		
        CPEControl.Api.Init();
	}
}
