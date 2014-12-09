using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class MainView : MonoBehaviour 
{
	public enum View {MainMenu, CoreGame};

	public class Transition
	{
		internal Action _callback = delegate {};
		public void AddListener(Action callback)
		{
			_callback = callback;
		}
	}

	public AnimationCurve TransitionCurve = new AnimationCurve();
	public float TransitionTime = 1f;

	private View _currentView;
	private Dictionary<View, RectTransform> _views;

	public static MainView Instance { get; private set;}
	
	void Awake()
	{
		Instance = this;
		_views = new Dictionary<View, RectTransform>();
		foreach(View v in (View[]) Enum.GetValues(typeof(View)))
		{
			_views.Add(v, transform.FindChild(v.ToString()+"Panel").GetComponent<RectTransform>());
		}
	}

	public Transition StartTransition(View view, int direction = -1)
	{
		var oldPanel = _views[_currentView];
		var newPanel= _views[view];
		var transition = new Transition();
		StartCoroutine(TransitionCoroutine(view, oldPanel, oldPanel.GetComponent<CanvasGroup>(), newPanel, newPanel.GetComponent<CanvasGroup>(), direction, transition));
		return transition;
	}

	private IEnumerator TransitionCoroutine(View view, RectTransform oldPanel, CanvasGroup oldCanvasGroup, RectTransform newPanel, CanvasGroup newCanvasGroup, int direction, Transition transition)
	{
		oldCanvasGroup.interactable = false;
		oldCanvasGroup.blocksRaycasts = false;

		var pos = oldPanel.anchoredPosition;
		pos.y = -1f*Mathf.Sign(direction)*GetComponent<RectTransform>().rect.height;
		var tween = HOTween.To(oldPanel, TransitionTime, new TweenParms().Prop("anchoredPosition", pos));
		tween.easeAnimationCurve = TransitionCurve;

		pos = newPanel.anchoredPosition;
		pos.y = Mathf.Sign(direction)*GetComponent<RectTransform>().rect.height;
		newPanel.anchoredPosition = pos;
		pos = newPanel.anchoredPosition;
		pos.y = 0f;
		HOTween.To(newPanel, TransitionTime, new TweenParms().Prop("anchoredPosition", pos)).easeAnimationCurve = TransitionCurve;

		oldCanvasGroup.alpha = 1f;
		HOTween.To(oldCanvasGroup, TransitionTime, new TweenParms().Prop("alpha", 0f)).easeAnimationCurve = TransitionCurve;

		newCanvasGroup.alpha = 0f;
		HOTween.To(newCanvasGroup, TransitionTime, new TweenParms().Prop("alpha", 1f)).easeAnimationCurve = TransitionCurve;


		while(!tween.isComplete)
		{
			yield return null;
		}

		newCanvasGroup.interactable = true;
		newCanvasGroup.blocksRaycasts = true;
		_currentView = view;
		
		transition._callback();
	}
}
