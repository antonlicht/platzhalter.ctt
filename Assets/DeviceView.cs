using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DeviceView : MonoBehaviour 
{
	public event Action<HostData> Tapped = delegate {};
	public Text NameLabel;
	public CTTTap TapHandler;

	public void SetDevice(HostData data)
	{
		NameLabel.text = data.gameName;
		TapHandler.Tap += () => Tapped(data);
	}
}
