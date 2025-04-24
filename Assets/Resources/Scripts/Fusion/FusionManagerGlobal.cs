using System;
using System.Collections.Generic;
using Fusion;
using System.Linq;
using UnityEngine;

namespace Resources
{
	public class FusionManagerGlobal : NetworkBehaviour, IFusionManagerGlobal
	{
		public Dictionary<int, PlayerRef> Players { get; } = new();
		private int[] PlayerIds => Players.Keys.ToArray();
		public int PlayerIx(int playerId) => Array.IndexOf(PlayerIds, playerId);
		public int PlayerIx(PlayerRef playerRef) => Array.IndexOf(PlayerIds, playerRef.PlayerId);
		public INetworkedGameState[] NetworkedGameStates { get; set; }
		public int PlayerCount => Players.Count;
		public TileTrackerServer TileTrackerServer;
		public TileTrackerClient TileTrackerClient;
		public TurnManagerServer TurnManagerServer;

		public override void Spawned()
		{
			// intialize NetworkedGameStates
			NetworkedGameStates = transform.GetComponentsInChildren<NetworkedGameState>().ToArray();
		}
	}
	
	public interface IFusionManagerGlobal
	{
		public Dictionary<int, PlayerRef> Players { get; }
		int PlayerIx(int playerId);
		int PlayerIx(PlayerRef playerRef);
		INetworkedGameState[] NetworkedGameStates { get; set; }
		int PlayerCount { get; }
	}
}