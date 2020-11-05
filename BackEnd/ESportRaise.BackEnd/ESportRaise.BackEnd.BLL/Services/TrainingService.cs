using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Constants;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    // TODO: every async method name with async suffix
    public class TrainingService : ITrainingService
    {
        private readonly int idlenessMinutesForNewTraining;

        private TrainingAsyncRepository trainings;

        public TrainingService(IConfiguration configuration, TrainingAsyncRepository trainings)
        {
            idlenessMinutesForNewTraining = configuration.GetValue<int>("IdlenessMinutesForNewTraining");
            this.trainings = trainings;
        }

        public async Task<InitiateTrainingServiceResponse> InitiateTrainingAsync(InitiateTrainingServiceRequest request)
        {
            int trainingId;
            try
            {
                int userId = request.UserId;
                trainingId = await trainings.GetTrainingIdAsync(userId, idlenessMinutesForNewTraining);
            }
            catch(SqlException ex)
            {
                if(ex.Number == SqlErrorCodes.USER_DOES_NOT_EXIST)
                {
                    throw new BadRequestException("Invalid user!");
                }
                throw ex;
            }

            return new InitiateTrainingServiceResponse
            {
                TrainingId = trainingId
            };
        }

    }
}
