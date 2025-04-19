using System;
using Fusion;

namespace Resources
{
	public class NetworkedGameState : NetworkBehaviour, INetworkedGameState
	{
        /* The server will have an instance of this class for each player, and will maintain the game state for each
         player via that player's instance. ReplicateTo will ensure that only the intended player will see this info.
         
         Each client (besides host) will get updates for just their own copy, which they'll use to track their game
         state.*/
        [Networked] public int PlayerId { get; set; }
        [Networked, Capacity(152)] private NetworkArray<CLoc> ClientGameStateNetArr => default;
        public CLoc[] ClientGameState => ClientGameStateNetArr.ToArray();
        [Networked, Capacity(4)] private NetworkArray<int> PrivateRackCountsNetArr => default;
        public int[] PrivateRackCounts => PrivateRackCountsNetArr.ToArray();
        private TileTrackerClient _tileTrackerClient;

        public override void Spawned()
        {
	        // To start, make sure nobody is getting these updates. Will be adjusted later in OnPlayerJoined
	        foreach (PlayerRef player in Runner.ActivePlayers)
	        {
		        ReplicateTo(player, false);
	        }
        }

        // TODO: is there a better way to do these methods, like a cast to NetworkArray?
        public void UpdateClientGameState(CLoc[] clientGameState)
        {
	        for (int i = 0; i < clientGameState.Length; i++)
	        {
		        ClientGameStateNetArr.Set(i, clientGameState[i]);
	        }
        }
        
        public void UpdatePrivateRackCounts(int[] privateRackCounts)
        {
	        for (int i = 0; i < privateRackCounts.Length; i++)
	        {
		       PrivateRackCountsNetArr.Set(i, privateRackCounts[i]);
	        }
        }

        public override void FixedUpdateNetwork()
        {
	        ChangeDetector changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
	        ChangeDetector.Enumerable changes = changeDetector.DetectChanges(this);
	        bool gameStateChanged = changes.Changed("ClientGameStateNetArr");
	        bool privateRacksChanged = changes.Changed("PrivateRackCounts");

	        if (gameStateChanged || privateRacksChanged)
	        {
		        // NEXT: Implement network inputs so that client can request changes from the server
		        _tileTrackerClient.UpdateGameState();
	        }
        }
    }	
	
	public interface INetworkedGameState
	{
		int PlayerId { get; set; }
		CLoc[] ClientGameState { get; }
		int[] PrivateRackCounts { get; }
		void UpdateClientGameState(CLoc[] clientGameState);
		void UpdatePrivateRackCounts(int[] privateRackCounts);
	}
}
