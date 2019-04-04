using System.Linq;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws
{
    public class Configurator
    {
        private static string defaultLogConfigurationKeyName = "ElasticSearchAwsConfiguration";
        private static string defaultLogConfigurationSecretsKeyName = "ElasticSearchAwsSecretsConfiguration";

        public static void Configure(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration)
        {
            Configure(appConfiguration, loggerConfiguration, defaultLogConfigurationKeyName, defaultLogConfigurationSecretsKeyName);
        }

        public static void Configure(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration, string logConfigurationKeyName, string logSecretConfigurationKeyName)
        {
            var logConfigurationSection = appConfiguration.GetSection(logConfigurationKeyName);
            var logSecretConfigurationSection = appConfiguration.GetSection(logSecretConfigurationKeyName);
            var logConfiguration = new LogConfiguration();
            var logSecretConfiguration = new LogSecretConfiguration();
        
            if (logConfigurationSection.AsEnumerable().Any() && logSecretConfigurationSection.AsEnumerable().Any())
            {
                logConfigurationSection.Bind(logConfiguration);
                logSecretConfigurationSection.Bind(logSecretConfiguration);
            }
            else
            {
                logConfiguration.Enabled = false;
            }

            Configure(appConfiguration, loggerConfiguration, logConfiguration, logSecretConfiguration);
        }

        public static void Configure(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration, LogConfiguration logConfiguration, LogSecretConfiguration logSecretConfiguration)
        {
            if (!logConfiguration.Enabled)
            {
                return;
            }

            var options = Configurator.CreateSinkOptions(logConfiguration, logSecretConfiguration);
            if (options != null)
            {
                loggerConfiguration.WriteTo.Elasticsearch(options);
            }
        }

        private static ElasticsearchSinkOptions CreateSinkOptions(LogConfiguration logConfiguration, LogSecretConfiguration logSecretConfiguration)
        {
            if (logConfiguration == null || logSecretConfiguration == null)
            {
                return null;
            }

            var singleNodeConnectionPool = new SingleNodeConnectionPool(logConfiguration.Uri);
            var awsHttpConnection = new AwsHttpConnection(logConfiguration.Region, new StaticCredentialsProvider(
                new AwsCredentials
                {
                    AccessKey = logSecretConfiguration.AccessKey,
                    SecretKey = logSecretConfiguration.SecretKey
                }));

            var options = new ElasticsearchSinkOptions(logConfiguration.Uri)
            {
                IndexFormat = logConfiguration.IndexFormat,
                InlineFields = logConfiguration.InlineFields,
                MinimumLogEventLevel = logConfiguration.MinimumLogLevel,
                ModifyConnectionSettings = conn =>
                {
                    return new ConnectionConfiguration(singleNodeConnectionPool, awsHttpConnection);
                }
            };

            return options;
        }
    }
}