using System.Collections.Generic;
using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;
using ESportRaise.BackEnd.DAL.Entities;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IStressRecognitionService
    {
        IEnumerable<TimeInterval> FindCriticalMoments(IEnumerable<StateRecord> states);
    }
}