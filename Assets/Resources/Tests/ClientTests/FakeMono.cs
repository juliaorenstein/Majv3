using Resources;

namespace Resources.ClientTests
{
	public class FakeMono : IMono
	{
		public void MoveTile(int tileId, CLoc loc, int ix) {}
		public void UpdatePrivateRackCount(CLoc privateRack, int count) {}
		public void SetActionButton(Action action, bool state) {}
		public void AddSpaceToDisplayRack(CLoc loc)
		{
			
		}
	}
}