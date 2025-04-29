using System;
using Fusion;
using System.Linq;

namespace Resources
{
	public class FusionManagerGlobal : NetworkBehaviour, IFusionManagerGlobal
	{
		[Networked, Capacity(4)] public NetworkArray<PlayerRef> Players { get; }
		[Networked] public int TurnPlayerIx { get; set; }
		[Networked] public int ExposingPlayerIx { get; set; }
		[Networked] public int numTilesExposedThisTurn { get; set; }
		[Networked] public int DiscardTileId { get; set; }
		[Networked] public TurnStage CurrentTurnStage { get; set; }
		
		private int[] PlayerIds => Players.Select(player => player.PlayerId).ToArray();
		public int LocalPlayerIx => PlayerIx(Runner.LocalPlayer);
		public bool IsMyTurn => PlayerIx(Runner.LocalPlayer) == TurnPlayerIx;
		public bool IsMyExpose => PlayerIx(Runner.LocalPlayer) == ExposingPlayerIx;
		
		public int PlayerIx(int playerId) => Array.IndexOf(PlayerIds, playerId);
		public int PlayerIx(PlayerRef playerRef) => Array.IndexOf(PlayerIds, playerRef.PlayerId);
		public int PlayerCount => Players.ToArray().Count(player => player != PlayerRef.None);
		
		public INetworkedGameState[] NetworkedGameStates { get; set; }
		public TurnManagerServer TurnManagerServer;
		

		public override void Spawned()
		{
			// initialize NetworkedGameState
			NetworkedGameStates = transform.GetComponentsInChildren<NetworkedGameState>().ToArray();
			CurrentTurnStage = TurnStage.Charleston;
			TurnPlayerIx = 1;
			ExposingPlayerIx = -1;
			numTilesExposedThisTurn = 0;
		}
	}

	public enum TurnStage
	{
		Charleston,
		PickUp,
		Discard,
		Call,
		Expose
	}
	
	public interface IFusionManagerGlobal
	{
		public NetworkArray<PlayerRef> Players { get; }
		public int TurnPlayerIx { get; }
		public int LocalPlayerIx { get; }
		public int DiscardTileId { get; }
		public bool IsMyTurn { get; }
		public bool IsMyExpose { get; }
		int PlayerIx(int playerId);
		int PlayerIx(PlayerRef playerRef);
		INetworkedGameState[] NetworkedGameStates { get; set; }
		int PlayerCount { get; }
		public TurnStage CurrentTurnStage { get; }
	}
}