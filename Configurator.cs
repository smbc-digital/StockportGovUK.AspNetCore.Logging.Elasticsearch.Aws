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
        private static string defaultLogConfigurationKey = "ElasticSearchAwsConfiguration";
        private static string defaultLogConfigurationSecretsKey = "ElasticSearchAwsSecretsConfiguration";

        public static Serilog.Core.Logger Create(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration)
        {
            Configure(appConfiguration, loggerConfiguration);

            return loggerConfiguration.CreateLogger();
        }

        public static void Configure(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration)
        {
            Configure(appConfiguration, loggerConfiguration, defaultLogConfigurationKey, defaultLogConfigurationSecretsKey);
        }

        public static Serilog.Core.Logger Create(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration, string logConfigurationKey, string logSecretConfigurationKey)
        {
            Configure(appConfiguration, loggerConfiguration, defaultLogConfigurationKey, defaultLogConfigurationSecretsKey);

            return loggerConfiguration.CreateLogger();
        }

        public static void Configure(IConfiguration appConfiguration, LoggerConfiguration loggerConfiguration, string logConfigurationKey, string logSecretConfigurationKey)
        {
            var logConfigurationSection = appConfiguration.GetSection(logConfigurationKey);
            var logSecretConfigurationSection = appConfiguration.GetSection(logSecretConfigurationKey);
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

            Configure(loggerConfiguration, logConfiguration, logSecretConfiguration);
        }

        public static Serilog.Core.Logger Create(LoggerConfiguration loggerConfiguration, LogConfiguration logConfiguration, LogSecretConfiguration logSecretConfiguration)
        {
            Configure(loggerConfiguration, logConfiguration, logSecretConfiguration);

            return loggerConfiguration.CreateLogger();
        }

        public static void Configure(LoggerConfiguration loggerConfiguration, LogConfiguration logConfiguration, LogSecretConfiguration logSecretConfiguration)
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