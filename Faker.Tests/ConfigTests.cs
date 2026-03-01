using Xunit;
using System;
using Faker;

namespace Faker.Tests
{
    public class ConfigTests
    {
        class User
        {
            public string Name { get; }
            public string City { get; set; }

            public User(string name) { Name = name; }
        }

        class ConstantGenerator : IValueGenerator<string>
        {
            public Type GeneratedType => typeof(string);
            public string Generate(GenerationContext ctx) => "CustomValue";
            object IValueGenerator.Generate(GenerationContext ctx) => Generate(ctx);
        }

        [Fact]
        public void Create_WithCustomConfig_ReturnsConfiguredValues()
        {
            var config = new FakerConfig();
            config.Add<User, string, ConstantGenerator>(u => u.City);
            config.Add<User, string, ConstantGenerator>(u => u.Name);

            var faker = new Faker(config);
            var user = faker.Create<User>();

            Assert.Equal("CustomValue", user.City);
            Assert.Equal("CustomValue", user.Name);
        }

        [Fact]
        public void Add_InvalidExpression_ThrowsArgumentException()
        {
            var config = new FakerConfig();
            Assert.Throws<ArgumentException>(() =>
                config.Add<User, string, ConstantGenerator>(u => "строка"));
        }
    }
}