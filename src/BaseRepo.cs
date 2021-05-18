using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace EPS.Extensions.B2CGraphUtil
{
    public class BaseRepo
    {
        protected readonly GraphServiceClient client;
        protected readonly ClientCredentialProvider provider;
        protected readonly IGraphServiceDomainsCollectionPage domains;

        protected BaseRepo(GraphUtilConfig config)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(config.AppId)
                .WithTenantId(config.TenantId)
                .WithClientSecret(config.Secret)
                .Build();
            provider = new ClientCredentialProvider(app);
            client = new GraphServiceClient(provider);
            domains = client.Domains.Request().GetAsync().Result;

        }
    }
}
