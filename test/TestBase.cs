using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EPS.Extensions.B2CGraphUtil.Test
{
    public abstract class TestBase
    {
        protected IServiceCollection service;

        protected  IConfiguration Configuration { get; set; }
        protected string AppId { get; set; }
        protected string TenantId { get; set; }
        protected string Secret { get; set; }
        protected string Tenant { get; set; }

        protected Config.GraphUtilConfig Config { get; set; }

        [OneTimeSetUp]
        protected void SetupBase()
        {
            // the type specified here is just so the secrets library can
            // find the UserSecretId we added in the csproj file
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<TestBase>();
            Configuration = builder.Build();

            Config = GraphUtilConfig.Construct(Configuration);
            TestContext.WriteLine(Config.AppId);
            // Config = new GraphUtilConfig(Configuration.GetSection("GraphUtilConfig"));
            //
            // AppId = Configuration["GraphUtilConfig:AppId"];
            // TenantId = Configuration["GraphUtilConfig:TenantId"];
            // Secret = Configuration["GraphUtilConfig:Secret"];
            // Tenant = Configuration["GraphUtilConfig:Tenant"];
            // Config = new GraphUtilConfig
            // {
            //     AppId = AppId,
            //     Secret = Secret,
            //     TenantId = TenantId
            // };

        }

    }
}
