using Fusion;
using UnityEngine;

namespace Resources
{
	public struct Input : INetworkInput
	{
		public NetworkButtons Action;
		public int TileId;
	}
	
	public enum Action
	{
		Discard = 0,
		PickUp = 1,
		Call = 2,
		Wait = 3,
		Pass = 4,
		NeverMind = 5,
		JokerSwap = 6
	}
	
	public class InputSender
	{
		public Input Input;

		public void RequestDiscard(int tileId)
		{
			Debug.Log($"InputSender: Requesting discard for tile {tileId}");
			Input.Action.SetDown(Action.Discard);
			Input.TileId = tileId;
		}

		public void RequestPickUp()
		{
			Debug.Log("InputSender: Requesting pick up");
			Input.Action.SetDown(Action.PickUp);
		}

		public void ClearInput()
		{
			//Debug.Log("InputSender: Clearing input");
			Input.TileId = -1;
			Input.Action.SetAllUp();
		}
	}
}