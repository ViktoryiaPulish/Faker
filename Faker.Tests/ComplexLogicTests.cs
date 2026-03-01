using Xunit;
using System;
using System.Collections.Generic;
using Faker;

namespace Faker.Tests;

public class ComplexLogicTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_Int_ReturnsNonDefaultValue()
    {
        var value = _faker.Create<int>();
        Assert.IsType<int>(value);
        Assert.NotEqual(0, value);
    }

    [Fact]
    public void Create_Long_ReturnsNonDefaultValue()
    {
        var value = _faker.Create<long>();
        Assert.IsType<long>(value);
        Assert.NotEqual(0L, value);
    }

    [Fact]
    public void Create_Double_ReturnsValueInRange()
    {
        var value = _faker.Create<double>();
        Assert.InRange(value, 0d, 1d);
    }

    [Fact]
    public void Create_Float_ReturnsValueInRange()
    {
        var value = _faker.Create<float>();
        Assert.InRange(value, 0f, 1f);
    }

    [Fact]
    public void Create_Byte_ReturnsNonDefaultValue()
    {
        var value = _faker.Create<byte>();
        Assert.IsType<byte>(value);
        Assert.NotEqual(0, value);
    }

    [Fact]
    public void Create_Short_ReturnsNonDefaultValue()
    {
        var value = _faker.Create<short>();
        Assert.IsType<short>(value);
        Assert.NotEqual(0, value);
    }

    [Fact]
    public void Create_Bool_ReturnsValidBoolean()
    {
        var value = _faker.Create<bool>();
        Assert.IsType<bool>(value);
    }

    [Fact]
    public void Create_Char_ReturnsNonDefaultValue()
    {
        var value = _faker.Create<char>();
        Assert.IsType<char>(value);
        Assert.NotEqual('\0', value);
    }

    [Fact]
    public void Create_Decimal_ReturnsNonDefaultValue()
    {
        var value = _faker.Create<decimal>();
        Assert.IsType<decimal>(value);
        Assert.NotEqual(0m, value);
    }

    [Fact]
    public void Create_String_ReturnsNonEmptyString()
    {
        var value = _faker.Create<string>();
        Assert.False(string.IsNullOrEmpty(value));
    }

    [Fact]
    public void Create_DateTime_ReturnsDateWithinRange()
    {
        var value = _faker.Create<DateTime>();
        Assert.True(value >= new DateTime(2000, 1, 1));
        Assert.True(value <= DateTime.Today);
    }

    [Fact]
    public void Create_NullableInt_ReturnsValueOrNull()
    {
        var value = _faker.Create<int?>();
        Assert.True(value == null || value.GetType() == typeof(int));
    }

    [Fact]
    public void Create_ListOfInts_ReturnsPopulatedList()
    {
        var list = _faker.Create<List<int>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }
}