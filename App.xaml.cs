using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GrammerMaterialOrder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string JsonConfigurationFile = @"Configuration\appsettings.json";
        private static IConfiguration _config;
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            // Initialize Dependency Injection
            var services = new ServiceCollection();

            // Serilog Setup Constants
            const string outputTemplate =
                @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}";
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");

            // Create Serilog
            var serilogLogger = new LoggerConfiguration()
                .WriteTo.File($"Logs/GrammerMaterialOrder_{timestamp}.log", outputTemplate: outputTemplate)
                .CreateLogger();

            // Add Serilog into services
            services.AddLogging(x =>
            {
                x.SetMinimumLevel(LogLevel.Information);
                x.AddSerilog(serilogLogger, true);
            });

            // Assign Configuration
            var builder = new ConfigurationBuilder().AddJsonFile(JsonConfigurationFile, true, true);
            _config = builder.Build();

            // Configure Services
            ConfigureServices(services);

            // Run Application
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Initialize Configuration
            // RegisterOptions(services);

            // Initialize DB
            //services.AddDbContext<RepositoryContext>(options =>
            //{
            //    options.UseSqlServer(_config.GetConnectionString("DataAll"));
            //});
            //services.AddSingleton<RepositoryContext>();

            // Initialize MVVM
            // RegisterViewModels(services);
            // RegisterRepositories(services);
            // RegisterForms(services);
        }
    }
}
