namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildContextSiblingTests
    {
        [Fact]
        public void GetSiblingReturnsDefaultWhenNoScopeOpen()
        {
            var sut = new BuildContext(new RandomSource(1));

            sut.GetSibling<string>("FirstName").Should().BeNull();
        }

        [Fact]
        public void GetSiblingReturnsDefaultWhenMemberNotRecorded()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.GetSibling<string>("Missing").Should().BeNull();
            }
        }

        [Fact]
        public void GetSiblingReturnsRecordedValueWithinScope()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("FirstName", "Janet");

                sut.GetSibling<string>("FirstName").Should().Be("Janet");
            }
        }

        [Theory]
        [InlineData("firstName", "FirstName")]
        [InlineData("FirstName", "firstname")]
        [InlineData("FIRSTNAME", "firstName")]
        public void GetSiblingMatchesNameCaseInsensitively(string recordedName, string lookupName)
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                // A camelCase constructor parameter must satisfy a PascalCase sibling lookup and vice versa.
                sut.RecordSibling(recordedName, "Janet");

                sut.GetSibling<string>(lookupName).Should().Be("Janet");
            }
        }

        [Fact]
        public void GetSiblingReturnsDefaultForWrongType()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("Age", 42);

                sut.GetSibling<string>("Age").Should().BeNull();
            }
        }

        [Fact]
        public void GetSiblingWithAliasesReturnsFirstRecordedMatch()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("GivenName", "Janet");

                // FirstName is not recorded, so the alias lookup falls through to GivenName.
                sut.GetSibling<string>("FirstName", "GivenName").Should().Be("Janet");
            }
        }

        [Fact]
        public void GetSiblingWithAliasesPrefersEarlierNameWhenBothRecorded()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("FirstName", "Janet");
                sut.RecordSibling("GivenName", "Other");

                sut.GetSibling<string>("FirstName", "GivenName").Should().Be("Janet");
            }
        }

        [Fact]
        public void GetSiblingWithAliasesReturnsDefaultWhenNoneRecorded()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("Age", 42);

                sut.GetSibling<string>("FirstName", "GivenName").Should().BeNull();
            }
        }

        [Fact]
        public void NestedScopeDoesNotLeakToOuterScope()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("Outer", "outer");

                using (sut.EnterSiblingScope())
                {
                    sut.RecordSibling("Inner", "inner");

                    sut.GetSibling<string>("Inner").Should().Be("inner");
                    sut.GetSibling<string>("Outer").Should().BeNull();
                }

                sut.GetSibling<string>("Outer").Should().Be("outer");
                sut.GetSibling<string>("Inner").Should().BeNull();
            }
        }

        [Fact]
        public void RecordSiblingIsNoOpWhenNoScopeOpen()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.RecordSibling("FirstName", "Janet");

            action.Should().NotThrow();
        }

        [Fact]
        public void GetSiblingThrowsWithNullMemberName()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.GetSibling<string>((string)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetSiblingThrowsWithNullMemberNames()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.GetSibling<string>((string[])null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetOrAddScopedValueCreatesValueWhenAbsent()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                var actual = sut.GetOrAddScopedValue("key", _ => 42);

                actual.Should().Be(42);
            }
        }

        [Fact]
        public void GetOrAddScopedValueReturnsCachedValueWithoutReinvokingFactory()
        {
            var sut = new BuildContext(new RandomSource(1));
            var calls = 0;

            using (sut.EnterSiblingScope())
            {
                var first = sut.GetOrAddScopedValue("key", _ =>
                {
                    calls++;

                    return 42;
                });
                var second = sut.GetOrAddScopedValue("key", _ =>
                {
                    calls++;

                    return 99;
                });

                first.Should().Be(42);
                // The second call returns the cached value and does not run its factory.
                second.Should().Be(42);
                calls.Should().Be(1);
            }
        }

        [Fact]
        public void GetOrAddScopedValueIsolatesValuesBetweenScopes()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.GetOrAddScopedValue("key", _ => 1).Should().Be(1);
            }

            using (sut.EnterSiblingScope())
            {
                // A fresh scope does not see the previous scope's cached value, so the factory runs again.
                sut.GetOrAddScopedValue("key", _ => 2).Should().Be(2);
            }
        }

        [Fact]
        public void GetOrAddScopedValueReplacesValueOfDifferentType()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.GetOrAddScopedValue<object>("key", _ => "text");

                // The cached value is a string, so requesting an int under the same key produces a new value.
                sut.GetOrAddScopedValue("key", _ => 7).Should().Be(7);
            }
        }

        [Fact]
        public void GetOrAddScopedValueCreatesValueWithoutCachingWhenNoScopeOpen()
        {
            var sut = new BuildContext(new RandomSource(1));
            var calls = 0;

            Func<int> create = () => sut.GetOrAddScopedValue("key", _ =>
            {
                calls++;

                return 5;
            });

            create().Should().Be(5);
            // With no scope to share within, each request creates a fresh value.
            create().Should().Be(5);
            calls.Should().Be(2);
        }

        [Fact]
        public void GetOrAddScopedValueThrowsWithNullKey()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.GetOrAddScopedValue(null!, _ => 1);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetOrAddScopedValueThrowsWithNullFactory()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.GetOrAddScopedValue<int>("key", null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
