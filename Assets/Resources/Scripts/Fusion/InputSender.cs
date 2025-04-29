using Fusion;
using UnityEngine;

namespace Resources
{
	public struct Input : INetworkInput
	{
		public NetworkButtons Action;
		public int TileId;
		public int TileId2;
		public int TileId3;
	}
	
	public enum Action
	{
		Discard = 0,
		PickUp = 1,
		Call = 2,
		Confirm = 3,
		Cancel = 4,
		Expose = 5,
		JokerSwap = 6,
		CharlestonPass = 7
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

		public void RequestCharlestonPass(int[] tileIds)
		{
			Debug.Log($"InputSender: Requesting charleston pass for tiles {tileIds[0]}, {tileIds[1]}, {tileIds[2]}");
			Input.Action.SetDown(Action.CharlestonPass);
			Input.TileId = tileIds[0];
			Input.TileId2 = tileIds[1];
			Input.TileId3 = tileIds[2];
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

		public void RequestConfirm()
		{
			Debug.Log("InputSender: Requesting confirm");
			Input.Action.SetDown(Action.Confirm);
		}

		public void RequestCancel()
		{
			Debug.Log("InputSender: Requesting cancel");
			Input.Action.SetDown(Action.Cancel);
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