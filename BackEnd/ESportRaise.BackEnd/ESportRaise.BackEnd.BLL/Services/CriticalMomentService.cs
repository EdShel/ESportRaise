using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;
using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class CriticalMomentService
    {
        private readonly CriticalMomentRepository criticalMoments;

        private readonly TrainingService trainingService;

        private readonly StateRecordRepository stateRecors;

        private readonly StressRecognitionService stressRecognitionService;

        private readonly int idlenessMinutesForNewTraining;

        public CriticalMomentService(
            CriticalMomentRepository criticalMoments, 
            TrainingService trainingService,
            StateRecordRepository stateRecors, 
            StressRecognitionService stressRecognitionService,
            IConfiguration configuration)
        {
            this.criticalMoments = criticalMoments;
            this.trainingService = trainingService;
            this.stateRecors = stateRecors;
            this.stressRecognitionService = stressRecognitionService;
            this.idlenessMinutesForNewTraining = configuration.GetValue<int>("IdlenessMinutesForNewTraining");
        }

        public async Task<IEnumerable<CriticalMomentDTO>> GetCriticalMomentsForTrainingAsync(int trainingId)
        {
            TrainingDTO training = await trainingService.GetTrainingAsync(trainingId);
            if (training == null)
            {
                throw new NotFoundException("Training doesn't exist!");
            }

            bool hasCachedInDb = await criticalMoments.IsCachedForTrainingAsync(trainingId);
            IEnumerable<CriticalMoment> moments;
            if (hasCachedInDb)
            {
                moments = await criticalMoments.GetForTrainingAsync(trainingId);
            }
            else
            {
                bool isTrainingOver = await trainingService.IsTrainingOver(trainingId);
                if (!isTrainingOver)
                {
                    throw new BadRequestException("Training hasn't finished yet! Stop it or wait!");
                }

                var states = await stateRecors.GetForTrainingAsync(trainingId);
                moments = stressRecognitionService
                    .FindCriticalMoments(states)
                    .Select(interval => new CriticalMoment
                    {
                        TrainingId = trainingId,
                        BeginTime = interval.Begin,
                        EndTime = interval.End
                    });
                await criticalMoments.SetCachedForTrainingAsync(trainingId);
                await criticalMoments.CreateManyAsync(moments);
            }

            return moments.Select(moment => new CriticalMomentDTO
            {
                Id = moment.Id,
                TrainingId = moment.TrainingId,
                BeginTime = moment.BeginTime,
                EndTime = moment.EndTime
            });
        }
    }
}
