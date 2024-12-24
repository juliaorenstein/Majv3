using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Resources
{
	public class FusionEventHandler : MonoBehaviour, INetworkRunnerCallbacks
	{
		public FusionManagerServer fusionManagerServer;
		private RpcC2SHandler _rpcC2SHandler;
		private RpcS2CHandler _rpcS2CHandler;
		private TileTrackerServer _tileTrackerServer;

		// Called on server when a client joins
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			
			// Only the host does the following step here. Everyone else does it in OnConnectedToServer
			if (player == runner.LocalPlayer)
			{
				SetupNetworkObject(runner);
				_tileTrackerServer = new SetupServer(this).StartGame(_rpcS2CHandler, _rpcC2SHandler);
			}
			
			// add player to Player Dictionary
			fusionManagerServer.Players.Add(player.PlayerId, player);
			_tileTrackerServer.SendGameStateToPlayer(player.PlayerId);
		}
		
		// Called on client when connecting to server
		public void OnConnectedToServer(NetworkRunner runner)
		{
			// if this is the server, executing this in OnPlayerJoined
			if (!runner.IsServer)
			{
				SetupNetworkObject(runner);
			}
			
			// Finish UI setup
			SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
			setupMono.StartGame(_rpcS2CHandler);
		}

		private void SetupNetworkObject(NetworkRunner runner)
		{
			// Set up new Network object
			GameObject fusionManagerPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/FusionManager");
			NetworkObject fusionManager = runner.Spawn(fusionManagerPrefab);
			FusionManagerClient fusionManagerClient = fusionManager.GetComponent<FusionManagerClient>();
			_rpcC2SHandler = fusionManager.GetComponent<RpcC2SHandler>();
			_rpcS2CHandler = fusionManager.GetComponent<RpcS2CHandler>();
			_rpcC2SHandler.fusionManagerClient = fusionManagerClient;
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