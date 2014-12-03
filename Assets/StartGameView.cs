using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StartGameView : MonoBehaviour 
{
	public GameObject WaitForDevices;
	public GameObject ConnectedDevices;
	public GameObject StartUnplugged;

	public float ScrollThreshold = 0.15f;

	private RectTransform _waitForDevices;
	private RectTransform _connectedDevices;
	private RectTransform _startUnplugged;

	private CTTScrollpanel _scrollpanel;
	private CTTTouchInput _touchInput;

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
		var go = (GameObject)Instantiate(StartUnplugged);
		_startUnplugged = go.GetComponent<RectTransform>();
		_startUnplugged.GetComponentInChildren<CTTTap>().Tap += HandleStartUnplugged;
		_scrollpanel.AddElement(_startUnplugged);
	}

	private void HandleStartUnplugged ()
	{
		StartCoroutine(SwitchToCoreGameCoroutine());
	}

	private IEnumerator SwitchToCoreGameCoroutine()
	{
		yield return new WaitForSeconds(1f);
		MainView.Instance.StartTransition(MainView.View.CoreGame).AddListener(GameManager.Instance.StartGame);
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
