using System;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
	public class ButtonHandlerMono : MonoBehaviour
	{
		public InputSender InputSender;
		public TurnManagerServer TurnManagerServer;
		
		public Button pickUpButton;
		public Button callButton;
		public Button waitButton;
		public Button passButton;
		public Button neverMindButton;
		public Button startGameButton;

		public void RequestPickUp()
		{
			pickUpButton.interactable = false;
			
			InputSender.RequestPickUp();
		}

		public void RequestCall()
		{
			callButton.interactable = false;
			waitButton.interactable = true;
			passButton.interactable = true;
			
			InputSender.RequestCall();
		}

		public void RequestWait()
		{
			waitButton.interactable = false;
			callButton.interactable = true;
			passButton.interactable = true;
			
			InputSender.RequestWait();
		}

		public void RequestPass()
		{
			passButton.interactable = false;
			callButton.interactable = true;
			waitButton.interactable = true;
			
			InputSender.RequestPass();
		}

		public void RequestNeverMind()
		{
			neverMindButton.interactable = false;
			
			InputSender.RequestNeverMind();
		}

		public void StartGame()
		{
			// should be coming from host so we don't need to send input through fusion
			startGameButton.gameObject.SetActive(false);

			TurnManagerServer.StartGame();
		}
	}
}
