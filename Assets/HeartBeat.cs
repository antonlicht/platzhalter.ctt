﻿using UnityEngine;
using System.Collections;

public class HeartBeat : MonoBehaviour 
{
	public bool Enabled;
	public void Beat()
	{
		if(Enabled)
		{
			Handheld.Vibrate();
		}
	}
}
