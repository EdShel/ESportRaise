using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESportRaise.BackEnd.BLL.DTOs.Training;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITrainingService
    {
        Task<TrainingDTO> GetCurrentTrainingForTeamAsync(int teamId);

        Task<TrainingDTO> GetTrainingAsync(int trainingId);

        Task<IEnumerable<TrainingDTO>> GetTrainingsBeforeDateTimeAsync(int teamId, DateTime dateTime, int hours);

        Task<IEnumerable<VideoStreamDTO>> GetVideoStreamsAsync(int trainingId);

        Task<int> InitiateTrainingAsync(int userId);

        Task<bool> IsTrainingOverAsync(int trainingId);

        Task StopTrainingAsync(int trainingId);
    }
}