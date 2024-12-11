using System;
using NUnit.Framework;

namespace Resources
{
	public class TileGeneratorTests
	{
		// A Test behaves as an ordinary method
		[Test]
		public void GenerateTiles_WhenCalled_GeneratesTiles()
		{
			TileGenerator tileGenerator = new();
			
			tileGenerator.GenerateTiles();
			
			Assert.AreEqual(152, Tile.AllTiles.Count);
		}

	}
}
