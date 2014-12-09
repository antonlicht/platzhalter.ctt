using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour 
{
	public GameObject StartGame;
	public GameObject Result;

	public float ScrollThreshold = 0.15f;
	
	private RectTransform _startGame;
	private RectTransform _result;

	private CTTScrollpanel _scrollpanel;
	private CTTTouchInput _touchInput;

	private Dictionary<string, RectTransform> _devices;

	public static MainMenu Instance { get; private set;}

	public Dictionary<string, RectTransform> Devices
	{
		get
		{
			if(_devices == null)
			{
				_devices = new Dictionary<string, RectTransform>();
			}
			return _devices;
		}
	}


	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		_scrollpanel = GetComponent<CTTScrollpanel>();
		_touchInput = GetComponent<CTTTouchInput>();

		BuildList ();
		RegisterEvents();
	}

	public void BuildList()
	{
		//Start in Unplugged Mode and Find Devices
		var go = (GameObject)Instantiate(StartGame);
		_startGame = go.GetComponent<RectTransform>();
		_startGame.GetComponentInChildren<CTTTap>().Tap += HandleStartGame;
		_scrollpanel.AddElement(_startGame);



	//	_findDevices = go.GetComponent<RectTransform>();
	//	_scrollpanel.AddElementAfter(_findDevices, _startGame);
	//	_findDevices.GetComponentInChildren<CTTTap>().Tap += HandleFindDevices;
	//	_scrollpanel.SelectImmediately(_startGame);
	}

	public void SwitchToResultScreen(float time)
	{
		var go = (GameObject)Instantiate(Result);
		_result = go.GetComponent<RectTransform>();
		_result.GetComponent<ResultView>().SetTime(time);
		_scrollpanel.AddElementAfter(_result, _startGame);
		_scrollpanel.SelectImmediately(_result);
		_scrollpanel.DeleteWhenUnselected.Add(_result);
		StartCoroutine(SwitchToResultScreenCoroutine());
	}

	public IEnumerator SwitchToResultScreenCoroutine()
	{
		yield return new WaitForSeconds(5f);
		MainView.Instance.StartTransition(MainView.View.MainMenu);
	}

	private void HandleStartGame ()
	{
		StartCoroutine(SwitchToStartGameCoroutine());
	}

	private IEnumerator SwitchToStartGameCoroutine()
	{
		GameManager.Instance.ResetGame();
		yield return new WaitForSeconds(1f);
		MainView.Instance.StartTransition(MainView.View.CoreGame).AddListener(() =>GameManager.Instance.StartGame());
	}

	#region Events
	public void RegisterEvents()
	{
		_touchInput.OnDragging += HandleDragging;
		_touchInput.OnDragEnd += HandleOnDragEnd;
	}

	void HandleOnDragEnd (PointerEventData obj)
	{
		if(_scrollpanel.ScrollState >= ScrollThreshold)
		{
			_scrollpanel.SelectNext(-1);
		}
		else if(_scrollpanel.ScrollState <= -ScrollThreshold)
		{
			_scrollpanel.SelectNext(1);
		}
		_scrollpanel.StartScroll();
	}


	private void HandleDragging(PointerEventData eventData)
	{
		_scrollpanel.SetRubberBandPosition(eventData.delta);
	}
	#endregion
}
