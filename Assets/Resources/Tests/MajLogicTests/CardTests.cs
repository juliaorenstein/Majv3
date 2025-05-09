using System;
using NUnit.Framework;
using Resources;

public class CardTests
{
    [Test]
    public void Card_Parse2024Card_CheckCard()
    {
        const string path = "/Users/juliaorenstein/Unity/Majv3/Assets/Resources/Scripts/MajLogic/2024Card.txt";
        Card _ = new(path);
        Assert.Pass();
        // I'm asserting pass because HandParserTests are responsible for accuracy.
        // I just want to make sure this process doesn't throw exceptions.
    }
}
