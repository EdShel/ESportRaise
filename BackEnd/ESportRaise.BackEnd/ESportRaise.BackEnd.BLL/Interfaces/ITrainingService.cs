using ESportRaise.BackEnd.BLL.DTOs.Training;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITrainingService
    {
        Task<InitiateTrainingServiceResponse> InitiateTrainingAsync(InitiateTrainingServiceRequest request);
    }
}
