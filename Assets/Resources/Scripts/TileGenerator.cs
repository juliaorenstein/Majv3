namespace Resources
{
	public class TileGenerator
	{
		private int _id = 0;
    
		public void GenerateTiles()
		{
			CreateNumberDragons();
			CreateFlowerWinds();
			CreateJokers();
		}

		void CreateNumberDragons()
		{
			// loop through suits
			foreach (Suit suit in Tile.Suits)
			{
				// loop through values
				for (int num = 0; num < 10; num++)
				{
					// four each
					for (int i = 0; i < 4; i++)
					{
						if (num == 0)
						{
							new Tile(Kind.Dragon, _id++, num, suit);
						}
						else
						{
							new Tile(Kind.Number, _id++, num, suit);
						}
					}
				}
			}
		}

		void CreateFlowerWinds()
		{
			// loop through winds
			foreach (Wind wind in Tile.Winds)
			{
				// four of each
				for (int i = 0; i < 4; i++)
				{
					new Tile(Kind.FlowerWind, _id++, wind: wind);
				}
			}
			
			// four more flowers
			for (int num = 0; num < 4; num++)
			{
				new Tile(Kind.FlowerWind, _id++, wind: Wind.Flower);
			}
		}

		void CreateJokers()
		{
			// 8 of these
			for (int i = 0; i < 8; i++)
			{
				new Tile(Kind.Joker, _id++);
			}
		}
	}
}
