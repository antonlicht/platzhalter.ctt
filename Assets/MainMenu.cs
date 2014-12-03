using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour 
{
	public GameObject StartGame;
	public GameObject FindDevices;
	public GameObject Device;
	public GameObject ConnectToDevice;
	public GameObject Cancel;
	public GameObject IncomingRequest;

	public float ScrollThreshold = 0.15f;
	
	private RectTransform _startGame;
	private RectTransform _findDevices;
	private RectTransform _device;
	private RectTransform _connectToDevice;
	private RectTransform _cancel;
	private RectTransform _incomingRequest;

	private CTTScrollpanel _scrollpanel;
	private CTTTouchInput _touchInput;

	private Dictionary<string, RectTransform> _devices;

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

	void Start()
	{
		_scrollpanel = GetComponent<CTTScrollpanel>();
		_touchInput = GetComponent<CTTTouchInput>();

		BuildList ();
		RegisterEvents();

		NetworkManager.Instance.OnHostlistRecieved += AddHostList;
	}

	public void BuildList()
	{
		//Start in Unplugged Mode and Find Devices
		var go = (GameObject)Instantiate(StartGame);
		_startGame = go.GetComponent<RectTransform>();
		_startGame.GetComponentInChildren<CTTTap>().Tap += HandleStartGame;
		_scrollpanel.AddElement(_startGame);


		go = (GameObject)Instantiate(FindDevices);
		_findDevices = go.GetComponent<RectTransform>();
		_scrollpanel.AddElementAfter(_findDevices, _startGame);
		_findDevices.GetComponentInChildren<CTTTap>().Tap += HandleFindDevices;
		_scrollpanel.SelectImmediately(_startGame);
	}

	private void HandleStartGame ()
	{
		StartCoroutine(SwitchToStartGameCoroutine());
	}
	private IEnumerator SwitchToStartGameCoroutine()
	{
		yield return new WaitForSeconds(1f);
		MainView.Instance.StartTransition(MainView.View.StartGame).AddListener(() =>NetworkManager.Instance.CreateGame());
	}


	private void HandleFindDevices ()
	{
		NetworkManager.Instance.RequestHosts();
	}



	public void AddHostList(HostData[] hosts)
	{
		var hostKeys = Devices.Keys.ToList();
		for(int i = hostKeys.Count-1; i>=0;i--)
		{
			var key = hostKeys[i];
			if(!hosts.Any(h => h.gameName == key))
			{
				var rectTrans = Devices[key];
				Devices.Remove(key);
				//Remove Devices in View
				Destroy (rectTrans.gameObject);
			}
		}
		foreach(var host in hosts.Where(h => !Devices.ContainsKey(h.gameName)))
		{
			var go = (GameObject)Instantiate(Device);
			var rectTrans = go.GetComponent<RectTransform>();
			Devices.Add(host.gameName,rectTrans);
			go.GetComponent<DeviceView>().SetDevice(host);
			_scrollpanel.AddElementBefore(rectTrans,_findDevices);
		}
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
