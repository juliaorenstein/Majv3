using Fusion;
using UnityEngine;

namespace Resources
{
	public class FusionManagerClient : NetworkBehaviour
	{
		public int PlayerId;
		
		public override void Spawned()
		{
			PlayerId = Runner.LocalPlayer.PlayerId;
		}
	}
}