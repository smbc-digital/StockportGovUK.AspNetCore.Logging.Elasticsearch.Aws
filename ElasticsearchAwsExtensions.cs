using System;
using System.Linq;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws
{
    public static class WriteToElasticsearchAwsExtension
    {
        private static string logConfigurationKey = "ElasticsearchAwsConfiguration";
        private static string logSecretConfigurationKey = "ElasticsearchAwsSecretsConfiguration";
        private static LogConfiguration logConfiguration = new LogConfiguration();
        private static LogSecretConfiguration logSecretConfiguration = new LogSecretConfiguration();
        private static ElasticsearchSinkOptions elasticsearchSinkOptions = new ElasticsearchSinkOptions();

        public static LoggerConfiguration WriteToElasticsearchAws(this LoggerConfiguration loggerConfiguration, IConfiguration appConfiguration)
        {
            SetConfiguration(appConfiguration);

            if (logConfiguration.Enabled)
            {
                return loggerConfiguration;
            }

            CreateSinkOptions(logConfiguration, logSecretConfiguration);
            
            loggerConfiguration.WriteTo.Elasticsearch(elasticsearchSinkOptions);
            
            return loggerConfiguration;
        }

        private static void SetConfiguration(IConfiguration appConfiguration)
        {
            var logConfigurationSection = appConfiguration.GetSection(logConfigurationKey);
            var logSecretConfigurationSection = appConfiguration.GetSection(logSecretConfigurationKey);

            if (logConfigurationSection.AsEnumerable().Any() && logSecretConfigurationSection.AsEnumerable().Any())
            {
                logConfigurationSection.Bind(logConfiguration);
                logSecretConfigurationSection.Bind(logSecretConfiguration);

                return;
            }
            
            throw new Exception("Could not set configuration");
        }

        private static void CreateSinkOptions(LogConfiguration logConfiguration, LogSecretConfiguration logSecretConfiguration)
        {
            var singleNodeConnectionPool = new SingleNodeConnectionPool(logConfiguration.Uri);
            var awsHttpConnection = new AwsHttpConnection(logConfiguration.Region, new StaticCredentialsProvider(
                new AwsCredentials
                {
                    AccessKey = logSecretConfiguration.AccessKey,
                    SecretKey = logSecretConfiguration.SecretKey
                }));

            elasticsearchSinkOptions = new ElasticsearchSinkOptions(logConfiguration.Uri)
            {
                IndexFormat = logConfiguration.IndexFormat,
                InlineFields = logConfiguration.InlineFields,
                MinimumLogEventLevel = logConfiguration.MinimumLogLevel,
                ModifyConnectionSettings = conn =>
                {
                    return new ConnectionConfiguration(singleNodeConnectionPool, awsHttpConnection);
                }
            };
        }
    }
}