using System;

namespace Resources
{
	public class FakeNetworkedGameState : INetworkedGameState
	{
		public int PlayerId { get; set; }
		public int TurnPlayerId { get; set; }
		public CLoc[] ClientGameState { get; set; }
		public int GameStateVersion { get; set; }
		public int[] PrivateRackCounts { get; set; } = { 14, 13, 13, 13 };
		public TileTrackerClient TileTracker { get; set; }

		public FakeNetworkedGameState()
		{
			ClientGameState = new CLoc[152];
			Array.Fill(ClientGameState, CLoc.Pool);

			// deal out private rack
			for (int tileId = 10; tileId < 44; tileId++)
			{
				ClientGameState[tileId] = CLoc.LocalPrivateRack;
			}
		}
		
		public void UpdateClientGameState(CLoc[] clientGameState)
		{
			ClientGameState = clientGameState;
		}

		public void UpdatePrivateRackCounts(int[] privateRackCounts)
		{
			PrivateRackCounts = privateRackCounts;	
		}
	}
}