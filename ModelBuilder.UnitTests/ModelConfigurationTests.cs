namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelConfigurationTests
    {
        [Fact]
        public void AddValueSourceWithNamesReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.AddValueSource(new DelegateValueSource<int>(_ => 5), "Count");

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void IgnoringExtractsMemberNameFromExpression()
        {
            var sut = new ModelConfiguration();

            var actual = sut.Ignoring<Sample>(x => x.Name);

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void IgnoringExtractsMemberNameFromValueTypeExpression()
        {
            var sut = new ModelConfiguration();

            var actual = sut.Ignoring<Sample>(x => x.Count);

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void IgnoringThrowsWithNullExpression()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.Ignoring<Sample>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsWhenExpressionIsNotMemberAccess()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.Ignoring<Sample>(x => x.ToString());

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void IgnoringByTypeAndNameReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.Ignoring(typeof(Sample), nameof(Sample.Name));

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void IgnoringByTypeAndNameThrowsWithNullDeclaringType()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.Ignoring(null!, nameof(Sample.Name));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringByTypeAndNameThrowsWithNullMemberName()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.Ignoring(typeof(Sample), null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringAnyReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.IgnoringAny(member => member.Name == nameof(Sample.Name));

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void IgnoringAnyThrowsWithNullPredicate()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.IgnoringAny(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappingReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.Mapping<IThing, Thing>();

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void PopulateInvokesLogSinkWhenWriteLogConfigured()
        {
            ModelBuilderSlot<Probe>.Instance = new ProbeBuilder();

            try
            {
                string? captured = null;
                var sut = new ModelConfiguration();
                sut.WriteLog(log => captured = log);

                sut.Populate(new Probe());

                captured.Should().NotBeNull();
            }
            finally
            {
                ModelBuilderSlot<Probe>.Instance = null;
            }
        }

        [Fact]
        public void PopulateReturnsInstanceWithoutLogSink()
        {
            ModelBuilderSlot<Probe>.Instance = new ProbeBuilder();

            try
            {
                var sut = new ModelConfiguration();
                var instance = new Probe();

                var actual = sut.Populate(instance);

                actual.Should().BeSameAs(instance);
            }
            finally
            {
                ModelBuilderSlot<Probe>.Instance = null;
            }
        }

        [Fact]
        public void SetOptionsReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.SetOptions(x => x.MaxCount = 3);

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void SetOptionsThrowsWithNullConfigure()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.SetOptions(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private interface IThing
        {
        }

        private sealed class Probe
        {
        }

        private sealed class ProbeBuilder : IModelBuilder<Probe>
        {
            public Probe Create(IBuildContext context)
            {
                return new Probe();
            }

            public Probe Populate(IBuildContext context, Probe instance)
            {
                return instance;
            }
        }

        private sealed class Sample
        {
            public int Count { get; set; }

            public string? Name { get; set; }
        }

        private sealed class Thing : IThing
        {
        }
    }
}
