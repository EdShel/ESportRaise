using System;
using System.Collections.Generic;
using System.Linq;

namespace ESportRaise.BackEnd.BLL.DTOs.CriticalMoment
{
    public class TimeInterval
    {
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public double GetDurationInSeconds()
        {
            return (End - Begin).TotalSeconds;
        }

        public static IEnumerable<TimeInterval> MergeIntervals(IEnumerable<TimeInterval> intervals)
        {
            var accumulator = intervals.First();
            intervals = intervals.Skip(1);

            foreach (var interval in intervals)
            {
                if (interval.Begin <= accumulator.End)
                {
                    accumulator = CombineIntervals(accumulator, interval);
                }
                else
                {
                    yield return accumulator;
                    accumulator = interval;
                }
            }

            yield return accumulator;
        }

        private static TimeInterval CombineIntervals(TimeInterval start, TimeInterval end)
        {
            return new TimeInterval
            {
                Begin = start.Begin,
                End = Max(start.End, end.End),
            };
        }

        private static DateTime Max(DateTime left, DateTime right)
        {
            return (left > right) ? left : right;
        }
    }

}
