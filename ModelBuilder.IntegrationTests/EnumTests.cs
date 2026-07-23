namespace ModelBuilder.IntegrationTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests that enum roots and members always resolve to a defined value, including
    ///     <c>[Flags]</c> enums.
    /// </summary>
    public class EnumTests
    {
        [Fact]
        public void CreateBuildsDefinedSimpleEnumValue()
        {
            var actual = Model.Create<Colour>();

            Enum.IsDefined(typeof(Colour), actual).Should().BeTrue();
        }

        [Fact]
        public void CreateBuildsDefinedFlagsEnumValue()
        {
            var actual = Model.Create<Permissions>();

            // A flags enum's build should stay within the declared bit range rather than an arbitrary
            // out-of-range integer.
            ((actual & ~Permissions.All) == 0).Should().BeTrue();
        }
    }
}
