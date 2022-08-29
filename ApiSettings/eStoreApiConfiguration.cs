using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eStoreLibrary
{
    public static class eStoreApiConfiguration
    {
        #region Private Members to get Configuration
        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.api.json", true, true);
            return builder.Build();
        }
        #endregion

        public static string ConnectionString => GetConfiguration().GetConnectionString("eStore");

        public static string DefaultAccount => JsonSerializer.Serialize(new
        {
            Email = GetConfiguration()["Account:DefaultAccount:Email"],
            Password = GetConfiguration()["Account:DefaultAccount:Password"]
        });

    }
}
