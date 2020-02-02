namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class NumericValueGeneratorTests
    {
        [Fact]
        public void CanCreateParameters()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<NumericValueGenerator>();

            var actual = config.Create<ParameterTest>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateProperties()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<NumericValueGenerator>();

            var actual = config.Create<PropertyTest>();

            actual.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetNullableParameters), typeof(ParameterTest), MemberType = typeof(DataSet))]
        public void GenerateForParameterInfoReturnsNullAndNonNullValues(ParameterInfo parameterInfo)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new NumericValueGenerator();

            var nullFound = false;
            var valueFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.Generate(parameterInfo, executeStrategy);

                if (nextValue == null)
                {
                    nullFound = true;
                }
                else
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(ParameterTest), MemberType = typeof(DataSet))]
        public void GenerateForParameterInfoReturnsRandomValues(ParameterInfo parameterInfo)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new NumericValueGenerator();

            var randomValueFound = false;
            var firstValue = target.Generate(parameterInfo, executeStrategy);

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.Generate(parameterInfo, executeStrategy);

                if (firstValue != nextValue)
                {
                    randomValueFound = true;
                }
            }

            randomValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(ParameterTest).GetConstructors().Single().GetParameters()
                .First();

            var sut = new NumericValueGenerator();

            Action action = () => sut.Generate(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetNullableProperties), typeof(ParameterTest), MemberType = typeof(DataSet))]
        public void GenerateForPropertyInfoReturnsNullAndNonNullValues(PropertyInfo propertyInfo)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new NumericValueGenerator();

            var nullFound = false;
            var valueFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.Generate(propertyInfo, executeStrategy);

                if (nextValue == null)
                {
                    nullFound = true;
                }
                else
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(ParameterTest), MemberType = typeof(DataSet))]
        public void GenerateForPropertyInfoReturnsRandomValues(PropertyInfo propertyInfo)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new NumericValueGenerator();

            var randomValueFound = false;
            var firstValue = target.Generate(propertyInfo, executeStrategy);

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.Generate(propertyInfo, executeStrategy);

                if (firstValue != nextValue)
                {
                    randomValueFound = true;
                }
            }

            randomValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(PropertyTest).GetProperties()
                .First();

            var sut = new NumericValueGenerator();

            Action action = () => sut.Generate(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateForTypeReturnsNullAndNonNullValues(Type type, bool typeSupported, double min,
            double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            if (type.IsNullable() == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new NumericValueGenerator();

            var nullFound = false;
            var valueFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.Generate(type, null, executeStrategy);

                if (nextValue == null)
                {
                    nullFound = true;
                }
                else
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateForTypeReturnsRandomValues(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new NumericValueGenerator();

            var randomValueFound = false;
            var firstValue = target.Generate(type, null, executeStrategy);

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.Generate(type, null, executeStrategy);

                if (firstValue != nextValue)
                {
                    randomValueFound = true;
                }
            }

            randomValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var sut = new NumericValueGenerator();

            Action action = () => sut.Generate(typeof(int), null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new NumericValueGenerator();

            Action action = () => sut.Generate((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new NumericValueGenerator();

            Action action = () => sut.Generate((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new NumericValueGenerator();

            Action action = () => sut.Generate(null, null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedForParameterInfoReturnsFalseForUnsupportedType()
        {
            var parameterInfo = typeof(Copy).GetConstructors().Single().GetParameters().Single();

            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            var actual = target.IsSupported(parameterInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(ParameterTest), MemberType = typeof(DataSet))]
        public void IsSupportedForParameterInfoReturnsTrue(ParameterInfo parameterInfo)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            var actual = target.IsSupported(parameterInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedForPropertyInfoReturnsFalseForUnsupportedType()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            var actual = target.IsSupported(propertyInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(PropertyTest), MemberType = typeof(DataSet))]
        public void IsSupportedForPropertyInfoReturnsTrue(PropertyInfo propertyInfo)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            var actual = target.IsSupported(propertyInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedForTypeEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            var actual = target.IsSupported(type, null, buildChain);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsSupportedForTypeThrowsExceptionWithNullBuildChangeTest()
        {
            var type = typeof(decimal);

            var target = new NumericValueGenerator();

            Action action = () => target.IsSupported(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedForTypeThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullParameterInfo()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new NumericValueGenerator();

            Action action = () => sut.IsSupported((ParameterInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullPropertyInfo()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new NumericValueGenerator();

            Action action = () => sut.IsSupported((PropertyInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class ParameterTest
        {
            public ParameterTest(
                sbyte param_sbyte,
                byte param_byte,
                short param_short,
                ushort param_ushort,
                int param_int,
                uint param_uint,
                long param_long,
                ulong param_ulong,
                double param_double,
                float param_float,
                decimal param_decimal,
                sbyte? paramNullable_sbyte,
                byte? paramNullable_byte,
                short? paramNullable_short,
                ushort? paramNullable_ushort,
                int? paramNullable_int,
                uint? paramNullable_uint,
                long? paramNullable_long,
                ulong? paramNullable_ulong,
                double? paramNullable_double,
                float? paramNullable_float,
                decimal? paramNullable_decimal
            )
            {
                Param_sbyte = param_sbyte;
                Param_byte = param_byte;
                Param_short = param_short;
                Param_ushort = param_ushort;
                Param_int = param_int;
                Param_uint = param_uint;
                Param_long = param_long;
                Param_ulong = param_ulong;
                Param_double = param_double;
                Param_float = param_float;
                Param_decimal = param_decimal;
                ParamNullable_sbyte = paramNullable_sbyte;
                ParamNullable_byte = paramNullable_byte;
                ParamNullable_short = paramNullable_short;
                ParamNullable_ushort = paramNullable_ushort;
                ParamNullable_int = paramNullable_int;
                ParamNullable_uint = paramNullable_uint;
                ParamNullable_long = paramNullable_long;
                ParamNullable_ulong = paramNullable_ulong;
                ParamNullable_double = paramNullable_double;
                ParamNullable_float = paramNullable_float;
                ParamNullable_decimal = paramNullable_decimal;
            }

            public byte Param_byte { get; }
            public decimal Param_decimal { get; }
            public double Param_double { get; }
            public float Param_float { get; }
            public int Param_int { get; }
            public long Param_long { get; }

            public sbyte Param_sbyte { get; }
            public short Param_short { get; }
            public uint Param_uint { get; }
            public ulong Param_ulong { get; }
            public ushort Param_ushort { get; }
            public byte? ParamNullable_byte { get; }
            public decimal? ParamNullable_decimal { get; }
            public double? ParamNullable_double { get; }
            public float? ParamNullable_float { get; }
            public int? ParamNullable_int { get; }
            public long? ParamNullable_long { get; }
            public sbyte? ParamNullable_sbyte { get; }
            public short? ParamNullable_short { get; }
            public uint? ParamNullable_uint { get; }
            public ulong? ParamNullable_ulong { get; }
            public ushort? ParamNullable_ushort { get; }
        }

        private class PropertyTest
        {
            public byte Prop_byte { get; set; }
            public decimal Prop_decimal { get; set; }
            public double Prop_double { get; set; }
            public float Prop_float { get; set; }
            public int Prop_int { get; set; }
            public long Prop_long { get; set; }
            public sbyte Prop_sbyte { get; set; }
            public short Prop_short { get; set; }
            public uint Prop_uint { get; set; }
            public ulong Prop_ulong { get; set; }
            public ushort Prop_ushort { get; set; }
            public byte? PropNullable_byte { get; set; }
            public decimal? PropNullable_decimal { get; set; }
            public double? PropNullable_double { get; set; }
            public float? PropNullable_float { get; set; }
            public int? PropNullable_int { get; set; }
            public long? PropNullable_long { get; set; }
            public sbyte? PropNullable_sbyte { get; set; }
            public short? PropNullable_short { get; set; }
            public uint? PropNullable_uint { get; set; }
            public ulong? PropNullable_ulong { get; set; }
            public ushort? PropNullable_ushort { get; set; }
        }
    }
}