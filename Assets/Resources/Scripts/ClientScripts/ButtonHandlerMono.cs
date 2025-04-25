using System;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
	public class ButtonHandlerMono : MonoBehaviour
	{
		public InputSender InputSender;
		public Button pickUpButton;
		public Button callButton;
		public Button waitButton;
		public Button passButton;
		public Button neverMindButton;

		public void RequestPickUp()
		{
			InputSender.RequestPickUp();
			pickUpButton.interactable = false;
		}

		public void RequestCall()
		{
			InputSender.RequestCall();
			callButton.interactable = false;
			waitButton.interactable = true;
			passButton.interactable = true;
		}

		public void RequestWait()
		{
			InputSender.RequestWait();
			waitButton.interactable = false;
			callButton.interactable = true;
			passButton.interactable = true;
		}

		public void RequestPass()
		{
			InputSender.RequestPass();
			passButton.interactable = false;
			callButton.interactable = true;
			waitButton.interactable = true;
		}

		public void RequestNeverMind()
		{
			InputSender.RequestNeverMind();
			neverMindButton.interactable = false;
		}
	}
}
