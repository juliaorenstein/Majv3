using System;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
	// buttons are also updated in TileTrackerClient (should probably be here but more setup work)
	public class ButtonHandlerMono : MonoBehaviour
	{
		public InputSender InputSender;
		public Button pickUpButton;
		public Button callButton;
		public Button confirmButton;
		public Button cancelButton;

		public void RequestPickUp()
		{
			InputSender.RequestPickUp();
			pickUpButton.interactable = false;
		}

		public void RequestCall()
		{
			InputSender.RequestCall();
			callButton.interactable = false;
		}

		public void RequestConfirm()
		{
			InputSender.RequestConfirm();
			confirmButton.interactable = false;
			cancelButton.interactable = false;
		}

		public void RequestCancel()
		{
			InputSender.RequestPass();
			confirmButton.interactable = false;
			cancelButton.interactable = false;
		}
	}
}
