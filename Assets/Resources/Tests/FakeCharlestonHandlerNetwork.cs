using Fusion;

namespace Resources
{
	public class FakeCharlestonHandlerNetwork : ICharlestonHandlerNetwork
	{
		public int CharlestonVersion { get; set; }
		public NetworkArray<NetworkBool> PlayersReady { get; }
		public bool[][] OccupiedSpots { get; }
		
		public FakeCharlestonHandlerNetwork()
		{
			OccupiedSpots = new[]
			{
				new[] {false, false, false },
				new[] {false, false, false },
				new[] {false, false, false },
				new[] {false, false, false }
			};
		}
		public void UpdateCharlestonState()
		{
		}
		
		public void SetOccupiedSpots(int playerIx, int spotIx, bool state)
		{
			OccupiedSpots[playerIx][spotIx] = state;
		}
	}
}