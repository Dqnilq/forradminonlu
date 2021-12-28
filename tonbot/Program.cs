using Microsoft.Extensions.Configuration;
using tonbot;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();

var api = new Api(config);


await api.Function();