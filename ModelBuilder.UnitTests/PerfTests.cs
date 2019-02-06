#if NETCOREAPP2_0
namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;
    using Watchium.Testing.Database;
    using Xunit;

    public class PerfTests
    {
        [Fact]
        public void CreateModels()
        {
            var count = EnumerableTypeCreator.DefaultAutoPopulateCount;

            try
            {
                EnumerableTypeCreator.DefaultAutoPopulateCount = 1000;
            
                Model.Create<List<MailHost>>();
            }
            finally
            {          
                EnumerableTypeCreator.DefaultAutoPopulateCount = count;
            }
        }
    }
}
#endif