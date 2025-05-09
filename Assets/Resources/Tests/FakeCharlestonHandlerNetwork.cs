using Fusion;

namespace Resources
{
	public class FakeCharlestonHandlerNetwork : ICharlestonHandlerNetwork
	{
		public int CharlestonVersion { get; set; }
		public bool[] PlayersReady { get; private set; } = { false, false, false, false };
		public int PassNum { get; set; }
		public int[] PassDir { get; } = { 1, 0, -1, -1, 0, 1, 0 };
		public int[] PartialPasses { get; } = { 2, 5, 6 };
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

		public void SetPlayerReadyState(int playerIx, bool state)
		{
			PlayersReady[playerIx] = state;
		}
	}
}