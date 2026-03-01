using Xunit;
using System;
using Faker;

namespace Faker.Tests;

public class SimpleTypesTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_PrimitiveNumbers_ReturnsValidValues()
    {
        Assert.NotEqual(0, _faker.Create<int>());
        Assert.NotEqual(0L, _faker.Create<long>());
        Assert.NotEqual((short)0, _faker.Create<short>());
        Assert.NotEqual((byte)0, _faker.Create<byte>());
    }

    [Fact]
    public void Create_FloatingPointNumbers_ReturnsValueInRange()
    {
        Assert.InRange(_faker.Create<double>(), 0d, 1d);
        Assert.InRange(_faker.Create<float>(), 0f, 1f);
        Assert.NotEqual(0m, _faker.Create<decimal>());
    }

    [Fact]
    public void Create_StringAndChar_ReturnsNonDefaultValues()
    {
        Assert.False(string.IsNullOrEmpty(_faker.Create<string>()));
        Assert.NotEqual('\0', _faker.Create<char>());
    }

    [Fact]
    public void Create_DateTime_ReturnsDateWithinRange()
    {
        var date = _faker.Create<DateTime>();
        Assert.True(date >= new DateTime(2000, 1, 1));
        Assert.True(date <= DateTime.Now);
    }

    [Fact]
    public void Create_Bool_ReturnsValidBoolean()
    {
        var value = _faker.Create<bool>();
        Assert.IsType<bool>(value);
    }
}