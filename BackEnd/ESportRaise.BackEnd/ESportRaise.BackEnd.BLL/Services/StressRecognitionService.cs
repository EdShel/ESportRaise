using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class StressRecognitionService : IStressRecognitionService
    {
        private readonly IndicatorParameters heartRateParameters;

        private readonly IndicatorParameters temperatureParameters;

        public StressRecognitionService(IConfiguration configuration)
        {
            var recognitionSettings = configuration.GetSection("StressRecognition");

            heartRateParameters = new IndicatorParameters();
            recognitionSettings.Bind("HeartRate", heartRateParameters);

            temperatureParameters = new IndicatorParameters();
            recognitionSettings.Bind("Temperature", temperatureParameters);
        }

        public IEnumerable<TimeInterval> FindCriticalMoments(IEnumerable<StateRecord> states)
        {
            var statesByMembers = states
                .OrderBy(state => state.CreateTime)
                .GroupBy(state => state.TeamMemberId);

            TimeInterval[] trainingMoments = new TimeInterval[0];

            foreach (var statesOfPerson in statesByMembers)
            {
                var abnormalHeartRateIntervals = GetStressfulSpansForHealthIndicator(
                    statesOfPerson,
                    state => state.HeartRate,
                    heartRateParameters)
                    .ToArray();

                var abnormalTemperatureIntervs = GetStressfulSpansForHealthIndicator(
                    statesOfPerson,
                    state => state.Temperature,
                    temperatureParameters)
                    .ToArray();

                var criticalMoments = new[]
                {
                    abnormalHeartRateIntervals,
                    abnormalTemperatureIntervs
                }.SelectMany(intervals => intervals);

                trainingMoments = TimeInterval.MergeIntervals(trainingMoments.Concat(criticalMoments)).ToArray();
            }

            return trainingMoments;
        }

        private static IEnumerable<TimeInterval> GetStressfulSpansForHealthIndicator(
            IEnumerable<StateRecord> statesOfPerson,
            Func<StateRecord, float> healthIndicatorExtractor,
            IndicatorParameters parameters)
        {
            DateTime begin = statesOfPerson.First().CreateTime;
            var normalizedStates = statesOfPerson.Select(state => new PhysicalState
            {
                StateIndicator = healthIndicatorExtractor(state),
                Time = state.CreateTime

            }).ToArray();

            double mean = normalizedStates.Select(state => state.StateIndicator).Average();
            double stdev = StandardDeviation(normalizedStates.Select(state => state.StateIndicator));

            int abnormalValuesInRow = 0;
            for (int i = 0; i < normalizedStates.Length; i++)
            {
                PhysicalState state = normalizedStates[i];
                float indicator = state.StateIndicator;

                bool isAbnormalValue = indicator - mean >= parameters.SigmaCoefficient * stdev;
                bool isLastRecord = i == normalizedStates.Length - 1;
                if (isAbnormalValue && !isLastRecord)
                {
                    abnormalValuesInRow++;
                }
                else if (abnormalValuesInRow > 0)
                {
                    var interval = new TimeInterval
                    {
                        Begin = normalizedStates[i - abnormalValuesInRow - 1].Time,
                        End = normalizedStates[i].Time
                    };
                    if (interval.GetDurationInSeconds() >= parameters.MinDurationSeconds)
                    {
                        yield return interval;
                    }
                    abnormalValuesInRow = 0;
                }
            }
        }

        private static double StandardDeviation(IEnumerable<float> values)
        {
            float avg = values.Average();
            var deviationsSquared = values
                .Select(x => x - avg)
                .Select(x => x * x);
            int n = values.Count();
            return Math.Sqrt(deviationsSquared.Sum() / n);
        }

        private class PhysicalState
        {
            public float StateIndicator { get; set; }

            public DateTime Time { get; set; }

            public override string ToString()
            {
                return $"{StateIndicator} - {Time.ToString("hh:mm:ss")}";
            }
        }

        private class IndicatorParameters
        {
            public double MinDurationSeconds { get; set; }

            public double SigmaCoefficient { get; set; }
        }
    }
}
