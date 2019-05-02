using Microsoft.Extensions.Configuration;
using Serilog;

namespace StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws
{
    public static class WriteToElasticsearchAwsExtension
    {
        public static LoggerConfiguration WriteToElasticsearchAws(this LoggerConfiguration loggerConfiguration, IConfiguration appConfiguration)
        {
            Configurator.Configure(appConfiguration, loggerConfiguration);
            return loggerConfiguration;
        }
    }
}