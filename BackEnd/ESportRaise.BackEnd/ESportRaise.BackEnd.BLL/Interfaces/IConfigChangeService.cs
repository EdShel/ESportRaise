using System.Collections.Generic;
using ESportRaise.BackEnd.BLL.DTOs.ConfigChange;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IConfigChangeService
    {
        void ChangeConfiguration(IEnumerable<ConfigurationOption> options);

        string GetConfigurations();
    }
}