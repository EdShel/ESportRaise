using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
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
    public class TrainingService : ITrainingService
    {
        private readonly int idlenessMinutesForNewTraining;

        private readonly TrainingRepository trainings;

        private readonly TeamMemberRepository members;

        private readonly YouTubeV3Service youTubeService;

        private readonly VideoStreamRepository videoStreams;

        public TrainingService(
            IConfiguration configuration,
            TrainingRepository trainings,
            TeamMemberRepository members,
            YouTubeV3Service youTubeService,
            VideoStreamRepository videoStreams)
        {
            idlenessMinutesForNewTraining = configuration.GetValue<int>("IdlenessMinutesForNewTraining");
            this.trainings = trainings;
            this.members = members;
            this.youTubeService = youTubeService;
            this.videoStreams = videoStreams;
        }

        public async Task<bool> IsTrainingOverAsync(int trainingId)
        {
            return await trainings.IsTrainingOver(trainingId, idlenessMinutesForNewTraining);
        }

        public async Task StopTrainingAsync(int trainingId)
        {
            Training training = await trainings.GetAsync(trainingId);
            if (training == null)
            {
                throw new NotFoundException("Training doesn't exist!");
            }
            await trainings.StopCurrentTrainingForTeamAsync(training.TeamId);
        }

        public async Task<TrainingDTO> GetTrainingAsync(int trainingId)
        {
            Training training = await trainings.GetAsync(trainingId);
            if (training == null)
            {
                throw new NotFoundException("Training doesn't exist!");
            }
            return new TrainingDTO
            {
                Id = training.Id,
                TeamId = training.TeamId,
                BeginTime = training.BeginTime
            };
        }

        public async Task<TrainingDTO> GetCurrentTrainingForTeamAsync(int teamId)
        {
            Training training = await trainings.GetCurrentTrainingForTeamAsync(teamId);
            if (training == null)
            {
                throw new NotFoundException("Team doesn't have any trainings!");
            }
            DateTime trainingEnd = await trainings.GetTrainingEndTimeAsync(training.Id);
            if ((DateTime.Now - trainingEnd).Minutes >= idlenessMinutesForNewTraining)
            {
                await trainings.StopCurrentTrainingForTeamAsync(teamId);
                throw new NotFoundException("The training is over");
            }
            return new TrainingDTO
            {
                Id = training.Id,
                TeamId = training.TeamId,
                BeginTime = training.BeginTime
            };
        }

        public async Task<IEnumerable<TrainingDTO>> GetTrainingsBeforeDateTimeAsync(int teamId, DateTime dateTime, int hours)
        {
            IEnumerable<Training> foundTrainings = await trainings.GetBeforeDateTimeAsync(teamId, dateTime, hours);
            return foundTrainings.Select(training => new TrainingDTO
            {
                Id = training.Id,
                TeamId = training.TeamId,
                BeginTime = training.BeginTime
            });
        }

        public async Task<int> InitiateTrainingAsync(int userId)
        {
            int trainingId;
            try
            {
                trainingId = await trainings.GiveNewTrainingIdAsync(userId, idlenessMinutesForNewTraining);

                TeamMember teamMember = await members.GetAsync(userId);
                if (teamMember == null)
                {
                    throw new BadRequestException("User doesn't belong to a team!");
                }

                LiveStreamResponseDTO streamResponse = await youTubeService.GetCurrentLiveStreamAsync(new LiveStreamRequestDTO
                {
                    LiveStreamingServiceUserId = teamMember.YouTubeId
                });
                if (streamResponse.HasLivestream)
                {
                    await videoStreams.CreateAsync(new VideoStream
                    {
                        TeamMemberId = userId,
                        TrainingId = trainingId,
                        YouTubeId = streamResponse.LiveStreamId
                    });
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == SqlErrorCodes.USER_DOES_NOT_EXIST)
                {
                    throw new BadRequestException("Invalid user!");
                }
                throw ex;
            }

            return trainingId;
        }

        public async Task<IEnumerable<VideoStreamDTO>> GetVideoStreamsAsync(int trainingId)
        {
            Training training = await trainings.GetAsync(trainingId);
            if (training == null)
            {
                throw new NotFoundException("Training doesn't exist");
            }
            var streams = await videoStreams.GetForTrainingAsync(trainingId);
            return streams.Select(stream => new VideoStreamDTO
            {
                Id = stream.Id,
                TeamMemberId = stream.TeamMemberId,
                StreamId = stream.YouTubeId
            });
        }
    }
}
