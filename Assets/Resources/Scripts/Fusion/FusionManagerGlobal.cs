using System;
using Fusion;
using System.Linq;

namespace Resources
{
	public class FusionManagerGlobal : NetworkBehaviour, IFusionManagerGlobal
	{
		[Networked, Capacity(4)] public NetworkArray<PlayerRef> Players { get; }
		private int[] PlayerIds => Players.Select(player => player.PlayerId).ToArray();
		[Networked] public int TurnPlayerIx { get; set; }
		public int LocalPlayerIx => PlayerIx(Runner.LocalPlayer);
		public bool IsMyTurn => PlayerIx(Runner.LocalPlayer) == TurnPlayerIx;
		public int PlayerIx(int playerId) => Array.IndexOf(PlayerIds, playerId);
		public int PlayerIx(PlayerRef playerRef) => Array.IndexOf(PlayerIds, playerRef.PlayerId);
		public INetworkedGameState[] NetworkedGameStates { get; set; }
		public int PlayerCount => Players.Length;
		public TileTrackerServer TileTrackerServer;
		public TileTrackerClient TileTrackerClient;
		public TurnManagerServer TurnManagerServer;
		

		public override void Spawned()
		{
			// intialize NetworkedGameState
			NetworkedGameStates = transform.GetComponentsInChildren<NetworkedGameState>().ToArray();
			TurnPlayerIx = 1;
		}
	}
	
	public interface IFusionManagerGlobal
	{
		public NetworkArray<PlayerRef> Players { get; }
		public int TurnPlayerIx { get; }
		public int LocalPlayerIx { get; }
		public bool IsMyTurn { get; }
		int PlayerIx(int playerId);
		int PlayerIx(PlayerRef playerRef);
		INetworkedGameState[] NetworkedGameStates { get; set; }
		int PlayerCount { get; }
	}
}