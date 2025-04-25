using NUnit.Framework;

namespace Resources.ClientTests
{
    public class TileTests
    {
        [Test]
        public void ToString_GreenDragon_ReturnsGreen()
        {
            Tile tile = new(Kind.Dragon, suit: Suit.Bam);
            
            string result = tile.ToString();
            
            Assert.AreEqual("Green", result);
        }
        
        [Test]
        public void ToString_RedDragon_ReturnsRed()
        {
            Tile tile = new(Kind.Dragon, suit: Suit.Crak);
            
            string result = tile.ToString();
            
            Assert.AreEqual("Red", result);
        }
        
        [Test]
        public void ToString_Soap_ReturnsSoap()
        {
            Tile tile = new(Kind.Dragon, suit: Suit.Dot);
            
            string result = tile.ToString();
            
            Assert.AreEqual("Soap", result);
        }

        [Test]
        public void ToString_1Bam_Returns1Bam()
        {
            Tile tile = new(Kind.Number, suit: Suit.Bam, num: 1);
            
            string result = tile.ToString();
            
            Assert.AreEqual("1 Bam", result);
        }
        
        [Test]
        public void ToString_2Crak_Returns2Crak()
        {
            Tile tile = new(Kind.Number, suit: Suit.Crak, num: 2);
            
            string result = tile.ToString();
            
            Assert.AreEqual("2 Crak", result);
        }
        
        [Test]
        public void ToString_3Dot_Returns3Dot()
        {
            Tile tile = new(Kind.Number, suit: Suit.Dot, num: 3);
            
            string result = tile.ToString();
            
            Assert.AreEqual("3 Dot", result);
        }

        [Test]
        public void ToString_FlowerWind_ReturnsWind()
        {
            Tile tile = new(Kind.FlowerWind, wind: Wind.North);
            
            string result = tile.ToString();
            
            Assert.AreEqual("North", result);
        }

        [Test]
        public void ToString_Joker_ReturnsName()
        {
            Tile tile = new(Kind.Joker);
            
            string result = tile.ToString();
            
            Assert.AreEqual("Joker", result);
        }

        [Test]
        public void IsJoker_Joker_ReturnsTrue()
        {
            Tile tile = new(Kind.Joker);

            bool result = tile.IsJoker();
            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsJoker_NotJoker_ReturnsFalse()
        {
            Tile tile = new(Kind.FlowerWind, wind: Wind.North);
            
            bool result = tile.IsJoker();
            
            Assert.IsFalse(result);
        }
    }
}
