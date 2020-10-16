using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MISA.LocationAPI.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationAPI.Extensions
{
    /// <summary>
    /// Static class chứa một extension method cho services, sẽ được thêm vào ở file Startup.cs
    /// </summary>
    /// Created by nmthang
    public static class ElasticsearchExtension
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:defaultIndex"];

            var settings = new ConnectionSettings(new Uri(url))
                                .DisableDirectStreaming()
                                .DefaultMappingFor<Location>(m => m.IndexName(defaultIndex).IdProperty(l => l.ID))
                                .DefaultFieldNameInferrer(l => l)
                                .EnableDebugMode()
                                .PrettyJson();
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
        }
    }
}
