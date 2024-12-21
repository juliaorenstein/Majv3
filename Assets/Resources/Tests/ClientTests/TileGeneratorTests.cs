using System.Collections.Generic;
using NUnit.Framework;

namespace Resources.ClientTests
{
	public class TileGeneratorTests
	{
		[Test]
		public void GenerateTiles_WhenCalled_GeneratesTiles()
		{
			TileGenerator tileGenerator = new();
			List<Tile> tiles = tileGenerator.GenerateTiles();
			
			Assert.AreEqual(152, tiles.Count);
		}
	}
}
