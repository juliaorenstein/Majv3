using Fusion;

namespace Resources
{
	public class FakeCharlestonHandlerNetwork : ICharlestonHandlerNetwork
	{
		public int CharlestonVersion { get; set; }
		public NetworkArray<NetworkBool> PlayersReady { get; }
		public int PassNum { get; set; }
		public int[] PassDir { get; }
		public int[] PartialPasses { get; }
		public bool IsPartialPass { get; }
		public bool[][] OccupiedSpots { get; } = {
			new[] {false, false, false },
			new[] {false, false, false },
			new[] {false, false, false },
			new[] {false, false, false }
		};

		public void UpdateCharlestonState()
		{
		}
		
		public void SetOccupiedSpots(int playerIx, int spotIx, bool state)
		{
			OccupiedSpots[playerIx][spotIx] = state;
		}
	}
}