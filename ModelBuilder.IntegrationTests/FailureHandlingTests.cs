namespace ModelBuilder.IntegrationTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end test that a value source throwing during a build surfaces as a
    ///     <see cref="ModelBuildException" /> categorized as <see cref="FailureKind.ValueSourceThrew" />
    ///     with the failing type and member identified.
    /// </summary>
    public class FailureHandlingTests
    {
        [Fact]
        public void CreateWrapsThrowingValueSourceInModelBuildException()
        {
            var source = new DelegateValueSource<string>(_ => throw new InvalidOperationException("boom"));

            Action act = () => Model.AddValueSource(source, nameof(Widget.Name)).Create<Widget>();

            var exception = act.Should().Throw<ModelBuildException>().Which;

            exception.FailureKind.Should().Be(FailureKind.ValueSourceThrew);
            exception.TargetMember.Should().Be(nameof(Widget.Name));
            exception.InnerException.Should().BeOfType<InvalidOperationException>();
        }
    }
}
