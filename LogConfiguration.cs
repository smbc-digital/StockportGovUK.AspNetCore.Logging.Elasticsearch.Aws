using System;
using Serilog.Events;

namespace StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws
{
    public class LogConfiguration
    {
        public bool Enabled { get; set; }

        public string Region { get; set; }

        public string IndexFormat { get; set; }

        public bool InlineFields { get; set; }

        public string Url { get; set; }

        public Uri Uri
        {
            get
            {
                return string.IsNullOrEmpty(Url) ? new Uri("http://www.stockport.gov.uk") : new Uri(Url);
            }
        }

        public string MinimumLevel { get; set; }

        public LogEventLevel MinimumLogLevel
        {
            get
            {
                LogEventLevel logEventLevel;
                Enum.TryParse(MinimumLevel, true, out logEventLevel);
                return logEventLevel;
            }
        }
    }
}
