using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Common;
using MoodleBot.Models;
using MoodleBot.Models.Context;
using System;

namespace MoodleBot.App_Start
{
    public class DbContextConfig
    {
        public static void Initialize(IConfiguration configuration, IWebHostEnvironment env, IServiceProvider svp)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(configuration.Database(), builder => builder.EnableRetryOnFailure());

            var context = new MoodleBotContext(optionsBuilder.Options, svp.GetService<ILogger>(), svp.GetService<IConcurrencyExceptionHandler>());
        }

        public static void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MoodleBotContext>(options =>
                options.UseSqlServer(configuration.Database()));
        }
    }
}
