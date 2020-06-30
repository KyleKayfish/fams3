﻿using BcGov.Fams3.Redis.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BcGov.Fams3.Redis.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCacheService(this IServiceCollection services, RedisConfiguration redisConfig)
        {


            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = $"{redisConfig.Host}:{redisConfig.Port},Password={redisConfig.Password}";

            });
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}
