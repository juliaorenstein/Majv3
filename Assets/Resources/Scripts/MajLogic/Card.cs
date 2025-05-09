using System.Collections.Generic;

namespace Resources
{
	public class Card
	{
		public List<Hand> BaseHands;
		
		public Card(string cardFilePath)
		{
			BaseHands = new List<Hand>();
			foreach (string line in System.IO.File.ReadLines(cardFilePath))
			{
				if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines
				Hand baseHand = Hand.GetBaseHand(line);
				BaseHands.Add(baseHand);
			}
		}
	}
}