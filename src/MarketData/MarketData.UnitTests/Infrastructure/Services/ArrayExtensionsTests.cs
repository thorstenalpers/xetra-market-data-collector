using AutoBogus;
using MarketData.Infratructure;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MarketData.UnitTests.Infrastructure.Services;

[TestFixture]
[Category("UnitTests")]
public class ArrayExtensionsTests
{
    private IAutoFaker _faker;

    [SetUp]
    public void SetUp()
    {
        _faker = AutoFaker.Create();
    }

    [Test]
    public void SplitIntoEqualSizedChunks_Ok()
    {
        // Arrange
        List<int> integerList = _faker.Generate<int>(100);
        int totalSize = 9;

        // Act
        var result = integerList.SplitIntoEqualSizedChunks(totalSize);

        // Assert
        Assert.AreEqual(9, result.Count);
    }

    [Test]
    public void SplitIntoBatches_Ok()
    {
        // Arrange
        List<int> integerList = _faker.Generate<int>(100);
        int batchSize = 9;

        // Act
        var result = integerList.SplitIntoBatches(batchSize);

        // Assert
        Assert.IsTrue(result.All(e => e.Count <= batchSize));
    }
}
