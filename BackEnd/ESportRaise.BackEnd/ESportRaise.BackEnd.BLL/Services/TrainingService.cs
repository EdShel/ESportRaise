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
    // TODO: every async method name with async suffix
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

        public async Task<bool> IsTrainingOver(int trainingId)
        {
            DateTime trainingEnd = await trainings.GetTrainingEndTimeAsync(trainingId);
            return (trainingEnd - DateTime.Now).Minutes >= idlenessMinutesForNewTraining;
        }

        public async Task StopTraining(int trainingId)
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
                trainingId = await trainings.GiveNewTrainingIdAsync(userId, idlenessMinutesForNewTraining);

                TeamMember teamMember = await members.GetAsync(userId);
                LiveStreamServiceResponse streamResponse = await youTubeService.GetCurrentLiveStream(new LiveStreamServiceRequest
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

            return new InitiateTrainingServiceResponse
            {
                TrainingId = trainingId
            };
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
