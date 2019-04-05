# StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws
This package wraps the setup and configuration of Serilog for logging to Elasticsearch running in AWS. It enables specification in configuration file by convention.

The default convention for configuration files is as below:
```json
"ElasticSearchAwsConfiguration": {
    "Region": "[AWS Region]<string>",
    "IndexFormat": "[Index Format]<string>",
    "InlineFields": false,
    "MinimumLevel": "[Minimum Warning Level]<string>",
    "Enabled": false,
    "Url": "[Elasticsearch URL]<string>"
}
"ElasticSearchAwsSecretsConfiguration": {
    "AccessKey": "[AWS Access Key]<string>",
    "SecretKey": "[AWS Secret Key]<string>"
}
```

Enable configuration in Startup as per example below:
```C#
public void ConfigureServices(IServiceCollection services)
{
    // Your logger config here eg.
    var loggerConfiguration = new LoggerConfiguration()
        .ReadFrom
        .Configuration(Configuration)

    Configurator.Configure(Configuration, loggerConfiguration);

    Log.Logger = loggerConfiguration.CreateLogger();
}
```