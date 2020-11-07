using System.Collections.Generic;
using System.Threading.Tasks;
using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ICriticalMomentService
    {
        Task<IEnumerable<CriticalMomentDTO>> GetCriticalMomentsForTrainingAsync(int trainingId);
    }
}