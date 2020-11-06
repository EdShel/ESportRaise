using ESportRaise.BackEnd.BLL.DTOs.StateRecord;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Constants;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class StateRecordService : IStateRecordService
    {
        private readonly StateRecordRepository stateRecords;

        public StateRecordService(StateRecordRepository stateRecords)
        {
            this.stateRecords = stateRecords;
        }

        public async Task SaveStateRecordAsync(StateRecordDTO request)
        {
            StateRecord stateRecord = new StateRecord
            {
                TeamMemberId = request.TeamMemberId,
                TrainingId = request.TrainingId,
                HeartRate = request.HeartRate,
                Temperature = request.Temperature
            };
            try
            {
                await stateRecords.CreateAsync(stateRecord);
            }
            catch (SqlException ex)
            {
                if (ex.Number == SqlErrorCodes.INVALID_TRAINING)
                {
                    throw new BadRequestException("Training id is invalid or outdated!");
                }
                throw ex;
            }
        }

        public async Task<StateRecordSlimDTO> GetRecentAsync(StateRecordRequestDTO request)
        {
            IEnumerable<StateRecord> records;
            if (request.TeamMemberId == default)
            {
                records = await stateRecords.GetForTrainingMostRecentAsync(request.TrainingId, request.TimeInSeconds);
            }
            else
            {
                records = await stateRecords.GetForTrainingAndUserMostRecentAsync(
                    request.TrainingId,
                    request.TimeInSeconds,
                    request.TeamMemberId.Value);
            }

            return new StateRecordSlimDTO
            {
                TrainingId = request.TrainingId,
                StateRecords = records.Select(rec => new StateRecordSlimDTO.StateRecord
                {
                    CreateTime = rec.CreateTime,
                    TeamMemberId = rec.TeamMemberId,
                    HeartRate = rec.HeartRate,
                    Temperature = rec.Temperature
                })
            };
        }

        public async Task<StateRecordSlimDTO> GetForTrainingAsync(int trainingId)
        {
            IEnumerable<StateRecord> records = await stateRecords.GetForTrainingAsync(trainingId);

            return new StateRecordSlimDTO
            {
                TrainingId = trainingId,
                StateRecords = records.Select(rec => new StateRecordSlimDTO.StateRecord
                {
                    CreateTime = rec.CreateTime,
                    TeamMemberId = rec.TeamMemberId,
                    HeartRate = rec.HeartRate,
                    Temperature = rec.Temperature
                })
            };
        }
    }
}
