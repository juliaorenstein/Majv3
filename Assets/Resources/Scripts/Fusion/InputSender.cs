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
		Expose = 5,
		NeverMind = 6,
		JokerSwap = 7
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

		public void RequestCall()
		{
			Debug.Log("InputSender: Requesting call");
			Input.Action.SetDown(Action.Call);
		}

		public void RequestExpose(int tileId)
		{
			Debug.Log("InputSender: Requesting expose");
			Input.Action.SetDown(Action.Expose);
			Input.TileId = tileId;
		}

		public void ClearInput()
		{
			//Debug.Log("InputSender: Clearing input");
			Input.Action.SetAllUp();
			Input.TileId = -1;
		}
	}
}