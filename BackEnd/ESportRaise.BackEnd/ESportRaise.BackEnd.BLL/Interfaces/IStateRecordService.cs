using ESportRaise.BackEnd.BLL.DTOs.StateRecord;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IStateRecordService
    {
        Task<SaveStateRecordServiceResponse> SaveStateRecordAsync(SaveStateRecordServiceRequest request);

        Task<GetStateRecordServiceResponse> GetRecentAsync(GetStateRecordServiceRequest request);
    }
}
