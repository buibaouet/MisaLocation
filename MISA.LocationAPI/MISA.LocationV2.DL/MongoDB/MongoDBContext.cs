using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using MongoDB.Driver;
using MISA.LocationV2.Webhook.Models;

namespace MISA.LocationV2.DL.MongoDB
{
    public static class MongoDBContext
    {
        public static void AddMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            var connect = configuration["mongodbConnect:ConnectionString"];
            var database = configuration["mongodbConnect:DatabaseName"];

            var client = new MongoClient(connect);
            var db = client.GetDatabase(database);
            services.AddSingleton<IMongoDatabase>(db);
        }
    }
}
