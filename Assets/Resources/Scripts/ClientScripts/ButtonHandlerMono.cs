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
		}
	}
}
