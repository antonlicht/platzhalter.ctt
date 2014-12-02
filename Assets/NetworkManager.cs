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
		Network.InitializeServer(4, Port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GameType, SystemInfo.deviceName);
	}
	
	public void RequestHosts()
	{
		MasterServer.RequestHostList(GameType);
	}
	
	public void ConnectToHost(HostData host)
	{
		Network.Connect(host);
	}
	
	
	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		switch (msEvent)
		{
		case (MasterServerEvent.HostListReceived):
			if (OnHostlistRecieved != null)
				OnHostlistRecieved(MasterServer.PollHostList());
			break;
		case (MasterServerEvent.RegistrationSucceeded):
			if (OnGameCreated != null)
				OnGameCreated();
			break;
		default:
			if (OnCreateGameFailed != null)
				OnCreateGameFailed();
			break;
		}
		
	}
	
	private void OnFailedToConnectToMasterServer()
	{
		if (OnMSConnectionFailed != null)
			OnMSConnectionFailed();
	}
	
	private void OnFailedToConnect()
	{
		if (OnConnectionFailed != null)
			OnConnectionFailed();
	}
	
	private void OnConnectedToServer()
	{
		if (OnConnected != null)
			OnConnected();
	}
	
	private void OnDisconnectedFromServer()
	{
		if (OnDisconnected != null)
			OnDisconnected();
	}
	
	private void OnPlayerConnected()
	{
		
	}
	
	private void OnPlayerDisconnected()
	{
		
	}
	
	
	
}

