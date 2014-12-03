using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public delegate void ServerEventHandler();
	public delegate void HostListEventHandler(HostData[] hosts);
	
	public const string GameType = "CTTSession";
	public const int Port = 23466;

	private static NetworkManager _instance;
	
	public event ServerEventHandler OnGameCreated;
	public event ServerEventHandler OnCreateGameFailed;
	public event ServerEventHandler OnMSConnectionFailed;
	public event ServerEventHandler OnConnectionFailed;
	public event ServerEventHandler OnConnected;
	public event ServerEventHandler OnDisconnected;
	public event HostListEventHandler OnHostlistRecieved;
	
	public static NetworkManager Instance 
	{ 
		get 
		{ 
			if(_instance == null) 
			{
				var go = new GameObject("NetworkManager");
				_instance = go.AddComponent<NetworkManager>();
			}
			return _instance;
		}
	}
	
	
	public void CreateGame()
	{
		Debug.Log("Initialize Server");
		Network.InitializeServer(64, Port, !Network.HavePublicAddress());
		Debug.Log("Register Host on MaserServer");
		MasterServer.RegisterHost(GameType, SystemInfo.deviceName);
		Debug.Log("Game Created");
	}
	
	public void RequestHosts()
	{
		MasterServer.RequestHostList(GameType);
		Debug.Log("Request Hosts");
	}
	
	public void ConnectToHost(HostData host)
	{
		Network.Connect(host);
		Debug.Log("Connect To Host");
	}
	
	
	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		switch (msEvent)
		{
		case (MasterServerEvent.HostListReceived):
			Debug.Log("Hostlist recieved!");
			if (OnHostlistRecieved != null)
			{
				Debug.Log("Start Poll Hostlist");
				OnHostlistRecieved(MasterServer.PollHostList());
			}
			else
			{
				Debug.Log("Hostlist is null");
			}
			break;
		case (MasterServerEvent.RegistrationSucceeded):
			Debug.Log("Registration succeeded!");
			if (OnGameCreated != null)
				OnGameCreated();
			break;
		default:
			Debug.Log("Create Game Failed!");
			if (OnCreateGameFailed != null)
				OnCreateGameFailed();
			break;
		}
		
	}
	
	private void OnFailedToConnectToMasterServer()
	{
		Debug.Log("OnFailedToConnectToMasterServer");
		if (OnMSConnectionFailed != null)
			OnMSConnectionFailed();
	}
	
	private void OnFailedToConnect()
	{
		Debug.Log("OnFailedToConnect");
		if (OnConnectionFailed != null)
			OnConnectionFailed();
	}
	
	private void OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer");
		if (OnConnected != null)
			OnConnected();
	}
	
	private void OnDisconnectedFromServer()
	{
		Debug.Log("OnDisconnectedFromServer");
		if (OnDisconnected != null)
			OnDisconnected();
	}
	
	private void OnPlayerConnected()
	{
		Debug.Log("OnPlayerConnected");
	}
	
	private void OnPlayerDisconnected()
	{
		Debug.Log("OnPlayerDisconnected");
	}

	void OnDestroy()
	{
		Network.Disconnect();
	}
	
	
}

