using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITrainingService
    {
        Task<InitiateTrainingServiceResponse> InitiateTraining(InitiateTrainingServiceRequest request);
    }
}

namespace ESportRaise.BackEnd.BLL.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly int idlenessMinutesForNewTraining;

        private TrainingAsyncRepository trainings;

        public TrainingService(IConfiguration configuration, TrainingAsyncRepository trainings)
        {
            idlenessMinutesForNewTraining = configuration.GetValue<int>("IdlenessMinutesForNewTraining");
            this.trainings = trainings;
        }

        public async Task<InitiateTrainingServiceResponse> InitiateTraining(InitiateTrainingServiceRequest request)
        {
            int trainingId;
            try
            {
                int userId = request.UserId;
                trainingId = await trainings.GetTrainingId(userId, idlenessMinutesForNewTraining);
            }
            catch(SqlException)
            {
                throw new BadRequestException("Invalid user!");
            }

            return new InitiateTrainingServiceResponse
            {
                TrainingId = trainingId
            };
        }
    }
}
