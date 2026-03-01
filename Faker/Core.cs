using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Faker
{
    public interface IValueGenerator
    {
        Type GeneratedType { get; }
        object? Generate(GenerationContext context);
    }

    public interface IValueGenerator<T> : IValueGenerator
    {
        new T? Generate(GenerationContext context);
    }

    public class GenerationContext
    {
        public Faker Faker { get; }
        public Type RequestedType { get; }
        public Random Random => Faker.Random;
        internal HashSet<Type> VisitedTypes { get; }

        public GenerationContext(Faker faker, Type type, HashSet<Type> visited)
        {
            Faker = faker;
            RequestedType = type;
            VisitedTypes = visited;
        }
    }

    public class FakerConfig
    {
        private readonly Dictionary<(Type Parent, string Member), IValueGenerator> _customGenerators = new();

        public void Add<T, TMember, TGenerator>(Expression<Func<T, TMember>> selector)
            where TGenerator : IValueGenerator<TMember>, new()
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (selector.Body is not MemberExpression memberExpr)
                throw new ArgumentException("Selector must be a member access expression.");

            var key = (typeof(T), memberExpr.Member.Name.ToLower());
            _customGenerators[key] = new TGenerator();
        }

        internal IValueGenerator? GetCustomGenerator(Type parent, string name)
        {
            return _customGenerators.TryGetValue((parent, name.ToLower()), out var gen) ? gen : null;
        }
    }

    public class Faker
    {
        private readonly Dictionary<Type, IValueGenerator> _generators = new();
        private readonly Dictionary<Type, Type> _genericGenerators = new();
        private readonly FakerConfig _config;
        public Random Random { get; } = new();

        public Faker() : this(new FakerConfig()) { }

        public Faker(FakerConfig config)
        {
            _config = config;
            RegisterDefaultGenerators();
        }

        private void RegisterDefaultGenerators()
        {
            _generators[typeof(int)] = new IntGenerator();
            _generators[typeof(long)] = new LongGenerator();
            _generators[typeof(double)] = new DoubleGenerator();
            _generators[typeof(float)] = new FloatGenerator();
            _generators[typeof(string)] = new StringGenerator();
            _generators[typeof(DateTime)] = new DateTimeGenerator();
            _generators[typeof(bool)] = new BoolGenerator();
            _generators[typeof(byte)] = new ByteGenerator();
            _generators[typeof(short)] = new ShortGenerator();
            _generators[typeof(char)] = new CharGenerator();
            _generators[typeof(decimal)] = new DecimalGenerator(); 

            _genericGenerators[typeof(List<>)] = typeof(ListGenerator<>);
            _genericGenerators[typeof(IEnumerable<>)] = typeof(ListGenerator<>);
            _genericGenerators[typeof(ICollection<>)] = typeof(ListGenerator<>);
            _genericGenerators[typeof(IList<>)] = typeof(ListGenerator<>);
        }

        public T? Create<T>() => (T?)Create(typeof(T), new HashSet<Type>());

        internal object? Create(Type type, HashSet<Type> visited)
        {
            if (visited.Contains(type)) return GetDefaultValue(type);

            if (_generators.TryGetValue(type, out var generator))
                return generator.Generate(new GenerationContext(this, type, visited));

            if (type.IsGenericType && _genericGenerators.TryGetValue(type.GetGenericTypeDefinition(), out var genType))
            {
                var concreteGenType = genType.MakeGenericType(type.GetGenericArguments());
                var instance = (IValueGenerator)Activator.CreateInstance(concreteGenType)!;
                return instance.Generate(new GenerationContext(this, type, visited));
            }

            if (type.IsEnum)
            {
                var values = Enum.GetValues(type);
                return values.GetValue(Random.Next(values.Length));
            }

            return CreateComplexObject(type, visited);
        }

        private object? CreateComplexObject(Type type, HashSet<Type> visited)
        {
            if (type.IsAbstract || type.IsInterface) return null;

            visited.Add(type);
            try
            {
                var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                                       .OrderByDescending(c => c.GetParameters().Length);

                object? instance = null;
                HashSet<string> usedInCtor = new(StringComparer.OrdinalIgnoreCase);

                foreach (var ctor in constructors)
                {
                    try
                    {
                        var parameters = ctor.GetParameters();
                        var args = new object?[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var p = parameters[i];
                            var custom = _config.GetCustomGenerator(type, p.Name!);
                            args[i] = custom != null
                                ? custom.Generate(new GenerationContext(this, p.ParameterType, visited))
                                : Create(p.ParameterType, visited);
                            usedInCtor.Add(p.Name!);
                        }
                        instance = ctor.Invoke(args);
                        break;
                    }
                    catch { continue; }
                }

                if (instance == null && type.IsValueType)
                    instance = Activator.CreateInstance(type);

                if (instance != null)
                    PopulateMembers(instance, type, usedInCtor, visited);

                return instance;
            }
            finally
            {
                visited.Remove(type);
            }
        }

        private void PopulateMembers(object obj, Type type, HashSet<string> skip, HashSet<Type> visited)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;

            foreach (var prop in type.GetProperties(flags))
            {
                if (!prop.CanWrite || skip.Contains(prop.Name)) continue;
                var custom = _config.GetCustomGenerator(type, prop.Name);
                prop.SetValue(obj, custom != null
                    ? custom.Generate(new GenerationContext(this, prop.PropertyType, visited))
                    : Create(prop.PropertyType, visited));
            }

            foreach (var field in type.GetFields(flags))
            {
                if (skip.Contains(field.Name)) continue;
                var custom = _config.GetCustomGenerator(type, field.Name);
                field.SetValue(obj, custom != null
                    ? custom.Generate(new GenerationContext(this, field.FieldType, visited))
                    : Create(field.FieldType, visited));
            }
        }

        private object? GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}