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
			TileTrackerClient tileTracker = new(new FakeMono());
			
			tileGenerator.GenerateTiles(tileTracker);
			
			Assert.AreEqual(152, tileTracker.AllTiles.Count);
		}
	}
}
