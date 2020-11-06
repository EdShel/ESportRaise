using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;
using ESportRaise.BackEnd.DAL.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class StressRecognitionService
    {
        private readonly int heartRateQuarterDuration;

        private readonly int temperatureQuarterDuration;

        public StressRecognitionService(IConfiguration configuration)
        {
            var recognitionSettings = configuration.GetSection("StressRecognition");
            heartRateQuarterDuration = recognitionSettings.GetValue<int>("HeartRateQuarterDurationSeconds");
            temperatureQuarterDuration = recognitionSettings.GetValue<int>("TemperatureQuarterDurationSeconds");
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
                    heartRateQuarterDuration);

                var abnormalTemperatureIntervs = GetStressfulSpansForHealthIndicator(
                    statesOfPerson,
                    state => state.Temperature,
                    temperatureQuarterDuration);

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
            int quarterDuration)
        {
            DateTime begin = statesOfPerson.First().CreateTime;
            var normalizedStates = statesOfPerson.Select(state => new PhysicalState
            {
                StateIndicator = healthIndicatorExtractor(state),
                RelativeTimeInSeconds = (state.CreateTime - begin).TotalSeconds
            });

            var stdevsForHeartRate = GetStdDevsForQuarters(
                    normalizedStates,
                    quarterDuration)
                .ToArray();

            var stressfulQuarters = GetStressfulQuartersIndices(stdevsForHeartRate);
            var criticalMoments = stressfulQuarters.Select(quarter => new TimeInterval
            {
                Begin = begin.AddSeconds(quarter.BeginQuarterIndex * quarterDuration),
                End = begin.AddSeconds(quarter.EndQuarterIndex * quarterDuration)
            });
            return criticalMoments;
        }
        private class CriticalQuarterSpan
        {
            public int BeginQuarterIndex { get; set; }

            public int EndQuarterIndex { get; set; }
        }

        private static IEnumerable<CriticalQuarterSpan> GetStressfulQuartersIndices(IEnumerable<QuarterStdevs> quartersStdevs)
        {
            bool isStressfulStateNow = false;
            int stressQuarterBegin = -1;
            foreach (var stdevs in quartersStdevs)
            {
                double stdevFromOneToThreeFourth = stdevs.FromOneToThreeFourth;
                double stdevFourFourth = stdevs.FourFourth;
                bool differByTwoSigma = stdevFourFourth / stdevFromOneToThreeFourth >= 2d;
                if (differByTwoSigma)
                {
                    if (isStressfulStateNow)
                    {
                        yield return new CriticalQuarterSpan
                        {
                            BeginQuarterIndex = stressQuarterBegin,
                            EndQuarterIndex = stdevs.QuarterEndIndex
                        };
                        isStressfulStateNow = false;
                    }
                    else
                    {
                        isStressfulStateNow = true;
                        stressQuarterBegin = stdevs.QuarterEndIndex;
                    }
                }
            }
        }

        private static IEnumerable<QuarterStdevs> GetStdDevsForQuarters(
            IEnumerable<PhysicalState> normalizedStates,
            int quarterDur)
        {
            var statesByQuarters = normalizedStates
                .GroupBy(state => (int)(state.RelativeTimeInSeconds / quarterDur));

            int quartersCount = statesByQuarters.Select(k => k.Key).Max();
            if (quartersCount < 3)
            {
                yield break;
            }

            for (int i = 3; i < quartersCount; i++)
            {
                double quarterStdev = StandardDeviation(
                    statesByQuarters
                    .Where(stateQuarter => stateQuarter.Key == i)
                    .SelectMany(stateQuarter => stateQuarter
                        .Select(state => state.StateIndicator)));
                double previousThreeQuartersStdev = StandardDeviation(
                    statesByQuarters
                        .Where(stateQuarter => i - stateQuarter.Key > 0 && i - stateQuarter.Key <= 3)
                        .SelectMany(stateQuarter => stateQuarter
                            .Select(state => state.StateIndicator)));

                yield return new QuarterStdevs
                {
                    QuarterBeginIndex = i - 3,
                    QuarterEndIndex = i,
                    FromOneToThreeFourth = previousThreeQuartersStdev,
                    FourFourth = quarterStdev
                };
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

            public double RelativeTimeInSeconds { get; set; }
        }

        private class QuarterStdevs
        {
            public int QuarterBeginIndex { get; set; }

            public int QuarterEndIndex { get; set; }

            public double FromOneToThreeFourth { get; set; }

            public double FourFourth { get; set; }
        }
    }
}
