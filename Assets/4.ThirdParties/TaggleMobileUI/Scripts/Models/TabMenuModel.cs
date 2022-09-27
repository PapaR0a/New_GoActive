using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Taggle.HealthApp.Components;
using UnityEngine.Events;

namespace SevenMins
{
	[System.Serializable]
	public class TabMenuItem{
		public string title;
		public Sprite icon;
		public PanelCommon pnlContent;

		public UnityEvent OnClick = new UnityEvent();
	}
}
