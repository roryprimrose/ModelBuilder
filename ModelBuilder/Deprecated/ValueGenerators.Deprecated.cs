// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [Obsolete(DeprecationMessages.ValueSource, true)]
    public interface IValueGenerator
    {
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class ValueGeneratorBase
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object? Generate(IExecuteStrategy executeStrategy, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object? Generate(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object? Generate(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected IRandomGenerator Generator
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class ValueGeneratorMatcher
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected ValueGeneratorMatcher(params Type[] types) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected ValueGeneratorMatcher(string referenceName, params Type[] types) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected ValueGeneratorMatcher(Regex expression, params Type[] types) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class RegexTypeNameValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected RegexTypeNameValueGenerator(Regex nameExpression, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class RelativeValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected RelativeValueGenerator(Regex targetNameExpression, params Type[] types) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected T GetValue<T>(Regex expression, object context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMale(IExecuteStrategy executeStrategy) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class NumericValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object GetMaximum(Type type, string? referenceName, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object GetMinimum(Type type, string? referenceName, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNegative
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class AddressValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public AddressValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class AgeValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public AgeValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MaxAge
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MinAge
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class BooleanValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public BooleanValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class CharValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CharValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class CityValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CityValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class CompanyValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CompanyValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class CountValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object GetMaximum(Type type, string? referenceName, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object GetMinimum(Type type, string? referenceName, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MaxCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class CountryValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CountryValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class CultureValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CultureValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class DateOfBirthValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public DateOfBirthValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class DateTimeValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public DateTimeValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class DomainNameValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public DomainNameValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class EmailValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public EmailValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static Regex SpecialCharacters
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected string? Domain
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class EnumValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class FirstNameValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public FirstNameValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class GenderValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public GenderValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class GuidValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public GuidValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AllowNull
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int NullPercentageChance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class IPAddressValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class LastNameValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public LastNameValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class MiddleNameValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public MiddleNameValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class MailinatorEmailValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected string Domain
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class PhoneValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public PhoneValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class PostCodeValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public PostCodeValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class StateValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public StateValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class StringValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public StringValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class SuburbValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public SuburbValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class TimeZoneInfoValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public TimeZoneInfoValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class TimeZoneValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public TimeZoneValueGenerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class UriValueGenerator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool IsMatch(IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }
}
