using Fusion;

namespace Resources
{
	public class FakeFusionManagerGlobal : IFusionManagerGlobal
	{
		public int HumanPlayerCount => 4;
		public TurnStage CurrentTurnStage { get; set; }
		public bool MahJongg { get; }
		public NetworkArray<PlayerRef> Players { get; set; } = new();
		public int TurnPlayerIx { get; set; }
		public int LocalPlayerIx => 0;
		public int DiscardTileId => 0;
		public bool IsMyTurn => true;
		public bool IsMyExpose => false;
		public TurnManagerServer TurnManagerServer { get; set; }
		public int PlayerIx(int playerId) => 0;
		public int PlayerIx(PlayerRef playerRef) => 0;

		public INetworkedGameState[] NetworkedGameStates { get; set; } = { 
			new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
		};

		public void NextTurn()
		{
		}
	}
}