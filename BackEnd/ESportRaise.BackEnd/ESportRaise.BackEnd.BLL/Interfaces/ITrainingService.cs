﻿using ESportRaise.BackEnd.BLL.DTOs.Training;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITrainingService
    {
        Task<InitiateTrainingServiceResponse> InitiateTrainingAsync(InitiateTrainingServiceRequest request);

        Task<TrainingDTO> GetLastTrainingForTeamAsync(int teamId);

        Task<IEnumerable<TrainingDTO>> GetTrainingsBeforeDateTime(int teamId, DateTime dateTime, int hours);
    }
}
