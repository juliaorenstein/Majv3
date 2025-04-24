using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Resources
{
	public class InClassName
	{
	}

	public class FusionEventHandler : MonoBehaviour, INetworkRunnerCallbacks
	{
		private FusionManagerGlobal _fusionManagerGlobal;
		private TurnManagerServer _turnManagerServer;
		private TileTrackerServer _tileTrackerServer;
		private InputSender _inputSender;
		
		// Called everywhere when a client joins
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("OnPlayerJoined");
			if (runner.IsServer && runner.LocalPlayer != player) AddPlayerToScene(runner, player);
		}

		private void AddPlayerToScene(NetworkRunner runner, PlayerRef player)
		{
			// get the fusion manager
			var fusionManagers = FindObjectsByType<FusionManagerGlobal>(FindObjectsSortMode.None);
			if (fusionManagers.Length > 1) throw new UnityException("More than one FusionManagerGlobal found");
			_fusionManagerGlobal = fusionManagers[0];
			// add player to the dictionary and send the game state to the player
			_fusionManagerGlobal.Players.Add(player.PlayerId, player);
			// Give the new player input authority over their network object
			NetworkObject thisPlayersNO = _fusionManagerGlobal.
				NetworkedGameStates[_fusionManagerGlobal.PlayerIx(player.PlayerId)].GetComponent<NetworkObject>();
			thisPlayersNO.AssignInputAuthority(player);
			runner.SetPlayerAlwaysInterested(player, thisPlayersNO, true);
			Debug.Log("Assigning player to Networked Game State");
			thisPlayersNO.GetComponent<NetworkedGameState>().Player = player;
		}
		
		// Called on client when connecting to server
		public void OnConnectedToServer(NetworkRunner runner)
		{
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
			/* Server: set up network objects and tile tracker */
			if (runner.IsServer)
			{
				// spawn the network object, which contains all NetworkBehaviours
				GameObject fusionManagerPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/FusionManager");
				NetworkObject fusionManagerNetworkObject = runner.Spawn(fusionManagerPrefab);
				fusionManagerNetworkObject.name = "FusionManager";
				_fusionManagerGlobal = fusionManagerNetworkObject.GetComponent<FusionManagerGlobal>();
				
				// tileTracker
				new SetupServer().StartGame(_fusionManagerGlobal, out _tileTrackerServer, out _turnManagerServer);
				
				// Run the contents of OnPlayerJoined for host here, because it was logged too early
				AddPlayerToScene(runner, runner.LocalPlayer);
			}
			
			// Everyone: Set up the rest of the UI
			SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
			int playerIx = _fusionManagerGlobal.PlayerIx(runner.LocalPlayer);
			setupMono.StartGame(playerIx, out _inputSender);
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