namespace EPS.Extensions.B2CGraphUtil.Config
{
    /// <summary>
    /// The configuration object for the Active Directory B2C directory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// API Permissions you'll need (with 'grant admin consent' enabled):
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>User.ReadWrite.All</description>
    /// </item>
    /// <item>
    /// <description>Group.ReadWrite.All</description>
    /// </item>
    /// <item>
    /// <description>Directory.ReadWrite.All</description>
    /// </item>
    /// <item>
    /// <description>People.ReadAll</description>
    /// </item>
    /// <item>
    /// <description>openid</description>
    /// </item>
    /// <item>
    /// <description>offline_access</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class GraphUtilConfig: ConfigOptionsBase<GraphUtilConfig>
    {
        /// <summary>
        /// The number of times to retry a graph operation.
        /// </summary>
        public int RetryCount { get; set; } = 5;
        
        /// <summary>
        /// Application (client) ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Tenant ID (guid)
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// App secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The section name to pull the configuration object from. Defaults to GraphUtilConfig
        /// </summary>
        protected override string SectionName => "GraphUtilConfig";
    }
}
