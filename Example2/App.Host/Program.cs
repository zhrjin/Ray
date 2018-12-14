using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Ray.Core;
using Ray.Core.Messaging;
using Ray.MongoDB;
using Ray.PostgreSQL;
using Ray.RabbitMQ;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using App.Grain;
using Ray.IGrains;

namespace App.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .UseDashboard()
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Account).Assembly).WithReferences())
                .ConfigureServices((context, servicecollection) =>
                {
                    servicecollection.AddRay();
                    servicecollection.AddSingleton<ISerializer, ProtobufSerializer>();//注册序列化组件
                    //注册postgresql为事件存储库
                    servicecollection.AddPSqlSiloGrain();
                    //注册mongodb为事件存储库
                    //servicecollection.AddMongoDbSiloGrain();
                })
                .Configure<SqlConfig>(c =>
                {
                    c.ConnectionDict = new Dictionary<string, string> {
                        { "core_event","Server=127.0.0.1;Port=5432;Database=Ray;User Id=postgres;Password=123456;Pooling=true;MaxPoolSize=20;"}
                    };
                })
                .Configure<MongoConfig>(c =>
                {
                    c.Connection = "mongodb://127.0.0.1:27017";
                })
                .Configure<RabbitConfig>(c =>
                {
                    c.UserName = "admin";
                    c.Password = "admin";
                    c.Hosts = new[] { "127.0.0.1:5672" };
                    c.MaxPoolSize = 100;
                    c.VirtualHost = "/";
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Error);
                    logging.AddConsole();
                });

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
