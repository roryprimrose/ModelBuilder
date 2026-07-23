namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of custom value sources: an ad-hoc <see cref="DelegateValueSource{T}" /> and a
    ///     reusable named <see cref="IValueSource{T}" /> class registered via <c>AddValueSource</c>.
    /// </summary>
    public class CustomValueSourceTests
    {
        [Fact]
        public void CreateUsesSingleArgumentDelegateValueSourceForNamedMember()
        {
            var actual = Model
                .AddValueSource(new DelegateValueSource<string>(_ => "fixed-name"), nameof(Widget.Name))
                .Create<Widget>();

            actual.Name.Should().Be("fixed-name");
        }

        [Fact]
        public void CreateUsesTwoArgumentDelegateValueSourceForNamedMember()
        {
            var actual = Model
                .AddValueSource(
                    new DelegateValueSource<string>((context, target) => $"{target.Type.Name}-{target.MemberName}"),
                    nameof(Widget.Name))
                .Create<Widget>();

            actual.Name.Should().Be("String-Name");
        }

        [Fact]
        public void CreateUsesReusableNamedValueSourceForMatchingMemberName()
        {
            var actual = Model.AddValueSource(new SkuValueSource(), nameof(Product.Sku)).Create<Product>();

            actual.Sku.Should().StartWith("SKU-");
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Price.Should().NotBe(0);
        }
    }
}
