using System;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;

namespace Simp.Rpc.Util
{
    public static class ConfigHelper
    {
        static ConfigHelper()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("MTIME_PROFILES_ACTIVE");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"app.{environmentVariable}.json")
                .Build();
        }

        private static string ProcessDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static IConfigurationRoot Configuration { get; }

    }
}