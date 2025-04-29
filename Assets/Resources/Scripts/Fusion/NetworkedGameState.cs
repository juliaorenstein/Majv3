using Fusion;
using UnityEngine;

namespace Resources
{
	public class NetworkedGameState : NetworkBehaviour, INetworkedGameState
	{
        /* The server will have an instance of this class for each player, and will maintain the game state for each
         player via that player's instance. ReplicateTo will ensure that only the intended player will see this info.
         
         Each client (besides host) will get updates for just their own copy, which they'll use to track their game
         state.*/
        [Networked] public int GameStateVersion { get; set; }
        private int _gameStateVersion;
        [Networked] public PlayerRef Player { get; set; }
        public int PlayerIx { get; private set; }
        [Networked, Capacity(152)] private NetworkArray<CLoc> ClientGameStateNetArr => default;
        public CLoc[] ClientGameState => ClientGameStateNetArr.ToArray();
        [Networked, Capacity(4)] private NetworkArray<int> PrivateRackCountsNetArr => default;
        public int[] PrivateRackCounts => PrivateRackCountsNetArr.ToArray();
        
        public TileTrackerClient TileTracker { get; set; }


        public override void Spawned()
        {
	        PlayerIx = transform.GetSiblingIndex();
	        // Setup UI once this is spawned
	        if (Player != Runner.LocalPlayer) return;
	        SetupMono setupMono = GameObject.Find("GameManager").GetComponent<SetupMono>();
	        setupMono.SetUp(this);
        }
      
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

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        public void RpcInvalidMoveNotification(PlayerRef player)
        {
	        Debug.Log("You did something wrong or there was a connection issue.");
	        // TODO: Make a pop-up for this
        }

        public override void FixedUpdateNetwork()
        {
	        if (TileTracker == null || _gameStateVersion == GameStateVersion) return;
	        TileTracker.UpdateGameState();
	        _gameStateVersion = GameStateVersion;
        }
    }	
	
	public interface INetworkedGameState
	{
		int PlayerIx { get; }
		CLoc[] ClientGameState { get; }
		int GameStateVersion { get; set; }
		int[] PrivateRackCounts { get; }
		void UpdateClientGameState(CLoc[] clientGameState);
		void UpdatePrivateRackCounts(int[] privateRackCounts);
		TileTrackerClient TileTracker { get; set; }
	}
}
