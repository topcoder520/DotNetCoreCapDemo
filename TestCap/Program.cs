using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using TestCap.efcore;
using TestCap.services;

namespace TestCap
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string connectString = string.Empty;
            connectString = builder.Configuration.GetConnectionString("Default");

            //efcore mysql
            var serverVersion = new MySqlServerVersion(new Version(6, 0, 3));
            builder.Services.AddDbContext<AppDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectString, serverVersion)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
             );

            //cap rabbitmq
            builder.Services.AddTransient<ISubscriberService, CheckMsgService>();
            builder.Services.AddCap(x =>
            {
                x.UseMySql(connectString);
                //x.UseEntityFramework<AppDbContext>();
                x.UseRabbitMQ(opt =>
                {
                    opt.HostName = "localhost";
                    //opt.ExchangeName = builder.Environment.WebRootPath;// 以物理路径作为应用程序标识
                });

                x.FailedRetryCount = int.MaxValue;// 尽可能重试
                x.SucceedMessageExpiredAfter = 3600 * 24 * 30;// 保留 30 天备考
                x.DefaultGroupName = builder.Environment.WebRootPath;// 以物理路径作为应用程序标识
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            DbInitializer.CreateDbIfNotExists(app);

            app.Run();
        }
    }
}
