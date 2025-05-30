using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Resources
{
	public class CharlestonHandlerNetwork : NetworkBehaviour, ICharlestonHandlerNetwork
	{
		public CharlestonHandlerClient CharlestonHandlerClient;
		private CharlestonUIHandlerMono _charlestonUI;
		private ChangeDetector _changeDetector;
		
		[Networked] public int CharlestonVersion { get; set; }
		[Networked] public int PassNum { get; set; }
		
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots0 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots1 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots2 { get; }
		[Networked, Capacity(3)] public NetworkArray<NetworkBool> OccupiedSpots3 { get; }
		
		[Networked, Capacity(4)] public NetworkArray<NetworkBool> PlayersReadyNetworked { get; }
		public bool[] PlayersReady => PlayersReadyNetworked.Select(b => (bool)b).ToArray();
		
		public int[] PassDir { get; } = { 1, 0, -1, -1, 0, 1, 0 };
		public int[] PartialPasses { get; } = { 2, 5, 6 };
		public bool IsPartialPass => PartialPasses.Contains(PassNum);

		private NetworkArray<NetworkBool>[] _occupiedSpots;
		public bool[][] OccupiedSpots => _occupiedSpots.Select(
			a => a.Select(b => (bool)b).ToArray()).ToArray();
		
		public override void Spawned()
		{
			_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
			_occupiedSpots = new[] {OccupiedSpots0, OccupiedSpots1, OccupiedSpots2, OccupiedSpots3};
			_charlestonUI = GameObject.Find("GameManager").GetComponent<CharlestonUIHandlerMono>();
		}
		
		private void Update()
		{
			foreach (string change in _changeDetector.DetectChanges(this))
			{
				switch (change)
				{
					case nameof(PassNum):
						CharlestonHandlerClient.InputSender.ClearInput();
						int dir = PassDir[PassNum - 1];
						int nextDir = -2; // -2 indicates end of passing
						if (PassNum < PassDir.Length) nextDir = PassDir[PassNum];
						_charlestonUI.DoPass(dir, nextDir);
						break;
					case nameof(CharlestonVersion):
						CharlestonHandlerClient.UpdateCharlestonState();
						break;
				}
			}
		}

		public void SetOccupiedSpots(int playerIx, int spotIx, bool state) => _occupiedSpots[playerIx].Set(spotIx, state);
		public void SetPlayerReadyState(int playerIx, bool state) => PlayersReadyNetworked.Set(playerIx, state);
	}
	
	public interface ICharlestonHandlerNetwork
	{
		int CharlestonVersion { get; set; }
		public bool[] PlayersReady { get; }
		public int PassNum { get; set; }
		public int[] PassDir { get; }
		public int[] PartialPasses { get; }
		public bool IsPartialPass { get; }
		bool[][] OccupiedSpots { get; }
		public void SetOccupiedSpots(int playerIx, int spotIx, bool state);
		public void SetPlayerReadyState(int playerIx, bool state);
	}
}