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

        public async Task<SaveStateRecordServiceResponse> SaveStateRecordAsync(SaveStateRecordServiceRequest request)
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
            return new SaveStateRecordServiceResponse();
        }

        public async Task<GetStateRecordServiceResponse> GetRecentAsync(GetStateRecordServiceRequest request)
        {
            IEnumerable<StateRecord> records;
            if (request.TeamMemberId == default)
            {
                records = await stateRecords.GetMostRecentAsync(request.TrainingId, request.TimeInSeconds);
            }
            else
            {
                records = await stateRecords.GetMostRecentForUserAsync(
                    request.TrainingId,
                    request.TimeInSeconds,
                    request.TeamMemberId.Value);
            }

            return new GetStateRecordServiceResponse
            {
                TrainingId = request.TrainingId,
                StateRecords = records.Select(rec => new GetStateRecordServiceResponse.StateRecord
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
