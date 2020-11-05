using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Constants;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        public async Task<TrainingDTO> GetLastTrainingForTeamAsync(int teamId)
        {
            Training training = await trainings.GetLastForTeamAsync(teamId);
            if (training == null)
            {
                throw new NotFoundException("Team doesn't have any trainings!");
            }
            return new TrainingDTO
            {
                Id = training.Id,
                TeamId = training.TeamId,
                BeginTime = training.BeginTime
            };
        }

        public async Task<IEnumerable<TrainingDTO>> GetTrainingsBeforeDateTime(int teamId, DateTime dateTime, int hours)
        {
            IEnumerable<Training> foundTrainings = await trainings.GetBeforeDateTimeAsync(teamId, dateTime, hours);
            return foundTrainings.Select(training => new TrainingDTO
            {
                Id = training.Id,
                TeamId = training.TeamId,
                BeginTime = training.BeginTime
            });
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
