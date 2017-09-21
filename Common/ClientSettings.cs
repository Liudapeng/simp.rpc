// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Common
{
    using System.Net;

    public class ClientSettings
    { 
        public static IPAddress Host => IPAddress.Parse(ConfigHelper.Configuration["host"]).MapToIPv6();

        public static int Port => int.Parse(ConfigHelper.Configuration["port"]); 
    }
}