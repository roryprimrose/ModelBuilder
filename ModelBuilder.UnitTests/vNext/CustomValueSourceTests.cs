namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class CustomValueSourceTests
    {
        [Fact]
        public void TypedValueSourceOverridesBuiltInRootValue()
        {
            var actual = Model.AddValueSource(new DelegateValueSource<int>(_ => 7)).Create<int>();

            actual.Should().Be(7);
        }

        [Fact]
        public void TypedValueSourceOverridesBuiltInNamedMember()
        {
            var configuration = new BuildConfiguration();

            configuration.AddValueSource(new DelegateValueSource<string>(_ => "CUSTOM"));

            var context = new BuildContext(new RandomSource(7), configuration: configuration);

            // A custom typed string source wins over the built-in FirstName named source.
            var actual = context.Build<string>(typeof(Sample), "FirstName");

            actual.Should().Be("CUSTOM");
        }

        [Fact]
        public void NamedValueSourceOverridesBuiltInNamedMember()
        {
            var configuration = new BuildConfiguration();

            configuration.AddValueSource(new DelegateValueSource<string>(_ => "custom@example.com"), "Email");

            var context = new BuildContext(new RandomSource(7), configuration: configuration);

            var actual = context.Build<string>(typeof(Sample), "Email");

            actual.Should().Be("custom@example.com");
        }

        [Fact]
        public void NamedValueSourceMatchesMemberNameAsWord()
        {
            var configuration = new BuildConfiguration();

            configuration.AddValueSource(new DelegateValueSource<string>(_ => "custom@example.com"), "Email");

            var context = new BuildContext(new RandomSource(7), configuration: configuration);

            // The named source matches a whole word within the member name, like the built-in sources do.
            var actual = context.Build<string>(typeof(Sample), "ContactEmailAddress");

            actual.Should().Be("custom@example.com");
        }

        [Fact]
        public void NamedValueSourceTakesPrecedenceOverTypedValueSource()
        {
            var configuration = new BuildConfiguration();

            configuration.AddValueSource(new DelegateValueSource<string>(_ => "TYPED"));
            configuration.AddValueSource(new DelegateValueSource<string>(_ => "NAMED"), "Email");

            var context = new BuildContext(new RandomSource(7), configuration: configuration);

            // The named source wins for a matching member.
            context.Build<string>(typeof(Sample), "Email").Should().Be("NAMED");

            // A member that does not match the named source falls back to the typed source.
            context.Build<string>(typeof(Sample), "Title").Should().Be("TYPED");
        }

        [Fact]
        public void ValueSourceReceivesBuildContext()
        {
            IBuildContext? captured = null;

            var configuration = new BuildConfiguration();

            configuration.AddValueSource(new DelegateValueSource<string>(context =>
            {
                captured = context;

                return "value";
            }));

            var sut = new BuildContext(new RandomSource(7), configuration: configuration);

            sut.Build<string>(typeof(Sample), "Title");

            captured.Should().BeSameAs(sut);
        }

        [Fact]
        public void CustomValueSourceCanReadSiblingData()
        {
            var configuration = new BuildConfiguration();

            // A custom source can use the same sibling mechanism the built-in sources use to stay
            // consistent with other members on the instance.
            configuration.AddValueSource(
                new DelegateValueSource<string>(context => "user:" + context.GetSibling<string>("FirstName")),
                "Slug");

            var context = new BuildContext(new RandomSource(7), configuration: configuration);

            using (context.EnterSiblingScope())
            {
                context.RecordSibling("FirstName", "Janet");

                context.Build<string>(typeof(Sample), "Slug").Should().Be("user:Janet");
            }
        }

        [Fact]
        public void ValueSourceAppliedThroughConfigurationModule()
        {
            var actual = Model.UsingModule<CustomSourceModule>().Create<int>();

            actual.Should().Be(42);
        }

        [Fact]
        public void AddValueSourceThrowsWithNullTypedSource()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddValueSource<string>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddValueSourceThrowsWithNullNamedSource()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddValueSource<string>(null!, "Email");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddValueSourceThrowsWithNullMemberNames()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddValueSource(new DelegateValueSource<string>(_ => "x"), null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddValueSourceThrowsWithEmptyMemberNames()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddValueSource(new DelegateValueSource<string>(_ => "x"), Array.Empty<string>());

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AddValueSourceThrowsWithInvalidMemberNameEntry(string? memberName)
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddValueSource(new DelegateValueSource<string>(_ => "x"), memberName!);

            action.Should().Throw<ArgumentException>();
        }

        private sealed class CustomSourceModule : IConfigurationModule
        {
            public void Configure(IBuildConfiguration configuration)
            {
                configuration.AddValueSource(new DelegateValueSource<int>(_ => 42));
            }
        }

        private sealed class Sample
        {
        }
    }
}
