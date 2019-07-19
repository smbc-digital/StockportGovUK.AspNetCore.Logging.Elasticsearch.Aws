<h1 align="center">StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws</h1>

<div align="center">
  :computer: :satellite: :cloud:
</div>
<div align="center">
  <strong>You're only as good as logging!</strong>
</div>
<div align="center">
  A NuGet package to encapsulate [Serilog.Sinks.Elasticsearch](https://www.nuget.org/packages/Serilog.Sinks.Elasticsearch) configuration for logging to Elasticsearch running in AWS
</div>

<br />

<div align="center">
  ![Nuget](https://img.shields.io/nuget/dt/StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws.svg?style=flat-square)
  ![Nuget](https://img.shields.io/nuget/v/StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws.svg?style=flat-square)
  ![Stability](https://img.shields.io/badge/stability-stable-green.svg?style=flat-square)
</div>

<div align="center">
  <h3>
    External Links
    <a href="https://github.com/smbc-digital/StockportGovUK.AspNetCore.Gateways">
      GitHub
    </a>
    <span> | </span>
    <a href="https://www.nuget.org/packages/StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws/">
      NuGet
    </a>
  </h3>
</div>

<div align="center">
  <sub>Built with ❤︎ by
  <a href="https://www.stockport.gov.uk">Stockport Council</a> and
  <a href="">
    contributors
  </a>
</div>

## Table of Contents
- [Features](#features)
- [Example](#example)
- [Philosophy](#philosophy)
- [Things we do](#things-we-do)
- [FAQ](#faq)
- [API](#api)
- [Installation](#installation)
- [See Also](#see-also)
- [Support](#support)

## Features
- __Serilog:__ encapsulating [Serilog.Sinks.Elasticsearch](https://www.nuget.org/packages/Serilog.Sinks.Elasticsearch) configuration
- __Extension Methods:__ Log.Logger extension method for easy setup

## Example
```c#
public void ConfigureServices(IServiceCollection services)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .WriteToElasticsearchAws(Configuration);
}
```

## Philosophy
Kibana is a great log aggrigation service, paired with AWS infrastructure it makes for an easy Logging As a Service experience. Serilog is an easy to implament and use logging diagnostics library, however, although there is a sink in place for Elasticsearch, it doesn't encapsulate the configuration needed for Elasticsearch in AWS.

In comes this NuGet package. :clap: :clap:

Written to allow setup via configuration you can now enjoy the benefits of Serilog, Elasticsearch, Kibana and AWS all together!

## Setup
The default convention for configuration files is as below:
```json
"ElasticsearchAwsConfiguration": {
    "Region": "[AWS Region]<string>",
    "IndexFormat": "[Index Format]<string>",
    "InlineFields": false,
    "MinimumLevel": "[Minimum Warning Level]<string>",
    "Enabled": false,
    "Url": "[Elasticsearch URL]<string>"
}
```
To ensure that your Access Key and Secret Key remain secret we reccomend storing these in a seperate config file to your standard application configuration.
```json
"ElasticsearchAwsSecretsConfiguration": {
    "AccessKey": "[AWS Access Key]<string>",
    "SecretKey": "[AWS Secret Key]<string>"
}
```
Once your configuration is setup you need to register Elasticsearch with your applications logger:
```c#
public void ConfigureServices(IServiceCollection services)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .WriteToElasticsearchAws(Configuration);
}
```

Badda bing, badda boom, you have your logging working.

## Installation
```bash
$ dotnet add package StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws
```

## Support
If you have any suggestions, fixes or new features please reach out to us here on GitHub!

## License
[MIT](https://tldrlegal.com/license/mit-license)