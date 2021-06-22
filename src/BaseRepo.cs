using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace EPS.Extensions.B2CGraphUtil
{
    /// <summary>
    /// The base repository used for the graph objects.
    /// </summary>
    public class BaseRepo
    {
        /// <summary>
        /// The graph service client.
        /// </summary>
        protected readonly GraphServiceClient client;
        /// <summary>
        /// The client credential provider.
        /// </summary>
        protected readonly ClientCredentialProvider provider;
        /// <summary>
        /// The domains provided by the graph API.
        /// </summary>
        protected readonly IGraphServiceDomainsCollectionPage domains;
        /// <summary>
        /// Instantiate a new instance of the base repo.
        /// </summary>
        /// <param name="config">The configuration object.</param>
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
