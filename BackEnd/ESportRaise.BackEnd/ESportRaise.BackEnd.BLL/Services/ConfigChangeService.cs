using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class ConfigChangeService
    {
        private readonly IConfiguration configuration;

        public ConfigChangeService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetConfigurations()
        {
            var config = JsonConvert.SerializeObject(configuration);
            return config;
        }
    }
}
