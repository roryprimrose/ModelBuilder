namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class GenerateModelBuilderAttributeTests
    {
        [Fact]
        public void AttributeUsageAllowsAssemblyClassAndStructTargets()
        {
            var usage = typeof(GenerateModelBuilderAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .Cast<AttributeUsageAttribute>()
                .Single();

            usage.ValidOn.Should().HaveFlag(AttributeTargets.Assembly);
            usage.ValidOn.Should().HaveFlag(AttributeTargets.Class);
            usage.ValidOn.Should().HaveFlag(AttributeTargets.Struct);
            usage.AllowMultiple.Should().BeTrue();
        }

        [Fact]
        public void ParameterlessConstructorLeavesTargetTypeNull()
        {
            var sut = new GenerateModelBuilderAttribute();

            sut.TargetType.Should().BeNull();
        }

        [Fact]
        public void TargetTypeConstructorSetsTargetType()
        {
            var sut = new GenerateModelBuilderAttribute(typeof(Uri));

            sut.TargetType.Should().Be(typeof(Uri));
        }

        [Fact]
        public void TargetTypeConstructorThrowsWithNullType()
        {
            Action action = () => _ = new GenerateModelBuilderAttribute(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
