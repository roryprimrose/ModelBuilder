namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelConfigurationLogTests
    {
        [Fact]
        public void WriteLogReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.WriteLog(_ => { });

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void WriteLogThrowsWithNullSink()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.WriteLog(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
