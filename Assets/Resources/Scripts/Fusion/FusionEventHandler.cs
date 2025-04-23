using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Resources
{
	public class FusionEventHandler : MonoBehaviour, INetworkRunnerCallbacks
	{
		private FusionManagerServer _fusionManagerServer;
		private TurnManagerServer _turnManagerServer;
		private NetworkedGameState[] _allNetworkedGameStates;
		private TileTrackerServer _tileTrackerServer;
		private InputSender _inputSender;

		public void OnDisable()
		{
			Debug.Log("OnDisable");
			GetComponent<NetworkRunner>().RemoveCallbacks(this);
		}
		
		// Called on server and client when a client joins
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("OnPlayerJoined");
			GetComponent<NetworkRunner>().AddCallbacks(this);
			// on the first connection (host player to its own server), set up the Network Object and start the game
			if (!(runner.IsServer && player == runner.LocalPlayer)) return;
			// set up the rest of the UI and start the game
			SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
			setupMono.StartGame(player.PlayerId, out _inputSender);
			// Do network object stuff
			Transform fusionTransform = SetupNetworkObject(runner);
			new SetupServer().StartGame(_fusionManagerServer, out _tileTrackerServer, out _turnManagerServer);
			// set TurnManagerServer on all the input receivers
			foreach (Transform child in fusionTransform)
			{
				child.GetComponent<InputReceiver>().TurnManager = _turnManagerServer;
			}
			// every time a player joins, add it to the dictionary and send the game state to the player
			_fusionManagerServer.Players.Add(player.PlayerId, player);
			// Give the new player input authority over their network object
			_allNetworkedGameStates[player.PlayerId].GetComponent<NetworkObject>().AssignInputAuthority(player);
		}
		
		// Called on client when connecting to server
		public void OnConnectedToServer(NetworkRunner runner)
		{
			Debug.Log("OnConnectedToServer");
			// set up the rest of the UI and start the game
			SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
			setupMono.StartGame(runner.LocalPlayer.PlayerId, out _inputSender);
		}

		private Transform SetupNetworkObject(NetworkRunner runner)
		{
			// find the prefab
			GameObject fusionManagerPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/FusionManager");
			// spawn or find the network object, which contains all NetworkBehaviours
			NetworkObject fusionManagerNetworkObject = runner.Spawn(fusionManagerPrefab);
			fusionManagerNetworkObject.name = "FusionManager";
			// create the fusionManagerServer, which is not a NetworkBehaviour but does fusion stuff for the server
			_fusionManagerServer = fusionManagerNetworkObject.GetComponent<FusionManagerServer>();
			// get all the GameState components. There are 4, one for each player
			_allNetworkedGameStates = fusionManagerNetworkObject.GetComponentsInChildren<NetworkedGameState>();

			if (!runner.IsServer) return fusionManagerNetworkObject.transform;
			// the server sets references to GameState components on the fusionManagerServer
			_fusionManagerServer.NetworkedGameStates = _allNetworkedGameStates;
			
			return fusionManagerNetworkObject.transform;
		}

		// Called on server when a client leaves
		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			
		}

		// Called on server when a client sends input
		public void OnInput(NetworkRunner runner, NetworkInput input)
		{
			if (_inputSender == null) return; // quit out if inputSender isn't initialized
			input.Set(_inputSender.Input);
		}

		// Called on client when disconnecting from server
		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
		}

		// Called on client when failing to connect to server
		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			throw new NotImplementedException();
		}
		
		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
		{
			
		}
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
			
		}
		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
		{
			
		}
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
		{
			
		}
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
		{
			
		}
		public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{
			
		}
		public void OnSceneLoadDone(NetworkRunner runner)
		{

		}

		private IEnumerator WaitForFusionManager(NetworkRunner runner)
		{
			NetworkObject fusionManager = null;
			while (fusionManager == null) {
				fusionManager = FindObjectsByType<NetworkObject>(FindObjectsSortMode.None)[0];  // Find it dynamically
				yield return null;
			}
		}
		
		public void OnSceneLoadStart(NetworkRunner runner)
		{
			
		}
		public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}
		public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}
		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
		{
		}
		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
			
		}
		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
			Debug.Log("OnConnectRequest");
			request.Accept();
		}
	}
}