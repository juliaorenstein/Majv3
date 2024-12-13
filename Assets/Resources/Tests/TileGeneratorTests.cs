using System;
using NUnit.Framework;

namespace Resources
{
	public class TileGeneratorTests
	{
		[Test]
		public void GenerateTiles_WhenCalled_GeneratesTiles()
		{
			TileGenerator tileGenerator = new();
			TileTrackerClient tileTracker = new(new FakeMono(), tileGenerator.GenerateTiles());
			
			Assert.AreEqual(152, tileTracker.AllTiles.Count);
		}
	}
}
