using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWLoadFirstView : MonoBehaviour {
	void Start ()
	{
		Application.runInBackground = true;//run in background
	    Screen.sleepTimeout = SleepTimeout.NeverSleep;//never sleep screen
		Application.targetFrameRate = 60;

        //BWControl.Api.Init(); // TEMP AND TO BE REMOVED ONCE SEGREGATION IS COMPLETED
		GAControl.Api.Init();
	}
}
