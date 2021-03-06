﻿using Microsoft.Extensions.DependencyInjection;
using Ray.Core.Internal;
using Ray.Core.EventBus;
using Ray.RabbitMQ;

namespace RayTest.Grains
{
    public static class Extensions
    {
        public static void AddPSqlSiloGrain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMQService();
            serviceCollection.AddSingleton<IStorageContainer, PSQLStorageContainer>();
        }

        private static void AddMQService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddRabbitMQ();
            serviceCollection.AddSingleton<IProducerContainer, ProducerContainer>();
        }
    }
}
