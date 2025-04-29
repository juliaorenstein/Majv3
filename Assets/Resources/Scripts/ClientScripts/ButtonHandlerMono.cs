using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
	// buttons are also updated in TileTrackerClient (should probably be here but more setup work)
	public class ButtonHandlerMono : MonoBehaviour
	{
		public InputSender InputSender;
		public TurnManagerServer TurnManager;
		
		public Button startGame;
		public Button pickUpButton;
		public Button callButton;
		public Button confirmButton;
		public Button cancelButton;
		public Button charlestonPass;
		private CharlestonPassArray charlestonPassArray;
		
		public void StartGame()
		{
			TurnManager.StartGame();
			startGame.gameObject.SetActive(false);
			charlestonPassArray = charlestonPass.GetComponent<CharlestonPassArray>();
		}

		public void RequestCharlestonPass()
		{
			InputSender.RequestCharlestonPass(charlestonPassArray.TilesToPass);
			charlestonPass.interactable = false;
		}
		
		public void RequestPickUp()
		{
			InputSender.RequestPickUp();
			pickUpButton.interactable = false;
		}

		public void RequestCall()
		{
			InputSender.RequestCall();
			callButton.interactable = false;
			confirmButton.interactable = true;
			cancelButton.interactable = true;
		}

		public void RequestConfirm()
		{
			InputSender.RequestConfirm();
			confirmButton.interactable = false;
			cancelButton.interactable = false;
		}

		public void RequestCancel()
		{
			InputSender.RequestCancel();
			confirmButton.interactable = false;
			cancelButton.interactable = false;
		}
	}
}
