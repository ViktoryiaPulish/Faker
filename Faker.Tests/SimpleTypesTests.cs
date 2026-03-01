using Xunit;
using System;
using Faker;

namespace Faker.Tests
{
    public class SimpleTypesTests
    {
        private Faker _faker = new Faker();

        [Fact]
        public void Create_PrimitiveTypes_ReturnsCorrectValues()
        {
            int i = _faker.Create<int>();
            string s = _faker.Create<string>();
            double d = _faker.Create<double>();
            bool b = _faker.Create<bool>();

            Assert.NotEqual(0, i);
            Assert.False(string.IsNullOrEmpty(s));
            Assert.InRange(d, 0.0, 1.0);
        }

        [Fact]
        public void Create_DateTime_ReturnsDateWithinRange()
        {
            DateTime date = _faker.Create<DateTime>();

            Assert.True(date >= new DateTime(2000, 1, 1));
            Assert.True(date <= DateTime.Now);
        }

        [Fact]
        public void Create_Decimal_ReturnsNonDefaultValue()
        {
            decimal dec = _faker.Create<decimal>();
            Assert.NotEqual(0m, dec);
        }
    }
}