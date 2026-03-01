using System;
using System.Collections.Generic;

namespace Faker
{
    public abstract class BaseGenerator<T> : IValueGenerator<T>
    {
        public Type GeneratedType => typeof(T);
        public abstract T? Generate(GenerationContext context);
        object? IValueGenerator.Generate(GenerationContext context) => Generate(context);
    }

    public class IntGenerator : BaseGenerator<int>
    {
        public override int Generate(GenerationContext context) => context.Random.Next(1, int.MaxValue);
    }

    public class ByteGenerator : BaseGenerator<byte>
    {
        public override byte Generate(GenerationContext context) => (byte)context.Random.Next(1, 256);
    }

    public class ShortGenerator : BaseGenerator<short>
    {
        public override short Generate(GenerationContext context) => (short)context.Random.Next(1, short.MaxValue);
    }

    public class CharGenerator : BaseGenerator<char>
    {
        public override char Generate(GenerationContext context)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return chars[context.Random.Next(chars.Length)];
        }
    }

    public class LongGenerator : BaseGenerator<long>
    {
        public override long Generate(GenerationContext context)
        {
            byte[] buffer = new byte[8];
            context.Random.NextBytes(buffer);
            long val = BitConverter.ToInt64(buffer, 0);
            return val == 0 ? 1 : val;
        }
    }

    public class DoubleGenerator : BaseGenerator<double>
    {
        public override double Generate(GenerationContext context) => context.Random.NextDouble();
    }

    public class FloatGenerator : BaseGenerator<float>
    {
        public override float Generate(GenerationContext context) => (float)context.Random.NextDouble();
    }

    public class DecimalGenerator : BaseGenerator<decimal>
    {
        public override decimal Generate(GenerationContext context)
            => (decimal)context.Random.NextDouble() + context.Random.Next(1, 100);
    }

    public class BoolGenerator : BaseGenerator<bool>
    {
        public override bool Generate(GenerationContext context) => context.Random.Next(2) == 0;
    }

    public class StringGenerator : BaseGenerator<string>
    {
        public override string Generate(GenerationContext context)
        {
            int len = context.Random.Next(5, 15);
            char[] res = new char[len];
            for (int i = 0; i < len; i++) res[i] = (char)context.Random.Next('a', 'z' + 1);
            return new string(res);
        }
    }

    public class DateTimeGenerator : BaseGenerator<DateTime>
    {
        public override DateTime Generate(GenerationContext context)
        {
            DateTime start = new DateTime(2000, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(context.Random.Next(range + 1));
        }
    }

    public class ListGenerator<T> : IValueGenerator<List<T>>
    {
        public Type GeneratedType => typeof(List<T>);

        public List<T>? Generate(GenerationContext context)
        {
            int count = context.Random.Next(2, 5);
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var item = context.Faker.Create(typeof(T), context.VisitedTypes);
                list.Add(item is T typed ? typed : default!);
            }
            return list;
        }

        object? IValueGenerator.Generate(GenerationContext context) => Generate(context);
    }
}