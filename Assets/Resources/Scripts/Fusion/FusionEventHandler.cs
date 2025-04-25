using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Resources
{
	public class FusionEventHandler : MonoBehaviour, INetworkRunnerCallbacks
	{
		private FusionManagerGlobal _fusionManagerGlobal;
		public InputSender InputSender;
		
		// Called everywhere when a client joins
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("OnPlayerJoined");
			if (runner.IsServer && runner.LocalPlayer != player) AddPlayerToScene(runner, player);
		}

		private void AddPlayerToScene(NetworkRunner runner, PlayerRef player)
		{
			// add player to the Players array in first empty position
			_fusionManagerGlobal.Players.Set(Array.IndexOf(_fusionManagerGlobal.Players.ToArray(), default), player);
			// Give the new player input authority over their network object
			var tmpArr = Array.ConvertAll(
				_fusionManagerGlobal.NetworkedGameStates, x => (NetworkedGameState)x);
			NetworkObject thisPlayersNO = tmpArr[_fusionManagerGlobal.PlayerIx(player.PlayerId)].GetComponent<NetworkObject>();
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
			if (InputSender == null) return; // quit out if inputSender isn't initialized
			input.Set(InputSender.Input);
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
			Debug.Log("OnSceneLoadDone");
			/* Server: set up network objects and tile tracker */
			if (!runner.IsServer) return;
			// spawn the network object, which contains all NetworkBehaviours
			GameObject fusionManagerPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/FusionManager");
			NetworkObject fusionManagerNetworkObject = runner.Spawn(fusionManagerPrefab);
			fusionManagerNetworkObject.name = "FusionManager";
			_fusionManagerGlobal = fusionManagerNetworkObject.GetComponent<FusionManagerGlobal>();
			CallHandler callHandler = fusionManagerNetworkObject.GetComponent<CallHandler>();
				
			// tileTracker
			new SetupServer().StartGame(_fusionManagerGlobal, callHandler);
				
			// Run the contents of OnPlayerJoined for host here, because it was logged too early
			AddPlayerToScene(runner, runner.LocalPlayer);
			
			// Do mono stuff just for the server, since Networked Game State wasn't assigned yet
			SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
			NetworkedGameState myNetworkedGameState =
				_fusionManagerGlobal.GetComponentsInChildren<NetworkedGameState>()[0];
			setupMono.StartGame(myNetworkedGameState);
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

// TODO: write unit tests for turn manager