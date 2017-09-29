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

    }
}