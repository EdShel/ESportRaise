using ESportRaise.BackEnd.BLL.DTOs.StateRecord;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IStateRecordService
    {
        Task SaveStateRecordAsync(StateRecordDTO request);

        Task<StateRecordSlimDTO> GetRecentAsync(StateRecordRequestDTO request);

        Task<StateRecordSlimDTO> GetForTrainingAsync(int trainingId);
    }
}
