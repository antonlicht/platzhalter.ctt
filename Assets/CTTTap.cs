﻿using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;
using Holoville.HOTween;

public class CTTTap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public event Action Tap = delegate {};

	public void OnPointerDown (PointerEventData eventData)
	{
		animation.Play();
	}

	public void StartBlocking()
	{
		CTTScrollpanel.BlockScrolling = true;
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		CTTScrollpanel.BlockScrolling = false;
		animation.Stop();
		FadeOut();
	}

	private void FadeOut()
	{
		HOTween.To(GetComponent<CanvasGroup>(),0.2f,new TweenParms().Prop("alpha",0f));
	}

	public void OnTap()
	{
		Debug.Log("Tap");
		CTTScrollpanel.BlockScrolling = false;
		Tap();
	}
}
