using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Resources
{
	public class FusionEventHandler : MonoBehaviour, INetworkRunnerCallbacks
	{
		private FusionManagerServer _fusionManagerServer;
		private FusionManagerClient _fusionManagerClient;
		private RpcC2SHandler _rpcC2SHandler;
		private RpcS2CHandler _rpcS2CHandler;
		private TileTrackerServer _tileTrackerServer;

		// Called on server when a client joins
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("OnPlayerJoined");
			// on the first connection (host player to its own server), set up the Network Object and start the game
			if (player == runner.LocalPlayer)
			{
				SetupNetworkObject(runner);
				_tileTrackerServer = new SetupServer(this).StartGame(_fusionManagerServer);
				
				// set up the rest of the UI and start the game
				SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
				TileTrackerClient tileTrackerClient = setupMono.StartGame();
			
				// update the game state on the client side to start the game!
				tileTrackerClient.UpdateGameState();
			}
			
			// every time a player joins, add it to the dictionary and send the game state to the player
			_fusionManagerServer.Players.Add(player.PlayerId, player);
		}
		
		// Called on client when connecting to server
		public void OnConnectedToServer(NetworkRunner runner)
		{
			Debug.Log("OnConnectedToServer");
			// all clients besides the host set up their network object at this point
			if (!runner.IsServer)
			{
				SetupNetworkObject(runner);
			}
			
			// set up the rest of the UI and start the game
			SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
			TileTrackerClient tileTrackerClient = setupMono.StartGame();
			
			// update the game state on the client side to start the game!
			tileTrackerClient.UpdateGameState();
		}

		private void SetupNetworkObject(NetworkRunner runner)
		{
			// find the prefab
			GameObject fusionManagerPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/FusionManager");
			// spawn the network object, which contains all NetworkBehaviours
			NetworkObject fusionManagerNetworkObject = runner.Spawn(fusionManagerPrefab);
			fusionManagerNetworkObject.name = "FusionManager";
			// create the fusionManagerServer, which is not a NetworkBehaviour but does fusion stuff for the server
			_fusionManagerServer = fusionManagerNetworkObject.GetComponent<FusionManagerServer>();
			// find the fusionManagerClient NetworkBehavior
			_fusionManagerClient = fusionManagerNetworkObject.GetComponent<FusionManagerClient>();
			// get all the NetworkedGameState components. There are 4, one for each player
			NetworkedGameState[] networkedGameStates = fusionManagerNetworkObject.GetComponents<NetworkedGameState>();

			// the server sets references to NetworkedGameState components on the fusionManagerServer
			if (runner.IsServer)
			{
				Debug.Assert(networkedGameStates.Length == 4);
				_fusionManagerServer.NetworkedGameStates = networkedGameStates;
				for (int i = 0; i < networkedGameStates.Length; ++i)
				{
					networkedGameStates[i].PlayerId = i;
				}
			}
			
			// The client's fusionManagerClient gets a reference to the only NetworkedGameState instance they care about.
			_fusionManagerClient.GameState = networkedGameStates[runner.LocalPlayer.PlayerId];
		}

		// Called on server when a client leaves
		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			
		}

		// Called on server when a client sends input
		public void OnInput(NetworkRunner runner, NetworkInput input)
		{
			
		}

		// Called on client when disconnecting from server
		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			throw new NotImplementedException();
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
			
		}
	}
}