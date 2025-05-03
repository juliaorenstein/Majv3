using System.Linq;
using Fusion;
using UnityEngine;

namespace Resources
{
	public class CharlestonHandlerNetwork : NetworkBehaviour, ICharlestonHandlerNetwork
	{
		public CharlestonHandlerClient CharlestonHandlerClient;
		private ChangeDetector _changeDetector;
		
		[Networked] public int CharlestonVersion { get; set; }
		
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots0 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots1 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots2 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots3 { get; }
		
		[Networked, Capacity(4)] public NetworkArray<NetworkBool> PlayersReady { get; }

		private NetworkArray<NetworkBool>[] _occupiedSpots;
		public bool[][] OccupiedSpots => _occupiedSpots.Select(
			a => a.Select(b => (bool)b).ToArray()).ToArray();
		
		public override void Spawned()
		{
			_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
			_occupiedSpots = new[] {OccupiedSpots0, OccupiedSpots1, OccupiedSpots2, OccupiedSpots3};
		}
		
		private void Update()
		{
			if (_changeDetector.DetectChanges(this).Changed(nameof(CharlestonVersion)))
			{
				CharlestonHandlerClient.UpdateCharlestonState();
			}
		}

		public void SetOccupiedSpots(int playerIx, int spotIx, bool state)
		{
			_occupiedSpots[playerIx].Set(spotIx, state);
		}
	}
	
	public interface ICharlestonHandlerNetwork
	{
		int CharlestonVersion { get; set; }
		NetworkArray<NetworkBool> PlayersReady { get; }
		bool[][] OccupiedSpots { get; }
		public void SetOccupiedSpots(int playerIx, int spotIx, bool state);
	}
}