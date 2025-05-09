using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
	// buttons are also updated in TileTrackerClient (should probably be here but more setup work)
	public class ButtonHandlerMono : MonoBehaviour
	{
		public InputSender InputSender;
		private UIHandlerMono _uiHandler;
		
		public Button startGame;
		public Button pickUpButton;
		public Button callButton;
		public Button confirmButton;
		public Button cancelButton;
		public Button charlestonPass;
		public Button skipCharlestons;
		
		private void Start()
		{
			startGame.gameObject.SetActive(false);
			_uiHandler = GameObject.Find("GameManager").GetComponent<UIHandlerMono>();
		}

		public void RequestCharlestonPass()
		{
			InputSender.RequestCharlestonPass();
			charlestonPass.interactable = false;
		}

		public void RequestSkipCharlestons()
		{
			InputSender.RequestSkipCharlestons();
			GameObject.Find("GameManager").GetComponent<CharlestonUIHandlerMono>().EndCharlestons();
			skipCharlestons.gameObject.SetActive(false);
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
