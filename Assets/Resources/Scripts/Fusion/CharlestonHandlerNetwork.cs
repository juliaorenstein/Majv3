using Fusion;
using UnityEngine;

namespace Resources
{
	public class CharlestonHandlerNetwork : NetworkBehaviour
	{
		public CharlestonHandlerClient CharlestonHandlerClient;
		private ChangeDetector _changeDetector;
		
		[Networked] public int CharlestonVersion { get; set; }
		
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots0 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots1 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots2 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots3 { get; }
		
		[Networked, Capacity(4)] public NetworkArray<NetworkBool> PlayersReady { get; }

		public NetworkArray<NetworkBool>[] OccupiedSpots;

		public override void Spawned()
		{
			_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
			OccupiedSpots = new[] {OccupiedSpots0, OccupiedSpots1, OccupiedSpots2, OccupiedSpots3};
		}
		
		private void Update()
		{
			if (_changeDetector.DetectChanges(this).Changed(nameof(CharlestonVersion)))
			{
				CharlestonHandlerClient.UpdateCharlestonState();
			}
		}
	}
}