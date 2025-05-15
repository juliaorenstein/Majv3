using System.Collections.Generic;
using Resources;

namespace Resources.ClientTests
{
	public class FakeIuiHandler : IUIHandler
	{
		public void MoveTile(int tileId, CLoc loc, int ix) {}
		public void UpdatePrivateRackCount(CLoc privateRack, int count) {}
		public void SetActionButton(Action action, bool state) {}
		public void AddSpaceToDisplayRack(CLoc loc)
		{
			
		}

		public void EndGame(int winnerIx, List<int>[] racks)
		{
			throw new System.NotImplementedException();
		}

		public void EndGame(int winnerIx)
		{
			
		}

		public void EndGame()
		{
			
		}
	}
}