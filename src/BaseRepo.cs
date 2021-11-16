using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Graph;
using Azure.Identity;
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
        private readonly ClientSecretCredential credential;
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

            credential = new ClientSecretCredential(config.TenantId,
                config.AppId,
                config.Secret);
            client = new GraphServiceClient(credential);
            domains = client.Domains.Request().GetAsync().Result;

        }
    }
}
