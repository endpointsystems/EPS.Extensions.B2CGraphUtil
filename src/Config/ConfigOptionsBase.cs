using Microsoft.Extensions.Configuration;

namespace EPS.Extensions.B2CGraphUtil.Config
{
    /// <summary>
    /// Builds a config object based on the section name.
    /// </summary>
    /// <typeparam name="T">The config type.</typeparam>
    public abstract class ConfigOptionsBase<T>
        where T : ConfigOptionsBase<T>, new()
    {
        /// <summary>
        /// The name of the section we're rehydrating.
        /// </summary>
        protected abstract string SectionName { get; }

        /// <summary>
        /// Construct a new config options instance based on IConfiguration.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <returns>The config object.</returns>
        public static T Construct(IConfiguration configuration)
        {
            var instance = new T();
            return configuration.GetSection(instance.SectionName).Get<T>();
        }

        /// <summary>
        /// Construct using the <see cref="IConfigurationSection"/>.
        /// </summary>
        /// <param name="section">The specified configuration section.</param>
        /// <returns>The configuration object.</returns>
        public static T Construct(IConfigurationSection section)
        {
            return section.Get<T>();
        }
    }

}