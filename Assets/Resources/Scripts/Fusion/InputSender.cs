using Fusion;
using UnityEngine;

namespace Resources
{
	public struct Input : INetworkInput
	{
		public NetworkButtons Action;
		public int TileId;
		public int SpotIx;
		public bool StartGame;
		public int charleston1;
		public int charleston2;
		public int charleston3;
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
		CharlestonUpdate = 7,
		CharlestonPass = 8,
		SkipCharlestons = 9
	}
	
	public class InputSender
	{
		public Input Input;

		public void RequestCharlestonUpdate(int[] tilesInCharlesotn)
		{
			Debug.Log($"InputSender: Requesting charleston update: {tilesInCharlesotn[0]}, {tilesInCharlesotn[1]}, {tilesInCharlesotn[2]}");
			Input.charleston1 = tilesInCharlesotn[0];
			Input.charleston2 = tilesInCharlesotn[1];
			Input.charleston3 = tilesInCharlesotn[2];
			Input.Action.SetDown(Action.CharlestonUpdate);
		}
		
		public void RequestCharlestonPass()
		{
			Debug.Log("InputSender: Requesting charleston pass.");
			Input.Action.SetDown(Action.CharlestonPass);
		}
		
		public void RequestSkipCharlestons()
		{
			Debug.Log("InputSender: Requesting skip charlestons.");
			Input.Action.SetDown(Action.SkipCharlestons);
		}

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
			// Debug.Log("InputSender: Clearing input");
			Input.Action.SetAllUp();
			Input.TileId = -1;
			Input.SpotIx = -1;
		}
	}
}