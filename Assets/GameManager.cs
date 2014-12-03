using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Holoville.HOTween;

public class GameManager : MonoBehaviour 
{
	public enum State {Stopped, Normal, Fast}

	public Animation HeartAnimation;
	public HeartBeat Effects;
	public Text Beats;
	public Text TimeTextView;
	
	private float _life;
	private bool _gameRunning;
	private State _heartState;
	private float _startTime;

	public static GameManager Instance { get; private set;}

	void Awake()
	{
		Instance = this;
		_life = GameConstants.TOTAL_BEATS;
		Beats.text = Mathf.CeilToInt(_life).ToString();
	}

	public void StartGame()
	{
		_life = GameConstants.TOTAL_BEATS;
		_gameRunning = true;
		_startTime = Time.time;
	}

	private State HeartState
	{
		get { return _heartState; }
		set
		{
			if(value == State.Normal)
				Effects.Enabled = _life <= GameConstants.START_BEATING_AT;
			if(_heartState == value) return;
			_heartState = value;
			if(_heartState == State.Normal)
			{
				if(!HeartAnimation.isPlaying)
					HeartAnimation.Play();
				HeartAnimation["Beat"].speed = GameConstants.BEATS_PER_SECOND;
			}
			else if (_heartState == State.Fast)
			{
				if(!HeartAnimation.isPlaying)
					HeartAnimation.Play();
				HeartAnimation["Beat"].speed = (GameConstants.DECREASE_LIFE_SPEED/GameConstants.BEATS_PER_SECOND)/5f;
				Effects.Enabled = true;
			}
			else
			{
				StartCoroutine(StopAnimation());
			}

		}
	}

	void Update ()
	{
		if(!_gameRunning)
			return;

		if(Input.GetMouseButton(0))
		{
			_life -= Time.deltaTime*GameConstants.DECREASE_LIFE_SPEED;
			HeartState = State.Fast;
		}
		else
		{
			_life -= Time.deltaTime*GameConstants.BEATS_PER_SECOND;
			HeartState = State.Normal;
		}
		if(_life <= 0)
		{
			Debug.Log("GameOver");
			Vibration.Vibrate(5000);
			TimeTextView.text = (Time.time-_startTime).ToString();
			_gameRunning = false;
			HeartState = State.Stopped;
			
		}
		Beats.text = Mathf.CeilToInt(_life).ToString();
	}


	private IEnumerator StopAnimation()
	{
		while(HeartAnimation.transform.localScale.x != 1f)
		{
			yield return null;
		}
		HeartAnimation.animation.Stop();
	}

}
