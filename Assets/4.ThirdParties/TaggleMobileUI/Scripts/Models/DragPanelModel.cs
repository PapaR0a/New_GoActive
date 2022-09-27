using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
namespace SevenMins
{

	[Serializable]
	public class DragPnlData{
		[JsonProperty(PropertyName = "title")]
		public string Title;
		[JsonProperty(PropertyName = "style")]
		public bool Style;
		[JsonProperty(PropertyName = "items")]
		public List<DragPnlItem> Items;
	}


	[Serializable]
	public class DragPnlItem{
		[JsonProperty(PropertyName = "style")]
		public int Style;
		[JsonProperty(PropertyName = "content")]
		public string Content;
	}
}
