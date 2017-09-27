using DotNetty.Common.Internal.Logging;

namespace Simp.Rpc
{
    using System; 
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Console;

    public static class ConfigHelper
    {
        static ConfigHelper()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static string ProcessDirectory
        {
            get
            {
  
                return AppDomain.CurrentDomain.BaseDirectory; 
            }
        }

        public static IConfigurationRoot Configuration { get; }

        public static void SetConsoleLogger() => InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));
    }
}