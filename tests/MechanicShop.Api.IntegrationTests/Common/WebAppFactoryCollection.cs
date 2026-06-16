using MechanicShop.Api.IntegrationTests.Common;
using Xunit;

namespace MechanicShop.Api.IntegrationTests
{
    [CollectionDefinition(CollectionName)]
    public class WebAppFactoryCollection : ICollectionFixture<WebAppFactory>
    {
        public const string CollectionName = "WebAppFactoryCollection";
    }
}
