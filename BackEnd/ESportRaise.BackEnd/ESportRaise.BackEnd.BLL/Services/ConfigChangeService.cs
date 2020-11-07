using ESportRaise.BackEnd.BLL.DTOs.ConfigChange;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class ConfigChangeService : IConfigChangeService
    {
        private readonly IConfiguration configuration;

        public ConfigChangeService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetConfigurations()
        {
            ConfigurationRoot root = configuration as ConfigurationRoot;
            var providers = root.AsEnumerable()
                .Where(pair => pair.Value != null);

            var config = JsonConvert.SerializeObject(providers);
            return config;
        }

        public void ChangeConfiguration(IEnumerable<ConfigurationOption> options)
        {
            foreach(var option in options)
            {
                configuration[option.Key] = option.Value;
            }
        }
    }
}
