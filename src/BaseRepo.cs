using System;
using System.Linq;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using GraphUtilConfig = EPS.Extensions.B2CGraphUtil.Config.GraphUtilConfig;
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace EPS.Extensions.B2CGraphUtil
{
    /// <summary>
    /// The base repository used for the graph objects.
    /// </summary>
    public class BaseRepo
    {
        /// <summary>
        /// The graph configuration.
        /// </summary>
        protected readonly GraphUtilConfig graphUtilConfig;
        /// <summary>
        /// The logger (optional)
        /// </summary>
        protected readonly ILogger log;
        /// <summary>
        /// The graph service client.
        /// </summary>
        protected GraphServiceClient client;

        /// <summary>
        /// The first domain picked up by the graph.
        /// </summary>
        protected Domain domain;

        /// <summary>
        /// The client credential provider.
        /// </summary>
        private ClientSecretCredential credential;
        /// <summary>
        /// Instantiate a new instance of the base repo.
        /// </summary>
        /// <param name="graphUtilConfig">The configuration object.</param>
        protected BaseRepo(GraphUtilConfig graphUtilConfig)
        {
            this.graphUtilConfig = graphUtilConfig;
            initGraph();
        }

        /// <summary>
        /// initialize with a logger.
        /// </summary>
        /// <param name="graphUtilConfig"></param>
        /// <param name="logger"></param>
        protected BaseRepo(GraphUtilConfig graphUtilConfig, ILogger logger)
        {
            this.graphUtilConfig = graphUtilConfig;
            log = logger;
            initGraph();
        }

        private void initGraph()
        {
            /*
            var app = ConfidentialClientApplicationBuilder
                .Create(graphUtilConfig.AppId)
                .WithTenantId(graphUtilConfig.TenantId)
                .WithClientSecret(graphUtilConfig.Secret)
                .Build();
            */
            credential = new ClientSecretCredential(graphUtilConfig.TenantId,
                graphUtilConfig.AppId,
                graphUtilConfig.Secret);
            client = new GraphServiceClient(credential);
            var domains = client.Domains.GetAsync().Result;
            if (domains != null) domain = domains.Value.FirstOrDefault();
        }
        
        /// <summary>
        /// log information message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        protected void info(string msg, Exception? exception = null)
        {
            if (log == null) return;
            log.LogInformation(exception: exception, message:msg);
        }

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        protected void warn(string msg, Exception? exception = null)
        {
            if (log == null) return;
            log.LogWarning(exception: exception, message: msg);
        }
        /// <summary>
        /// Log trace message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        protected void trace(string msg, Exception? exception = null)
        {
            if (log == null) return;
            log.LogTrace(exception: exception, message: msg);
        }
    
        /// <summary>
        /// Log trace message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        protected void debug(string msg, Exception? exception)
        {
            if (log == null) return;
            log.LogDebug(exception: exception, message: msg);
        }
    
        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        protected void err(string msg, Exception? exception)
        {
            if (log == null) return;
            log.LogError(exception: exception, message: msg);
        }
    
        /// <summary>
        /// Log critical message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        protected void crit(string msg, Exception? exception)
        {
            if (log == null) return;
            log.LogCritical(exception: exception, message: msg);
        }
        
    }
}
