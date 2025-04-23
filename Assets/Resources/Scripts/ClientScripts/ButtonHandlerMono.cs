using System;
using UnityEngine;

namespace Resources
{
	public class ButtonHandlerMono : MonoBehaviour
	{
		public InputSender InputSender;

		public void RequestPickUp()
		{
			InputSender.RequestPickUp();
		}
	}

}
